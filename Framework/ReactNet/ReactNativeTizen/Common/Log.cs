using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace ReactNative.Common
{
    //
    // Summary:
    //     Provides methods to print log messages to the Tizen logging system.
    public class Log
    {
        private const string Tag = "ReactNative";
        private static readonly Dictionary<string, TraceSource> LogSource = new Dictionary<string, TraceSource>();

        public enum LogLevel
        {
            Fatal,
            Error,
            Warn,
            Info,
            Debug,
            Verbose
        }

        private static TraceSource GetLogger(string tag)
        {
            lock (LogSource)
            {
                if (!LogSource.TryGetValue(tag, out var value))
                {
                    value = new TraceSource($"{Tag}.{tag}");
                    LogSource.Add(tag, value);
                }
                return value;
            }
        }

        private const string format = "{1}: {2}:{3} > {0}";

        //
        // Summary:
        //     Prints a log message with the DEBUG priority.
        //
        // Parameters:
        //   tag:
        //     The tag name of the log message.
        //
        //   message:
        //     The log message to print.
        //
        //   file:
        //     The source file path of the caller function. This argument will be set automatically
        //     by the compiler.
        //
        //   func:
        //     The function name of the caller function. This argument will be set automatically
        //     by the compiler.
        //
        //   line:
        //     The line number of the calling position. This argument will be set automatically
        //     by the compiler.

        [Conditional("TRACE")]
        public static void Debug(string tag, string message, [CallerFilePath] string file = "", [CallerMemberName] string func = "", [CallerLineNumber] int line = 0)
        {
            GetLogger(tag).TraceEvent(TraceEventType.Verbose, 1, format, message, file, func, line);
        }

        //
        // Summary:
        //     Prints a log message with the ERROR priority.
        //
        // Parameters:
        //   tag:
        //     The tag name of the log message.
        //
        //   message:
        //     The log message to print.
        //
        //   file:
        //     The source file path of the caller function. This argument will be set automatically
        //     by the compiler.
        //
        //   func:
        //     The function name of the caller function. This argument will be set automatically
        //     by the compiler.
        //
        //   line:
        //     The line number of the calling position. This argument will be set automatically
        //     by the compiler.
        [Conditional("TRACE")]
        public static void Error(string tag, string message, [CallerFilePath] string file = "", [CallerMemberName] string func = "", [CallerLineNumber] int line = 0)
        {
            GetLogger(tag).TraceEvent(TraceEventType.Error, 0, format, message, file, func, line);
        }

        //
        // Summary:
        //     Prints a log message with the FATAL priority.
        //
        // Parameters:
        //   tag:
        //     The tag name of the log message.
        //
        //   message:
        //     The log message to print.
        //
        //   file:
        //     The source file path of the caller function. This argument will be set automatically
        //     by the compiler.
        //
        //   func:
        //     The function name of the caller function. This argument will be set automatically
        //     by the compiler.
        //
        //   line:
        //     The line number of the calling position. This argument will be set automatically
        //     by the compiler.
        [Conditional("TRACE")]
        public static void Fatal(string tag, string message, [CallerFilePath] string file = "", [CallerMemberName] string func = "", [CallerLineNumber] int line = 0)
        {
            GetLogger(tag).TraceEvent(TraceEventType.Critical, 0, format, message, file, func, line);
        }

        //
        // Summary:
        //     Prints a log message with the INFO priority.
        //
        // Parameters:
        //   tag:
        //     The tag name of the log message.
        //
        //   message:
        //     The log message to print.
        //
        //   file:
        //     The source file path of the caller function. This argument will be set automatically
        //     by the compiler.
        //
        //   func:
        //     The function name of the caller function. This argument will be set automatically
        //     by the compiler.
        //
        //   line:
        //     The line number of the calling position. This argument will be set automatically
        //     by the compiler.

        [Conditional("TRACE")]
        public static void Info(string tag, string message, [CallerFilePath] string file = "", [CallerMemberName] string func = "", [CallerLineNumber] int line = 0)
        {
            GetLogger(tag).TraceEvent(TraceEventType.Information, 0, format, message, file, func, line);
        }

        //
        // Summary:
        //     Prints a log message with the VERBOSE priority.
        //
        // Parameters:
        //   tag:
        //     The tag name of the log message.
        //
        //   message:
        //     The log message to print.
        //
        //   file:
        //     The source file path of the caller function. This argument will be set automatically
        //     by the compiler.
        //
        //   func:
        //     The function name of the caller function. This argument will be set automatically
        //     by the compiler.
        //
        //   line:
        //     The line number of the calling position. This argument will be set automatically
        //     by the compiler.

        [Conditional("TRACE")]
        public static void Verbose(string tag, string message, [CallerFilePath] string file = "", [CallerMemberName] string func = "", [CallerLineNumber] int line = 0)
        {
            GetLogger(tag).TraceEvent(TraceEventType.Verbose, 0, format, message, file, func, line);
        }

        //
        // Summary:
        //     Prints a log message with the WARNING priority.
        //
        // Parameters:
        //   tag:
        //     The tag name of the log message.
        //
        //   message:
        //     The log message to print.
        //
        //   file:
        //     The source file path of the caller function. This argument will be set automatically
        //     by the compiler.
        //
        //   func:
        //     The function name of the caller function. This argument will be set automatically
        //     by the compiler.
        //
        //   line:
        //     The line number of the calling position. This argument will be set automatically
        //     by the compiler.
        [Conditional("TRACE")]
        public static void Warn(string tag, string message, [CallerFilePath] string file = "", [CallerMemberName] string func = "", [CallerLineNumber] int line = 0)
        {
            GetLogger(tag).TraceEvent(TraceEventType.Warning, 0, format, message, file, func, line);
        }

    }
}
