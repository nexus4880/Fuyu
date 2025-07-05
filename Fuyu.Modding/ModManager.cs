using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fuyu.Modding;

// This should live during the entire lifetime of the application
public class ModManager
{
    private readonly ILogger<ModManager> _logger;

    public ModManager(ILogger<ModManager> logger)
    {
        _logger = logger;
    }

    private readonly List<AbstractMod> _mods = new List<AbstractMod>();

    private ResourceDescription[] GetResources(string assemblyName, string rootPath)
    {
        var resourceRootPath = Path.Combine(rootPath, "res");
        var resourcePaths = Array.Empty<string>();

        if (Directory.Exists(resourceRootPath))
        {
            resourcePaths = Directory.GetFiles(resourceRootPath, "*.*", SearchOption.AllDirectories);
        }

        if (resourcePaths.Length == 0)
        {
            return null;
        }

        var resources = resourcePaths.Select(resourcePath =>
        {
            // convert resource path to resx format
            var resxPath = resourcePath
                .Replace(resourceRootPath + "\\", string.Empty) // exp. Fuyu/Mods/Launcher
                .Replace("../", string.Empty)                   // relative path ./
                .Replace("./", string.Empty)                    // relative path ./
                .Replace("\\", ".")                             // windows \
                .Replace("/", ".");                             // unix /

            // add resource
            var fileName = $"{assemblyName}.Resources.{resxPath}";

            return new ResourceDescription(
                fileName,
                () => new FileStream(resourcePath, FileMode.Open, FileAccess.Read, FileShare.None),
                isPublic: true
            );
        }).ToArray();

        return resources;
    }

