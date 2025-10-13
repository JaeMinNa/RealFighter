using UnityEngine;

public static class DamageUtil
{
    public static int GetSkillDamage(HeroData data, int num)
    {
        if (num > 2 || num < 0)
            return 0;

        // 기본 데미지
        int defaultDamage = 10;

        // 등급 데미지
        int gradeDamage = data.Grade * 3;

        // 숙련도 데미지
        int proficiencyDamage = data.Skillproficiencies[num] < 1001 ? (int)((float)data.Skillproficiencies[num] * 0.01f) : 10;

        // 데미지 계산 = 기본 데미지 + 레벨 데미지 + 숙련도 데미지 + 등급 데미지
        int damage = defaultDamage + (data.Level - 1) + proficiencyDamage + gradeDamage;

        return damage;
    }
}
