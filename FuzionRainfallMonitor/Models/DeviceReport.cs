namespace FuzionRainfallMonitor.Models
{
    public enum RainfallStatus { Green, Amber, Red, NoData }
    public enum RainfallTrend { Increasing, Decreasing, Stable, Insufficient }

    public class DeviceReport
    {
        public int DeviceId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public double AverageRainfallMm { get; set; }
        public RainfallStatus Status { get; set; }
        public RainfallTrend Trend { get; set; }
        public int ReadingCount { get; set; }
    }
}