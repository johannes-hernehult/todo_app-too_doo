using TooDoo;
using TooDoo.Services;

Console.WriteLine("TooDoo - Your personal todo console app");

var taskManager = new TaskManager();
var userInput = new UserInput();

var app = new App(taskManager, userInput);
app.Run();