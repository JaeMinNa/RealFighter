using System;
using System.Collections.Generic;
using UnityEngine.Events;

public static class ClientDef
{
    // 인게임
    public static readonly int SkillMaxCount = 3;
    public static readonly float TurnTime = 60f;
    public static readonly int MaxRound = 9;    // SkillMaxCount x 3 보다 무조건 같거나 커야 된다!!
    public static readonly int MaxHeroLevel = 10;
    public static readonly int WinExp = 5;
    public static readonly int LoseExp = 2;
    public static readonly int DrawExp = 1;
    public static readonly int WinGold = 1000;
    public static readonly int LoseGold = 500;
    public static readonly int DrawGold = 200;

    // 히어로
    public static readonly List<string> HeroNames = new List<string>
    {
        "REX", "BLAZE", "DRAKE", "DOMINICK", "MAVERICK", "STEELTON", "IRIS", "SERENA", "ORIANNA", "JIN"
    };
    public static readonly int MaxGradeExp = 3;

    // Shop
    public static readonly List<ShopData> ShopList_Hero = new List<ShopData>
    {
        new ShopData("NORMAL HERO PACK", 3000, 1, 2),
        new ShopData("RARE HERO PACK", 10000, 1, 5)
    };
    public static readonly List<ShopData> ShopList_Gold = new List<ShopData>
    {
        new ShopData("FREE GOLD", 0, 1000),
        new ShopData("AD GOLD", 0, 2000)
    };

    // Photon
    public static readonly float RoomWaitTime = 10f;

    // LocalPush
    public static readonly string LOCALKEY_Push_FreeGold = string.Empty;
}

#region UserData
public class UserData
{
    public UserData_Common UserCommonData = null;
    public UserData_Hero UserHeroData = null;
    public UserData_Contents UserContentsData = null;
}

public class UserData_Common
{
    public string AccountCode = string.Empty;
    public string UID = string.Empty;
    public string NickName = string.Empty;
    public int RankPoint = 0;
    public string Image = string.Empty;
    public int Gold = 0;
}

public class UserData_Hero
{
    public HeroData EquipHero = new HeroData();
    public List<HeroData> MyHeroes = new List<HeroData>();
}

public class UserData_Contents
{
    public bool IsFirstLogin = true;
    public DateTime LastLoginTime = DateTime.MinValue;
    public bool IsGotFreeGold = false;
    public int AdGoldBuyCount = 0;
}
#endregion

public class HeroData
{
    public string HeroName = string.Empty;
    public int[] Skillproficiencies = { 0, 0, 0 };
    public int Level = 0;
    public int Exp = 0;
    public int Grade = 0;
    public int GradeExp = 0;
}

public class RankData
{
    public string NickName = string.Empty;
    public int Rank = 0;
    public int RankPoint = 0;
    public string Image = string.Empty;
}

public class MessageData
{
    public PopupType Type;
    public string Title;
    public string Message;
    public UnityAction OkAction;
}

public enum PopupType
{
    None,

    OkOnly,
    OkCancel,

    Max
}

public enum LocalPushType
{
    None,

    FreeGold,

    Max
}

public class ItemData
{
    public string Name = string.Empty;
    public int Count = 0;

    public ItemData(string name, int count)
    {
        Name = name;
        Count = count;
    }
}

public class ShopData
{
    public string Name = string.Empty;
    public int Price = 0;
    public int Count = 0;
    public int Value = 0;

    public ShopData(string name, int price, int count = 0, int value = 0)
    {
        Name = name;
        Price = price;
        Count = count;
        Value = value;
    }
}
