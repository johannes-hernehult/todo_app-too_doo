using System.Diagnostics.CodeAnalysis;
using TooDoo.ConsoleUI;
using TooDoo.Models;

namespace TooDoo.Views;

/// <summary>Asks for a task's details. Used for both adding and editing.</summary>
public static class TaskDetailsPrompt
{
    /// <param name="existingProjects">Shown as suggestions before the project prompt.</param>
    /// <param name="existingTask">When editing, its values are offered as defaults.</param>
    /// <returns>False if the user cancelled.</returns>
    public static bool TryAsk(
        IReadOnlyList<string> existingProjects,
        TaskItem? existingTask,
        [NotNullWhen(true)] out TaskDetails? details)
    {
        details = null;

        if (!Prompts.TryAskText("Title", existingTask?.Title, out string? title))
        {
            return false;
        }

        if (!Prompts.TryAskDate("Due date", existingTask?.DueDate, out DateOnly dueDate))
        {
            return false;
        }

        WriteExistingProjects(existingProjects);

        if (!Prompts.TryAskOptionalText("Project", existingTask?.Project, out string? project))
        {
            return false;
        }

        details = new TaskDetails(title, dueDate, project);
        return true;
    }

    private static void WriteExistingProjects(IReadOnlyList<string> projects)
    {
        if (projects.Count == 0)
        {
            return;
        }

        string names = Colors.Paint(string.Join(", ", projects), Colors.Accent);
        Console.WriteLine($"Existing projects: {names}");
    }
}
