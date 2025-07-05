using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace Fuyu.Backend.Logging;

public sealed class CustomConsoleFormatter : ConsoleFormatter
{
    private readonly IDisposable _optionsReloadToken;
    private CustomConsoleFormatterOptions _formatterOptions;

    public CustomConsoleFormatter(IOptionsMonitor<CustomConsoleFormatterOptions> options)
        : base("custom")
    {
        _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
        _formatterOptions = options.CurrentValue;
    }

    private void ReloadLoggerOptions(CustomConsoleFormatterOptions options)
    {
        _formatterOptions = options;
    }

    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider scopeProvider,
        TextWriter textWriter)
    {
        var message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        if (message == null)
        {
            return;
        }

        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        var categoryName = logEntry.Category;
        var logLevel = GetLogLevelString(logEntry.LogLevel);
        var typeName = categoryName.Split('.').LastOrDefault() ?? categoryName;

        textWriter.WriteLine($"[{logLevel}] {timestamp} - {typeName}: {message}");
    }

    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "TRACE",
            LogLevel.Debug => "DEBUG",
            LogLevel.Information => "INFO",
            LogLevel.Warning => "WARN",
            LogLevel.Error => "ERROR",
            LogLevel.Critical => "CRITICAL",
            _ => logLevel.ToString().ToUpper()
        };
    }

    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
    }
}