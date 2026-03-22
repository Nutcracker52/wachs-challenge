using FuzionRainfallMonitor.Helpers;
using FuzionRainfallMonitor.Services.Interfaces;
using FuzionRainfallMonitor.Tests;

namespace FuzionRainfallMonitor.Display
{
    public class ConsoleUI
    {
        private readonly IDeviceReader _deviceReader;
        private readonly IReadingReader _readingReader;
        private readonly IRainfallService _rainfallService;

        public ConsoleUI(IDeviceReader deviceReader, IReadingReader readingReader, IRainfallService rainfallService)
        {
            _deviceReader = deviceReader;
            _readingReader = readingReader;
            _rainfallService = rainfallService;
        }

        public void Run()
        {
            ConsoleMenu.ShowWelcome();

            var running = true;

            while (running)
            {
                ConsoleMenu.ShowMainMenu();

                var input = Console.ReadLine()?.Trim();

                switch (input)
                {
                    case "1":
                        RunSmokeTests();
                        break;

                    case "2":
                        LoadAndShowDevices();
                        break;

                    case "3":
                        GenerateRainfallReport();
                        break;

                    case "4":
                        running = false;
                        ConsoleMenu.ShowGoodbye();
                        break;

                    default:
                        AppLogger.LogWarning("Invalid option — please enter 1, 2, 3 or 4");
                        break;
                }

                if (running)
                {
                    Console.WriteLine();
                    Console.WriteLine("  Press any key to return to menu...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        // ─── Menu Actions ─────────────────────────────────────────

        private void RunSmokeTests()
        {
            Console.Clear();
            SmokeTests.Run(_deviceReader, _readingReader);
        }

        private void LoadAndShowDevices()
        {
            Console.Clear();
            Console.WriteLine("----- Fuzion Rainfall Monitor — CSV Load -----");
            Console.WriteLine();

            var devices = _deviceReader.ReadDevices(PathHelper.GetDataFilePath("Devices.csv"));
            ConsoleDisplay.ShowDevicesSummary(devices);

            Console.WriteLine();

            var (allReadings, fileSummaries) = _readingReader.LoadAllReadings(PathHelper.GetAllReadingFiles());
            ConsoleDisplay.ShowReadingFilesSummary(fileSummaries, allReadings.Count);
        }

        private void GenerateRainfallReport()
        {
            Console.Clear();
            Console.WriteLine("----- Fuzion Rainfall Monitor — Generate Reports -----");
            Console.WriteLine();

            var devices = _deviceReader.ReadDevices(PathHelper.GetDataFilePath("Devices.csv"));
            var (allReadings, _) = _readingReader.LoadAllReadings(PathHelper.GetAllReadingFiles());
            var reports = _rainfallService.GenerateReports(devices, allReadings);

            Console.WriteLine();
            Console.WriteLine("=== Rainfall Status Report ===");
            Console.WriteLine();

            ConsoleDisplay.ShowDeviceReports(reports);
        }
    }
}