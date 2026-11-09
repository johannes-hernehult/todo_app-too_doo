using System.Globalization;
using TooDoo.Models;
using TaskStatus = TooDoo.Models.TaskStatus;

namespace TooDoo.ConsoleUI;

[Flags]
public enum TaskColumns
{
    None = 0,
    Id = 1,
    Title = 2,
    DueDate = 4,
    Project = 8,
    Status = 16,
    All = Id | Title | DueDate | Project | Status
}

public static class UI
{
    public static string RenderMenu(string title, List<string> items)
    {
        Console.Clear();
        UI.RenderList(title, items);

        return UserInput.Prompt("Pick an option: ");
    }


    public static void RenderHeading(string title)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(new string('-', Console.WindowWidth));
        Console.WriteLine(title);
        Console.WriteLine(new string('-', Console.WindowWidth));
        Console.ResetColor();
    }

    public static void RenderList(string title, IReadOnlyList<string> items)
    {
        Console.Clear();
        Console.WriteLine(title);

        foreach (string item in items)
        {
            Console.WriteLine(item);
        }
    }

    public static void RenderTasks(List<TaskItem> tasks, TaskColumns columnsToShow = TaskColumns.All)
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks yet.");
            return;
        }

        List<TaskColumn> columns = CreateTaskColumns(columnsToShow);

        if (columns.Count == 0)
        {
            Console.WriteLine("No columns selected.");
            return;
        }

        RenderHeader(columns);
        RenderRows(tasks, columns);
    }

    private static List<TaskColumn> CreateTaskColumns(TaskColumns columnsToShow)
    {
        var columns = new List<TaskColumn>();

        if ((columnsToShow & TaskColumns.Id) != 0)
            columns.Add(new("ID", 6, task => task.Id.ToString()));

        if ((columnsToShow & TaskColumns.Title) != 0)
            columns.Add(new("Title", 56, task => task.Title));

        if ((columnsToShow & TaskColumns.DueDate) != 0)
            columns.Add(new("Due", 12, task => task.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));

        if ((columnsToShow & TaskColumns.Project) != 0)
            columns.Add(new("Project", 12, task => task.Project ?? "-"));

        if ((columnsToShow & TaskColumns.Status) != 0)
            columns.Add(new("Status", 10, task => task.Status.ToString()));

        return columns;
    }

    private static void RenderHeader(List<TaskColumn> columns)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(string.Join("", columns.Select(column => column.Header.PadRight(column.Width))));
        Console.ResetColor();
    }

    private static void RenderRows(List<TaskItem> tasks, List<TaskColumn> columns)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);

        for (int i = 0; i < tasks.Count; i++)
        {
            RenderRow(tasks[i], columns, today, hasAlternateBackground: i % 2 == 1);
        }
    }

    private static void RenderRow(
        TaskItem task,
        List<TaskColumn> columns,
        DateOnly today,
        bool hasAlternateBackground)
    {
        string row = string.Join("", columns.Select(column =>
            FitColumnValue(column.Value(task), column.Width)));

        if (hasAlternateBackground)
            Console.Write("\u001b[48;2;40;40;40m");

        if (task.Status == TaskStatus.Done)
            Console.ForegroundColor = ConsoleColor.DarkGray;
        else
            Console.Write(GetDueDateColor(task.DueDate, today));

        Console.WriteLine(row);
        Console.Write("\u001b[0m");
    }

    private static string FitColumnValue(string value, int width)
    {
        if (value.Length > width)
            value = value[..(width - 2)].TrimEnd() + "… ";

        return value.PadRight(width);
    }

    private static string GetDueDateColor(DateOnly dueDate, DateOnly today)
    {
        int daysOverdue = today.DayNumber - dueDate.DayNumber;

        switch (daysOverdue)
        {
            case < 0:
                return ""; // Future: no color change

            case 0:
                return "\u001b[38;2;100;200;120m"; // Today: green

            case 1:
                return "\u001b[38;2;230;145;70m"; // Yesterday: orange

            default:
                return "\u001b[38;2;220;90;90m"; // More than 1 day overdue: red
        }
    }

    private sealed record TaskColumn(
        string Header,
        int Width,
        Func<TaskItem, string> Value);
}