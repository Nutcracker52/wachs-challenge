using FuzionRainfallMonitor.Models;

namespace FuzionRainfallMonitor.Services.Interfaces
{
    public interface IRainfallService
    {
        List<DeviceReport> GenerateReports(
            List<Device> devices,
            List<RainfallReading> readings
        );
    }
}