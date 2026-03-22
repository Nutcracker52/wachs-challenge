using FuzionRainfallMonitor.Helpers;
using FuzionRainfallMonitor.Models;
using FuzionRainfallMonitor.Services.Interfaces;

namespace FuzionRainfallMonitor.Services
{
    public class RainfallService : IRainfallService
    {
        private const int WindowHours = 4;
        private const double GreenThreshold = 10.0;
        private const double AmberThreshold = 15.0;
        private const double RedSpikeThreshold = 30.0;

        public List<DeviceReport> GenerateReports(
            List<Device> devices,
            List<RainfallReading> readings)
        {
            // Guard against empty data
            if (devices == null || devices.Count == 0)
            {
                AppLogger.LogError("No devices found — cannot generate reports");
                return new List<DeviceReport>();
            }

            if (readings == null || readings.Count == 0)
            {
                AppLogger.LogError("No readings found — cannot generate reports");
                return new List<DeviceReport>();
            }
            // Step 1 — determine "current time" from the data
            var currentTime = readings.Max(r => r.Timestamp);
            var windowStart = currentTime.AddHours(-WindowHours);

            AppLogger.LogInfo($"Current time (from data): {currentTime:dd/MM/yyyy HH:mm}");
            AppLogger.LogInfo($"4hr window start:         {windowStart:dd/MM/yyyy HH:mm}");

            // Step 2 — filter readings to the 4hr window only
            var windowReadings = readings
                .Where(r => r.Timestamp >= windowStart && r.Timestamp <= currentTime)
                .ToList();

            // Step 3 — warn about readings from unknown devices
            var knownDeviceIds = devices
                .Select(d => d.DeviceId)
                .ToHashSet();

            var unknownIds = windowReadings
                .Select(r => r.DeviceId)
                .Distinct()
                .Where(id => !knownDeviceIds.Contains(id))
                .ToList();

            foreach (var id in unknownIds)
                AppLogger.LogWarning($"Readings found for unknown Device ID: {id} — skipping");

            // Step 4 — warn about duplicate device IDs in devices list
            var duplicateDeviceIds = devices
                .GroupBy(d => d.DeviceId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            foreach (var id in duplicateDeviceIds)
                AppLogger.LogWarning($"Duplicate Device ID found: {id} — readings will be shared across both devices");

            // Step 5 — generate one report per device
            var reports = new List<DeviceReport>();

            foreach (var device in devices)
            {
                var deviceReadings = windowReadings
                    .Where(r => r.DeviceId == device.DeviceId)
                    .OrderBy(r => r.Timestamp)
                    .ToList();

                if (deviceReadings.Count == 0)
                {
                    AppLogger.LogWarning($"No readings in 4hr window for [{device.DeviceId}] {device.DeviceName} — reporting as No Data");
                    reports.Add(new DeviceReport
                    {
                        DeviceId = device.DeviceId,
                        DeviceName = device.DeviceName,
                        Location = device.Location,
                        AverageRainfallMm = -1,
                        Status = RainfallStatus.NoData,
                        Trend = RainfallTrend.Insufficient,
                        ReadingCount = 0
                    });
                    continue;
                }

                var average = deviceReadings.Average(r => r.RainfallMm!.Value);
                var status = DetermineStatus(average, deviceReadings);
                var trend = DetermineTrend(deviceReadings);

                reports.Add(new DeviceReport
                {
                    DeviceId = device.DeviceId,
                    DeviceName = device.DeviceName,
                    Location = device.Location,
                    AverageRainfallMm = Math.Round(average, 2),
                    Status = status,
                    Trend = trend,
                    ReadingCount = deviceReadings.Count
                });
            }

            return reports;
        }

        // ─── Private Helpers ──────────────────────────────────────

        private static RainfallStatus DetermineStatus(
            double average,
            List<RainfallReading> deviceReadings)
        {
            // Red if ANY single reading exceeds 30mm
            var hasSpike = deviceReadings
                .Any(r => r.RainfallMm!.Value > RedSpikeThreshold);

            if (hasSpike || average >= AmberThreshold)
                return RainfallStatus.Red;

            if (average >= GreenThreshold)
                return RainfallStatus.Amber;

            return RainfallStatus.Green;
        }

        private static RainfallTrend DetermineTrend(List<RainfallReading> deviceReadings)
        {
            // Minimum 2 readings to determine a trend
            if (deviceReadings.Count < 2)
                return RainfallTrend.Insufficient;

            // Split into first half and second half, compare averages
            var midpoint = deviceReadings.Count / 2;

            var firstHalfAvg = deviceReadings
                .Take(midpoint)
                .Average(r => r.RainfallMm!.Value);

            var secondHalfAvg = deviceReadings
                .Skip(midpoint)
                .Average(r => r.RainfallMm!.Value);

            if (secondHalfAvg > firstHalfAvg)
                return RainfallTrend.Increasing;

            if (secondHalfAvg < firstHalfAvg)
                return RainfallTrend.Decreasing;

            return RainfallTrend.Stable;
        }
    }
}