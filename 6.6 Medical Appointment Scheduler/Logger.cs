using System;
using System.IO;
using System.Text;

namespace MedScheduler
{
    public static class Logger
    {
        private static readonly object _gate = new();
        private static readonly string _path = "scheduler.log";

        public static void Info(string message) => Write("INFO", message);
        public static void Warn(string message) => Write("WARN", message);
        public static void Error(string message) => Write("ERROR", message);

        private static void Write(string level, string msg)
        {
            var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {level}: {msg}";

            lock (_gate)
            {
                try
                {
                    using var sw = new StreamWriter(_path, true, Encoding.UTF8);
                    sw.WriteLine(line);
                }
                catch
                {
                    Console.Error.WriteLine("LOGGING FAILURE: " + line);
                }
            }
        }
    }
}
