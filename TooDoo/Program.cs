using TooDoo;
using TooDoo.ConsoleUI;
using TooDoo.Services;

Console.WriteLine("TooDoo - Your personal todo console app");

var taskManager = new TaskService();

var app = new App(taskManager);
app.Run();