using FuzionRainfallMonitor.Display;
using FuzionRainfallMonitor.Helpers;
using FuzionRainfallMonitor.Models;
using FuzionRainfallMonitor.Services;
using FuzionRainfallMonitor.Services.Interfaces;

namespace FuzionRainfallMonitor.Tests
{
    public static class SmokeTests
    {
        public static void Run(IDeviceReader deviceReader, IReadingReader readingReader)
        {
            TestPathHelper();
            TestModels();
            TestAppLogger();
            TestConsoleDisplayEmptyGuards();
            TestCsvService(deviceReader, readingReader);
            TestRainfallServiceStatus();
            TestRainfallServiceTrend();
            TestRainfallServiceWindow();
            TestRainfallServiceEdgeCases();
        }

        // ─── 1. PathHelper ────────────────────────────────────────
        private static void TestPathHelper()
        {
            Console.WriteLine("----- Test: PathHelper -----");
            Console.WriteLine();

            AppLogger.LogInfo("Testing PathHelper");
            Console.WriteLine($"  Project Data Folder: {PathHelper.DataFolder}");

            var readingFiles = PathHelper.GetAllReadingFiles();
            Console.WriteLine($"  Reading files found: {readingFiles.Length}");
            foreach (var file in readingFiles)
                Console.WriteLine($"    → {Path.GetFileName(file)}");

            Console.WriteLine($"  Devices file path: {PathHelper.GetDataFilePath("Devices.csv")}");

            Console.WriteLine();
        }

        // ─── 2. Models ────────────────────────────────────────────
        private static void TestModels()
        {
            Console.WriteLine("----- Test: Models -----");
            Console.WriteLine();

            AppLogger.LogInfo("Testing Models...");

            var testDevice = new Device
            {
                DeviceId = 10451,
                DeviceName = "Gauge 1",
                Location = "Biyamiti"
            };
            Console.WriteLine($"  Device: [{testDevice.DeviceId}] {testDevice.DeviceName} @ {testDevice.Location}");

            var testReading = new RainfallReading
            {
                DeviceId = 10451,
                Timestamp = DateTime.Now,
                RainfallRaw = "8.5",
                RainfallMm = 8.5
            };
            Console.WriteLine($"  Reading: Device {testReading.DeviceId} | {testReading.Timestamp:yyyy-MM-dd HH:mm} | {testReading.RainfallMm}mm");

            var testReport = new DeviceReport
            {
                DeviceId = 10451,
                DeviceName = "Gauge 1",
                Location = "Biyamiti",
                AverageRainfallMm = 12.4,
                Status = RainfallStatus.Amber,
                Trend = RainfallTrend.Increasing,
                ReadingCount = 8
            };
            Console.WriteLine($"  Report: [{testReport.Status}] {testReport.DeviceName} | Avg: {testReport.AverageRainfallMm}mm | Trend: {testReport.Trend}");

            var noDataReport = new DeviceReport
            {
                DeviceId = 70060,
                DeviceName = "Gauge 5",
                Location = "Mopani",
                AverageRainfallMm = -1,
                Status = RainfallStatus.NoData,
                Trend = RainfallTrend.Insufficient,
                ReadingCount = 0
            };
            Console.WriteLine($"  NoData Report: [{noDataReport.Status}] {noDataReport.DeviceName} | Readings: {noDataReport.ReadingCount}");

            Console.WriteLine();
        }

        // ─── 3. AppLogger ─────────────────────────────────────────
        private static void TestAppLogger()
        {
            Console.WriteLine("----- Test: AppLogger -----");
            Console.WriteLine();

            AppLogger.LogInfo("Testing AppLogger levels");
            AppLogger.LogInfo("This is an INFO message");
            AppLogger.LogWarning("This is a WARN message");
            AppLogger.LogError("This is an ERROR message");

            AppLogger.LogInfo("Smoke test complete — all helpers and models OK!");

            Console.WriteLine();
        }

