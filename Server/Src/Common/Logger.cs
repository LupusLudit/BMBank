namespace BMBank.Src.Common
{
    public enum LogType
    {
        Info,
        Warning,
        Error
    }

    /// <include file='../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="Logger"]/*'/>
    public static class Logger
    {

        private static object lockObject = new object();
        private static string logDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        static Logger()
        {
            if (!Directory.Exists(logDirectoryPath))
            {
                Directory.CreateDirectory(logDirectoryPath);
            }
        }

        /// <summary>
        /// Gets the todays log file path.
        /// </summary>
        /// <returns>Path to the log file (string).</returns>
        private static string GetTodaysFilePath()
        {
            string path = Path.Combine(logDirectoryPath, $"{DateTime.Now.ToString("yyyy-MM-dd")}.log");
            return path;
        }

        /// <summary>
        /// Writes the a passed message (with a specific type) into todays log file.
        /// </summary>
        /// <param name="type">The type of the message (LogType enum).</param>
        /// <param name="message">The specific message to be written into the log file (string).</param>
        private static void Write(LogType type, string message)
        {
            string line = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] <{type}>: {message}";
            string filePath = GetTodaysFilePath();

            lock (lockObject)
            {
                using (StreamWriter logWriter = new StreamWriter(filePath, true))
                {
                    logWriter.WriteLine(line);
                }
            }
        }

        public static void Info(string message) => Write(LogType.Info, message);
        public static void Warning(string message) => Write(LogType.Warning, message);
        public static void Error(string message) => Write(LogType.Error, message);
    }
}
