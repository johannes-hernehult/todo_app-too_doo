using System.Text.Json;
using System.Text.Json.Serialization;
using TooDoo.Models;

namespace TooDoo.Services;

/// <summary>Reads and writes tasks as JSON.</summary>
public class TaskRepository(string filePath)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly string _filePath = Path.GetFullPath(filePath);

    public static bool IsStorageError(Exception exception)
    {
        return exception is IOException or UnauthorizedAccessException;
    }

    /// <returns>The saved tasks, or an empty list if there is no file yet.</returns>
    /// <exception cref="InvalidDataException">The file exists but its contents are invalid.</exception>
    public async Task<List<TaskItem>> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        List<StoredTask?>? storedTasks;

        try
        {
            await using FileStream stream = File.OpenRead(_filePath);
            storedTasks = await JsonSerializer.DeserializeAsync<List<StoredTask?>>(
                stream, JsonOptions, cancellationToken);
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException("The task file is damaged or not in the expected format.", ex);
        }

        if (storedTasks is null)
        {
            throw new InvalidDataException("The task file is empty.");
        }

        return storedTasks.Select(StoredTask.ToTaskItem).ToList();
    }

    public async Task SaveAsync(
        IReadOnlyList<TaskItem> tasks,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);

        // Write to a temp file first so a crash mid-write can't corrupt the real file.
        string temporaryPath = $"{_filePath}.{Guid.NewGuid():N}.tmp";

        try
        {
            await using (FileStream stream = File.Create(temporaryPath))
            {
                List<StoredTask> storedTasks = tasks.Select(StoredTask.FromTaskItem).ToList();
                await JsonSerializer.SerializeAsync(stream, storedTasks, JsonOptions, cancellationToken);
            }

            File.Move(temporaryPath, _filePath, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    /// <summary>Moves the task file out of the way so it is never overwritten.</summary>
    /// <returns>The path the file was moved to.</returns>
    public string BackUpUnreadableFile()
    {
        string backupPath = $"{_filePath}.corrupt-{DateTime.Now:yyyyMMdd-HHmmss-fff}";
        File.Move(_filePath, backupPath);
        return backupPath;
    }

    /// <summary>
    /// The on-disk format, kept separate from <see cref="TaskItem"/> so the file layout
    /// can change independently. Also reads the old format, which stored a numeric
    /// "Status" (0 = done, 1 = not done) instead of "IsDone".
    /// </summary>
    private sealed record StoredTask(
        string Id,
        string Title,
        DateOnly DueDate,
        string? Project,
        bool? IsDone = null,
        [property: JsonPropertyName("Status")] int? LegacyStatus = null)
    {
        private const int LegacyDoneStatus = 0;

        public static StoredTask FromTaskItem(TaskItem task)
        {
            return new StoredTask(task.Id, task.Title, task.DueDate, task.Project, task.IsDone);
        }

        public static TaskItem ToTaskItem(StoredTask? stored)
        {
            if (stored is null)
            {
                throw new InvalidDataException("The task file contains an empty entry.");
            }

            try
            {
                bool isDone = stored.IsDone ?? (stored.LegacyStatus == LegacyDoneStatus);
                return new TaskItem(stored.Id, stored.Title, stored.DueDate, stored.Project, isDone);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidDataException(
                    $"The task file contains an invalid task: {ex.Message}", ex);
            }
        }
    }
}
