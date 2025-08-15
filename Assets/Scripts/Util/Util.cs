using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using UnityEngine.UI;

public static class Util
{
    #region Json
    public static T ToObjectJson<T>(string JsonData)
    {
        return JsonConvert.DeserializeObject<T>(JsonData);
    }

    public static bool TryToObjectJson<T>(string JsonData, out T JsonObject)
    {
        try
        {
            JsonObject = JsonConvert.DeserializeObject<T>(JsonData);
            if (JsonObject == null)
                return false;
            else
                return true;
        }
        catch (JsonException e)
        {
            Console.WriteLine(e.Message);
            JsonObject = default;
            return false;
        }
    }

    public static string ToJson(object JsonObject)
    {
        return JsonConvert.SerializeObject(JsonObject);
    }

    public static bool TryToJson(object JsonObject, out string JsonData)
    {
        try
        {
            JsonData = JsonConvert.SerializeObject(JsonObject);
            return true;
        }
        catch (JsonException e)
        {
            Console.WriteLine(e.Message);
            JsonData = string.Empty;
            return false;
        }
    }
    #endregion

    #region float
    public static float UniformVelocity_float(float fStartPos, float fEndPos, float nCurrentTime, float nMaxTime)
    {
        if (nCurrentTime >= nMaxTime)
            return fEndPos;

        if (nCurrentTime <= 0)
            return fStartPos;

        return fStartPos + nCurrentTime * (fEndPos - fStartPos) / nMaxTime;
    }

    public static float UniformVelocity_float(int fStartPos, int fEndPos, float nCurrentTime, float nMaxTime)
    {
        if (nCurrentTime >= nMaxTime)
            return Convert.ToSingle(fEndPos);

        if (nCurrentTime <= 0)
            return Convert.ToSingle(fStartPos);

        return fStartPos + nCurrentTime * (fEndPos - fStartPos) / nMaxTime;
    }

    public static float UniformVelocity_float(long fStartPos, long fEndPos, float nCurrentTime, float nMaxTime)
    {
        if (nCurrentTime >= nMaxTime)
            return Convert.ToSingle(fEndPos);

        if (nCurrentTime <= 0)
            return Convert.ToSingle(fStartPos);

        return fStartPos + nCurrentTime * (fEndPos - fStartPos) / nMaxTime;
    }

    public static float UniformVelocity_float(float fStartPos, float fEndPos, int nCurrentTime, int nMaxTime)
    {
        if (nCurrentTime >= nMaxTime)
            return fEndPos;

        if (nCurrentTime <= 0)
            return fStartPos;

        return fStartPos + nCurrentTime * (fEndPos - fStartPos) / nMaxTime;
    }

    public static float UniformVelocity_float(float fStartPos, float fEndPos, long nCurrentTime, long nMaxTime)
    {
        if (nCurrentTime >= nMaxTime)
            return fEndPos;

        if (nCurrentTime <= 0)
            return fStartPos;

        return fStartPos + nCurrentTime * (fEndPos - fStartPos) / nMaxTime;
    }


    public static string FormatUpto3Decimal(this float value)
    {
        return value % 1 == 0 ? value.ToString("0") : value.ToString("0.###");
    }
    #endregion

    #region Int
    public static int UniformVelocity_int(float fStartPos, float fEndPos, float nCurrentTime, float nMaxTime)
    {
        if (nCurrentTime >= nMaxTime)
            return Convert.ToInt32(fEndPos);

        if (nCurrentTime <= 0)
            return Convert.ToInt32(fStartPos);

        return Convert.ToInt32((fStartPos + nCurrentTime * (fEndPos - fStartPos) / nMaxTime));
    }

    public static int UniformVelocity_int(int fStartPos, int fEndPos, float nCurrentTime, float nMaxTime)
    {
        if (nCurrentTime >= nMaxTime)
            return Convert.ToInt32(fEndPos);

        if (nCurrentTime <= 0)
            return Convert.ToInt32(fStartPos);

        return Convert.ToInt32((fStartPos + nCurrentTime * (fEndPos - fStartPos) / nMaxTime));
    }

    public static int UniformVelocity_int(long fStartPos, long fEndPos, float nCurrentTime, float nMaxTime)
    {
        if (nCurrentTime >= nMaxTime)
            return Convert.ToInt32(fEndPos);

        if (nCurrentTime <= 0)
            return Convert.ToInt32(fStartPos);

        return Convert.ToInt32((fStartPos + nCurrentTime * (fEndPos - fStartPos) / nMaxTime));
    }

    public static int UniformVelocity_int(float fStartPos, float fEndPos, int nCurrentTime, int nMaxTime)
    {
        if (nCurrentTime >= nMaxTime)
            return Convert.ToInt32(fEndPos);

        if (nCurrentTime <= 0)
            return Convert.ToInt32(fStartPos);

        return Convert.ToInt32((fStartPos + nCurrentTime * (fEndPos - fStartPos) / nMaxTime));
    }

