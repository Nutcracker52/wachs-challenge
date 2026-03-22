namespace FuzionRainfallMonitor.Helpers
{
    public static class AppLogger
    {
        public static void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  [INFO]  {message}");
            Console.ResetColor();
        }

        public static void LogWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  [WARN]  {message}");
            Console.ResetColor();
        }

        public static void LogError(string message, Exception? ex = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  [ERROR] {message}");
            if (ex != null)
                Console.WriteLine($"          {ex.Message}");
            Console.ResetColor();
        }
    }
}