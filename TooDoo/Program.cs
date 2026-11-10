using System.Text.Json;
using TooDoo;
using TooDoo.Services;

Console.WriteLine("TooDoo - Your personal todo console app");

var projectDirectory = Path.GetFullPath(
    Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
var dataFilePath = Path.Combine(projectDirectory, "Data", "tasks.json");

var taskManager = new TaskService();
var fileService = new FileService();
try
{
    var savedTasks = await fileService.LoadAsync(dataFilePath);

    if (savedTasks is not null)
    {
        taskManager.ReplaceTasks(savedTasks);
    }
}
catch (Exception ex) when (
    ex is JsonException or IOException or UnauthorizedAccessException)
{
    Console.WriteLine($"Could not load saved tasks: {ex.Message}");
    Console.WriteLine("Starting with an empty task list.");
}

var app = new App(taskManager, fileService, dataFilePath);
await app.RunAsync();