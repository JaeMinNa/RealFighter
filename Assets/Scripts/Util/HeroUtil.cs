using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public static class HeroUtil
{
    #region AI
    public static HeroData GetRandomAIHeroData()
    {
        List<string> heroNames = new List<string>() { "REX", "BLAZE", "DRAKE", "DOMINICK", "MAVERICK", "STEELTON", "IRIS", "SERENA", "ORIANNA", "JIN" };
        var randomValue = RandomUtil.GetRandomIndex(0, heroNames.Count - 1);

        var randomLevel = RandomUtil.GetRandomIndex(1, 3);
        var randomProficiency_0 = RandomUtil.GetRandomIndex(0, 300);
        var randomProficiency_1 = RandomUtil.GetRandomIndex(0, 300);
        var randomProficiency_2 = RandomUtil.GetRandomIndex(0, 300);

        HeroData heroData = new HeroData();
        heroData.HeroName = heroNames[randomValue];
        heroData.Skillproficiencies[0] = randomProficiency_0;
        heroData.Skillproficiencies[1] = randomProficiency_1;
        heroData.Skillproficiencies[2] = randomProficiency_2;
        heroData.Level = randomLevel;
        heroData.Exp = 0;

        return heroData;
    }
    #endregion

    #region Exp
    // 현재 장착한 히어로의 경험치를 올림
    public static void AddHeroExp(int value)
    {
        if (value <= 0)
            return;

        int curLevel = DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Level;
        int curExp = DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Exp;
        int levelUpExp = curLevel * 10;

        if(curExp + value >= levelUpExp)
        {
            if(curLevel >= ClientDef.MaxHeroLevel)
            {
                DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Exp = curExp + value;
            }
            else
            {
                DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Exp = curExp + value - levelUpExp;
                curExp = DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Exp;

                while (true)
                {
                    curLevel = DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Level++;
                    levelUpExp = curLevel * 10;

                    if (curExp < levelUpExp)
                        return;
                }
            }
        }
        else
        {
            DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Exp = curExp + value;
        }
    }
    #endregion
}