    public void AddMods(string directory, IServiceCollection services)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            return;
        }

        var subdirectories = Directory.GetDirectories(directory);

        foreach (var subdirectory in subdirectories)
        {
#if NET6_0_OR_GREATER
            var fileInfo = new FileInfo(subdirectory);
            if (fileInfo.LinkTarget is not null)
            {
                ProcessModDirectory(fileInfo.LinkTarget, services);
                continue;
            }
#endif
            var modDirectory = Path.GetFullPath(subdirectory);
            ProcessModDirectory(modDirectory, services);
        }
    }

    private void ProcessModDirectory(string directory, IServiceCollection services)
    {
        var modType = GetModType(directory);

        switch (modType)
        {
            case EModType.DLL:
                ProcessDLLMod(directory, services);
                break;
            case EModType.Source:
                ProcessSourceFiles(directory, services);
                break;
            case EModType.Disabled:
                break;
            default:
                throw new Exception($"{directory} does not contain a valid mod setup");
        }
    }

    private EModType GetModType(string directory)
    {
        if (Directory.GetFiles(directory, ".disabled").Length > 0)
        {
            return EModType.Disabled;
        }

        if (Directory.GetFiles(directory, "*.dll").Length > 0)
        {
            return EModType.DLL;
        }

        if (Directory.Exists(Path.Combine(directory, "src")))
        {
            if (Directory.GetFiles(Path.Combine(directory, "src"), "*.cs").Length > 0)
            {
                return EModType.Source;
            }
        }

        return EModType.Invalid;
    }

    private void ProcessDLLMod(string directory, IServiceCollection services)
    {
        var dllPaths = Directory.GetFiles(directory, "*.dll");

        foreach (var dllPath in dllPaths)
        {
            var assembly = Assembly.LoadFrom(dllPath);
            ProcessAssembly(assembly, EModType.DLL, services);
        }
    }

    private void ProcessAssembly(Assembly assembly, EModType assemblyModType, IServiceCollection services)
    {
        // Get types where T inherits from Mod
        var modTypes = assembly.GetExportedTypes()
            .Where(t => typeof(AbstractMod).IsAssignableFrom(t));

        // Technically you could export multiple mods per assembly
        foreach (var modType in modTypes)
        {
            services.AddSingleton(typeof(AbstractMod), modType);
            modType.ConfigureServices(services);
        }
    }

    public async Task Load(IServiceProvider provider)
    {
        _mods.AddRange(provider.GetServices<AbstractMod>());
        CheckDependencies();
        SortMods();

        foreach (var mod in _mods)
        {
            if (!mod.IsLoaded)
            {
                _logger.LogInformation("[{ModName} - ({ModId})] loading", mod.Name, mod.Id);
                await mod.OnLoad();
                mod.IsLoaded = true;
            }
        }
    }

    private void CheckDependencies()
    {
        for (var i = 0; i < _mods.Count; i++)
        {
            var mod = _mods[i];

            if (mod.Dependencies != null)
            {
                foreach (var dependency in mod.Dependencies)
                {
                    if (_mods.FindIndex(m => m.Id == dependency) == -1)
                    {
                        throw new Exception($"{mod.Id} is missing dependency {dependency} and will not be loaded");
                    }
                }
            }
        }
    }

    private void SortMods()
    {
        // TODO: readd later, based on dependency tree
    }

    public async Task UnloadAll()
    {
        while (_mods.Count > 0)
        {
            await UnloadMod(_mods[_mods.Count - 1]);
        }
    }

    private async Task UnloadMod(AbstractMod mod)
    {
        if (mod.IsLoaded)
        {
            _logger.LogInformation("[{ModName} - ({ModId})] unloading", mod.Name, mod.Id);
            await mod.OnShutdown();
            mod.IsLoaded = false;
        }

        _mods.Remove(mod);
    }

    private CSharpCompilation CreateCompilation(
        string assemblyName,
        IEnumerable<SyntaxTree> syntaxTrees,
        bool includeReferences)
    {
        // TODO: Simplify this later
        IEnumerable<PortableExecutableReference> references;

        if (includeReferences)
        {
            references =
                // Mods can reference everything we have loaded
                AppDomain.CurrentDomain.GetAssemblies()
                    // Where it is a disk on file
                    .Where(a => !string.IsNullOrEmpty(a.Location))
                    // Ensure type inclusion
                    .Append(typeof(DataContractAttribute).Assembly)
                    .Append(typeof(HttpClient).Assembly)
                    .Append(typeof(CompressionLevel).Assembly)
                    // Create a MetadataReference from it
                    .Select(a => MetadataReference.CreateFromFile(a.Location));
        }
        else
        {
            references = Array.Empty<PortableExecutableReference>();
        }

        return CSharpCompilation.Create(
            assemblyName,
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
    }

    private void ProcessSourceFiles(string directory, IServiceCollection services)
    {
        var sourceFiles = Directory.GetFiles(Path.Combine(directory, "src"), "*.cs", SearchOption.AllDirectories);

        if (sourceFiles.Length == 0)
        {
            throw new Exception($"{directory} was marked as a source mod yet no source files were found");
        }

        var assemblyName = Path.GetFileName(directory);
        var syntaxTrees = new List<SyntaxTree>(sourceFiles.Length);

        foreach (var file in sourceFiles)
        {
            var fileContents = File.ReadAllText(file);
            var syntaxTree = CSharpSyntaxTree.ParseText(
                fileContents,
                new CSharpParseOptions(
                    LanguageVersion.Latest,
                    DocumentationMode.None,
                    SourceCodeKind.Regular
                ),
                file
            );

            syntaxTrees.Add(syntaxTree);
        }

        var resources = GetResources(assemblyName, directory);
        var compilation = CreateCompilation(assemblyName, syntaxTrees, true);

        Assembly assembly;

        // Intentionally left open so that we can use it later
        using (var ms = new MemoryStream())
        {
            var emitResult = compilation.Emit(ms, manifestResources: resources);
            if (!emitResult.Success)
            {
                var errors = emitResult.Diagnostics
                    .Where(d => !d.IsSuppressed && d.Severity == DiagnosticSeverity.Error);

                // Technically no cleanup is being done here but that's because this is considered a
                // failed state and the ModManager should no longer live (and therefore the program)
                // -- nexus4880, 2024-12-3

                throw new Exception(string.Join(Environment.NewLine, errors));
            }

            ms.Position = 0;
            assembly = Assembly.Load(ms.ToArray());
        }

        ProcessAssembly(assembly, EModType.Source, services);
    }
}