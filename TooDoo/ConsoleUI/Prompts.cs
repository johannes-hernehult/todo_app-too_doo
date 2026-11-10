using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;

namespace TooDoo.ConsoleUI;

/// <summary>
/// Console input helpers. Every "TryAsk" method returns false if the user cancels with 'q'.
/// When <c>current</c> is given, pressing Enter keeps it.
/// </summary>
public static class Prompts
{
    private const string CancelInput = "q";
    private const string ClearInput = "-";

    public static string ReadLine(string prompt)
    {
        Console.Write(Colors.Paint(prompt, Colors.Neutral));
        return (Console.ReadLine() ?? string.Empty).Trim();
    }

    public static bool IsCancel(string input)
    {
        return input.Equals(CancelInput, StringComparison.OrdinalIgnoreCase);
    }

    public static bool Confirm(string question)
    {
        string answer = ReadLine($"{question} (y/N): ");

        return answer.Equals("y", StringComparison.OrdinalIgnoreCase)
            || answer.Equals("yes", StringComparison.OrdinalIgnoreCase);
    }

    public static void ShowError(string message)
    {
        Console.WriteLine(Colors.Paint($"{message}", Colors.Error));
    }

    public static void Pause(string message)
    {
        Console.WriteLine(message);
        Console.WriteLine(Colors.Paint("Press any key to continue.", Colors.Muted));
        Console.ReadKey(intercept: true);
    }

    public static bool TryAskText(string label, string? current, [NotNullWhen(true)] out string? value)
    {
        string[] options = current is null ? [] : ["Enter to keep"];

        while (true)
        {
            string input = ReadLine(FormatPrompt(label, current, options));

            if (IsCancel(input))
            {
                value = null;
                return false;
            }

            if (input.Length > 0)
            {
                value = input;
                return true;
            }

            if (current is not null)
            {
                value = current;
                return true;
            }

            ShowError($"{label} can't be empty.");
        }
    }

    public static bool TryAskDate(string label, DateOnly? current, out DateOnly value)
    {
        string? currentText = current?.ToString(ConsoleFormats.DateFormat, CultureInfo.InvariantCulture);
        string[] options = current is null
            ? [ConsoleFormats.DateFormat]
            : [ConsoleFormats.DateFormat, "Enter to keep"];

        while (true)
        {
            string input = ReadLine(FormatPrompt(label, currentText, options));

            if (IsCancel(input))
            {
                value = default;
                return false;
            }

            if (input.Length == 0 && current is not null)
            {
                value = current.Value;
                return true;
            }

            if (DateOnly.TryParseExact(
                    input,
                    ConsoleFormats.DateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out value))
            {
                return true;
            }

            string example = DateTime.Today.ToString(ConsoleFormats.DateFormat, CultureInfo.InvariantCulture);
            ShowError($"Use the format {ConsoleFormats.DateFormat}, for example {example}.");
        }
    }

    /// <summary>Like <see cref="TryAskText"/>, but the value may be empty (null).</summary>
    public static bool TryAskOptionalText(string label, string? current, out string? value)
    {
        string[] options = current is null
            ? ["Enter for none"]
            : ["Enter to keep", $"{ClearInput} to clear"];

        string input = ReadLine(FormatPrompt(label, current, options));

        if (IsCancel(input))
        {
            value = null;
            return false;
        }

        value = input switch
        {
            "" => current,
            ClearInput => null,
            _ => input
        };

        return true;
    }

    private static string FormatPrompt(string label, string? current, string[] options)
    {
        string currentPart = current is null ? string.Empty : $" [{current}]";
        string optionsPart = string.Join(", ", options.Append($"{CancelInput} to cancel"));

        return $"{label}{currentPart} ({optionsPart}): ";
    }
}
