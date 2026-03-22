using FuzionRainfallMonitor.Models;

namespace FuzionRainfallMonitor.Display
{
    public static class ConsoleDisplay
    {
        public static void ShowDevicesSummary(List<Device> devices)
        {
            Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine($"{"Device ID",-12} | {"Device Name",-15} | {"Location",-20}");
            Console.WriteLine("------------------------------------------------------------------------------------------");

            if (devices == null || devices.Count == 0)
            {
                Console.WriteLine("  No devices found.");
            }
            else
            {
                foreach (var device in devices)
                    Console.WriteLine($"{device.DeviceId,-12} | {device.DeviceName,-15} | {device.Location,-20}");
            }

            Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine($"  Total devices loaded: {devices?.Count ?? 0}");
            Console.WriteLine("------------------------------------------------------------------------------------------");
        }

        public static void ShowReadingFilesSummary(List<(string FileName, int Count)> fileSummaries, int totalCount)
        {
            Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine($"{"File",-30} | {"Records Loaded",14}");
            Console.WriteLine("------------------------------------------------------------------------------------------");

            if (fileSummaries == null || fileSummaries.Count == 0)
            {
                Console.WriteLine("  No reading files found.");
            }
            else
            {
                foreach (var (fileName, count) in fileSummaries)
                    Console.WriteLine($"{fileName,-30} | {count,14}");
            }

            Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine($"  Total valid readings loaded: {totalCount}");
            Console.WriteLine("------------------------------------------------------------------------------------------");
        }

        public static void ShowDeviceReports(List<DeviceReport> reports)
        {
            Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine($"{"Device",-15} | {"Location",-15} | {"Avg (mm)",8} | {"Status",-6} | {"Trend",-11} | {"Readings",8}");
            Console.WriteLine("------------------------------------------------------------------------------------------");

            if (reports == null || reports.Count == 0)
            {
                Console.WriteLine("  No reports to display.");
            }
            else
            {
                foreach (var report in reports)
                {
                    // Print everything up to Status
                    var avgDisplay = report.Status == RainfallStatus.NoData
                        ? "     N/A"
                        : $"{report.AverageRainfallMm,8:F2}";

                    Console.Write($"{report.DeviceName,-15} | {report.Location,-15} | {avgDisplay} | ");

                    // Colour coded status
                    Console.ForegroundColor = report.Status switch
                    {
                        RainfallStatus.Green => ConsoleColor.Green,
                        RainfallStatus.Amber => ConsoleColor.Yellow,
                        RainfallStatus.Red => ConsoleColor.Red,
                        RainfallStatus.NoData => ConsoleColor.DarkGray,
                        _ => ConsoleColor.White
                    };
                    Console.Write($"{report.Status,-6}");
                    Console.ResetColor();

                    // Trend with arrow
                    var trendArrow = report.Trend switch
                    {
                        RainfallTrend.Increasing => "Rising",
                        RainfallTrend.Decreasing => "Falling",
                        RainfallTrend.Stable => "Stable",
                        RainfallTrend.Insufficient => "Unknown",
                        _ => "-"
                    };

                    Console.WriteLine($" | {trendArrow,-11} | {report.ReadingCount,8}");
                }
            }

            Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine($"  Total devices reported: {reports?.Count ?? 0}");
            Console.WriteLine("------------------------------------------------------------------------------------------");
        }
    }
}