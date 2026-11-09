using System.Security.Cryptography;
using TooDoo.Models;
using TaskStatus = TooDoo.Models.TaskStatus;

namespace TooDoo.Services;

public class TaskService
{
    private readonly List<TaskItem> _tasks = new();

    public IReadOnlyList<TaskItem> GetTasks()
    {
        return _tasks.AsReadOnly();
    }

    public TaskItem AddTask(
        string title,
        DateOnly dueDate,
        string? project,
        TaskStatus status = TaskStatus.NotDone)
    {
        var task = new TaskItem(
            GenerateUniqueId(),
            title,
            dueDate,
            project,
            status);

        _tasks.Add(task);
        return task;
    }

    private string GenerateUniqueId()
    {
        const string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        if (_tasks.Count >= 36 * 36 * 36 * 36 * 36)
        {
            throw new InvalidOperationException("No task IDs are available.");
        }

        string id;
        do
        {
            id = string.Create(5, alphabet, static (characters, chars) =>
            {
                for (int i = 0; i < characters.Length; i++)
                {
                    characters[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
                }
            });
        } while (_tasks.Any(task => string.Equals(task.Id, id, StringComparison.Ordinal)));

        return id;
    }
}