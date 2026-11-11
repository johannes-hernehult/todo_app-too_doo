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
}