        // ─── 4. ConsoleDisplay Empty Guards ───────────────────────
        private static void TestConsoleDisplayEmptyGuards()
        {
            Console.WriteLine("----- Test: ConsoleDisplay Empty Guards -----");
            Console.WriteLine();

            AppLogger.LogInfo("Testing ConsoleDisplay empty guards...");
            ConsoleDisplay.ShowDevicesSummary(new List<Device>());
            ConsoleDisplay.ShowReadingFilesSummary(new List<(string, int)>(), 0);
            ConsoleDisplay.ShowDeviceReports(new List<DeviceReport>());

            Console.WriteLine();
        }

        // ─── 5. CsvService ────────────────────────────────────────
        private static void TestCsvService(IDeviceReader deviceReader, IReadingReader readingReader)
        {
            Console.WriteLine("----- Test: CsvService -----");
            Console.WriteLine();

            // Missing file guard
            var missingDevices = deviceReader.ReadDevices(PathHelper.GetDataFilePath("nonexistent.csv"));
            AssertTest("Missing file returns empty list", missingDevices.Count == 0);

            var missingReadings = readingReader.ReadRainfallReadings(PathHelper.GetDataFilePath("nonexistent.csv"));
            AssertTest("Missing readings file returns empty list", missingReadings.Count == 0);

            // Real data loads correctly
            var realDevices = deviceReader.ReadDevices(PathHelper.GetDataFilePath("Devices.csv"));
            AssertTest("Devices.csv loads 8 devices", realDevices.Count == 8);

            // Dirty data filtered
            var (realReadings, _) = readingReader.LoadAllReadings(PathHelper.GetAllReadingFiles());
            AssertTest("Readings loaded from all files", realReadings.Count > 0);
            AssertTest("Dirty data filtered (2k4, 2ff, year 3030 skipped)", realReadings.All(r => r.RainfallMm.HasValue));

            Console.WriteLine();
        }

        // ─── 6. RainfallService — Status ──────────────────────────
        private static void TestRainfallServiceStatus()
        {
            Console.WriteLine("----- Test: RainfallService Status Thresholds -----");
            Console.WriteLine();

            IRainfallService rainfallService = new RainfallService();
            var baseTime = new DateTime(2020, 6, 5, 14, 0, 0);

            var devices = new List<Device>
            {
                new Device { DeviceId = 1, DeviceName = "Test Green",     Location = "Zone A" },
                new Device { DeviceId = 2, DeviceName = "Test Amber",     Location = "Zone B" },
                new Device { DeviceId = 3, DeviceName = "Test Red Avg",   Location = "Zone C" },
                new Device { DeviceId = 4, DeviceName = "Test Red Spike", Location = "Zone D" },
            };

            var readings = new List<RainfallReading>
            {
                // Device 1 — Green (avg 5mm)
                new RainfallReading { DeviceId = 1, Timestamp = baseTime.AddHours(-3), RainfallMm = 4 },
                new RainfallReading { DeviceId = 1, Timestamp = baseTime.AddHours(-2), RainfallMm = 5 },
                new RainfallReading { DeviceId = 1, Timestamp = baseTime.AddHours(-1), RainfallMm = 6 },
                new RainfallReading { DeviceId = 1, Timestamp = baseTime,              RainfallMm = 5 },

                // Device 2 — Amber (avg 12mm)
                new RainfallReading { DeviceId = 2, Timestamp = baseTime.AddHours(-3), RainfallMm = 10 },
                new RainfallReading { DeviceId = 2, Timestamp = baseTime.AddHours(-2), RainfallMm = 11 },
                new RainfallReading { DeviceId = 2, Timestamp = baseTime.AddHours(-1), RainfallMm = 13 },
                new RainfallReading { DeviceId = 2, Timestamp = baseTime,              RainfallMm = 14 },

                // Device 3 — Red avg (avg 16mm)
                new RainfallReading { DeviceId = 3, Timestamp = baseTime.AddHours(-3), RainfallMm = 15 },
                new RainfallReading { DeviceId = 3, Timestamp = baseTime.AddHours(-2), RainfallMm = 16 },
                new RainfallReading { DeviceId = 3, Timestamp = baseTime.AddHours(-1), RainfallMm = 17 },
                new RainfallReading { DeviceId = 3, Timestamp = baseTime,              RainfallMm = 16 },

                // Device 4 — Red spike (avg 8mm but one reading > 30)
                new RainfallReading { DeviceId = 4, Timestamp = baseTime.AddHours(-3), RainfallMm = 5  },
                new RainfallReading { DeviceId = 4, Timestamp = baseTime.AddHours(-2), RainfallMm = 5  },
                new RainfallReading { DeviceId = 4, Timestamp = baseTime.AddHours(-1), RainfallMm = 31 },
                new RainfallReading { DeviceId = 4, Timestamp = baseTime,              RainfallMm = 5  },
            };

            var reports = rainfallService.GenerateReports(devices, readings);

            AssertTest("Green status (avg 5mm)", GetStatus(reports, 1) == RainfallStatus.Green);
            AssertTest("Amber status (avg 12mm)", GetStatus(reports, 2) == RainfallStatus.Amber);
            AssertTest("Red status (avg 16mm)", GetStatus(reports, 3) == RainfallStatus.Red);
            AssertTest("Red status (spike > 30mm)", GetStatus(reports, 4) == RainfallStatus.Red);

            Console.WriteLine();
        }

