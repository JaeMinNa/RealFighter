using System;

public static class TextUtil
{
    private static readonly string[] Suffixes = { "", "K", "M", "B", "T" };

    public static string ConvertKMB(long value, bool showDecimal = false)
    {
        int suffixIndex = 0;
        double decimalValue = value;
        while (decimalValue >= 1000 && suffixIndex < Suffixes.Length - 1)
        {
            decimalValue /= 1000;
            suffixIndex++;
        }

        return showDecimal
            ? $"{Math.Floor(decimalValue * 1000) / 1000:0.###}{Suffixes[suffixIndex]}"
            : $"{Math.Floor(decimalValue):0}{Suffixes[suffixIndex]}";
    }

    public static string AddComma_3digits(long value)
    {
        return value.ToString("#,##0");
    }
}
