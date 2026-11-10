using System.Globalization;
using TooDoo.Models;

namespace TooDoo.ConsoleUI;

[Flags]
public enum TaskColumns
{
    None = 0,
    Id = 1,
    Title = 2,
    DueDate = 4,
    Project = 8,
    All = Id | Title | DueDate | Project
}

public static class TaskTableRenderer
{
    private const int ColumnGap = 1;

    public static void Render(
        IReadOnlyList<TaskItem> tasks,
        TaskColumns columnsToShow,
        string emptyMessage)
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine(emptyMessage);
            return;
        }

        IReadOnlyList<TaskColumn> columns = CreateColumns(columnsToShow);
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);

        WriteHeaderRow(columns);

        for (int i = 0; i < tasks.Count; i++)
        {
            bool isAlternateRow = i % 2 == 1;
            WriteTaskRow(tasks[i], columns, today, isAlternateRow);
        }
    }

    private static List<TaskColumn> CreateColumns(TaskColumns columnsToShow)
    {
        var columns = new List<TaskColumn>();

        if (columnsToShow.HasFlag(TaskColumns.Id))
            columns.Add(new("ID", 6, task => task.Id));

        if (columnsToShow.HasFlag(TaskColumns.Title))
            columns.Add(new("Title", 56, task => task.Title));

        if (columnsToShow.HasFlag(TaskColumns.DueDate))
            columns.Add(new("Due", 12, task => task.DueDate.ToString(ConsoleFormats.DateFormat, CultureInfo.InvariantCulture)));

        if (columnsToShow.HasFlag(TaskColumns.Project))
            columns.Add(new("Project", 16, task => task.Project ?? "-"));

        return columns;
    }

    private static void WriteHeaderRow(IReadOnlyList<TaskColumn> columns)
    {
        string header = string.Concat(columns.Select(column => column.Header.PadRight(column.Width)));
        Console.WriteLine(Colors.Paint(header, Colors.Neutral));
    }

    private static void WriteTaskRow(
        TaskItem task,
        IReadOnlyList<TaskColumn> columns,
        DateOnly today,
        bool isAlternateRow)
    {
        string row = string.Concat(columns.Select(column =>
            FitToColumn(column.GetValue(task), column.Width)));

        string background = isAlternateRow ? Colors.AlternateRowBackground : string.Empty;
        string foreground = task.IsDone ? Colors.Muted : GetDueDateColor(task.DueDate, today);

        Console.WriteLine($"{background}{foreground}{row}{Colors.Reset}");
    }

    /// <summary>Truncates with an ellipsis and always leaves a gap before the next column.</summary>
    private static string FitToColumn(string value, int width)
    {
        int textWidth = width - ColumnGap;

        string fitted = value.Length <= textWidth
            ? value
            : value[..(textWidth - 1)].TrimEnd() + "…";

        return fitted.PadRight(width);
    }

    private static string GetDueDateColor(DateOnly dueDate, DateOnly today)
    {
        int daysPastDue = today.DayNumber - dueDate.DayNumber;

        return daysPastDue switch
        {
            < 0 => string.Empty,
            0 => Colors.DueToday,
            1 => Colors.DueYesterday,
            _ => Colors.Overdue
        };
    }

    private sealed record TaskColumn(string Header, int Width, Func<TaskItem, string> GetValue);
}