    public static int UniformVelocity_int(float fStartPos, float fEndPos, long nCurrentTime, long nMaxTime)
    {
        if (nCurrentTime >= nMaxTime)
            return Convert.ToInt32(fEndPos);

        if (nCurrentTime <= 0)
            return Convert.ToInt32(fStartPos);

        return Convert.ToInt32((fStartPos + nCurrentTime * (fEndPos - fStartPos) / nMaxTime));
    }
    #endregion

    #region Long
    public static long UniformVelocity_long(float fStartPos, float fEndPos, float nCurrentTime, float nMaxTime)
    {
        if (nCurrentTime >= nMaxTime)
            return Convert.ToInt64(fEndPos);

        if (nCurrentTime <= 0)
            return Convert.ToInt64(fStartPos);

        return Convert.ToInt64((fStartPos + nCurrentTime * (fEndPos - fStartPos) / nMaxTime));
    }

    public static long UniformVelocity_long(int fStartPos, int fEndPos, float nCurrentTime, float nMaxTime)
    {
        if (nCurrentTime >= nMaxTime)
            return Convert.ToInt64(fEndPos);

        if (nCurrentTime <= 0)
            return Convert.ToInt64(fStartPos);

        return Convert.ToInt64((fStartPos + nCurrentTime * (fEndPos - fStartPos) / nMaxTime));
    }

    public static long UniformVelocity_long(long fStartPos, long fEndPos, float nCurrentTime, float nMaxTime)
    {
        if (nCurrentTime >= nMaxTime)
            return Convert.ToInt64(fEndPos);

        if (nCurrentTime <= 0)
            return Convert.ToInt64(fStartPos);

        return Convert.ToInt64((fStartPos + nCurrentTime * (fEndPos - fStartPos) / nMaxTime));
    }

    public static long UniformVelocity_long(float fStartPos, float fEndPos, int nCurrentTime, int nMaxTime)
    {
        if (nCurrentTime >= nMaxTime)
            return Convert.ToInt64(fEndPos);

        if (nCurrentTime <= 0)
            return Convert.ToInt64(fStartPos);

        return Convert.ToInt64((fStartPos + nCurrentTime * (fEndPos - fStartPos) / nMaxTime));
    }

    public static long UniformVelocity_long(float fStartPos, float fEndPos, long nCurrentTime, long nMaxTime)
    {
        if (nCurrentTime >= nMaxTime)
            return Convert.ToInt64(fEndPos);

        if (nCurrentTime <= 0)
            return Convert.ToInt64(fStartPos);

        return Convert.ToInt64((fStartPos + nCurrentTime * (fEndPos - fStartPos) / nMaxTime));
    }
    #endregion

    #region Time
    public static DateTime DateTimeNow
    {
        get
        {
            return DateTime.UtcNow.AddHours(9);
        }
    }

    public static DateTime ToDateTime(string time)
    {
        if (string.IsNullOrEmpty(time))
            return DateTimeNow;

        if (time.Contains("����"))
            time = time.Replace("����", "AM");

        if (time.Contains("����"))
            time = time.Replace("����", "PM");

        if (DateTime.TryParse(time, out DateTime Result))
            return Result;
        else
            return DateTimeNow;
    }

    public static string FormatTimeSpan(TimeSpan timeSpan)
    {
        if (timeSpan.Days > 0)
        {
            return $"{timeSpan.Days}:{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
        else if (timeSpan.Hours > 0)
        {
            return $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
        else
        {
            return $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
    }

    public static long GetTotalSeconds(TimeSpan timeSpan)
    {
        return timeSpan.Days * 24 * 3600 + timeSpan.Hours * 3600 + timeSpan.Minutes * 60 + timeSpan.Seconds;
    }
    #endregion


    public static string StringCompress(string data)
    {
        byte[] sourceArray = Encoding.UTF8.GetBytes(data);

        using MemoryStream inputStream = new MemoryStream(sourceArray);
        using MemoryStream outputStream = new MemoryStream();
        using BrotliStream bs = new BrotliStream(outputStream, System.IO.Compression.CompressionLevel.Optimal);

        inputStream.CopyTo(bs);
        bs.Flush();

        sourceArray = null;

        return Convert.ToBase64String(outputStream.ToArray());
    }

    public static string StringDecompress(string data)
    {
        byte[] sourceArray = Convert.FromBase64String(data);

        using MemoryStream inputStream = new MemoryStream(sourceArray);
        using MemoryStream outputStream = new MemoryStream();
        using BrotliStream bs = new BrotliStream(inputStream, CompressionMode.Decompress);

        bs.CopyTo(outputStream);
        outputStream.Flush();

        sourceArray = null;

        return Encoding.UTF8.GetString(outputStream.ToArray());
    }

    public static T ConvertTo<T>(object source)
    {
        string jsonString = JsonConvert.SerializeObject(source);
        return JsonConvert.DeserializeObject<T>(jsonString);
    }
}
