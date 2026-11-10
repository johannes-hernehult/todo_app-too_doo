using TooDoo.ConsoleUI;
using TooDoo.Models;
using TooDoo.Services;

namespace TooDoo.Views;

public class EditTasksView(TaskService taskService) : TaskListView(taskService)
{
    protected override TaskSortOrder SortOrder => TaskSortOrder.DueDate;

    protected override string Heading =>
        ShowCompleted ? "Edit completed tasks" : "Edit tasks to do";

    protected override string CommandLegend =>
        "edit <id> | delete <id> | toggle <id> (done/not done) | todo, done (switch list) | q (back)";

    protected override async Task<bool> TryHandleCommandAsync(
        string command,
        string[] arguments,
        IReadOnlyList<TaskItem> visibleTasks)
    {
        switch (command)
        {
            case "edit":
                await EditTaskAsync(arguments, visibleTasks);
                return true;

            case "delete":
                await DeleteTaskAsync(arguments, visibleTasks);
                return true;

            default:
                return false;
        }
    }

    private async Task EditTaskAsync(string[] arguments, IReadOnlyList<TaskItem> visibleTasks)
    {
        if (!TryFindTask("edit", arguments, visibleTasks, out TaskItem? task))
        {
            return;
        }

        Console.WriteLine();

        if (TaskDetailsPrompt.TryAsk(Tasks.GetProjectNames(), task, out TaskDetails? details))
        {
            await Tasks.UpdateAsync(task.Id, details);
        }
    }

    private async Task DeleteTaskAsync(string[] arguments, IReadOnlyList<TaskItem> visibleTasks)
    {
        if (!TryFindTask("delete", arguments, visibleTasks, out TaskItem? task))
        {
            return;
        }

        if (Prompts.Confirm($"Delete '{task.Title}'? This can't be undone."))
        {
            await Tasks.DeleteAsync(task.Id);
        }
    }
}
