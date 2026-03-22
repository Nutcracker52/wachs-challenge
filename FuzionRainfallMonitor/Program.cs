using FuzionRainfallMonitor.Display;
using FuzionRainfallMonitor.Services;
using FuzionRainfallMonitor.Services.Interfaces;

CsvService csvService = new CsvService();
IRainfallService rainfallService = new RainfallService();

new ConsoleUI(csvService, csvService, rainfallService).Run();