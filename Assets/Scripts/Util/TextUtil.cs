using NUnit.Framework;
using System;
using System.Collections.Generic;

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

    // float�� 00:00.0 �� ���·� ��Ÿ����
    public static string ConvertTime(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        string formatted = string.Format("{0:00}:{1:00}.{2:0}",
                                          timeSpan.Minutes,
                                          timeSpan.Seconds,
                                          timeSpan.Milliseconds / 100);

        return formatted;
    }

    #region AI
    public static string GetRandomAINickName()
    {
        List<string> nickNames = 
            new List<string> { "감자주먹", "제주도아침", "레이스", "나이텐", "굿감", "공대생", "NoobSlayer900", 
                "PotatoKing", "Lazyzz", "asddd", "달빛검객", "밤하늘토끼", "불꽃반달", "도토리", "초코송이러버", 
                "푸른산호", "은빛여우", "바람남", "새벽냥", "눈물젤리", "ShadowStrike", "MoonRunner", "PixelCrush", 
                "SilentArrow", "NovaPunch", "DriftAce", "FireMint", "BlueComet", "SnackMaster", "HoneyToast", "인생레전드", "재미있을까" };

        var randomIndex = RandomUtil.GetRandomIndex(0, nickNames.Count - 1);

        return nickNames[randomIndex];
    }
    #endregion
}
