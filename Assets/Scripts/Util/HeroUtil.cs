using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public static class HeroUtil
{
    #region AI
    public static HeroData GetRandomAIHeroData()
    {
        //List<string> heroNames = new List<string>() { "REX", "BLAZE", "DRAKE", "DOMINICK", "MAVERICK", "STEELTON", "IRIS", "SERENA", "ORIANNA", "JIN" };
        List<string> heroNames = new List<string>() {"JIN" };
        var randomValue = RandomUtil.GetRandomIndex(0, heroNames.Count - 1);

        var randomLevel = RandomUtil.GetRandomIndex(1, 5);

        HeroData heroData = new HeroData();
        heroData.HeroName = heroNames[randomValue];
        heroData.SkillDamages[0] = 10 + randomLevel - 1;
        heroData.SkillDamages[1] = 10 + randomLevel - 1;
        heroData.SkillDamages[2] = 10 + randomLevel - 1;
        heroData.Level = randomLevel;
        heroData.Exp = 0;

        return heroData;
    }
    #endregion
}
