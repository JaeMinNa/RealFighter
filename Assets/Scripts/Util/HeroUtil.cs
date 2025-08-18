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

        var randomLevel = RandomUtil.GetRandomIndex(1, 5);

        HeroData heroData = new HeroData()
        {
            HeroName = heroNames[randomValue],
            SkillDamage_0 = 10 + randomLevel -1,
            SkillDamage_1 = 10 + randomLevel - 1,
            SkillDamage_2 = 10 + randomLevel - 1,
            Level = randomLevel,
            Exp = 0
        };

        return heroData;
    }
    #endregion
}
