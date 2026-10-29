Console.Clear();

using var animationToken = CassetteHeader.StartBackgroundAnimation(140);

Console.SetCursorPosition(0, 4);
Console.Write("Enter task name: ");

string task = Console.ReadLine();