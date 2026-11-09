using System.Globalization;

namespace TooDoo;

public enum DataType
{
    String,
    Int,
    DateOnly
}

public static class Validate
{
    private static bool IsRequired(string? input)
    {
        return !string.IsNullOrWhiteSpace(input);
    }

    private static bool IsInt(string? input)
    {
        return int.TryParse(input, out _);
    }

    private static bool IsDateOnly(string? input)
    {
        return DateOnly.TryParseExact(
            input,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _);
    }

    public static string? GetError(DataType dataType, string? input)
    {
        if (!IsRequired(input))
        {
            return "A value is required.";
        }

        switch (dataType)
        {
            case DataType.String:
                return null;

            case DataType.Int:
                return IsInt(input) ? null : "Enter a valid whole number.";

            case DataType.DateOnly:
                return IsDateOnly(input) ? null : "Enter a valid date.";

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(dataType),
                    dataType,
                    "Unsupported data type.");
        }
    }
}