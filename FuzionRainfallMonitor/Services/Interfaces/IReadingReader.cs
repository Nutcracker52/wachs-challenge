using FuzionRainfallMonitor.Models;

namespace FuzionRainfallMonitor.Services.Interfaces
{
    public interface IReadingReader
    {
        List<RainfallReading> ReadRainfallReadings(string filePath);
        (List<RainfallReading> Readings, List<(string FileName, int Count)> Summaries) LoadAllReadings(string[] filePaths);
    }
}