using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ENV.Runtime
{
    public static class App
    {
        [Test]
        public static void Test()
        {
            Init(new AppConfig(AppConfig.AppConfigLogFormat.Default, "log.log"));
            Info("Test");
            Warning("warning");
        }

        public struct AppConfig : ICloneable
        {
            public struct AppConfigLogFormat : ICloneable
            {
                public enum LogLevel
                {
                    Trace,
                    Debug,
                    Info,
                    Warning,
                    Error
                }

                public enum TraceLogMode
                {
                    NoTraceLogging,
                    LogCalling,
                    LogStackTrace
                }

                public enum AppLogSerializationMode
                {
                    ToString,
                    AppSerialization
                }

                public AppLogSerializationMode SerializationMode { get; }
                public bool LogLogLevel { get; }
                public bool LogAbsoluteDate { get; }
                public bool LogAbsoluteTime { get; }
                public bool LogTimeSinceStartup { get; }
                public bool LogTimeSinceLastLogLine { get; }
                public TraceLogMode TraceTraceLogMode { get; }
                public TraceLogMode DebugTraceLogMode { get; }
                public TraceLogMode InfoTraceLogMode { get; }
                public TraceLogMode WarningTraceLogMode { get; }
                public TraceLogMode ErrorTraceLogMode { get; }

                public AppConfigLogFormat(
                    AppLogSerializationMode serializationMode = AppLogSerializationMode.AppSerialization,
                    bool logLogLevel = true,
                    bool logAbsoluteDate = true,
                    bool logAbsoluteTime = true,
                    bool logTimeSinceStartup = true,
                    bool logTimeSinceLastLogLine = true,
                    TraceLogMode traceTraceLogMode = TraceLogMode.LogCalling,
                    TraceLogMode debugTraceLogMode = TraceLogMode.LogCalling,
                    TraceLogMode infoTraceLogMode = TraceLogMode.NoTraceLogging,
                    TraceLogMode warningTraceLogMode = TraceLogMode.LogCalling,
                    TraceLogMode errorTraceLogMode = TraceLogMode.LogStackTrace)
                {
                    SerializationMode = serializationMode;
                    LogLogLevel = logLogLevel;
                    LogAbsoluteDate = logAbsoluteDate;
                    LogAbsoluteTime = logAbsoluteTime;
                    LogTimeSinceStartup = logTimeSinceStartup;
                    LogTimeSinceLastLogLine = logTimeSinceLastLogLine;
                    TraceTraceLogMode = traceTraceLogMode;
                    DebugTraceLogMode = debugTraceLogMode;
                    InfoTraceLogMode = infoTraceLogMode;
                    WarningTraceLogMode = warningTraceLogMode;
                    ErrorTraceLogMode = errorTraceLogMode;
                }

                public object Clone() => new AppConfigLogFormat(SerializationMode, LogLogLevel, LogAbsoluteDate,
                    LogAbsoluteTime, LogTimeSinceStartup, LogTimeSinceLastLogLine, TraceTraceLogMode, DebugTraceLogMode,
                    InfoTraceLogMode, WarningTraceLogMode, ErrorTraceLogMode);

                public static AppConfigLogFormat Default =>
                    new AppConfigLogFormat(AppLogSerializationMode.AppSerialization, true, true, true, true, true,
                        TraceLogMode.LogCalling, TraceLogMode.LogCalling, TraceLogMode.NoTraceLogging,
                        TraceLogMode.LogCalling, TraceLogMode.LogStackTrace);
            }

            public AppConfigLogFormat LogFormat { get; }
            public string LogFilePath { get; }

            public AppConfig(AppConfigLogFormat logFormat, string logFilePath)
            {
                LogFormat = logFormat;
                LogFilePath = logFilePath;
            }

            public object Clone() => new AppConfig((AppConfigLogFormat) LogFormat.Clone(), LogFilePath);
        }

        private static AppConfig Config;
        private static DateTime InitTime;
        private static DateTime LastLogLineTime;
        private static FileStream LogFileStream;
        private static bool Exiting;

        public static void Init(AppConfig appConfig = new())
        {
            InitTime = DateTime.Now;
            Config = (AppConfig) appConfig.Clone();
            Exiting = false;

            if (!string.IsNullOrWhiteSpace(Config.LogFilePath)) LogFileStream = File.OpenWrite(Config.LogFilePath);

            Info("Start of log");
        }

        private static void Log(
            ConsoleColor backgroundColor,
            ConsoleColor textColor,
            object messageObject,
            AppConfig.AppConfigLogFormat.LogLevel logLevel,
            AppConfig.AppConfigLogFormat.TraceLogMode traceLogMode)
        {
            var lastTextColor = Console.ForegroundColor;
            var lastBackgroundColor = Console.BackgroundColor;
            Console.ForegroundColor = textColor;
            Console.BackgroundColor = backgroundColor;
            var message = string.Join("\n", (Config.LogFormat.SerializationMode switch
                    {
                        AppConfig.AppConfigLogFormat.AppLogSerializationMode.AppSerialization =>
                            messageObject.Serialize(),
                        _ => messageObject.ToString()
                    })
                    .Split('\n')
                    .Select(line => string.IsNullOrWhiteSpace(line) ? "" : line))
                .Trim();
            if (message.Contains("\n")) messageObject = $"\n{messageObject}";

            var now = DateTime.Now;
            var formattedMessage = "";
            if (Config.LogFormat.LogLogLevel) formattedMessage += $"{logLevel} | ";
            if (Config.LogFormat.LogAbsoluteDate) formattedMessage += $"{AbsoluteDate(now)} ";
            if (Config.LogFormat.LogAbsoluteTime) formattedMessage += $"{AbsoluteTime(now)} | ";
            if (Config.LogFormat.LogTimeSinceStartup) formattedMessage += $"{TimeSinceStartup(now)} | ";
            if (Config.LogFormat.LogTimeSinceLastLogLine) formattedMessage += $"{TimeSinceLastLine(now)} | ";
            switch (traceLogMode)
            {
                case AppConfig.AppConfigLogFormat.TraceLogMode.LogCalling:
                    formattedMessage += $"{new StackTrace().GetFrame(2)} | ";
                    break;
                case AppConfig.AppConfigLogFormat.TraceLogMode.LogStackTrace:
                    formattedMessage += $"{new StackTrace()} | ";
                    break;
            }

            formattedMessage += messageObject;
            Console.WriteLine(formattedMessage);
            if (LogFileStream != null)
            {
                var buff = new UTF8Encoding().GetBytes($"{formattedMessage}\n");
                LogFileStream?.Write(buff, 0, buff.Length);
            }

            Console.ForegroundColor = lastTextColor;
            Console.BackgroundColor = lastBackgroundColor;
        }

        private static void Log(ConsoleColor backgroundColor, ConsoleColor textColor, object messageObject,
            AppConfig.AppConfigLogFormat.LogLevel logLevel)
        {
            Log(backgroundColor,
                textColor,
                messageObject,
                logLevel,
                logLevel switch
                {
                    AppConfig.AppConfigLogFormat.LogLevel.Trace => Config.LogFormat.TraceTraceLogMode,
                    AppConfig.AppConfigLogFormat.LogLevel.Debug => Config.LogFormat.DebugTraceLogMode,
                    AppConfig.AppConfigLogFormat.LogLevel.Info => Config.LogFormat.InfoTraceLogMode,
                    AppConfig.AppConfigLogFormat.LogLevel.Warning => Config.LogFormat.WarningTraceLogMode,
                    AppConfig.AppConfigLogFormat.LogLevel.Error => Config.LogFormat.ErrorTraceLogMode
                });
        }

        public static void Trace(object message, AppConfig.AppConfigLogFormat.TraceLogMode traceLogModeOverride) => Log(
            ConsoleColor.White, ConsoleColor.Black, message, AppConfig.AppConfigLogFormat.LogLevel.Trace,
            traceLogModeOverride);

        public static void Debug(object message, AppConfig.AppConfigLogFormat.TraceLogMode traceLogModeOverride) => Log(
            ConsoleColor.Black, ConsoleColor.Green, message, AppConfig.AppConfigLogFormat.LogLevel.Debug,
            traceLogModeOverride);

        public static void Info(object message, AppConfig.AppConfigLogFormat.TraceLogMode traceLogModeOverride) => Log(
            ConsoleColor.Black, ConsoleColor.White, message, AppConfig.AppConfigLogFormat.LogLevel.Info,
            traceLogModeOverride);

        public static void Warning(object message, AppConfig.AppConfigLogFormat.TraceLogMode traceLogModeOverride) =>
            Log(ConsoleColor.Black, ConsoleColor.Yellow, message, AppConfig.AppConfigLogFormat.LogLevel.Warning,
                traceLogModeOverride);

        public static void Error(object message, AppConfig.AppConfigLogFormat.TraceLogMode traceLogModeOverride) => Log(
            ConsoleColor.Black, ConsoleColor.Red, message, AppConfig.AppConfigLogFormat.LogLevel.Error,
            traceLogModeOverride);

        public static void Trace(object message) => Log(ConsoleColor.White, ConsoleColor.Black, message,
            AppConfig.AppConfigLogFormat.LogLevel.Trace);

        public static void Debug(object message) => Log(ConsoleColor.Black, ConsoleColor.Green, message,
            AppConfig.AppConfigLogFormat.LogLevel.Debug);

        public static void Info(object message) => Log(ConsoleColor.Black, ConsoleColor.White, message,
            AppConfig.AppConfigLogFormat.LogLevel.Info);

        public static void Warning(object message) => Log(ConsoleColor.Black, ConsoleColor.Yellow, message,
            AppConfig.AppConfigLogFormat.LogLevel.Warning);

        public static void Error(object message) => Log(ConsoleColor.Black, ConsoleColor.Red, message,
            AppConfig.AppConfigLogFormat.LogLevel.Error);

        private static string AbsoluteDate(DateTime dateTime)
        {
            var dy = dateTime.Year.ToString().PadLeft(4, '0');
            var dm = dateTime.Month.ToString().PadLeft(2, '0');
            var dd = dateTime.Day.ToString().PadLeft(2, '0');
            return $"{dy}-{dm}-{dd}";
        }

        private static string AbsoluteTime(DateTime dateTime)
        {
            var th = dateTime.Hour.ToString().PadLeft(2, '0');
            var tm = dateTime.Minute.ToString().PadLeft(2, '0');
            var ts = dateTime.Second.ToString().PadLeft(2, '0');
            var tms = dateTime.Millisecond.ToString().PadLeft(3, '0');
            return $"{th}:{tm}:{ts}.{tms}";
        }

        private static string TimeSinceStartup(DateTime now) => DeltaTime(InitTime, now);

        private static string TimeSinceLastLine(DateTime now)
        {
            var result = DeltaTime(LastLogLineTime, now);
            LastLogLineTime = now;
            return result;
        }

        private static string DeltaTime(DateTime from, DateTime to)
        {
            var delta = to - from;
            var h = delta.Hours.ToString().PadLeft(2, '0');
            var m = delta.Minutes.ToString().PadLeft(2, '0');
            var s = delta.Seconds.ToString().PadLeft(2, '0');
            var ms = delta.Milliseconds.ToString().PadLeft(3, '0');
            return (delta.Days > 0 ? $"{delta.Days} days, " : "") + $"{h}:{m}:{s}.{ms}";
        }
    }
}