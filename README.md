# 🖥️ Real Fighter
### ❗ 유료 에셋 사용으로 에셋 부분은 삭제하였습니다. ❗

+ 히어로를 수집하고 육성하세요!
+ 상대방과 1:1 배틀에서 승리하세요!
+ 상대방의 공격을 예측하여 방어하세요!
+ 랭킹을 올려보세요!
<br/>

## 📽️ 개인 프로젝트 소개
 - 게임 이름 : Real Fighter
 - 플랫폼 : Android
 - 장르 : 3D 액션 멀티 PVP 전략
 - 개발 기간 : 25.08.12 ~ 25.11.12
<br/>

## 🎯 개발 목표
 - Photon을 사용한 실시간 PVP 구현
 - 뒤끝서버를 사용한 랭킹 구현
 - 구글 애드몹 광고 구현
 - UIManager를 사용한 깔끔한 UI 구현
 - CSV파일을 이용한 금칙어 적용
 - Mobile Notifications를 이용한 로컬 푸시 구현
 - 서버를 직접 구현하지 않고, 멀티 플레이 구현
 - 실무 코드 스타일을 적용한 구조적이고 일관된 코드 작성
<br/>

## ⚙️ Environment

- `Unity 6000.0.40f1`
- **IDE** : Visual Studio 2019, MonoDevelop
- **VCS** : Git (GitHub Desktop)
- **Envrionment** : Android
- **Resolution** : 1920 x 1080 `FHD`
<br/>

## ▶️ 게임 스크린샷

<p align="center">
  <img src="https://github.com/user-attachments/assets/462ee72c-3eae-4e0c-8efa-6e53d270ce69" width="49%"/>
  <img src="https://github.com/user-attachments/assets/e37abeea-9c4a-46a7-8b04-b6270da7d7cd" width="49%"/>
</p>
<p align="center">
  <img src="https://github.com/user-attachments/assets/2145dd92-5e80-4e16-81e5-bb4830ac3fc9" width="49%"/>
  <img src="https://github.com/user-attachments/assets/80d81112-3fbe-4bb6-9a1c-e2adaac49bf8" width="49%"/>
</p>
<p align="center">
  <img src="https://github.com/user-attachments/assets/eeab084b-888b-4943-96ef-6878ae9fc768" width="49%"/>
  <img src="https://github.com/user-attachments/assets/5c055e61-ca2c-44f2-8c19-e9504c606d9d" width="49%"/>
</p>
<p align="center">
  <img src="https://github.com/user-attachments/assets/a6eac821-f6c9-4b02-951e-a5e4fc5278f6" width="49%"/>
  <img src="https://github.com/user-attachments/assets/a37cfeda-a959-4555-ae2b-74bbd18976a4" width="49%"/>
</p>
<br/>