        // ─── 7. RainfallService — Trend ───────────────────────────
        private static void TestRainfallServiceTrend()
        {
            Console.WriteLine("----- Test: RainfallService Trend Calculation -----");
            Console.WriteLine();

            IRainfallService rainfallService = new RainfallService();
            var baseTime = new DateTime(2020, 6, 5, 14, 0, 0);

            var devices = new List<Device>
            {
                new Device { DeviceId = 1, DeviceName = "Test Increasing",   Location = "Zone A" },
                new Device { DeviceId = 2, DeviceName = "Test Decreasing",   Location = "Zone B" },
                new Device { DeviceId = 3, DeviceName = "Test Stable",       Location = "Zone C" },
                new Device { DeviceId = 4, DeviceName = "Test Insufficient", Location = "Zone D" },
            };

            var readings = new List<RainfallReading>
            {
                // Device 1 — Increasing (2, 4, 6, 8)
                new RainfallReading { DeviceId = 1, Timestamp = baseTime.AddHours(-3), RainfallMm = 2 },
                new RainfallReading { DeviceId = 1, Timestamp = baseTime.AddHours(-2), RainfallMm = 4 },
                new RainfallReading { DeviceId = 1, Timestamp = baseTime.AddHours(-1), RainfallMm = 6 },
                new RainfallReading { DeviceId = 1, Timestamp = baseTime,              RainfallMm = 8 },

                // Device 2 — Decreasing (8, 6, 4, 2)
                new RainfallReading { DeviceId = 2, Timestamp = baseTime.AddHours(-3), RainfallMm = 8 },
                new RainfallReading { DeviceId = 2, Timestamp = baseTime.AddHours(-2), RainfallMm = 6 },
                new RainfallReading { DeviceId = 2, Timestamp = baseTime.AddHours(-1), RainfallMm = 4 },
                new RainfallReading { DeviceId = 2, Timestamp = baseTime,              RainfallMm = 2 },

                // Device 3 — Stable (5, 5, 5, 5)
                new RainfallReading { DeviceId = 3, Timestamp = baseTime.AddHours(-3), RainfallMm = 5 },
                new RainfallReading { DeviceId = 3, Timestamp = baseTime.AddHours(-2), RainfallMm = 5 },
                new RainfallReading { DeviceId = 3, Timestamp = baseTime.AddHours(-1), RainfallMm = 5 },
                new RainfallReading { DeviceId = 3, Timestamp = baseTime,              RainfallMm = 5 },

                // Device 4 — Insufficient (only 1 reading)
                new RainfallReading { DeviceId = 4, Timestamp = baseTime, RainfallMm = 5 },
            };

            var reports = rainfallService.GenerateReports(devices, readings);

            AssertTest("Increasing trend", GetTrend(reports, 1) == RainfallTrend.Increasing);
            AssertTest("Decreasing trend", GetTrend(reports, 2) == RainfallTrend.Decreasing);
            AssertTest("Stable trend", GetTrend(reports, 3) == RainfallTrend.Stable);
            AssertTest("Insufficient trend (1 reading)", GetTrend(reports, 4) == RainfallTrend.Insufficient);

            Console.WriteLine();
        }

