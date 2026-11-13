using System.Security.Cryptography;
using TooDoo.Models;

namespace TooDoo.Services;

/// <summary>
/// Owns all task operations. Every change is saved immediately; if saving fails,
/// the change stays in memory and <see cref="SaveFailed"/> is raised.
/// </summary>
public class TaskService(TaskRepository repository)
{
    // No 0/O or 1/I, so IDs are easy to read and type.
    private const string IdAlphabet = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";
    private const int IdLength = 4;
    private static readonly int MaxTaskCount = (int)Math.Pow(IdAlphabet.Length, IdLength);

    private readonly List<TaskItem> _tasks = [];
    private bool _hasUnsavedChanges;

    public event Action<string>? SaveFailed;

    public int TotalCount => _tasks.Count;

    public int DoneCount => _tasks.Count(task => task.IsDone);

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        List<TaskItem> loadedTasks = await repository.LoadAsync(cancellationToken);
        EnsureUniqueIds(loadedTasks);

        _tasks.Clear();
        _tasks.AddRange(loadedTasks);
    }

    public IReadOnlyList<TaskItem> GetTasks(bool isDone, TaskSortOrder sortOrder)
    {
        IEnumerable<TaskItem> tasks = _tasks.Where(task => task.IsDone == isDone);

        IOrderedEnumerable<TaskItem> orderedTasks = sortOrder switch
        {
            TaskSortOrder.Project => tasks
                .OrderBy(task => task.Project == "-")
                .ThenBy(task => task.Project, StringComparer.OrdinalIgnoreCase)
                .ThenBy(task => task.DueDate)
                .ThenBy(task => task.Title, StringComparer.OrdinalIgnoreCase),
            _ => tasks
                .OrderBy(task => task.DueDate)
                .ThenBy(task => task.Project, StringComparer.OrdinalIgnoreCase)
                .ThenBy(task => task.Title, StringComparer.OrdinalIgnoreCase)
        };

        return orderedTasks.ToList();
    }

    public IReadOnlyList<string> GetProjectNames()
    {
        return _tasks
            .Select(task => task.Project)
            .OfType<string>()
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task<TaskItem> AddAsync(TaskDetails details)
    {
        var task = new TaskItem(
            GenerateUniqueId(),
            details.Title,
            details.DueDate,
            details.Project);

        _tasks.Add(task);
        await SaveAsync();
        return task;
    }

    /// <exception cref="KeyNotFoundException">No task has this ID.</exception>
    public async Task UpdateAsync(string id, TaskDetails details)
    {
        GetById(id).Update(details);
        await SaveAsync();
    }

    /// <exception cref="KeyNotFoundException">No task has this ID.</exception>
    public async Task ToggleDoneAsync(string id)
    {
        GetById(id).ToggleDone();
        await SaveAsync();
    }

    /// <exception cref="KeyNotFoundException">No task has this ID.</exception>
    public async Task DeleteAsync(string id)
    {
        _tasks.Remove(GetById(id));
        await SaveAsync();
    }

    /// <summary>Retries saving if an earlier save failed.</summary>
    public async Task SaveIfNeededAsync()
    {
        if (_hasUnsavedChanges)
        {
            await SaveAsync();
        }
    }

    private async Task SaveAsync()
    {
        _hasUnsavedChanges = true;

        try
        {
            await repository.SaveAsync(_tasks);
            _hasUnsavedChanges = false;
        }
        catch (Exception ex) when (TaskRepository.IsStorageError(ex))
        {
            SaveFailed?.Invoke(ex.Message);
        }
    }

    private TaskItem GetById(string id)
    {
        return _tasks.FirstOrDefault(task => HasId(task, id))
            ?? throw new KeyNotFoundException($"No task with ID '{id}'.");
    }

    private string GenerateUniqueId()
    {
        if (_tasks.Count >= MaxTaskCount)
        {
            throw new InvalidOperationException("No task IDs are available.");
        }

        string id;
        do
        {
            id = RandomNumberGenerator.GetString(IdAlphabet, IdLength);
        } while (_tasks.Any(task => HasId(task, id)));

        return id;
    }

    private static bool HasId(TaskItem task, string id)
    {
        return task.Id.Equals(id, StringComparison.OrdinalIgnoreCase);
    }

    private static void EnsureUniqueIds(IEnumerable<TaskItem> tasks)
    {
        string? duplicateId = tasks
            .GroupBy(task => task.Id, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(group => group.Count() > 1)
            ?.Key;

        if (duplicateId is not null)
        {
            throw new InvalidDataException(
                $"The task file contains the task ID '{duplicateId}' more than once.");
        }
    }
}