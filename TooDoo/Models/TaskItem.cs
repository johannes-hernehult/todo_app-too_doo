using TooDoo.ConsoleUI;

namespace TooDoo.Models;

public enum TaskStatus
{
    Done,
    NotDone
}

public class TaskItem
{
    public string Id { get; }
    public string Title { get; private set; }
    public DateOnly DueDate { get; private set; }
    public string? Project { get; private set; }
    public TaskStatus Status { get; private set; }

    public TaskItem(
        string id,
        string title,
        DateOnly dueDate,
        string? project,
        TaskStatus status = TaskStatus.NotDone)
    {
        ValidateString(id, nameof(id));
        ValidateString(title, nameof(title));
        ValidateDueDate(dueDate, nameof(dueDate));
        ValidateStatus(status, nameof(status));

        Id = id;
        Title = title;
        DueDate = dueDate;
        Project = NormalizeProject(project);
        Status = status;
    }

    public void UpdateTitle(string title)
    {
        ValidateString(title, nameof(title));
        Title = title;
    }

    public void UpdateDueDate(DateOnly date)
    {
        ValidateDueDate(date, nameof(date));
        DueDate = date;
    }

    public void UpdateProject(string project)
    {
        if (string.IsNullOrWhiteSpace(project))
        {
            throw new ArgumentException("Project name is required.", nameof(project));
        }

        Project = project;
    }

    public void ClearProject()
    {
        Project = null;
    }

    public static string RenderProjectNames(IEnumerable<TaskItem> taskItems)
    {
        return string.Join(", ",
            taskItems
                .Select(task => task.Project)
                .Where(project => !string.IsNullOrWhiteSpace(project))
                .Select(project => project!)
                .Distinct(StringComparer.OrdinalIgnoreCase));
    }

    public void UpdateStatus(TaskStatus status)
    {
        ValidateStatus(status, nameof(status));
        Status = status;
    }

    private static void ValidateString(string? value, string parameterName)
    {
        string? error = Validate.GetError(DataType.String, value);

        if (error is not null)
        {
            throw new ArgumentException(error, parameterName);
        }
    }

    private static void ValidateDueDate(DateOnly date, string parameterName)
    {
        if (date == default)
        {
            throw new ArgumentException("A due date is required.", parameterName);
        }
    }

    private static void ValidateStatus(TaskStatus status, string parameterName)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                status,
                "The task status is not valid.");
        }
    }

    private static string? NormalizeProject(string? project)
    {
        if (string.IsNullOrWhiteSpace(project))
        {
            return null;
        }

        return project;
    }
}