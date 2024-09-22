using System;
using System.Collections.Generic;
using System.Threading;

namespace UnionContainers.Shared.Common;

/// <summary>
/// By default this is not used but if needed can provide more information for the source analyzers by logging messages during compile/build time
/// </summary>
internal static class GeneratorLogger
{
    private static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
    private static readonly List<string> _logMessages = new List<string>();
    private static string? logFilePath = null;

    private static string logInitMessage = "[+] Generated Log File\n" + "[+] This file contains log messages from the source generator\n\n";
    private static LoggingLevel _loggingLevel = LoggingLevel.Info;
    
    public static void SetLoggingLevel(LoggingLevel level)
    {
        _loggingLevel = level;
    }
    
    public static void SetLogFilePath(string path)
    {
        logFilePath = path;
    }
    
    public static LoggingLevel GetLoggingLevel()
    {
        return _loggingLevel;
    }
    
    public static void LogMessage(string message, LoggingLevel messageLogLevel = LoggingLevel.Info)
    {
        try
        {
            _lock.EnterWriteLock();
            if (File.Exists(logFilePath) is false)
            {
                File.WriteAllText(logFilePath, logInitMessage);
                File.AppendAllText(logFilePath, $"Logging started at {GetDateTimeUtc()}\n\n");
            }
            if (messageLogLevel < _loggingLevel)
            {
                return;
            }
            string _logMessage = message + "\n";
            if (messageLogLevel > LoggingLevel.Info)
            {
                _logMessage = $"[{messageLogLevel} start]\n" + _logMessage + $"[{messageLogLevel} end]\n\n";
            }
            if (!_logMessages.Contains(_logMessage))
            {
                File.AppendAllText(logFilePath, _logMessage);
                _logMessages.Add(_logMessage);
            }
        }
        catch (Exception ex)
        {
            File.AppendAllText(logFilePath, $"[-] Exception occurred in logging: {ex.Message} \n");
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }
    
    public static void EndLogging()
    {
        if (File.Exists(logFilePath))
        {
            File.AppendAllText(logFilePath, $"[+] Logging ended at {GetDateTimeUtc()}\n");
        }
    }
    
    public static string GetDateTimeUtc()
    {
        return DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
    }
}

public enum LoggingLevel
{
    Trace,
    Debug,
    Info,
    Warning,
    Error,
    Fatal
}