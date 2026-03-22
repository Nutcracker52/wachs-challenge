using FuzionRainfallMonitor.Display;
using FuzionRainfallMonitor.Helpers;
using FuzionRainfallMonitor.Services;
using FuzionRainfallMonitor.Services.Interfaces;
using FuzionRainfallMonitor.Tests;

ICsvService csvService = new CsvService();
IRainfallService rainfallService = new RainfallService();

// ─── SMOKE TESTS ──────────────────────────────────────────
SmokeTests.Run(csvService);



// ─── CSV LOAD ─────────────────────────────────────────────
Console.WriteLine("----- Fuzion Rainfall Monitor — CSV Load -----");

// Load devices
var devices = csvService.ReadDevices(PathHelper.GetDataFilePath("Devices.csv"));
ConsoleDisplay.ShowDevicesSummary(devices);

// Load all reading files — track per-file counts for display
var (allReadings, fileSummaries) = csvService.LoadAllReadings(PathHelper.GetAllReadingFiles());

// Display all readings summary table
ConsoleDisplay.ShowReadingFilesSummary(fileSummaries, allReadings.Count);

// ─── GENERATE REPORTS ─────────────────────────────────────
Console.WriteLine("----- Fuzion Rainfall Monitor — Generate Reports -----");

var reports = rainfallService.GenerateReports(devices, allReadings);

Console.WriteLine();
Console.WriteLine("=== Rainfall Status Report ===");
Console.WriteLine();

ConsoleDisplay.ShowDeviceReports(reports);