        // ─── 8. RainfallService — Window ──────────────────────────
        private static void TestRainfallServiceWindow()
        {
            Console.WriteLine("----- Test: RainfallService 4hr Window -----");
            Console.WriteLine();

            IRainfallService rainfallService = new RainfallService();
            var baseTime = new DateTime(2020, 6, 5, 14, 0, 0);

            var devices = new List<Device>
            {
                new Device { DeviceId = 1, DeviceName = "Test Window", Location = "Zone A" },
            };

            var readings = new List<RainfallReading>
            {
                // Inside window — should be included
                new RainfallReading { DeviceId = 1, Timestamp = baseTime.AddHours(-4), RainfallMm = 5 },
                new RainfallReading { DeviceId = 1, Timestamp = baseTime.AddHours(-3), RainfallMm = 5 },
                new RainfallReading { DeviceId = 1, Timestamp = baseTime.AddHours(-2), RainfallMm = 5 },
                new RainfallReading { DeviceId = 1, Timestamp = baseTime.AddHours(-1), RainfallMm = 5 },
                new RainfallReading { DeviceId = 1, Timestamp = baseTime,              RainfallMm = 5 },

                // Outside window — should be excluded
                new RainfallReading { DeviceId = 1, Timestamp = baseTime.AddHours(-5), RainfallMm = 99 },
                new RainfallReading { DeviceId = 1, Timestamp = baseTime.AddHours(-6), RainfallMm = 99 },
            };

            var reports = rainfallService.GenerateReports(devices, readings);

            AssertTest("4hr window excludes old readings", GetCount(reports, 1) == 5);
            AssertTest("Outside readings do not affect avg", GetAverage(reports, 1) == 5.0);

            Console.WriteLine();
        }

        // ─── 9. RainfallService — Edge Cases ──────────────────────
        private static void TestRainfallServiceEdgeCases()
        {
            Console.WriteLine("----- Test: RainfallService Edge Cases -----");
            Console.WriteLine();

            IRainfallService rainfallService = new RainfallService();
            var baseTime = new DateTime(2020, 6, 5, 14, 0, 0);

            var devices = new List<Device>
            {
                new Device { DeviceId = 1, DeviceName = "Test NoData",  Location = "Zone A" },
                new Device { DeviceId = 2, DeviceName = "Test Unknown", Location = "Zone B" },
            };

            var readings = new List<RainfallReading>
            {
                // Device 1 — No readings in window (NoData)
                new RainfallReading { DeviceId = 1, Timestamp = baseTime.AddHours(-10), RainfallMm = 5 },

                // Unknown device ID — should warn and skip
                new RainfallReading { DeviceId = 999, Timestamp = baseTime, RainfallMm = 10 },
            };

            // Empty lists guard
            var emptyReports = rainfallService.GenerateReports(new List<Device>(), new List<RainfallReading>());
            AssertTest("Empty devices returns empty reports", emptyReports.Count == 0);

            var reports = rainfallService.GenerateReports(devices, readings);
            AssertTest("Device with no readings in window → NoData", GetStatus(reports, 1) == RainfallStatus.NoData);
            AssertTest("All devices appear in report", reports.Count == 2);

            Console.WriteLine();
        }

        // ─── Private Helpers ──────────────────────────────────────

        private static void AssertTest(string testName, bool passed)
        {
            Console.ForegroundColor = passed ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"  [{(passed ? "PASS" : "FAIL")}] {testName}");
            Console.ResetColor();
        }

        private static RainfallStatus GetStatus(List<DeviceReport> reports, int deviceId) =>
            reports.First(r => r.DeviceId == deviceId).Status;

        private static RainfallTrend GetTrend(List<DeviceReport> reports, int deviceId) =>
            reports.First(r => r.DeviceId == deviceId).Trend;

        private static int GetCount(List<DeviceReport> reports, int deviceId) =>
            reports.First(r => r.DeviceId == deviceId).ReadingCount;

        private static double GetAverage(List<DeviceReport> reports, int deviceId) =>
            reports.First(r => r.DeviceId == deviceId).AverageRainfallMm;
    }
}