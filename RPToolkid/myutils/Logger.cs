namespace RPToolkid.myutils
{
    public class Logger
    {
        public static void LogInfo(string message)
        {
            Log(message, ConsoleColor.White, "[INFO]");
        }

        public static void LogError(string message)
        {
            Log(message, ConsoleColor.Red, "[ERROR]");
        }

        public static void LogSuccess(string message)
        {
            Log(message, ConsoleColor.Green, "[SUCCESS]");
        }

        public static void LogWarning(string message)
        {
            Log(message, ConsoleColor.Yellow, "[Warning]");
        }

        private static void Log(string message, ConsoleColor color, string prefix)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"{prefix} {message}");
            Console.ResetColor();
        }

        public static bool ConfirmAction(string message)
        {
            if (!string.IsNullOrEmpty(message)) LogInfo(message);
            string input = Console.ReadLine()?.Trim().ToLower();
            return input == "y";
        }
    }
}
