namespace TooDoo.Models;

public sealed class TaskItem
{
    public TaskItem(
        string id,
        string title,
        DateOnly dueDate,
        string? project = null,
        bool isDone = false)
    {
        Id = RequireText(id, nameof(id));
        Title = RequireText(title, nameof(title));
        DueDate = RequireDueDate(dueDate);
        Project = NormalizeProject(project);
        IsDone = isDone;
    }

    public string Id { get; }
    public string Title { get; private set; }
    public DateOnly DueDate { get; private set; }
    public string? Project { get; private set; }
    public bool IsDone { get; private set; }

    public void Update(TaskDetails details)
    {
        // Validate everything first so a bad value never leaves the task half-updated.
        string title = RequireText(details.Title, nameof(details.Title));
        DateOnly dueDate = RequireDueDate(details.DueDate);

        Title = title;
        DueDate = dueDate;
        Project = NormalizeProject(details.Project);
    }

    public void ToggleDone() => IsDone = !IsDone;

    private static string RequireText(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"The {name} can't be empty.");
        }

        return value.Trim();
    }

    private static DateOnly RequireDueDate(DateOnly dueDate)
    {
        if (dueDate == default)
        {
            throw new ArgumentException("A due date is required.");
        }

        return dueDate;
    }

    private static string? NormalizeProject(string? project)
    {
        return string.IsNullOrWhiteSpace(project) ? null : project.Trim();
    }
}
