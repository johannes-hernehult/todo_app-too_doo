using TooDoo.ConsoleUI;
using TooDoo.Models;
using TooDoo.Services;

namespace TooDoo.Views;

public class AddTaskView(TaskService taskService)
{
    public async Task ShowAsync()
    {
        List<TaskItem> addedTasks = [];

        while (true)
        {
            Console.Clear();
            ConsoleMenu.WriteHeading("Add task");

            if (!TaskDetailsPrompt.TryAsk(taskService.GetProjectNames(), existingTask: null, out TaskDetails? details))
            {
                return;
            }

            addedTasks.Add(await taskService.AddAsync(details));

            Console.Clear();
            ConsoleMenu.WriteHeading("Added this session");
            TaskTableRenderer.Render(
                addedTasks,
                TaskColumns.Title | TaskColumns.DueDate | TaskColumns.Project,
                emptyMessage: "No tasks added yet.");
            Console.WriteLine();

            if (!AskToAddAnother())
            {
                return;
            }
        }
    }

    private static bool AskToAddAnother()
    {
        while (true)
        {
            string input = Prompts.ReadLine("Add another task? (a to add, q to go back): ")
                .ToLowerInvariant();

            if (input is "a")
            {
                return true;
            }

            if (input is "q")
            {
                return false;
            }

            Prompts.ShowError("Enter 'a' to add another task or 'q' to go back.");
        }
    }
}
