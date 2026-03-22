using FuzionRainfallMonitor.Models;

namespace FuzionRainfallMonitor.Display
{
    public static class ConsoleDisplay
    {
        public static void ShowDevicesSummary(List<Device> devices)
        {
            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine($"{"Device ID",-12} | {"Device Name",-15} | {"Location",-20}");
            Console.WriteLine("---------------------------------------------------------------");

            foreach (var device in devices)
                Console.WriteLine($"{device.DeviceId,-12} | {device.DeviceName,-15} | {device.Location,-20}");

            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine($"  Total devices loaded: {devices.Count}");
            Console.WriteLine("---------------------------------------------------------------");
        }

        public static void ShowReadingFilesSummary(List<(string FileName, int Count)> fileSummaries, int totalCount)
        {
            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine($"{"File",-30} | {"Records Loaded",14}");
            Console.WriteLine("---------------------------------------------------------------");

            foreach (var (fileName, count) in fileSummaries)
                Console.WriteLine($"{fileName,-30} | {count,14}");

            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine($"  Total valid readings loaded: {totalCount}");
            Console.WriteLine("---------------------------------------------------------------");
        }
    }
}