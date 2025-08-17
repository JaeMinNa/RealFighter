using System.Collections.Generic;
using UnityEngine.Events;

public static class ClientDef
{
    public static readonly int MAX_PARTYMEMBER = 3;

    #region Player Preference
    public static readonly string PomeSortOrderKey = "PomeSortOrder";
    public static readonly string LastTutorialGroupKey = "LastTutorialGroup";
    public static readonly string IsPomeSavedKey = "IsPomeSaved";
    public static readonly string PomeGameOptionKey = "PomeGameOption";
    #endregion
}

#region UserData
public class UserData_Common
{
    public string AccountCode = string.Empty;
    public string UID = string.Empty;
    public string NickName = string.Empty;
    public int Score = 0;
    public string Image = string.Empty;
    public int Gold = 0;
}

public class UserData_Hero
{
    public HeroData EquipHero = new HeroData();
    public List<HeroData> MyHeroes = new List<HeroData>();
}
#endregion

public class HeroData
{
    public string HeroName = string.Empty;
    public int SkillDamage_0 = 0;
    public int SkillDamage_1 = 0;
    public int SkillDamage_2 = 0;
    public int Level = 0;
    public int Exp = 0;
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

//public enum eItemType
//{
//    None,

//    Goods = 1,
//    Consume = 2,
//    Material = 3,
//    Equip = 4,
//    ProfileIcon = 5,

//    Max
//}

//public enum eTutorialStep
//{
//    None,

//    String_Welcome,
//    String_Intro,
//    String_PomeCheck_Start,
//    Click_SubMenu,
//}