using FuzionRainfallMonitor.Display;
using FuzionRainfallMonitor.Helpers;
using FuzionRainfallMonitor.Models;
using FuzionRainfallMonitor.Services;
using FuzionRainfallMonitor.Services.Interfaces;


Console.WriteLine("----- Fuzion Rainfall Monitor — Smoke Test -----");
Console.WriteLine();

// 1. Test PathHelper
AppLogger.LogInfo("Testing PathHelper");
Console.WriteLine($"  Project Data Folder: {PathHelper.DataFolder}");

var readingFiles = PathHelper.GetAllReadingFiles();
Console.WriteLine($"  Reading files found: {readingFiles.Length}");
foreach (var file in readingFiles)
    Console.WriteLine($"    → {Path.GetFileName(file)}");

Console.WriteLine();

// 2. Testing Models can be instantiated
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

Console.WriteLine();

// 3. Testing AppLogger levels
AppLogger.LogInfo("Testing AppLogger levels");
AppLogger.LogInfo("This is an INFO message");
AppLogger.LogWarning("This is a WARN message");
AppLogger.LogError("This is an ERROR message");

AppLogger.LogInfo("\n  Smoke test complete — all helpers and models OK!");

// 4. Testing CSV loading

ICsvService csvService = new CsvService();

// Load devices
var devices = csvService.ReadDevices(PathHelper.GetDataFilePath("Devices.csv"));
ConsoleDisplay.ShowDevicesSummary(devices);

// Load all reading files — track per-file counts for display
var allReadings = new List<RainfallReading>();
var fileSummaries = new List<(string FileName, int Count)>();

foreach (var file in PathHelper.GetAllReadingFiles())
{
    var readings = csvService.ReadRainfallReadings(file);
    allReadings.AddRange(readings);
    fileSummaries.Add((Path.GetFileName(file), readings.Count));
}

// Display all readings summary table
ConsoleDisplay.ShowReadingFilesSummary(fileSummaries, allReadings.Count);
