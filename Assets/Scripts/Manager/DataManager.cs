using System;
using UnityEditor;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public static DataManager Instance
    {
        get
        {
            if (m_Instance == null && Application.isPlaying)
            {
                GameObject Obj = GameObject.Find("[Managers]");
                if (Obj == null)
                {
                    Obj = new GameObject("[Managers]");
                    DontDestroyOnLoad(Obj);
                }

                GameObject managerObj = GameObject.Find("[Managers]/DataManager");
                if (managerObj == null)
                {
                    managerObj = new GameObject("DataManager");
                    managerObj.transform.SetParent(Obj.transform);
                }

                m_Instance = managerObj.GetComponent<DataManager>();
                if (m_Instance == null)
                {
                    m_Instance = managerObj.AddComponent<DataManager>();
                }

                m_Instance.CreateInstance();
            }

            return m_Instance;
        }
    }

    // 로컬 저장 DataLoader
    private DataLoader m_DataLoader = null;

    #region Override Method
    public override void DestroyInstance()
    {

    }

    protected override void CreateInstance()
    {

    }
    #endregion

    #region public Method
    #region Data
    [ContextMenu("Save Data")]
    public void SaveData()
    {
        if (m_DataLoader == null)
        {
            Debug.LogWarning("DataLoader is null");
            return;
        }

        ES3.Save("UserData", m_DataLoader);

        Debug.LogWarning("UserData 저장 완료!");
    }

    [ContextMenu("Load Data")]
    public void LoadData()
    {
        if (m_DataLoader == null)
        {
            Debug.LogWarning("DataLoader is null");
            return;
        }

        if (ES3.FileExists("SaveFile.txt"))
        {
            ES3.LoadInto("UserData", m_DataLoader);

            Debug.LogWarning("UserData 불러오기 완료!");
        }
        else
        {
            Debug.LogWarning("UserData 불러오기 실패");

            SetUserData();
        }
    }

    [ContextMenu("Delete Data")]
    public void DeleteData()
    {
        // 데이터 제거
        ES3.DeleteFile("SaveFile.txt");
        Debug.LogWarning("UserData 제거 완료");

        // PlayerPrefs 제거
        PlayerPrefs.DeleteAll();
        Debug.LogWarning("PlayerPrefs 제거 완료");

        ExitGame();
    }

    public void SetDataLoader()
    {
        m_DataLoader = GameObject.Find("DataLoader").GetComponent<DataLoader>();
    }

    public UserData GetMyUserData()
    {
        return m_DataLoader.MyUserData;
    }

    public UserData GetAIUserData()
    {
        return SetAIUserData();
    }

    public UserData GetTutorialAIUserData()
    {
        return SetTutorialAIUserData();
    }
    #endregion

    #region Game
    public void ExitGame()
    {
        GameManager.Instance.ExitGame();

    }
    #endregion
    #endregion

    #region Private Method
    private void SetUserData()
    {
        m_DataLoader.MyUserData = new UserData();

        string UID = Guid.NewGuid().ToString("N").Substring(0, 8);

        // Common 기본값 설정
        UserData_Common userData_Common = new UserData_Common()
        {
            AccountCode = UID,
            UID = UID,
            NickName = UID,
            RankPoint = 0,
            Image = "0",
            //Gold = 0
            Gold = 1000
        };

        m_DataLoader.MyUserData.UserCommonData = userData_Common;

        // Hero 기본값 설정
        HeroData heroData = new HeroData();
        heroData.HeroName = "BLAZE";
        heroData.Skillproficiencies[0] = 0;
        heroData.Skillproficiencies[1] = 0;
        heroData.Skillproficiencies[2] = 0;
        //heroData.Level = 6;
        heroData.Level = 1;
        heroData.Exp = 0;
        heroData.Grade = 0;
        heroData.GradeExp = 0;

        UserData_Hero userData_Hero = new UserData_Hero();
        userData_Hero.EquipHero = heroData;
        userData_Hero.MyHeroes.Add(heroData);

        m_DataLoader.MyUserData.UserHeroData = userData_Hero;

        // Contents 기본값 설정
        UserData_Contents userData_Contents = new UserData_Contents()
        {
            LastLoginTime = Util.DateTimeNow,
            IsGotFreeGold = false,
            AdGoldBuyCount = 0,
            //TutorialIndex = 0
            TutorialIndex = 10
        };

        m_DataLoader.MyUserData.UserContentsData = userData_Contents;

        Debug.LogWarning("UserData 설정 완료!");
    }

    private UserData SetAIUserData()
    {
        UserData AIUserData = new UserData();

        // Common 기본값 설정
        UserData_Common aiData_Common = new UserData_Common()
        {
            AccountCode = "AI",
            UID = "AI",
            NickName = TextUtil.GetRandomAINickName(),
            RankPoint = RandomUtil.GetRandomIndex(0, 5),
            Image = RandomUtil.GetRandomIndex(0, 4).ToString(),
            Gold = 0
        };

        AIUserData.UserCommonData = aiData_Common;

        // Hero 설정
        HeroData heroData = HeroUtil.GetRandomAIHeroData();
        UserData_Hero aiData_Hero = new UserData_Hero();
        aiData_Hero.EquipHero = heroData;
        aiData_Hero.MyHeroes.Add(heroData);

        AIUserData.UserHeroData = aiData_Hero;

        Debug.LogWarning("AI EnemyUserData 설정 완료");

        return AIUserData;
    }

    private UserData SetTutorialAIUserData()
    {
        UserData AIUserData = new UserData();

        // Common 기본값 설정
        UserData_Common aiData_Common = new UserData_Common()
        {
            AccountCode = "TutorialAI",
            UID = "TutorialAI",
            NickName = "Tutorial Bot",
            RankPoint = 0,
            Image = "3",
            Gold = 0
        };

        AIUserData.UserCommonData = aiData_Common;

        // Hero 설정
        HeroData heroData = new HeroData();
        heroData.HeroName = "JIN";
        heroData.Skillproficiencies[0] = 0;
        heroData.Skillproficiencies[1] = 0;
        heroData.Skillproficiencies[2] = 0;
        heroData.Level = 1;
        heroData.Exp = 0;
        heroData.Grade = 0;
        heroData.GradeExp = 0;

        UserData_Hero aiData_Hero = new UserData_Hero();
        aiData_Hero.EquipHero = heroData;
        aiData_Hero.MyHeroes.Add(heroData);

        AIUserData.UserHeroData = aiData_Hero;

        Debug.LogWarning("Tutorial AI UserData 설정 완료");

        return AIUserData;
    }
    #endregion
}
