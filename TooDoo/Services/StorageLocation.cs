namespace TooDoo.Services;

public static class StorageLocation
{
    private const string AppFolderName = "TooDoo";
    private const string DataFileName = "tasks.json";

    public static string GetDataFilePath()
    {
        // SpecialFolderOption.Create: without it, a missing folder (e.g. a fresh ~/.config
        // on Linux) makes this return an empty string.
        string appDataFolder = Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData,
            Environment.SpecialFolderOption.Create);

        return Path.Combine(appDataFolder, AppFolderName, DataFileName);
    }

    /// <summary>
    /// Earlier versions saved tasks inside the project folder (Data/tasks.json).
    /// Copies that file to the new location if the new file doesn't exist yet.
    /// The old file is left in place.
    /// </summary>
    public static void MigrateLegacyFile(string dataFilePath)
    {
        string legacyPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "Data", DataFileName));

        if (File.Exists(dataFilePath) || !File.Exists(legacyPath))
        {
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(dataFilePath)!);
        File.Copy(legacyPath, dataFilePath);
    }
}
