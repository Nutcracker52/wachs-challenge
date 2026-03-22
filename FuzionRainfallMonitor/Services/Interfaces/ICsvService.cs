using FuzionRainfallMonitor.Models;

namespace FuzionRainfallMonitor.Services.Interfaces
{
    public interface ICsvService
    {
        List<Device> ReadDevices(string filePath);
        List<RainfallReading> ReadRainfallReadings(string filePath);
    }
}