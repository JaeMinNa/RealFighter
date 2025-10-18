using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public static class HeroUtil
{
    public static HeroData GetRandomHeroDataByGrade(int grade)
    {
        List<string> heroNames = ClientDef.HeroNames;
        var randomValue = RandomUtil.GetRandomIndex(0, heroNames.Count - 1);

        HeroData heroData = new HeroData();
        heroData.HeroName = heroNames[randomValue];
        heroData.Skillproficiencies[0] = 0;
        heroData.Skillproficiencies[1] = 0;
        heroData.Skillproficiencies[2] = 0;
        heroData.Level = 1;
        heroData.Exp = 0;
        heroData.Grade = grade;
        heroData.GradeExp = 0;

        return heroData;
    }

    public static HeroData GetRandomAIHeroData()
    {
        List<string> heroNames = ClientDef.HeroNames;
        var randomValue = RandomUtil.GetRandomIndex(0, heroNames.Count - 1);

        var randomLevel = RandomUtil.GetRandomIndex(1, 3);
        var randomProficiency_0 = RandomUtil.GetRandomIndex(0, 300);
        var randomProficiency_1 = RandomUtil.GetRandomIndex(0, 300);
        var randomProficiency_2 = RandomUtil.GetRandomIndex(0, 300);
        int randomGrade = 0;

        // Grade (0 : 80%, 1 : 15%, 2 : 5%)
        var randomGradePercent = RandomUtil.GetRandomIndex(1, 100);
        if (randomGradePercent <= 5)
            randomGrade = 2;
        else if (randomGradePercent <= 20)
            randomGrade = 1;
        else
            randomGrade = 0;

        HeroData heroData = new HeroData();
        heroData.HeroName = heroNames[randomValue];
        heroData.Skillproficiencies[0] = randomProficiency_0;
        heroData.Skillproficiencies[1] = randomProficiency_1;
        heroData.Skillproficiencies[2] = randomProficiency_2;
        heroData.Level = randomLevel;
        heroData.Exp = 0;
        heroData.Grade = randomGrade;
        heroData.GradeExp = 0;

        return heroData;
    }

    // 현재 장착한 히어로의 경험치를 올림
    public static void AddHeroExp(int value)
    {
        if (value <= 0)
            return;

        int curLevel = DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Level;
        int curExp = DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Exp;
        int levelUpExp = curLevel * 10;
        string heroName = DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.HeroName;
        HeroData myHeroData = null;

        for (int index = 0; index < DataManager.Instance.GetMyUserData().UserHeroData.MyHeroes.Count; ++index)
        {
            if (DataManager.Instance.GetMyUserData().UserHeroData.MyHeroes[index].HeroName == heroName)
            {
                myHeroData = DataManager.Instance.GetMyUserData().UserHeroData.MyHeroes[index];
                break;
            }
        }

        if (myHeroData == null)
            return;

        // 장착, 보유 히어로 각각 모두 경험치를 증가
        if (curExp + value >= levelUpExp)
        {
            if(curLevel >= ClientDef.MaxHeroLevel)
            {
                DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Exp = curExp + value;
                myHeroData.Exp = curExp + value;
            }
            else
            {
                DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Exp = curExp + value - levelUpExp;
                myHeroData.Exp = curExp + value - levelUpExp;

                curExp = DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Exp;

                while (true)
                {
                    DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Level++;
                    myHeroData.Level++;

                    curLevel = DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Level;
                    levelUpExp = curLevel * 10;

                    if (curExp < levelUpExp)
                        return;
                }
            }
        }
        else
        {
            DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Exp = curExp + value;
            myHeroData.Exp = curExp + value;
        }
    }

    // 해당 히어로의 등급 경험치를 올림
    public static void AddHeroGradeExp(HeroData data)
    {
        if (data == null)
            return;

        data.GradeExp++;

        if (ClientDef.MaxGradeExp <= data.GradeExp)
        {
            data.GradeExp = 0;
            data.Grade++;

            TryMergeHero(data);
        }
    }

    #region Private Method
    private static void TryMergeHero(HeroData data)
    {
        // 내 히어로 인벤토리 가져오기
        var heroList = DataManager.Instance.GetMyUserData().UserHeroData.MyHeroes;

        // 동일 이름, 동일 등급, 자기 자신이 아닌 대상 찾기
        var target = heroList.Find(Data =>
            Data != data &&
            Data.HeroName == data.HeroName &&
            Data.Grade == data.Grade);

        // 동일 히어로가 있다면 병합
        if (target != null)
        {
            target.GradeExp++;

            // 현재 data는 제거
            heroList.Remove(data);

            // 등급 경험치가 다시 MaxGradeExp에 도달하면 재귀 처리
            if (target.GradeExp >= ClientDef.MaxGradeExp)
            {
                target.GradeExp = 0;
                target.Grade++;
                TryMergeHero(target);
            }
        }
    }
    #endregion
}
