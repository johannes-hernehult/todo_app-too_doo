using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

public static class CassetteHeader
{
    private static readonly string Amber = "\x1b[38;2;255;176;0m";  // CRT Amber
    private static readonly string Dim   = "\x1b[38;2;120;90;0m";   // Dim border
    private static readonly string Bold  = "\x1b[1m";
    private static readonly string Reset = "\x1b[0m";

    private static readonly string[] Spokes = { "/", "-", "\\", "|" };
    private static int _frame = 0;

    static CassetteHeader()
    {
        EnableAnsiColors();
    }

    /// <summary>
    /// Renders a single frame of the cassette header at the specified screen position.
    /// </summary>
    public static void Render(int left = 0, int top = 0)
    {
        Console.SetCursorPosition(left, top);

        string s = Spokes[_frame % Spokes.Length];

        Console.WriteLine($"{Dim}  .-------.   ---------------------------{Reset}");
        Console.WriteLine($"{Dim}  |{Amber}({s}){Dim} {Amber}({s}){Dim}|   {Bold}{Amber}TOODOO - Rewind your tasks.{Reset}");
        Console.WriteLine($"{Dim}  '-------'   ---------------------------{Reset}");

        _frame++;
    }

    /// <summary>
    /// Starts non-blocking background animation at the top of the console.
    /// Returns a CancellationTokenSource to stop it.
    /// </summary>
    public static CancellationTokenSource StartBackgroundAnimation(int intervalMs = 140)
    {
        var cts = new CancellationTokenSource();
        Task.Run(async () =>
        {
            while (!cts.Token.IsCancellationRequested)
            {
                lock (Console.Out)
                {
                    int currentLeft = Console.CursorLeft;
                    int currentTop = Console.CursorTop;

                    Render(0, 0);

                    // Restore cursor so user input stays where it belongs
                    Console.SetCursorPosition(currentLeft, currentTop);
                }

                try
                {
                    await Task.Delay(intervalMs, cts.Token);
                }
                catch (TaskCanceledException) { break; }
            }
        }, cts.Token);

        return cts;
    }

    private static void EnableAnsiColors()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var handle = GetStdHandle(-11);
            if (GetConsoleMode(handle, out uint mode))
            {
                SetConsoleMode(handle, mode | 0x0004);
            }
        }
    }

    [DllImport("kernel32.dll")] private static extern IntPtr GetStdHandle(int n);
    [DllImport("kernel32.dll")] private static extern bool GetConsoleMode(IntPtr h, out uint m);
    [DllImport("kernel32.dll")] private static extern bool SetConsoleMode(IntPtr h, uint m);
}