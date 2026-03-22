FUZION INC — FLOOD DETECTION RAINFALL MONITOR
==============================================
Author:     Kasun Hathurusinghe
Date:       22 March 2026
Repository: https://github.com/Nutcracker52/wachs-challenge

OVERVIEW
--------
A C# .NET 10 console application that reads rainfall data from field
devices and displays a colour-coded status report for each device.

HOW TO RUN
----------
1. Open FuzionRainfallMonitor.sln in Visual Studio 2026
2. Ensure Data/ folder contains Devices.csv and reading CSV files
3. Press F5 to run
4. Use the menu to navigate

TOOLS USED
----------
- Visual Studio 2026
- .NET 10
- CsvHelper by Josh Close (https://joshclose.github.io/CsvHelper/)
- Git + GitHub
- https://learn.microsoft.com

ASSUMPTIONS
-----------
- Last timestamp across all files is treated as current time
- 4-hour window is calculated backwards from that timestamp
- Timestamps parsed as dd/MM/yyyy H:mm (South African locale)
- All devices are in South Africa (SAST UTC+2)
- Timezone does not affect results as timestamps are compared relatively
- Any CSV in Data/ folder except Devices.csv is treated as a readings file
- Device ID 10506 appears twice in Devices.csv — treated as a data issue
- Unknown Device IDs in readings are ignored and logged as warnings
- Invalid values like "2k4" and "2ff" are skipped and logged as warnings
- Impossible timestamps like year 3030 are skipped and logged as warnings
- Devices with no readings in the 4-hour window show as NoData in report