## 🔳 초기 와이어프레임
![image](https://github.com/user-attachments/assets/09923004-558f-4711-81ee-6829d915d297)


## 🧩 클라이언트 구조

### Managers
![image](https://github.com/user-attachments/assets/580f4e1e-c8da-431b-afe0-315b241640d5)

### 데이터 전달 방식
![image](https://github.com/user-attachments/assets/b9296579-8850-48c3-8f3c-b2830247693a)


## ✏️ 구현 기능

### 1. UIManager 구현
<img src="https://github.com/user-attachments/assets/e37abeea-9c4a-46a7-8b04-b6270da7d7cd" width="50%"/>

#### 구현 이유
- 프로젝트의 UI를 체계적으로 관리하기 위해
- 유지보수성과 확장성을 극대화하기 위해
- 게임의 규모가 커질수록 UI 요소를 개별적으로 제어하기 힘들기 때문에

#### 구현 방법
- 프로젝트 내 모든 UI 오브젝트가 공통적으로 가져야 할 동작을 추상화(Abstract)한 베이스 클래스
```C#
public abstract class UIElement : MonoBehaviour
{
    public string UIName = string.Empty;
    public RectTransform RectTransform;
    public UI UIParent;

    // 초기화
    public abstract void Init();
    // UI가 열릴 때 호출
    public abstract void OnOpen(List<object> Args);
    // UI가 닫힐 때 호출
    public abstract void OnClose();
    // UI 갱신
    public abstract void OnRefresh();

    #region Async
    public virtual async UniTask InitAsync()
    {
        await UniTask.Yield();
    }

    public virtual async UniTask OnOpenAsync(List<object> Args)
    {
        await UniTask.Yield();
    }

    public virtual async UniTask OnCloseAsync()
    {
        await UniTask.Yield();
    }

    public virtual async UniTask OpenAction()
    {
        await UniTask.Yield();
    }

    public virtual async UniTask CloseAction()
    {
        await UniTask.Yield();
    }
    #endregion
}
```
<br/>

- 캔버스들을 자동으로 찾아 Dictionary에 저장하여, UI 계층 구조를 명확하게 하고 정렬 순서 충돌 방지
```C#
public void SetUIRoot(GameObject Obj)
{
    m_UIRootObjects.Clear();

    m_UIRootObjects.Add(UI.Root, Obj.GetComponent<Canvas>());

    Transform Panel = null;
    Panel = Obj.transform.Find("Background");
    if (Panel != null)
        m_UIRootObjects.Add(UI.BackGround, Panel.GetComponent<Canvas>());

    Panel = Obj.transform.Find("Main");
    if (Panel != null)
        m_UIRootObjects.Add(UI.Main, Panel.GetComponent<Canvas>());

    Panel = Obj.transform.Find("Top");
    if (Panel != null)
        m_UIRootObjects.Add(UI.Top, Panel.GetComponent<Canvas>());

    Panel = Obj.transform.Find("Popup");
    if (Panel != null)
        m_UIRootObjects.Add(UI.Popup, Panel.GetComponent<Canvas>());

    Panel = Obj.transform.Find("Mask");
    if (Panel != null)
        m_UIRootObjects.Add(UI.Mask, Panel.GetComponent<Canvas>());

    Panel = Obj.transform.Find("Fade");
    if (Panel != null)
        m_UIRootObjects.Add(UI.Fade, Panel.GetComponent<Canvas>());

    Panel = Obj.transform.Find("TouchBlock");
    if (Panel != null)
        m_UIRootObjects.Add(UI.TouchBlock, Panel.GetComponent<Canvas>());

    m_EventSystem = Obj.transform.Find("EventSystem").GetComponent<EventSystem>();

    SwitchRoot();
}
```
<br/>

- 제네릭 기반의 UI 동적 로딩
```C#
public T Open<T>(UI Depth, string PrefabPath, List<object> Args = null, bool SetFirst = false, bool IsBundle = false) where T : UIElement
{
    if (m_UIDictionary.ContainsKey(typeof(T)))
    {
        if (m_UIDictionary[typeof(T)] != null)
        {
            m_UIDictionary[typeof(T)].gameObject.SetActive(true);
            m_UIDictionary[typeof(T)].OnOpen(Args);
            OnOpen?.Invoke(Depth, m_UIDictionary[typeof(T)]);
            return m_UIDictionary[typeof(T)] as T;
        }
        else
        {
            m_UIDictionary.TryRemove(typeof(T), out _);
        }
    }

    GameObject prefab;

    if (IsBundle)
    {
        PrefabPath = $"Prefab/{PrefabPath}";
        string AssetName = PrefabPath.Split('/').Last();
        prefab = ResourceLoader.LoadAsset<GameObject>(PrefabPath, AssetName);
    }
    else
    {
        prefab = ResourceLoader.LoadAssetResources<GameObject>(PrefabPath);
    }

    if (prefab == null)
        return null;

    GameObject obj = Instantiate(prefab, GetRootTransform(Depth));
    T comp = obj.GetComponent<T>();

    if (comp == null)
        return null;

    m_UIDictionary.TryAdd(typeof(T), comp);
    m_UIDictionary[typeof(T)].UIParent = Depth;
    m_UIDictionary[typeof(T)].UIName = PrefabPath;
    m_UIDictionary[typeof(T)].RectTransform = obj.GetComponent<RectTransform>();

    if (SetFirst)
        m_UIDictionary[typeof(T)].RectTransform.SetAsFirstSibling();

    m_UIDictionary[typeof(T)].Init();
    m_UIDictionary[typeof(T)].OnOpen(Args);
    OnOpen?.Invoke(Depth, m_UIDictionary[typeof(T)]);

    return comp;
}
```
<br/>

- UI 닫기 기능 관리
```C#
public void Close<T>(bool IsDestroy = true) where T : UIElement
{
    if (m_UIDictionary.ContainsKey(typeof(T)) && m_UIDictionary[typeof(T)] != null)
    {
        UIElement temp = m_UIDictionary[typeof(T)];
        temp.OnClose();

        if (IsDestroy)
        {
            m_UIDictionary.TryRemove(typeof(T), out _);
            Destroy(temp.gameObject);
        }
        else
            temp.gameObject.SetActive(false);
    }
}
```
<br/>

- UI 전체 자동 업데이트를 위한 Refresh 기능
```C#
 public void Refresh()
 {
     foreach (var Elements in m_UIDictionary)
     {
         if (Elements.Value != null && Elements.Value.gameObject.activeInHierarchy)
             Elements.Value.OnRefresh();
     }
 }
```
<br/>

- UI를 레이어별로 구분하여 UIManager가 UI를 정확한 우선순위와 규칙에 따라 배치할 수 있도록 Root UI 구조를 설계
<img src="https://github.com/user-attachments/assets/1cee13a4-3d1a-4adb-a381-8b8ee7ef2b05" width="50%"/>
<br/>
<br/>

### 2. SystemPopup 구현
<img src="https://github.com/user-attachments/assets/e890102f-a2c2-4371-9139-193bb282d0f6" width="50%"/>

#### 구현 이유
- 반복적으로 사용되는 공통 팝업 UI를 호출하기 위해
- 일관된 팝업 생성 로직으로 UI 관리의 안정성을 높히기 위해

#### 구현 방법
- SystemPopup 호출
```C#
public void OpenSystemPopup(MessageData Data)
{
    Open<Popup_System>(UI.Popup, "Prefabs/UI/Popup/Popup_System", new List<object> { Data });
}
```
<br/>

- Popup 유형을 enum으로 정의
```C#
public enum PopupType
{
    None,

    OkOnly,
    OkCancel,

    Max
}
```
<br/>

- Popup 에 필요한 정보를 정의
```C#
public class MessageData
{
    public PopupType Type;
    public string Title;
    public string Message;
    public UnityAction OkAction;
}
```
<br/>

- PopupType에 따른 UI 동적 구성 및 버튼 적용
```C#
public override void OnOpen(List<object> Args)
{
    if (Args.Count == 0)
    {
        Debug.LogWarning("MessageData is Null");
        return;
    }

    MyData = Args[0] as MessageData;

    if (!string.IsNullOrEmpty(MyData.Title))
    {
        Text_Title.text = MyData.Title;
    }
    else
    {
        Text_Title.text = "Notice";
    }
    
    Text_Message.text = MyData.Message;

    switch (MyData.Type)
    {
        case PopupType.OkOnly:
            {
                Btn_OK.gameObject.SetActive(true);
                Btn_Cancel.gameObject.SetActive(false);
            }
            break;
        case PopupType.OkCancel:
            {
                Btn_OK.gameObject.SetActive(true);
                Btn_Cancel.gameObject.SetActive(true);
            }
            break;
    }
}
```
<br/>

### 3. Photon을 이용한 멀티 플레이 구현
<p align="center">
  <img src="https://github.com/user-attachments/assets/acf17504-67c4-405c-9ca6-ee35ce4e7c47" width="49%"/>
  <img src="https://github.com/user-attachments/assets/493a5f77-5be8-4938-ba49-e5765738ac92" width="49%"/>
</p>

#### 구현 이유
- 서버를 직접 구축하지 않고도 실시간 멀티플레이 기능을 구현하기 위해
- 서버리스(Serverless) 구조 기반의 게임을 개발하기 위해

#### 구현 방법
- OnPhotonSerializeView를 통한 프레임 단위 동기화로 마스터클라이언트에서 계산한 게임 상태값을 상대 플레이어에게 빠르게 전달
- 실시간으로 데이터를 송수신해야하는 준비여부, 선택 스킬, 남은 턴 시간 데이터를 동기화 함
```C#
 public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
 {
     if (m_pvpModule == null)
     {
         m_pvpModule = BattleModule.Instance as PVPModule;
         if (m_pvpModule == null)
             return;
     }

     // 데이터를 전송할 때
     if (stream.IsWriting)
     {
         // Ingame Data
         stream.SendNext(m_pvpModule.IsMyReady);
         stream.SendNext(m_pvpModule.MySelectBtnNum);
         stream.SendNext(PhotonNetwork.IsMasterClient? m_pvpModule.CurTime : 0);
     }
     // 데이터를 받을 때
     else
     {
         // Ingame Data
         m_pvpModule.IsEnemyReady = (bool)stream.ReceiveNext();
         m_pvpModule.EnemySelectBtnNum = (int)stream.ReceiveNext();
         float curTime = (float)stream.ReceiveNext();

         if (!PhotonNetwork.IsMasterClient)
             m_pvpModule.CurTime = curTime;
     }
 }
```
<br/>

- 다른 클라이언트의 함수를 직접 호출하기 위해 RPC 함수 사용
- RPC 함수로 실시간 이모티콘 전송 기능을 구현
```C#
[PunRPC]
public void RPCPlayEmoticon(bool isLeft, int num)
{
    if (m_IngameWindow == null)
        m_IngameWindow = UIManager.Instance.GetOpened<IngameWindow>();

    if (m_IngameWindow != null)
        m_IngameWindow.SetEmoticon(isLeft, num);
}
```
<br/>

### 4. 금칙어 적용
<img src="https://github.com/user-attachments/assets/8650e8bd-839c-4ff8-9b9a-ee5ee70f5c0b" width="50%"/>

#### 구현 이유
- 부적절한 닉네임 사용을 제한하기 위해
- 유지보수와 업데이트가 쉽기 때문에 CSV 파일 기반 금칙어 구현

#### 구현 방법
- CSV 파일로 관리되는 금칙어 목록을 읽어와 메모리에 로드
```C#
private void LoadBannedWords()
{
    if (m_BannedWords.Count > 0) return;

    TextAsset csvFile = ResourceLoader.LoadAssetResources<TextAsset>("CSV/BannedWord/BannedWord");
    if (csvFile == null)
    {
        Debug.LogError("금칙어 CSV 파일을 찾을 수 없습니다.");
        return;
    }

    // 줄 단위로 분리
    string[] lines = csvFile.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

    foreach (var line in lines)
    {
        string word = line.Trim();
        if (!string.IsNullOrEmpty(word))
        {
            // 중복 방지
            if (!m_BannedWords.Contains(word))
                m_BannedWords.Add(word);
        }
    }

    Debug.Log($"금칙어 {m_BannedWords.Count}개 로드 완료");
}
```
<br/>

- 금칙어가 포함되어 있는지 체크
```C#
private bool IsBannedNickName(string nickname)
{
    foreach (var banned in m_BannedWords)
    {
        if (nickname.Contains(banned, StringComparison.OrdinalIgnoreCase))
            return true;
    }
    return false;
}
```
<br/>

### 5. 뒤끝 서버를 이용한 랭킹 구현
<img src="https://github.com/user-attachments/assets/ca5b51b5-cf98-40a8-a349-d212802c868f" width="50%"/>

#### 구현 이유
- 서버를 직접 구축하지 않고도 안정적이고 관리 가능한 랭킹 구현을 위해

#### 구현 방법
- 뒤끝 서버 설치 및 서버 접속
```C#
private void BackendSetup()
{
BackendReturnObject bro = Backend.Initialize(true);

if (bro.IsSuccess())
{
    Debug.Log("뒤끝 서버 연동 성공 : " + bro); // 성공일 경우 statusCode 204 Success
}
else
{
    Debug.LogError("뒤끝 서버 연동 실패 : " + bro); // 실패일 경우 statusCode 400대 에러 발생
}
}
```
<br/>

- 뒤끝 서버에서 비교할 데이터의 데이터 테이블 생성
<img src="https://github.com/user-attachments/assets/e9fc4d4c-2d22-4af5-bf05-ffef142f4600" width="50%"/>
<br/>
<br/>

```C#
// 데이터 테이블에 추가하는 함수
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

// Param : 데이터를 송수신할 때 사용하는 class
private Param GetUserDataParam()
{
    Param param = new Param();
    param.Add("RankPoint", DataManager.Instance.GetMyUserData().UserCommonData.RankPoint);
    param.Add("CharacterImg", DataManager.Instance.GetMyUserData().UserCommonData.Image);

    return param;
}
```
<br/>

- 뒤끝 서버 랭킹 추가
<img src="https://github.com/user-attachments/assets/eca92b5d-5868-4f2e-ba13-717a0060c88d" width="50%"/>
<br/>
<br/>

- 랭킹 데이터 갱신
```C#
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
```
<br/>

- 뒤끝 서버 Json 데이터를 파싱해서 나의 랭킹 불러오기
```C#
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
```
<br/>

- 뒤끝 서버 Json 데이터를 파싱해서 유저 랭킹 불러오기
```C#
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
```
<br/>

### 6. Google Admob 광고 구현
<img src="https://github.com/user-attachments/assets/5351db87-58f9-4f79-999d-e2a5df0e39c7" width="50%"/> 

#### 구현 이유
- 유저들이 광고를 시청함으로써, 게임의 수익화를 실현하기 위해

#### 구현 방법
- Google Admob에서 보상형 광고 구현
- SDK 초기화, 광고 로딩, 실패 처리, 보상 지급까지 모든 과정을 AdManager에서 통제
- 로딩 → 표시 → 보상 지급 → 재로딩 까지 한 사이클을 자동 처리하도록 설계
```C#
    private void Init()
    {
        if (IsTestMode)
        {
            // 테스트용 광고 단위 ID
#if UNITY_ANDROID
            m_AdRewardUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
            m_AdRewardUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            m_AdRewardUnitId = "unused";
#endif
        }
        else
        {
            // 실제 배포용 광고 단위 ID (수정 필요)
#if UNITY_ANDROID
            m_AdRewardUnitId = "ca-app-pub-5906820670754550/8653741011";
#elif UNITY_IPHONE
            m_AdRewardUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            m_AdRewardUnitId = "unused";
#endif
        }

        // Google Mobile Ads SDK 초기화
        MobileAds.Initialize((InitializationStatus initStatus) => { });
    }

// 리워드 광고 로드 및 표시
public void LoadRewardedAd(Action action)
{
    if (m_IsLoadingReward) return;
    m_IsLoadingReward = true;

    m_Action = action;

    // 이전 광고 객체가 남아 있다면 정리
    if (m_RewardedAd != null)
    {
        m_RewardedAd.Destroy();
        m_RewardedAd = null;
    }

    // 광고 요청 생성
    var adRequest = new AdRequest();

    // 광고 요청 전송
    RewardedAd.Load(m_AdRewardUnitId, adRequest,
        (RewardedAd ad, LoadAdError error) =>
        {
            m_IsLoadingReward = false;

            // 빌드에서 오류가 나기 때문에, 메인 스레드에서 실행해야 한다!
            UniTask.Post(() =>
            {
                // 에러 처리
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);

                    //광고 불러오기 실패
                    UIManager.Instance.OpenSystemPopup(new MessageData
                    {
                        Type = PopupType.OkOnly,
                        Title = "알림",
                        Message = "광고 불러오기를 실패 했습니다."
                    });

                    return;
                }

                Debug.LogWarning("Rewarded ad loaded with response : "
                      + ad.GetResponseInfo());

                m_RewardedAd = ad;
                RegisterEventHandlers(m_RewardedAd);
                ShowRewardedAd();
            });
        });
}

// 리워드 광고 표시
private void ShowRewardedAd()
{
    if (m_RewardedAd != null && m_RewardedAd.CanShowAd())
    {
        m_RewardedAd.Show((Reward reward) =>
        {
            Debug.Log($"User earned reward: {reward.Amount} {reward.Type}");

            UniTask.Post(() =>
            {
                try
                {
                    // 보상 처리 로직
                    m_Action?.Invoke();
                    m_Action = null;

                    // 완료
                    UIManager.Instance.OpenSystemPopup(new MessageData
                    {
                        Type = PopupType.OkOnly,
                        Title = "알림",
                        Message = "광고 골드를 획득 했습니다."
                    });
                }
                catch (Exception e)
                {
                    Debug.LogError($"[AdManager] Exception during reward handling: {e}");
                }
            });
        });
    }
    else
    {
        Debug.LogWarning("Rewarded ad is not ready yet.");
        LoadRewardedAd(m_Action);
    }
}

// 리워드 광고 이벤트 등록
private void RegisterEventHandlers(RewardedAd ad)
{
    ad.OnAdPaid += (AdValue adValue) =>
    {
        Debug.LogWarning(String.Format("Rewarded ad paid {0} {1}.",
            adValue.Value,
            adValue.CurrencyCode));
    };
    ad.OnAdImpressionRecorded += () =>
    {
        Debug.LogWarning("Rewarded ad recorded an impression.");
    };
    ad.OnAdClicked += () =>
    {
        Debug.LogWarning("Rewarded ad was clicked.");
    };
    ad.OnAdFullScreenContentOpened += () =>
    {
        Debug.LogWarning("Rewarded ad full screen content opened.");
    };
    ad.OnAdFullScreenContentClosed += () =>
    {
        Debug.LogWarning("Rewarded ad full screen content closed.");
    };
    ad.OnAdFullScreenContentFailed += (AdError error) =>
    {
        Debug.LogError("Rewarded ad failed to open full screen content " +
                       "with error : " + error);
        LoadRewardedAd(m_Action);
    };
}
```
<br/>

### 7. Mobile Notifications를 사용한 로컬 푸시 기능
<img src="https://github.com/user-attachments/assets/3f398d92-d012-4917-9791-0496ea45824e" width="50%"/>

#### 구현 이유
- 게임의 재접속 유도 및 플레이 지속률(리텐션)을 높이기 위해
- 서버 없이도 알림 기능 구현

#### 구현 방법
- 앱 시작 시 Notification 환경 초기화
```C#
private void RegisterAndroidChannel()
{
    var channel = new AndroidNotificationChannel()
    {
        Id = "my_channel_id",
        Name = "Real Fighter",
        Importance = Importance.High,
        Description = "Generic notifications",
    };
    AndroidNotificationCenter.RegisterNotificationChannel(channel);

    Debug.LogWarning("Register Android Channel");
}
```
<br/>

- 로컬 푸시의 종류 정의
```C#
public enum LocalPushType
{
    None,

    Test,
    FreeGold,

    Max
}
```
<br/>
​
- 로컬 푸시 예약 기능 구현
```C#
public void SchedulePushNotification(LocalPushType pushType, string title, string message, DateTime scheduleTime)
{
    // 예약 시간이 현재보다 미래인지 확인
    if (scheduleTime <= Util.DateTimeNow)
    {
        Debug.LogWarning("The time is earlier or equal to the current time. Please enter a valid future time.");
        return;
    }

    try
    {
        // Android: 알림 객체 생성 및 설정
        var notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = message;
        notification.FireTime = scheduleTime;
        notification.LargeIcon = "icon_0";
        notification.SmallIcon = "icon_1";
        notification.ShowInForeground = true;
        string channelId = "my_channel_id";

        int pushCode = AndroidNotificationCenter.SendNotification(notification, channelId);
        switch (pushType)
        {
            case LocalPushType.FreeGold:
                PlayerPrefs.SetInt(ClientDef.LOCALKEY_Push_FreeGold, pushCode);
                break;

            default:
                break;
        }
    }
    catch (Exception e)
    {
        Debug.LogWarning("푸시알람 예약 중 오류 발생: " + e.ToString());
    }
}
```
<br/>
<br/>
​
- 로컬 푸시 취소 기능 구현
```C#
public void CancelPushNotification(LocalPushType pushType)
{
    int pushCode = 0;
    switch (pushType)
    {
        case LocalPushType.FreeGold:
            pushCode = PlayerPrefs.GetInt(ClientDef.LOCALKEY_Push_FreeGold, 0);
            break;

        default:
            break;
    }

    if (pushCode == 0)
        return;

    AndroidNotificationCenter.CancelScheduledNotification(pushCode);

    Debug.LogWarning("Complete Cancel to Push Notification.");
}
```
<br/>
<br/>

### 8. 튜토리얼 구현
<img src="https://github.com/user-attachments/assets/462ee72c-3eae-4e0c-8efa-6e53d270ce69" width="50%"/>  

#### 구현 이유
- 캐주얼 게임 수준의 직관적인 UI 안내 시스템 구현을 위해
- 사용자가 처음 접하면 이해하기 어려운 규칙들을 설명하기 위해
- 유저가 쉽게 게임 플레이를 학습할 수 있도록 하기 위해
- 초반 이탈률을 줄이기 위해
- 대사 기반 튜토리얼, 클릭 기반 튜토리얼을 공용으로 사용할 수 있도록 설계
- 보상을 지급하여 게임의 성장 구조를 자연스럽게 맛보게하고, 초기 플레이 동기부여 강화를 위해

#### 구현 방법
- 튜토리얼 각 단계를 enum으로 정의
```C#
public enum TutorialStep
{
    None,

    LobbyChat_0,
    LobbyChat_1,
    ClickBattle,

    IngameChat_0,
    IngameChat_1,
    ClickAttack,
    ClickReady_0,
    IngameChat_2,
    IngameChat_3,
    IngameChat_4,
    IngameChat_5,
    IngameChat_6,
    IngameChat_7,
    ClickDeffence,
    ClickReady_1,
    IngameChat_8,
    IngameChat_9,
    IngameChat_10,
    IngameChat_11,
    IngameChat_12,
    IngameChat_13,
    IngameChat_14,
    ClickLobby,

    LobbyChat_2,
    LobbyChat_3,
    LobbyChat_4,
    LobbyChat_5,

    Max
}
```
<br/>

- 각각 튜토리얼 단계에 대한 정보를 class로 정의
```C#
public class TutorialData
{
    public float TimeScale = -1;
    public Vector2 MaskSize = new Vector2 (0, 0);
    public Vector2 MaskPos = new Vector2(0, 0);
    public Action Action_Mask = null;
    public string ChatText = string.Empty;
    public bool IsUp = false;
    public bool IsDown = false;
}
```
<br/>

- 공용으로 사용할 수 있는 TutorialMask 프리팹 생성
<img src="https://github.com/user-attachments/assets/6ef6e171-ad95-44d8-99e9-f2c664b77a34" width="50%"/>
<br/>

- 클릭 했을 때, 실행할 함수들을 TutorialController에 정리
```C#
public void OnClick_Ready()
{
    m_IngameWindow.OnClick_Ready();
}

public void OnClick_Deffence()
{
    m_IngameWindow.OnClick_MyDefences(0);
}
```
<br/>

- 각각 튜토리얼 스텝을 구현
```C#
public async UniTask StartTutorial(TutorialStep step)
{
    m_IsClickButton = false;

    switch (step)
    {

        case TutorialStep.LobbyChat_0:

            // TutorialController 가져올 때 까지 대기
            await UniTask.WaitUntil(() => UIManager.Instance.GetOpened<LobbyWindow>().GetComponent<TutorialController>() != null);
            m_TutorialController = UIManager.Instance.GetOpened<LobbyWindow>().GetComponent<TutorialController>();

            TutorialData data_0 = new TutorialData()
            {

                ChatText = "어서와라. 여긴 네가 실력을 증명해야 하는 결투장이다!",
                Action_Mask = async () =>
                {
                    if (m_IsClickButton)
                        return;

                    m_IsClickButton = true;

                    await StartTutorial(TutorialStep.LobbyChat_1);
                }
            };
            await SetTutorial(data_0);

            break;

        case TutorialStep.LobbyChat_1:

            TutorialData data_1 = new TutorialData()
            {
                ChatText = "긴 말 필요 없이 바로 실전으로 가보자고! BATTLE을 클릭해봐!",
                Action_Mask = async () =>
                {
                    if (m_IsClickButton)
                        return;

                    m_IsClickButton = true;

                    await StartTutorial(TutorialStep.ClickBattle);
                }
            };
            await SetTutorial(data_1);

            break;
    }
}
```
<br/>


## 💥 트러블 슈팅

### 1. Photon PUN2를 이용한 PVP 구현
#### 문제 상황
- 다른 클라이언트와 연동 가능한 서버가 필요

#### 해결 방안
##### Photon PUN2 사용
- 참고할 자료 및 내용이 많이 공유되어 있음
- 많은 개발자들이 대표적으로 가장 많이 사용
- 무료 버전으로도 비교적 많은 인원을 수용할 수 있음
- Shared 네트워크 구조 방식만 제공
##### Photon Fusion2 사용
- 많은 인원을 수용할 수 있음
- 직관적이고 간단하게 변수 동기화 가능
- 여러가지 네트워크 구조 방식 제공하고 네트워크 지연 보간 기능 제공
- 기능과 성능이 우수함
- 비교적 어려운 사용 방법
##### 서버 직접 개발
- 직접 게임 특성에 맞게 서버를 개발 가능
- 서버를 직접 개발하기에는 많은 시간과 노력이 필요
 
#### 의견 결정
##### Photon PUN2 사용
- 1:1 PVP 게임이므로, 많은 인원을 수용할 필요 없음
- 멀티 게임 개발 경험이 없기 때문에 많은 참고할 자료 및 내용이 필요
- 무료 버전으로도 충분히 기획한 게임 구현 가능
- 클라이언트 개발자로서 서버를 직접 개발할 필요성을 느끼지 못함
<br/>

### 2. OnPhotonSerializeView 동기화를 이용한 끊김 현상 개선
<img src="https://github.com/user-attachments/assets/f8dedc98-a67c-41a0-892b-8849f21cc587" width="50%"/>
<br/>
<br/>

#### PhotonTransformView 컴포넌트로 동기화
<img src="https://github.com/user-attachments/assets/ec1c8a19-9eda-4746-bbc6-4e96269e4043" width="50%"/>
<br/>
<br/>

- 간단하고 직관적으로 Position, Rotation 동기화 가능
- 끊김 현상, 딜레이가 심하게 발생
- 점프 시, Position Y 값을 제대로 동기화하지 못함
- 유니티 3D의 빠른 움직임을 동기화 할때는 적합하지 않음

#### OnPhotonSerializeView 함수를 통해 Transform 데이터 실시간 송수신으로 개선
- 실시간으로 전달된 데이터를 통해 각각 클라이언트에서 직접 움직임을 실행
```C#
public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
{
    // 데이터 보내기 (isMine == true)
    if (stream.IsWriting)
    {
        stream.SendNext(transform.position);
        stream.SendNext(transform.rotation);
    }
    // 데이터 받기 (isMine == false)
    else
    {
        _playerPosition = (Vector3)stream.ReceiveNext();
        _playerRotation = (Quaternion)stream.ReceiveNext();
    }
}
```
<br/>

- OnPhotonSerializeView 호출 빈도를 직접 설정
```C#
private void Awake()
{
    PhotonNetwork.SendRate = 60;
}
```
<br/>

#### 결과
<img src="https://github.com/user-attachments/assets/c0625e71-1016-48cc-8893-512b0c9db764" width="50%"/>
<br/>
<br/>

- 끊김 현상, 딜레이 개선
- 점프 시, Position Y 값을 제대로 동기화하지 못하는 현상 해결
<br/>

### 3. 뒤끝 서버를 이용한 랭킹 구현
<img src="https://github.com/user-attachments/assets/cd2b2bc5-b430-4ebd-8731-a8660d90513c" width="50%"/>
<br/>
<br/>

#### 문제 상황
- 랭킹 시스템에 사용할 서버 필요

#### 해결 방안
##### 뒤끝 서버 사용
- 이미 랭킹 시스템이 구현되어 있음
- 참고 가능한 자료, 정보가 비교적 많음
- 일정 사용량 초과 시, 발생하는 사용료가 타 서버에 비해서 비쌈
##### Firebase 서버 사용
- 매우 저렴한 비용
- 빠른 속도
- 직관적인 코드로 쉽게 사용 가능
##### 서버 직접 개발
- 직접 게임 특성에 맞게 서버를 개발 가능
- 서버를 직접 개발하기에는 많은 시간과 노력이 필요
 
#### 의견 결정
##### 뒤끝 서버 사용
- 이미 데이터를 비교해서 순위를 결정하는 랭킹 시스템이 구현되어 있기 때문에 사용 방법만 익히면 됨
- 멀티 구현이 미숙하기 때문에 참고 가능한 자료, 정보가 많은 뒤끝 서버로 구현하는 것이 좋다고 판단
- 랭킹 시스템만 구현하고 사용하는 유저가 적기 때문에 무료 버전의 사용량으로도 충분하다고 판단
<br/>

### 4. List 데이터 수정 시, 원본 데이터도 수정
#### 문제 상황
```C#
public class CharacterData
{
    public string Tag;
    public bool IsEquip;
    public int Level;
    public float Speed;
    public float Atk;
    public float Def;
}

public List<CharacterData> CharacterInventory;
public CharacterData[] CharacterDatas;

if (!CharacterIsGet(_data)) 
{
	GameManager.I.DataManager.DataWrapper.CharacterInventory.Add(_data);
}
```
<br/>

- CharacterData의 초기 데이터를 CharacterDatas 배열에서 관리
- 캐릭터를 얻게 되면 해당 CharacterData를 CharacterInventory List에 추가
- CharacterInventory List의 데이터가 수정되면, CharacterDatas 배열의 데이터도 함께 변경됨
- CharacterData가 class이기 때문에 Heap 영역에 할당되고, 참조 형식이기 때문에 원본 데이터도 함께 변경

#### 해결 방안
##### struct 사용
- struct는 stack 영역에 할당되고, 값 형식이기 때문에 근본적인 해결 가능
- 현재 구현한 데이터 저장 방식이 class 형식만 저장 가능하기 때문에 데이터 저장 방식 변경 필요
##### 별도의 인벤토리 List를 사용하지 않기
- List를 사용하지 않고 각각 데이터마다 IsGet이라는 bool 값을 설정
- 매번 CharacterDatas 배열 전체를 순회하여 캐릭터를 가지고 있는지 판단하기 때문에 비효율적이라고 판단
##### class를 참조하지 않고 값 형식 복사
- class를 값 형식으로 복사하는 깊은 복사 구현
- 객체의 내부까지 모두 복사하는 복잡한 깊은 복사를 굳이 구현하는 것은 비효율적이라고 판단
##### class에 수정하지 않을 원본 값을 추가
- class에 별도의 원본 데이터를 추가
 
#### 의견 결정
##### class에 수정하지 않을 원본 값을 추가
- 변경하지 않을 별도의 원본 데이터를 추가
- 근본적인 해결 방법은 아니지만, 가장 합리적인 해결 방법이라고 판단
```C#
public class CharacterData
{
    public string Tag;
    public bool IsEquip;
    public int Level;
    public float Speed;
    public float Atk;
    public float Def;
    public float OriginSpeed;
    public float OriginAtk;
    public float OriginDef;
}
```
<br/>

### 5. 상태 패턴을 사용한 Enemy 구현
<img src="https://github.com/user-attachments/assets/6a1b9b91-f1d2-46c2-b2b7-12ef1f311f80" width="50%"/>
<br/>
<br/>

#### 문제 상황
- 적의 독립적인 움직임을 구현하기 위한 방법이 필요

#### 해결 방안
##### 조건문과 스위치문 사용
- 간단하고 직관적으로 구현 가능
- 행동이 많다면 코드가 복잡해짐
##### 상태 패턴
- 새로운 상태 추가가 쉬움
- 확장성이 용이
  
#### 의견 결정
##### 상태 패턴으로 구현
- 특정 조건에 따라 각각 다른 행동을 할 수 있음
- 특정 행동을 추가해도 유지 관리가 용이
<br/>

### 6. Physics.Raycast 이용한 Enemy의 Player 인식
<img src="https://github.com/user-attachments/assets/ed55961a-99b7-4192-a008-ba255791fff5" width="50%"/>
<br/>
<br/>

#### 문제 상황
- 적의 Walk State에서 Attack State로 전환하기 위한 Player 인식 방법이 필요

#### 해결 방안
##### Physics.Raycast 사용
- 적의 정면으로 Ray를 쏴서 Player 인식 가능
- 직관적이고 간단한 방법
- 원거리 적의 경우 Ray의 길이만 길게 하면 됨
```C#
public bool CheckPlayer()
{
    Debug.DrawRay(transform.position + new Vector3(0, 0.7f, 0), transform.forward, Color.green, 1.5f);

    if (Physics.Raycast(transform.position + new Vector3(0, 0.7f, 0), transform.forward, out hit, 1.5f))
    { 
        if (hit.transform.CompareTag("Player"))
        {
            return true;
        }
    }

    return false;
}
```
<br/>

##### Player와의 거리로 판별
- 정확한 거리로 판별 가능
- magnitude 함수로 매 프레임 Player와 거리를 판별해야 함
```C#
Distance = (Target.transform.position - transform.position).magnitude;
```
<br/>

##### Collider로 판별
- 범위를 나타내는 추가 Collider 생성
<br/>
  
#### 의견 결정
##### Physics.Raycast 사용
- 가장 일반적인 방법으로 쉽게 사용 가능
- magnitude 함수를 매 프레임 실행하면 성능 저하의 원인이 됨
- Collider를 생성하면 Player의 공격에 Enemy가 인식되기 때문에 추가 설정이 필요
<br/>

## 📋 프로젝트 회고
### 잘한 점
 - 초기 기획과 크게 벗어나지 않게 게임 개발 성공
 - 초기 기획대로 멀티 PVP 구현 완료
 - 최대한 유료 에셋을 사용하여 게임 퀄리티 상승
 - 서버를 이용해서 랭킹을 구현하여 유저들의 흥미를 높임
 - 실제 런칭중인 모바일 게임과 유사한 퀄리티로 제작
 - Admob 배너 광고 적용 완료
<br/>

### 한계
- iOS 빌드에 대한 공부가 더 필요
- 목표 기간에 맞추지 못함
- PVP의 특성 상, 상대 유저와 동시에 접속하지 않으면 PVP를 즐길 수 없음
- 출시 후, 홍보 및 광고의 한계
- 목표 기간에 맞추지 못함
- 최적화를 제대로하지 못함
- PVP 플레이 시, 약간의 끊김과 딜레이 발생
<br/>

### 소감
이번 프로젝트에서는 두가지의 목표가 있었습니다. 첫번째는 멀티 게임 구현이고, 두번째는 유료 에셋을 사용하여 게임 퀄리티를 상승시키는 것입니다. 결과적으로 두가지 모두 성공적으로 시도하였습니다. 서버를 직접 개발할 수 없기 때문에 포톤 PUN2 서버와 뒤끝 서버를 사용해서 PVP와 랭킹 시스템을 구현했습니다. 그리고 캐릭터, UI, 데이터 저장 등 많은 개발자들이 사용하는 유료 에셋을 사용해서 게임 퀄리티를 최대한 높일 수 있었습니다. 완성 프로젝트를 직접 플레이 해 보니, 실제 런칭중인 모바일 게임을 플레이하는 느낌과 유사했습니다. 실제로 주변 지인들의 플레이 평도 이전 프로젝트에 비해 많이 좋아졌습니다. 처음 시도하는 멀티 서버에 대해서 공부를 하면서 프로젝트를 진행하다 보니, 예상 시간보다 많이 소요했습니다. 하지만 구현한 내용은 블로그에 잘 정리했기 때문에, 비슷한 기능을 구현할 때, 시간을 많이 절약할 수 있을 것 같습니다. 진행했던 프로젝트 중, 가장 실제 모바일 게임과 유사하게 개발한 정말 의미있는 프로젝트라고 생각합니다. 그리고 이전 프로젝트보다 앱 다운로드 수와 수익은 확실히 늘었지만, 역시나 개인의 홍보의 한계를 느낀 프로젝트였습니다.
  
