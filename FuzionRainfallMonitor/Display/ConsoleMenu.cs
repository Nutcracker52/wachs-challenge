namespace FuzionRainfallMonitor.Display
{
    public static class ConsoleMenu
    {
        public static void ShowWelcome()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==========================================================================================");
            Console.WriteLine("            FUZION INC — FLOOD DETECTION RAINFALL MONITOR");
            Console.WriteLine("==========================================================================================");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("  Monitors rainfall readings from field devices across Kruger National Park, South Africa.");
            Console.WriteLine("  Provides real-time status and trend analysis based on the last 4 hours of data.");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==========================================================================================");
            Console.ResetColor();
            Console.WriteLine();
        }

        public static void ShowMainMenu()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  What would you like to do?");
            Console.WriteLine("  ------------------------------------------------------------------------------------------");
            Console.ResetColor();
            Console.WriteLine("  [1]  Load devices and reading files");
            Console.WriteLine("  [2]  Generate rainfall status report");
            Console.WriteLine("  [3]  Run system tests");
            Console.WriteLine("  [Q]  Quit");
            Console.WriteLine();
            Console.Write("  Enter your choice: ");
        }

        public static void ShowGoodbye()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==========================================================================================");
            Console.WriteLine("            Thank you for using Fuzion Rainfall Monitor. Goodbye!");
            Console.WriteLine("==========================================================================================");
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}