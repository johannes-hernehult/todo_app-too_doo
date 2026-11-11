using TooDoo;
using TooDoo.ConsoleUI;
using TooDoo.Services;

string dataFilePath = StorageLocation.GetDataFilePath();
var repository = new TaskRepository(dataFilePath);
var taskService = new TaskService(repository);

if (!await TryLoadTasksAsync())
{
    Environment.ExitCode = 1;
    return;
}

await new App(taskService).RunAsync();

async Task<bool> TryLoadTasksAsync()
{
    try
    {
        await taskService.LoadAsync();
        return true;
    }
    catch (InvalidDataException ex)
    {
        return TryStartWithEmptyList(ex);
    }
    catch (Exception ex) when (TaskRepository.IsStorageError(ex))
    {
        ReportStartupFailure(ex);
        return false;
    }
}

// A corrupt file is moved aside (never overwritten) so the user can recover it.
bool TryStartWithEmptyList(InvalidDataException problem)
{
    try
    {
        string backupPath = repository.BackUpUnreadableFile();
        Prompts.Pause(
            $"{problem.Message}{Environment.NewLine}" +
            $"The file was moved to: {backupPath}{Environment.NewLine}" +
            "Starting with an empty task list.");
        return true;
    }
    catch (Exception ex) when (TaskRepository.IsStorageError(ex))
    {
        ReportStartupFailure(ex);
        return false;
    }
}

void ReportStartupFailure(Exception exception)
{
    Console.WriteLine($"Could not read the task file at {dataFilePath}: {exception.Message}");
    Console.WriteLine("Your data was not changed. Fix the problem and start TooDoo again.");
}
