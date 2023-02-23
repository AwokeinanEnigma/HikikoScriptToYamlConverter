using System.Runtime.CompilerServices;

namespace HikikoScriptToYamlConverter
{
    //based on https://github.com/NoelFB/Foster/blob/master/Framework/Logging/Log.cs
    //with some minor changes
    public static partial class DBG

    {
        private static LogLevel Verbosity = LogLevel.Debug;

        private static readonly List<string> log = new List<string>();

        public enum LogLevel
        {
            System,
            Assert,
            Error,
            Warning,
            Info,
            Debug,
            Lua,
            Trace,
        }

        private static readonly Dictionary<LogLevel, ConsoleColor> logColors = new Dictionary<LogLevel, ConsoleColor>
        {
            [LogLevel.System] = ConsoleColor.White,
            [LogLevel.Assert] = ConsoleColor.DarkRed,
            [LogLevel.Error] = ConsoleColor.Red,
            [LogLevel.Warning] = ConsoleColor.Yellow,
            [LogLevel.Info] = ConsoleColor.White,
            [LogLevel.Debug] = ConsoleColor.Gray,
            [LogLevel.Lua] = ConsoleColor.Magenta,
            [LogLevel.Trace] = ConsoleColor.Cyan,
        };

        /// <summary>
        /// Generic logging function that logs the given message as a system log.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="callerFilePath">The file path of the caller. This parameter is automatically set and used for internal purposes.</param>
        /// <param name="callerLineNumber">The line number of the caller. This parameter is automatically set and used for internal purposes.</param>
        public static void Log(
        object message,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            LogInternal(LogLevel.System, message, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Logs an assertion message and throws an error if the given condition is false.
        /// </summary>
        /// <param name="condition">The condition to check. If it is false, the game will go into an error scene and the assertion message will be logged.</param>
        /// <param name="message">The message to log if the condition is false. Default is "Assertion failed.".</param>
        /// <param name="callerFilePath">The file path of the caller. This parameter is automatically set and used for internal purposes.</param>
        /// <param name="callerLineNumber">The line number of the caller. This parameter is automatically set and used for internal purposes.</param>
        public static void LogAssertion(
        bool condition,
        string message = "Assertion failed.",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            if (condition == false)
            {
                LogInternal(LogLevel.Assert, message, callerFilePath, callerLineNumber);
                throw new Exception("Assertion failed.");
            }
        }

        /// <summary>
        /// Logs an error message and throws an exception, if provided.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        /// <param name="exception">The exception to throw, if any.</param>
        /// <param name="callerFilePath">The file path of the caller. This parameter is automatically set and used for internal purposes.</param>
        /// <param name="callerLineNumber">The line number of the caller. This parameter is automatically set and used for internal purposes.</param>
        public static void LogError(
        object message,
        Exception exception,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
        )
        
        {
            LogInternal(LogLevel.Error, message, callerFilePath, callerLineNumber);
            if (exception != null)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Logs a warning message to the console.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="callerFilePath">The file path of the caller. This parameter is automatically set and used for internal purposes.</param>
        /// <param name="callerLineNumber">The line number of the caller. This parameter is automatically set and used for internal purposes.</param>
        public static void LogWarning(
        object message,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            LogInternal(LogLevel.Warning, message, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Used to send info messages to the console.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="callerFilePath">The file path of the caller. This parameter is automatically set and used for internal purposes.</param>
        /// <param name="callerLineNumber">The line number of the caller. This parameter is automatically set and used for internal purposes.</param>
        public static void LogInfo(
        object message,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            LogInternal(LogLevel.Info, message, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Logs a message with the "Lua" level. 
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="callerFilePath">The file path of the caller. This parameter is automatically set and used for internal purposes.</param>
        /// <param name="callerLineNumber">The line number of the caller. This parameter is automatically set and used for internal purposes.</param>
        public static void LogLua(
        object message,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            LogInternal(LogLevel.Lua, message, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Logs a message with the "Debug" level. 
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="callerFilePath">The file path of the caller. This parameter is automatically set and used for internal purposes.</param>
        /// <param name="callerLineNumber">The line number of the caller. This parameter is automatically set and used for internal purposes.</param>
        public static void LogDebug(
        object message,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            LogInternal(LogLevel.Debug, message, callerFilePath, callerLineNumber);
        }


        /// <summary>
        /// Sets the level of verbosity for log messages.
        /// </summary>
        /// <param name="level">The level of verbosity to set.</param>
        /// <param name="callerFilePath">The file path of the caller. This parameter is automatically set and used for internal purposes.</param>
        /// <param name="callerLineNumber">The line number of the caller. This parameter is automatically set and used for internal purposes.</param>
        public static void SetVerbosity(
        LogLevel level,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            LogInternal(LogLevel.System, $"Current log level is {Verbosity}, setting it to {level}.", callerFilePath, callerLineNumber);
            Verbosity = level;
        }

        /// <summary>
        /// This logs the message with an unique color and identifier. This bypasses verbosity completely.
        /// </summary>
        /// <param name="logIdentifier">The identifier of the message.</param>
        /// <param name="color">The color of the message.</param>
        /// <param name="message">The actual message to log</param>
        /// <param name="callerFilePath">The file path of the caller. This parameter is automatically set and used for internal purposes.</param>
        /// <param name="callerLineNumber">The line number of the caller. This parameter is automatically set and used for internal purposes.</param>
        public static void LogUnique(
        string logIdentifier,
        ConsoleColor color,
        object message,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            LogInternalUnique(logIdentifier, color, message, callerFilePath, callerLineNumber);
        }

        private static void LogInternal(LogLevel logLevel, object message, string callerFilePath, int callerLineNumber)
        {
            if (Verbosity < logLevel)
            {
                return;
            }

            string callsite = $"{Path.GetFileName(callerFilePath)}:{callerLineNumber}";
            string dateTimeNow = DateTime.Now.ToString("HH:mm:ss");

            Console.ForegroundColor = logColors[logLevel];
            Console.WriteLine($"{logLevel}, {dateTimeNow}, {callsite}>>> {message}");
            Console.ResetColor();
            log.Add($"{dateTimeNow} [{logLevel}] {callsite}>>> {message}");
        }

        private static void LogInternalUnique(string logLevelName, ConsoleColor color, object message, string callerFilePath, int callerLineNumber)
        {
            string callsite = $"{Path.GetFileName(callerFilePath)}:{callerLineNumber}";
            string dateTimeNow = DateTime.Now.ToString("HH:mm:ss");

            Console.ForegroundColor = color;
            Console.WriteLine($"{logLevelName}, {dateTimeNow}, {callsite}>>> {message}");
            Console.ResetColor();
            log.Add($"{dateTimeNow} [{logLevelName}] {callsite}>>> {message}");
        }

        public static void DumpLogs()
        {
            StreamWriter streamWriter = new StreamWriter("logs.log");
            log.ForEach(x => streamWriter.WriteLine(x));
            streamWriter.Close();
        }

        public static void Initialize()
        {
            SetVerbosity(LogLevel.Trace);
        }
    }
}