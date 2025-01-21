using System;
using System.Collections.Generic;
using System.IO;
using Fuyu.Common.Delegates;
using Fuyu.Common.IO;
using Fuyu.Common.Serialization;

namespace Fuyu.Common.Config;

public class ConfigService
{
    private static readonly Lazy<Dictionary<string, ConfigService>> instances = new(() => new());
    private readonly Lazy<Dictionary<string, AbstractConfig>> lazyConfigs = new(() => new());
    private readonly string _platform;

    public static ConfigCallback Loaded;
    public static ConfigCallback Unloaded;

    private ConfigService(string platform)
    {
        _platform = platform;
        Loaded?.Invoke(platform);
    }

    public static ConfigService GetInstance(string platform)
    {
        if (!instances.Value.ContainsKey(platform))
        {
            instances.Value[platform] = new ConfigService(platform);
        }

        return instances.Value[platform];
    }

    public static void FreeInstance(string platform)
    {
        instances.Value.Remove(platform);
        Unloaded?.Invoke(platform);
    }

    private string GetConfigPath(string configName)
    {
        return $"./Fuyu/Configs/{_platform}/{configName}.json";
    }

    #region Regular config

    public T GetConfig<T>(string configName) where T : AbstractConfig, new()
    {
        var file = GetConfigPath(configName);
        if (VFS.Exists(file))
        {
            return Json.Parse<T>(VFS.ReadTextFile(file));
        }

        return new();
    }

    public bool IsConfigExists(string configName)
    {
        var file = GetConfigPath(configName);
        return VFS.Exists(file);
    }

    public void SaveConfig<T>(string configName, T Config) where T : AbstractConfig
    {
        var file = GetConfigPath(configName);
        var json = Json.Stringify(Config);
        VFS.WriteTextFile(file, json);
    }

    public void DeleteConfig(string configName)
    {
        var file = GetConfigPath(configName);
        File.Delete(file);
    }

    #endregion

    #region Lazy config

    public void GetConfigLazy<T>(string configName, ref T value, bool reload = false)
        where T : AbstractConfig, new()
    {
        value = new();

        if (lazyConfigs.Value.ContainsKey(configName))
        {
            // Check if can parse to that type.
            var x = lazyConfigs.Value[configName];

            if (typeof(T).IsAssignableFrom(x.GetType()))
            {
                value = (T)lazyConfigs.Value[configName];
                // if we dont want to reload from the file, we just return
                if (!reload)
                {
                    return;
                }
            }
        }
        // we loading here if we dont already have it.
        var file = GetConfigPath(configName);
        if (VFS.Exists(file))
        {
            value = Json.Parse<T>(VFS.ReadTextFile(file));
        }

        // storing the config name and the value.
        if (!lazyConfigs.Value.ContainsKey(configName))
        {
            lazyConfigs.Value.Add(configName, value);
        }

        lazyConfigs.Value[configName] = value;
    }

    public T GetOrCreate<T>(string configName, bool reload = false)
        where T : AbstractConfig, new()
    {
        T value = new();

        if (lazyConfigs.Value.ContainsKey(configName))
        {
            // Check if can parse to that type.
            var x = lazyConfigs.Value[configName];

            if (typeof(T).IsAssignableFrom(x.GetType()))
            {
                value = (T)lazyConfigs.Value[configName];
                // if we dont want to reload from the file, we just return
                if (!reload)
                {
                    return value;
                }
            }
        }
        // we loading here if we dont already have it.
        var file = GetConfigPath(configName);
        if (VFS.Exists(file))
        {
            value = Json.Parse<T>(VFS.ReadTextFile(file));
        }

        // storing the config name and the value.
        if (!lazyConfigs.Value.ContainsKey(configName))
        {
            lazyConfigs.Value.Add(configName, value);
        }

        lazyConfigs.Value[configName] = value;
        return value;
    }

    public bool IsConfigExistsLazy(string configName)
    {
        return lazyConfigs.Value.ContainsKey(configName);
    }

    public void SaveConfigLazy(string configName)
    {
        var Config = lazyConfigs.Value[configName];
        var file = GetConfigPath(configName);
        var json = Json.Stringify(Config);
        VFS.WriteTextFile(file, json);
    }

    public void DeleteConfigLazy(string configName, bool deleteFile = true)
    {
        lazyConfigs.Value.Remove(configName);
        if (!deleteFile)
        {
            return;
        }

        var file = GetConfigPath(configName);
        File.Delete(file);
    }

    #endregion
}