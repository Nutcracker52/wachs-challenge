using CsvHelper.Configuration.Attributes;

namespace FuzionRainfallMonitor.Models
{
    public class Device
    {
        [Name("Device ID")]
        public int DeviceId { get; set; }

        [Name("Device Name")]
        public string DeviceName { get; set; } = string.Empty;

        [Name("Location")]
        public string Location { get; set; } = string.Empty;
    }
}