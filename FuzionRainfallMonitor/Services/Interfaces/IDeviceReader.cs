using FuzionRainfallMonitor.Models;

namespace FuzionRainfallMonitor.Services.Interfaces
{
    public interface IDeviceReader
    {
        List<Device> ReadDevices(string filePath);
    }
}