using BackEnd;
using UnityEngine;
using LitJson;
using System.Collections.Generic;

public class BackendManager : Singleton<BackendManager>
{
    private const string RANK_UUID = "019a54d9-07e7-7abe-a5bd-c06b7b3e24ad";

    public static BackendManager Instance
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

                GameObject managerObj = GameObject.Find("[Managers]/BackendManager");
                if (managerObj == null)
                {
                    managerObj = new GameObject("BackendManager");
                    managerObj.transform.SetParent(Obj.transform);
                }

                m_Instance = managerObj.GetComponent<BackendManager>();
                if (m_Instance == null)
                {
                    m_Instance = managerObj.AddComponent<BackendManager>();
                }

                m_Instance.CreateInstance();
            }

            return m_Instance;
        }
    }

    #region Override Method
    public override void DestroyInstance()
    {

    }

    protected override void CreateInstance()
    {
        BackendSetup();
    }
    #endregion

    #region Public Method
    public void SignUp()
    {
        BackendReturnObject bro = Backend.BMember.CustomSignUp(DataManager.Instance.GetMyUserData().UserCommonData.UID, "1234");
        if (bro.IsSuccess())
        {
            Debug.LogWarning("뒤끝 서버 회원가입 성공!");

            InsertData();
            SaveData();
            Login();
            UpdateNickname(DataManager.Instance.GetMyUserData().UserCommonData.NickName);
        }
        else
        {
            Debug.LogWarning("뒤끝 서버 회원가입 실패");
        }
    }

    public void Login()
    {
        BackendReturnObject bro = Backend.BMember.CustomLogin(DataManager.Instance.GetMyUserData().UserCommonData.UID, "1234");
        if (bro.IsSuccess())
        {
            Debug.LogWarning("뒤끝 서버 로그인 성공!");
        }
        else
        {
            Debug.LogWarning("뒤끝 서버 로그인 실패");

            SignUp();
        }
    }

    public void AutoLogin()
    {
        BackendReturnObject bro = Backend.BMember.LoginWithTheBackendToken();
        if (bro.IsSuccess())
        {
            Debug.LogWarning("뒤끝 서버 자동 로그인 성공!");
        }
        else
        {
            Debug.LogWarning("뒤끝 서버 자동 로그인 실패");
        }
    }

    public void DeleteUserID()
    {
        BackendReturnObject bro = Backend.BMember.WithdrawAccount();

        Debug.LogWarning("뒤끝 서버 아이디 삭제 완료!");
    }

    // 닉네임 변경을 시도하고, 성공 여부를 bool 값으로 반환
    public bool UpdateNickname(string nickName)
    {
        BackendReturnObject bro = Backend.BMember.UpdateNickname(nickName);

        if (bro.IsSuccess())
        {
            Debug.LogWarning($"뒤끝 서버 닉네임 변경 성공 : {nickName}");
            return true;
        }
        else
        {
            if (bro.GetStatusCode() == "409")
            {
                Debug.LogWarning("뒤끝 서버 이미 사용 중인 닉네임");
                return false;
            }
            else
            {
                Debug.LogWarning($"뒤끝 서버 닉네임 변경 실패 (기타 오류) : {bro}");
                return false;
            }
        }
    }
    public void SaveData()
    {
        if (!Backend.IsInitialized)
        {
            Debug.LogWarning("뒤끝 서버와 연결이 끊어짐");
            return;
        }

        // 유저 데이터 저장
        Param param = GetUserDataParam();
        BackendReturnObject bro = Backend.GameData.Update("USER_DATA", new Where(), param);

        if (bro.IsSuccess())
        {
            Debug.LogWarning("뒤끝 서버 데이터 저장 성공!");
        }
        else
        {
            Debug.LogWarning("뒤끝 서버 데이터 저장 실패");
        }

        // 랭킹 데이터 저장
        SaveMyRank();
    }

    // 서버로부터 데이터를 불러와서 Parsing하는 함수
    public void LoadData()
    {
        if (!Backend.IsInitialized)
        {
            Debug.LogWarning("뒤끝 서버와 연결이 끊어짐");
            return;
        }

        BackendReturnObject bro = Backend.GameData.GetMyData("USER_DATA", new Where());

        if (bro.IsSuccess())
        {
            // 서버에서 불러온 Json 데이터를 파싱
            // Json 데이터 중, rows의 값만 가져옴
            Debug.LogWarning("뒤끝 서버 데이터 로드 성공!");
            ParsingData(bro.GetReturnValuetoJSON()["rows"][0]);
        }
        else
        {
            Debug.LogWarning("뒤끝 서버 데이터 로드 실패");
        }
    }

    // 회원 가입한 후 첫 데이터 삽입을 위한 함수
    public void InsertData()
    {
        Param param = GetUserDataParam();
        BackendReturnObject bro = Backend.GameData.Insert("USER_DATA", param); // USER_DATA 테이블 이름

        if (bro.IsSuccess())
        {
            Debug.LogWarning("뒤끝 서버 데이터 추가 성공!");
        }
        else
        {
            Debug.LogWarning("뒤끝 서버 데이터 추가 실패");
        }
    }

    public void SaveMyRank()
    {
        string rowInDate = string.Empty;

        // 랭킹 데이터를 업데이트하려면 게임 데이터에서 사용하는 데이터의 inDate 값 필요
        BackendReturnObject bro = Backend.GameData.GetMyData("USER_DATA", new Where());

        if (!bro.IsSuccess())
        {
            Debug.LogWarning("뒤끝 서버 랭킹 업데이트를 위한 데이터 조회 중 문제 발생");
            return;
        }

        Debug.LogWarning("뒤끝 서버 랭킹 업데이트를 위한 데이터 조회 성공!");

        if (bro.FlattenRows().Count > 0)
        {
            rowInDate = bro.FlattenRows()[0]["inDate"].ToString();
        }
        else
        {
            Debug.LogWarning("뒤끝 서버 랭킹 업데이트를 위한 데이터가 존재하지 않음");
        }

        Param param = new Param()
        {
            {"RankPoint",  DataManager.Instance.GetMyUserData().UserCommonData.RankPoint}
        };

        // 해당 데이터테이블의 데이터를 갱신하고, 랭킹 데이터 정보 갱신
        bro = Backend.URank.User.UpdateUserScore(RANK_UUID, "USER_DATA", rowInDate, param);

        if (bro.IsSuccess())
        {
            Debug.LogWarning("뒤끝 서버 랭킹 등록 성공!");
        }
        else
        {
            Debug.LogWarning("뒤끝 서버 랭킹 등록 실패");
        }
    }

    public RankData GetMyRankData()
    {
        // 내 랭킹 정보 불러오기 
        BackendReturnObject bro = Backend.URank.User.GetMyRank(RANK_UUID);

        if (bro.IsSuccess())
        {
            try
            {
                JsonData rankDataJson = bro.FlattenRows();

                // 받아온 데이터의 개수가 0 -> 데이터가 없음
                if (rankDataJson.Count <= 0)
                {
                    Debug.LogWarning("뒤끝 서버 나의 랭킹 데이터가 존재하지 않음");
                    return null;

                }
                else
                {
                    Debug.LogWarning("뒤끝 서버 나의 랭킹 조회 성공!");

                    RankData data = new RankData()
                    {
                        NickName = rankDataJson[0]["nickname"].ToString(),
                        Rank = int.Parse(rankDataJson[0]["rank"].ToString()),
                        RankPoint = int.Parse(rankDataJson[0]["score"].ToString()),

                        // 추가 항목 데이터
                        Image = rankDataJson[0]["CharacterImg"].ToString()
                    };

                    return data;
                }
            }
            // 나의 랭킹 정보 JSON 데이터 파싱에 실패했을 때
            catch (System.Exception e)
            {
                Debug.LogWarning($"뒤끝 서버 나의 랭킹 데이터 파싱 실패 : {e}");
                return null;
            }
        }
        else
        {
            Debug.LogWarning("뒤끝 서버 나의 랭킹 데이터 불러오기 실패");
            return null;
        }
    }

    public List<RankData> GetRankDataList()
    {
        int maxRankList = ClientDef.RankingCount;

        // 랭킹 테이블에 있는 유저의 offset ~ offset + limit 순위 랭킹 정보를 불러옴
        BackendReturnObject bro = Backend.URank.User.GetRankList(RANK_UUID, maxRankList, 0);

        if (bro.IsSuccess())
        {
            try
            {
                JsonData rankDataJson = bro.FlattenRows();

                // 받아온 데이터의 개수가 0 -> 데이터가 없음
                if (rankDataJson.Count <= 0)
                {
                    Debug.LogWarning("뒤끝 서버 랭킹 데이터가 존재하지 않음");

                    return null;
                }
                else
                {
                    Debug.LogWarning("뒤끝 서버 랭킹 조회 성공!");

                    List<RankData> rankData = new List<RankData>();
                    int rankCount = rankDataJson.Count;

                    for (int index = 0; index < rankCount; ++index)
                    {
                        RankData data = new RankData()
                        {
                            NickName = rankDataJson[index]["nickname"].ToString(),
                            Rank = int.Parse(rankDataJson[index]["rank"].ToString()),
                            RankPoint = int.Parse(rankDataJson[index]["score"].ToString()),

                            // 추가 항목 데이터
                            Image = rankDataJson[index]["CharacterImg"].ToString()
                        };

                        rankData.Add(data);
                    }

                    return rankData;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"뒤끝 서버 랭킹 데이터 파싱 실패 : {e}");
                return null;
            }
        }
        else
        {
            Debug.LogWarning("뒤끝 서버 랭킹 데이터 불러오기 실패");
            return null;
        }
    }
    #endregion

    #region Private Method
    private void BackendSetup()
    {
        // 뒤끝 초기화
        BackendReturnObject bro = Backend.Initialize();

        // 뒤끝 초기화에 대한 응답값
        if (bro.IsSuccess())
        {
            Debug.Log("뒤끝 서버 연동 성공 : " + bro); // 성공일 경우 statusCode 204 Success
        }
        else
        {
            Debug.LogError("뒤끝 서버 연동 실패 : " + bro); // 실패일 경우 statusCode 400대 에러 발생
        }
    }

    // param : 데이터를 송수신할 때 사용하는 class
    private Param GetUserDataParam()
    {
        Param param = new Param();
        param.Add("RankPoint", DataManager.Instance.GetMyUserData().UserCommonData.RankPoint);
        param.Add("CharacterImg", DataManager.Instance.GetMyUserData().UserCommonData.Image);

        // 랭킹 데이터에서 복수 데이터의 추가 항목 사용 시
        //param.Add("WinLose", GameManager.I.DataManager.GameData.Win.ToString() + "|" + GameManager.I.DataManager.GameData.Lose.ToString());

        return param;
    }

    private void ParsingData(JsonData json)
    {
        // 파싱된 데이터를 저장
        //GameManager.I.DataManager.GameData.RankPoint = int.Parse(json["RankPoint"][0].ToString());

        // 랭킹 데이터에서 복수 데이터의 추가 항목 사용 시
        //string[] extraData = json["extraData"].ToString().Split("|");
        //GameManager.I.DataManager.GameData.Win = int.Parse(extraData[0].ToString());
        //GameManager.I.DataManager.GameData.Lose = int.Parse(extraData[1].ToString());

        //GameData의 변수가 배열이라면 ?
        //for (int i = 0; i < json["Items"]["L"].Count; i++)
        //{
        //    GameManager.I.DataManager.GameData.Items[i] = int.Parse(json["Items"]["L"][i][0].ToString());
        //}
    }
    #endregion
}
