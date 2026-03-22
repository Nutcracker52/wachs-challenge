namespace FuzionRainfallMonitor.Helpers
{
    public static class PathHelper
    {
        private static readonly string ProjectRoot = GetProjectRoot();

        private static string GetProjectRoot()
        {
            var current = Directory.GetCurrentDirectory();
            var dir = new DirectoryInfo(current);

            while (dir != null && !dir.GetFiles("*.csproj").Any())
            {
                dir = dir.Parent;
            }

            return dir?.FullName ?? current;
        }

        public static string DataFolder => Path.Combine(ProjectRoot, "Data");

        public static string GetDataFilePath(string fileName)
        {
            return Path.Combine(DataFolder, fileName);
        }

        public static string[] GetAllReadingFiles()
        {
            return Directory.GetFiles(DataFolder, "*.csv")
                .Where(f => !Path.GetFileName(f)
                .Equals("Devices.csv", StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }
    }
}