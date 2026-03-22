using CsvHelper.Configuration.Attributes;

namespace FuzionRainfallMonitor.Models
{
    public class RainfallReading
    {
        [Name("Device ID")]
        public int DeviceId { get; set; }

        [Name("Time")]
        public DateTime Timestamp { get; set; }

        // Raw string so we can validate it ourselves
        [Name("Rainfall")]
        public string RainfallRaw { get; set; } = string.Empty;

        // Parsed value — null means invalid
        [Ignore]
        public double? RainfallMm { get; set; }
    }
}