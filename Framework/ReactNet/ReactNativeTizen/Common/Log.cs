using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace ReactNative.Common
{
    //
    // Summary:
    //     Provides methods to print log messages to the Tizen logging system.
    public class Log
    {
        public enum LogLevel
        {
            Fatal,
            Error,
            Warn,
            Info,
            Debug,
            Verbose
        }

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
            Debugger.Log((int) LogLevel.Debug, tag, $"[{file}:{line}] {func}: {message}");
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
            Debugger.Log((int)LogLevel.Error, tag, $"[{file}:{line}] {func}: {message}");
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
            Debugger.Log((int)LogLevel.Fatal, tag, $"[{file}:{line}] {func}: {message}");
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
            Debugger.Log((int)LogLevel.Info, tag, $"[{file}:{line}] {func}: {message}");
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
            Debugger.Log((int)LogLevel.Verbose, tag, $"[{file}:{line}] {func}: {message}");
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
            Debugger.Log((int)LogLevel.Warn, tag, $"[{file}:{line}] {func}: {message}");
        }

    }
}
