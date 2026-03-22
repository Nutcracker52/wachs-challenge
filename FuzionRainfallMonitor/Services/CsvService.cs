using CsvHelper;
using CsvHelper.Configuration;
using FuzionRainfallMonitor.Helpers;
using FuzionRainfallMonitor.Models;
using FuzionRainfallMonitor.Services.Interfaces;
using System.Globalization;

namespace FuzionRainfallMonitor.Services
{
    public class CsvService : ICsvService
    {
        public List<Device> ReadDevices(string filePath)
        {
            AppLogger.LogInfo($"Reading devices from: {Path.GetFileName(filePath)}");

            if (!IsValidFile(filePath))
                return new List<Device>();

            try
            {
                using var reader = new StreamReader(filePath);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var devices = csv.GetRecords<Device>().ToList();
                AppLogger.LogInfo($"Loaded {devices.Count} devices");
                return devices;
            }
            catch (HeaderValidationException ex)
            {
                AppLogger.LogError("Devices CSV headers do not match expected format", ex);
                return new List<Device>();
            }
            catch (Exception ex)
            {
                AppLogger.LogError("Unexpected error reading devices CSV", ex);
                return new List<Device>();
            }
        }

        public List<RainfallReading> ReadRainfallReadings(string filePath)
        {
            AppLogger.LogInfo($"Reading readings from: {Path.GetFileName(filePath)}");

            if (!IsValidFile(filePath))
                return new List<RainfallReading>();

            var validReadings = new List<RainfallReading>();

            try
            {
                // BadDataFound — silently skip malformed rows
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    BadDataFound = null
                };

                using var reader = new StreamReader(filePath);
                using var csv = new CsvReader(reader, config);

                // Read row by row to handle the errors and capture valid readings
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    var reading = ParseReading(csv);
                    if (reading != null)
                        validReadings.Add(reading);
                }

                AppLogger.LogInfo($"Loaded {validReadings.Count} valid readings from {Path.GetFileName(filePath)}");
            }
            catch (HeaderValidationException ex)
            {
                AppLogger.LogError("Readings CSV headers do not match expected format", ex);
            }
            catch (Exception ex)
            {
                AppLogger.LogError("Unexpected error reading readings CSV", ex);
            }

            return validReadings;
        }

        // ─── Private Helpers ──────────────────────────────────────

        private static RainfallReading? ParseReading(CsvReader csv)
        {
            // 1. Parse Device ID
            if (!int.TryParse(csv.GetField("Device ID"), out var deviceId))
            {
                AppLogger.LogWarning($"Skipping row — invalid Device ID: '{csv.GetField("Device ID")}'");
                return null;
            }

            // 2. Parse Timestamp — reject impossible dates like year 3030
            if (!DateTime.TryParseExact(
                    csv.GetField("Time"),
                    "dd/MM/yyyy H:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var timestamp))
            {
                AppLogger.LogWarning($"Skipping row — invalid timestamp: '{csv.GetField("Time")}'");
                return null;
            }

            if (timestamp.Year > DateTime.Now.Year + 10)
            {
                AppLogger.LogWarning($"Skipping row — suspicious future timestamp: {timestamp:yyyy-MM-dd} for Device {deviceId}");
                return null;
            }

            // 3. Parse Rainfall — reject values like "2k4", "2ff"
            var rainfallRaw = csv.GetField("Rainfall") ?? string.Empty;
            if (!double.TryParse(rainfallRaw, NumberStyles.Any, CultureInfo.InvariantCulture, out var rainfallMm))
            {
                AppLogger.LogWarning($"Skipping row — invalid rainfall value: '{rainfallRaw}' for Device {deviceId}");
                return null;
            }

            return new RainfallReading
            {
                DeviceId = deviceId,
                Timestamp = timestamp,
                RainfallRaw = rainfallRaw,
                RainfallMm = rainfallMm
            };
        }

        private static bool IsValidFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                AppLogger.LogError($"File not found: {filePath}");
                return false;
            }

            if (Path.GetExtension(filePath).ToLower() != ".csv")
            {
                AppLogger.LogError($"Invalid file type: {Path.GetExtension(filePath)}");
                return false;
            }

            if (new FileInfo(filePath).Length == 0)
            {
                AppLogger.LogWarning($"File is empty: {filePath}");
                return false;
            }

            return true;
        }
    }
}