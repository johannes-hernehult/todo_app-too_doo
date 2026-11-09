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

    public TaskItem(string id, string title, DateOnly dueDate, string? project,
        TaskStatus status = TaskStatus.NotDone)
    {
        Id = id;
        Title = title;
        DueDate = dueDate;
        Project = project;
        Status = status;
    }

    public void UpdateTitle(string title)
    {
        this.Title = title;
    }

    public void UpdateDueDate(DateOnly date)
    {
        DueDate = date;
    }

    public void UpdateProject(string? project)
    {
        Project = project;
    }

    public void UpdateStatus(TaskStatus status)
    {
        Status = status;
    }
}