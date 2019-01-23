using System;
using System.IO;
using System.Text;

namespace ENV.Debug
{
    public class Logger
    {
        public readonly string LogFilePath = null;
        public readonly DateTime LogStart = default(DateTime);

        private readonly FileStream _fileStream = null;

        public Logger(bool logToFIle = true, string logFilePath = ".")
        {
            var fileTimestamp = Timestamp(LogStart).Replace(":", "-");
            _fileStream = logToFIle
                ? File.OpenWrite(LogFilePath = Path.GetFullPath($"{logFilePath}/{fileTimestamp}.log"))
                : null;

            LogStart = DateTime.Now;
            Info("START OF LOG");
        }

        private void Log(ConsoleColor backgroundColor, ConsoleColor textColor, string message, string errorLevel)
        {
            var lastTextColor = Console.ForegroundColor;
            var lastBackgroundColor = Console.BackgroundColor;
            Console.ForegroundColor = textColor;
            Console.BackgroundColor = backgroundColor;
            message = (message.Contains("\n") ? ("\n" + message).Replace("\n", "\n| ") : ($"| {message}")) + "\n";
            string formattedMessage = $"{errorLevel.PadRight(7)} | {Timestamp()} | {DeltaTime()} {message}";
            Console.WriteLine(formattedMessage);
            if (_fileStream != null)
            {
                var buff = new UTF8Encoding().GetBytes(formattedMessage + "\n");
                _fileStream?.Write(buff, 0, buff.Length);
            }

            Console.ForegroundColor = lastTextColor;
            Console.BackgroundColor = lastBackgroundColor;
        }

        public void Info(string message) => Log(ConsoleColor.Black, ConsoleColor.White, message, "INFO");
        public void Warning(string message) => Log(ConsoleColor.Black, ConsoleColor.Yellow, message, "WARNING");
        public void Error(string message) => Log(ConsoleColor.Black, ConsoleColor.Red, message, "ERROR");

        private static string Timestamp(DateTime dateTime = default(DateTime))
        {
            if (dateTime == default(DateTime)) dateTime = DateTime.Now;
            var dy = (dateTime.Year + "").PadLeft(4, '0');
            var dm = (dateTime.Month + "").PadLeft(2, '0');
            var dd = (dateTime.Day + "").PadLeft(2, '0');
            var th = (dateTime.Hour + "").PadLeft(2, '0');
            var tm = (dateTime.Minute + "").PadLeft(2, '0');
            var ts = (dateTime.Second + "").PadLeft(2, '0');
            var tms = (dateTime.Millisecond + "").PadLeft(3, '0');
            return $"{dy}-{dm}-{dd} {th}:{tm}:{ts}.{tms}";
        }

        private string DeltaTime()
        {
            var delta = DateTime.Now - LogStart;
            var h = (delta.Hours + "").PadLeft(2, '0');
            var m = (delta.Minutes + "").PadLeft(2, '0');
            var s = (delta.Seconds + "").PadLeft(2, '0');
            var ms = (delta.Milliseconds + "").PadLeft(3, '0');
            return (delta.Days > 0 ? $"{delta.Days} days, " : "") + $"{h}:{m}:{s}.{ms}";
        }
    }
}