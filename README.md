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

## 🔳 와이어 프레임
![image](https://github.com/user-attachments/assets/0099b12e-d1b4-4eeb-bbde-5fdb65517eed)


## 🧩 클라이언트 구조

### GameManager
![image](https://github.com/user-attachments/assets/38eb976c-8e82-4986-9e37-44602d08803a)

### Enemy
![image](https://github.com/user-attachments/assets/5a388a6d-9ddf-48c5-be22-65edad0331ef)


## ✏️ 구현 기능

### 1. 멀티 대전 입장
<img src="https://github.com/user-attachments/assets/ca915275-4091-425c-84de-1c4774e1dbed" width="50%"/>

#### 구현 이유
- PUN2 멀티 서버 연결
- PVP 시작 전, 대기방 구현

#### 구현 방법
- NetworkManager 생성 : 서버 접속, Room 생성 및 참가 관리
```C#
public void Connect()
{
    PhotonNetwork.ConnectUsingSettings();
}

public void JoinRandomOrCreateRoom()
{
    PhotonNetwork.JoinRandomOrCreateRoom(expectedMaxPlayers : 2, roomOptions : new RoomOptions() { MaxPlayers = 2 });
}

public override void OnJoinedRoom()
{
    Debug.Log("방참가완료");
    PhotonNetwork.Instantiate("PUN2/Room/RoomController", transform.position, Quaternion.identity);
}
``` 
​<br/>

- RoomController 생성 : OnPhotonSerializeView 함수를 통해, Room 데이터를 송수신
<img src="https://github.com/user-attachments/assets/4188147c-cc8d-45b1-a50b-b33c786f97c0" width="50%"/>
<br/>
<br/>

```C#
public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
{
    if (stream.IsWriting)
    {
        stream.SendNext(GameManager.I.DataManager.PlayerData.KoreaTag);
        stream.SendNext(GameManager.I.DataManager.PlayerData.Level);
        stream.SendNext(GameManager.I.DataManager.PlayerData.CharacterRank.ToString());
        stream.SendNext(GameManager.I.DataManager.GameData.UserName);
        stream.SendNext(GameManager.I.DataManager.GameData.RankPoint);
        stream.SendNext(GameManager.I.DataManager.PlayerData.Star);
        stream.SendNext(GameManager.I.DataManager.PlayerData.Tag);
    }
    else
    {
        _roomEnemyCharacterName = (string)stream.ReceiveNext();
        _roomEnemyCharacterLevel = (int)stream.ReceiveNext();
        _roomEnemyCharacterRank = (string)stream.ReceiveNext();
        _roomEnemyUserName = (string)stream.ReceiveNext();
        _roomEnemyRankPoint = (int)stream.ReceiveNext();
        _roomEnemyCharacterStar = (int)stream.ReceiveNext();
        _roomEnemyCharacterKorTag = (string)stream.ReceiveNext();
    }
}
```
<br/>

### 2. PUN2 멀티 채팅 구현
<img src="https://github.com/user-attachments/assets/3c3123b6-2357-4c31-8c5c-70267dd60e79" width="50%"/>

#### 구현 이유
- 입력한 string 데이터를 송수신

#### 구현 방법
- RPC 함수를 통해, 모든 Player가 동시에 함수 실행
```C#
private void SendChat()
{
    if (_photonView.IsMine)
    {
        string chat = PhotonNetwork.NickName + " : " + _networkManager.ChatInputText.text;
        _photonView.RPC("ChatRPC", RpcTarget.All, chat);
        _networkManager.ChatInputText.text = "";
    }
}

[PunRPC]
public void ChatRPC(string str)
{
    bool isInput = false;

    for (int i = 0; i < _chatTexts.Length; i++)
    {
        if (_chatTexts[i].text == "")
        {
            isInput = true;
            _chatTexts[i].text = str;
            break;
        }
    }

    if (!isInput)
    {
        for (int i = 1; i < _chatTexts.Length; i++)
        {
            _chatTexts[i - 1].text = _chatTexts[i].text;
        }

        _chatTexts[_chatTexts.Length - 1].text = str;
    }
}
```
<br/>

### 3. PUN2 멀티 애니메이션 동기화
<img src="https://github.com/user-attachments/assets/6baf68b3-0a0b-416c-924b-703abcb2b105" width="50%"/>

#### 구현 이유
- 멀티 PVP에서 애니메이션을 동기화

#### 구현 방법
- PhotonAnimatorView 컴포넌트 추가
<img src="https://github.com/user-attachments/assets/05fb9546-1e0b-41c4-8435-0e27bb8e57a3" width="50%"/>
<br/>
<br/>

- PhotonView 컴포넌트 추가 및 Observed Components에 PhotonAnimatorView 추가
<img src="https://github.com/user-attachments/assets/caa1a31b-b577-4593-b510-28426f1ff30c" width="50%"/>
<br/>
<br/>

- Synchronize Parameters에서 Bool Parameter를 Continuous 설정
- Trigger Parameter의 경우, RPC 함수를 통해 동기화해야 하므로, Disabled로 설정
<img src="https://github.com/user-attachments/assets/fe0ac5fd-96f7-44a6-8ec1-49d043f0c73a" width="50%"/>
<br/>
<br/>

### 4. PUN2 멀티 전투 구현
<img src="https://github.com/user-attachments/assets/eea1b5b6-0044-4df5-b82e-da66317591f7" width="50%"/>

#### 구현 이유
- 멀티 대전의 전투 시스템 구현을 위해

#### 구현 방법
- Weapon Collider를 활성화하는 함수 작성
<img src="https://github.com/user-attachments/assets/30d10b39-db25-4103-8f9e-aacb0703196f" width="50%"/>
<br/>
<br/>

```C#
public void AttackColliderActive(float time)
{
	for (int i = 0; i < _weaponColliders.Length; i++)
	{
	    _weaponColliders[i].enabled = true;
	}
	
	StartCoroutine(COAttackColliderInactive(time));
}

private IEnumerator COAttackColliderInactive(float time)
{
	yield return new WaitForSeconds(time);
	
	for (int i = 0; i < _weaponColliders.Length; i++)
	{
	    _weaponColliders[i].enabled = false;
	}
}
```
<br/>

- Animation Events 등록
<img src="https://github.com/user-attachments/assets/04cffd69-7ba3-4f76-85e7-ee4d40f5b176" width="50%"/>
<br/>
<br/>

- OnPhotonSerializeView 함수를 통해, 능력치 데이터 송수신
```C#
public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
{
    // 데이터 보내기 (isMine == true)
    if (stream.IsWriting)
    {
        stream.SendNext(GameManager.I.DataManager.PlayerData.Atk);
        stream.SendNext(GameManager.I.DataManager.PlayerData.SkillAtk);
        stream.SendNext(GameManager.I.DataManager.PlayerData.Def);
    }
    // 데이터 받기 (isMine == false)
    else
    {
        Atk = (float)stream.ReceiveNext();
        SkillAtk = (float)stream.ReceiveNext();
        Def = (float)stream.ReceiveNext();
    }
}
``` 
​<br/>

- RPC를 통해, 공격 애니메이션 실행
```C#
if (PhotonView.IsMine) PhotonView.RPC("PlayerAttackRPC", RpcTarget.AllViaServer);

[PunRPC]
public void PlayerAttackRPC()
{
    _anim.SetTrigger("Attack");
}
```
<br/>

- RPC를 통해, 넉백 구현
```C#
private void OnTriggerEnter(Collider other)
{
    if (other.gameObject.CompareTag("Player")
    {
	if (!_photonView.IsMine)
	{
	    _atk = _playerCharacter.Atk;
	    other.GetComponent<PlayerCharacter>().PhotonView.RPC("RPCPlayerNuckback", RpcTarget.AllViaServer, _playerCharacter.transform.position, _atk);
	}
    }
}

[PunRPC]
public void RPCPlayerNuckback(Vector3 attackPosition, float power)
{
    if (PhotonView.IsMine) PhotonView.RPC("PlayerHitRPC", RpcTarget.AllViaServer);
    Vector3 dir = (transform.position - attackPosition).normalized;

    if (PhotonView.IsMine) _rigidbody.velocity = new Vector3(dir.x, 0, dir.z) * (power - _playerData.Def);
    else _rigidbody.velocity = new Vector3(dir.x, 0, dir.z) * (power - Def);

    transform.LookAt(attackPosition);
}

[PunRPC]
public void PlayerHitRPC()
{
    _anim.SetTrigger("Hit");
}
```
<br/>

### 5. 랭킹 구현
<img src="https://github.com/user-attachments/assets/cfbc5009-0507-46f7-a827-3ff786e20206" width="50%"/>

#### 구현 이유
- 경쟁 심리를 이용해서 유저들이 더 게임을 플레이 하도록 하기 위해

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
    BackendReturnObject bro = Backend.GameData.Insert("USER_DATA", param);

    if (bro.IsSuccess())
    {
        Debug.Log("데이터 추가를 성공했습니다");
    }
    else
    {
        Debug.Log("데이터 추가를 실패했습니다");
    }
}

// Param : 데이터를 송수신할 때 사용하는 class
private Param GetUserDataParam()
{
    Param param = new Param();
    param.Add("RankPoint", GameManager.I.DataManager.GameData.RankPoint);
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
// 데이터 테이블에 추가하는 함수
private void UpdateMyRankData(int value)
{
	string rowInDate = string.Empty;

	// 랭킹 데이터를 업데이트하려면 게임 데이터에서 사용하는 데이터의 inDate 값 필요
	BackendReturnObject bro = Backend.GameData.GetMyData("USER_DATA", new Where());
	
	if (!bro.IsSuccess())
	{
	    Debug.LogError("랭킹 업데이트를 위한 데이터 조회 중 문제가 발생했습니다.");
	    return;
	}
	
	Debug.Log("랭킹 업데이트를 위한 데이터 조회에 성공했습니다.");
	
	if(bro.FlattenRows().Count > 0)
	{
	    rowInDate = bro.FlattenRows()[0]["inDate"].ToString();
	}
	else
	{
	    Debug.LogError("데이터가 존재하지 않습니다.");
	}
	
	Param param = new Param()
	{
	    {"RankPoint",  value}
	};
	
	// 해당 데이터테이블의 데이터를 갱신하고, 랭킹 데이터 정보 갱신
	bro = Backend.URank.User.UpdateUserScore(RANK_UUID, "USER_DATA", rowInDate, param);
	
	if(bro.IsSuccess())
	{
	    Debug.Log("랭킹 등록에 성공했습니다.");
	}
	else
	{
	    Debug.LogError("랭킹 등록에 실패했습니다.");
	}
}
```
<br/>

- 뒤끝 서버 Json 데이터를 파싱해서 나의 랭킹 불러오기
```C#
public void GetMyRank()
{
    // 내 랭킹 정보 불러오기
    BackendReturnObject bro = Backend.URank.User.GetMyRank(RANK_UUID);

    if(bro.IsSuccess())
    {
        try
        {
            JsonData rankDataJson = bro.FlattenRows();

            // 받아온 데이터의 개수가 0 -> 데이터가 없음
            if (rankDataJson.Count <= 0)
            {
                Debug.Log("나의 랭킹 데이터가 존재하지 않습니다.");
            }
            else
            {
                _rankPoint = int.Parse(rankDataJson[0]["score"].ToString());
                _rank = int.Parse(rankDataJson[0]["rank"].ToString());
                _userName = rankDataJson[0]["nickname"].ToString();
            }
        }
        // 나의 랭킹 정보 JSON 데이터 파싱에 실패했을 때
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }
    else
    {
        // 나의 랭킹 정보를 불러오는데 실패했을 때
    }
}
```
<br/>

- 뒤끝 서버 Json 데이터를 파싱해서 유저 랭킹 불러오기
```C#
private const int MAX_RANK_LIST = 10;

public void GetRankList()
{
    // 랭킹 테이블에 있는 유저의 offset ~ offset + limit 순위 랭킹 정보를 불러옴
    BackendReturnObject bro = Backend.URank.User.GetRankList(RANK_UUID, MAX_RANK_LIST, 0);

    if(bro.IsSuccess())
    {
        // JSON 데이터 파싱 성공
        try
        {
            Debug.Log("랭킹 조회에 성공했습니다.");
            JsonData rankDataJson = bro.FlattenRows();

            // 받아온 데이터의 개수가 0 -> 데이터가 없음
            if(rankDataJson.Count <= 0)
            {
                Debug.Log("랭킹 데이터가 존재하지 않습니다.");
            }
            else
            {
                int rankCount = rankDataJson.Count;

                // 받아온 rank 데이터의 숫자만큼 데이터 출력
                for (int i = 0; i < rankCount; i++)
                {
                    _rankPoint = int.Parse(rankDataJson[i]["score"].ToString());
                    _rank = int.Parse(rankDataJson[i]["rank"].ToString());
                    _userName = rankDataJson[i]["nickname"].ToString();
                }
                // rankCount가 Max값만큼 존재하지 않을 때, 나머지 랭킹
                for (int i = rankCount; i < MAX_RANK_LIST; i++)
                {
                    // 랭킹 데이터 비활성화
                }
            }
        }
        // JSON 데이터 파싱 실패
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }
    else
    {
        Debug.LogError("랭킹 조회에 실패했습니다.");
    }
}
```
<br/>

### 6. Admob 광고 구현
<img src="https://github.com/user-attachments/assets/cee62c9f-3df8-4753-bbd6-969521b3afab" width="50%"/> 

#### 구현 이유
- 유저들이 광고를 시청하면 Coin을 얻게하기 위해
- 유저들이 광고를 시청함으로써, 게임의 수익화를 실현하기 위해

#### 구현 방법
- Google Admob에서 보상형 광고와 배너 광고 생성
<img src="https://github.com/user-attachments/assets/b98f1d69-bf1b-4164-8eab-1878be77beb0" width="50%"/>
<br/>
<br/>

- Unity plugin을 설치 후, 프로젝트에 Import
- 테스트 ID와 광고 ID를 적용해서 스크립트 작성

```C#
public void Init()
{
	if (IsTestMode)
	{
	    // 테스트용 ID
	    _adRewardUnitId = "ca-app-pub-3940256099942544/5224354917";
	    _adBannerUnitId = "ca-app-pub-3940256099942544/6300978111";
	}
	else
	{
	    #if UNITY_ANDROID
	    // 광고 ID
	    _adRewardUnitId = "";
	    _adBannerUnitId = "";
	    #elif UNITY_IPHONE
	    // 테스트용 ID
	    _adRewardUnitId = "ca-app-pub-3940256099942544/1712485313";
	    _adBannerUnitId = "ca-app-pub-3940256099942544/2934735716";
	    #else
	    _adRewardUnitId = "unused";
	    _adBannerUnitId = "unused";
	    #endif
	}

	MobileAds.Initialize((InitializationStatus initStatus) => { });
}

//보상형 광고 로드, 사용 시 호출
public void LoadRewardedAd()
{
	if (_rewardedAd != null)
	{
	    _rewardedAd.Destroy();
	    _rewardedAd = null;
	}

	var adRequest = new AdRequest();

	RewardedAd.Load(_adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
	{
		if (error != null || ad == null)
		{
		    Debug.LogError("Rewarded ad failed to load an ad " +
				   "with error : " + error);
		    return;
		}

		Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());

		_rewardedAd = ad;
		RegisterEventHandlers(_rewardedAd);
		ShowRewardedAd();
	});
}

public void ShowRewardedAd()
{
	if (_rewardedAd != null && _rewardedAd.CanShowAd())
	{
	    _rewardedAd.Show((Reward reward) =>
	    {
		// 광고 보상 입력
	    });
	}
}

private void RegisterEventHandlers(RewardedAd ad)
{
	ad.OnAdPaid += (AdValue adValue) => { };
	ad.OnAdImpressionRecorded += () => { };
	ad.OnAdClicked += () => { };
	ad.OnAdFullScreenContentOpened += () => { };
	ad.OnAdFullScreenContentClosed += () => { }; // 광고 창을 닫을 때, 실행할 내용
	// 광고 불러오기를 실패했을 때
	ad.OnAdFullScreenContentFailed += (AdError error) =>
	{
	    LoadRewardedAd();
	};
}

//배너 광고 로드, 사용 시 호출
public void LoadBannerAd()
{
	if (_bannerView == null)
	{
	    CreateBannerView();
	}
	
	var adRequest = new AdRequest();
	_bannerView.LoadAd(adRequest);
}

//배너 광고 보여주기
private void CreateBannerView()
{
	if (_bannerView != null)
	{
	    DestroyAd();
	}
	
	_bannerView = new BannerView(_adBannerUnitId, AdSize.Banner, AdPosition.Top);
}

//배너 광고 제거
public void DestroyAd()
{
	if (_bannerView != null)
	{
	    _bannerView.Destroy();
	    _bannerView = null;
	}
}
```
<br/>

### 7. Enemy 상태 패턴 구현
<img src="https://github.com/user-attachments/assets/3b2e2a93-5a9b-42e1-9e36-638c8fd8ec4c" width="50%"/>

#### 구현 이유
- 다양한 상태를 가진 Enemy 움직임 구현
- 끊임없이 독립적으로 행동해야 함
- 유연한 상태 관리로 필요에 따라 상태를 추가하거나 수정이 가능해야 함

#### 구현 방법
- IState 인터페이스 : 구체적인 상태 클래스로 연결할 수 있도록 설정
```C#
public interface IEnemyState
{
    void Handle(EnemyController controller);
}
``` 
​
- Context 스크립트 : 클라이언트가 객체의 내부 상태를 변경할 수 있도록 요청하는 인터페이스를 정의
```C#
public void Transition()
{
    CurrentState.Handle(_enemyController);
}

public void Transition(IEnemyState state)
{
    CurrentState = state;
    CurrentState.Handle(_enemyController);
}
```
​
- EnemyController 스크립트 : 각 State 컴포넌트 연결, State 실행
```C#

// Start문과 동일하게 사용
private void Start()
{
	_enemyStateContext = new EnemyStateContext(this);
	_walkState = gameObject.AddComponent<EnemyWalkState>();
}

public void WalkStart()
{
	_enemyStateContext.Transition(_walkState);
}
```
<br/>
<br/>

- State 스크립트 : 각 State를 정의, State 변경 조건 설정
<img src="https://github.com/user-attachments/assets/b85edb66-b5ad-4c2b-b50e-de1237b26c55" width="50%"/>
<br/>
<br/>

```C#
// Start문과 동일하게 사용
public void Handle(EnemyController enemyController)
{
	if (!_enemyController)
	    _enemyController = enemyController;
	
	Debug.Log("Walk 상태 시작");
	StartCoroutine(COUpdate());
}

// Update문과 동일하게 사용
private IEnumerator COUpdate()
{
	while (true)
	{
	    _dir = (_enemyController.Target.transform.position - transform.position).normalized;
	    transform.position += _dir * _enemyController.Speed * Time.deltaTime;
	
	    if (_enemyController.CheckPlayer())
	    {
		_enemyController.AttackStart();
		_enemyController.EnemyAnimator.SetBool("Attack", true);
		break;
	    }
	
	    if (_enemyController.IsHit_attack || _enemyController.IsHit_skill)
	    {
		_enemyController.HitStart();
		_enemyController.EnemyAnimator.SetTrigger("Hit");
		break;
	    }
		
	    yield return null;
	}
}
```
<br/>

### 8. 비동기 방식 로딩 씬 구현
<img src="https://github.com/user-attachments/assets/a26ffcc9-fdc0-4628-ba8f-952d1d6d90ba" width="50%"/>  

#### 구현 이유
- 씬이 전환 될 때, 다음 씬에서 사용될 리소스들을 읽어와서 게임을 위한 준비 작업 필요
- 로딩 화면이 없다면 가만히 멈춘 화면이나 까만 화면만 보일 수 있음
- 씬이 전환 될 때, 지루한 대기 시간을 지루하지 않게 하기 위해

#### 구현 방법
- 씬을 불러오는 도중에 다른 작업이 가능 비동기 방식 씬 전환 구현
```C#
IEnumerator LoadScene()
{
    yield return null;
    AsyncOperation op = SceneManager.LoadSceneAsync(NextScene);
    op.allowSceneActivation = false;
    float timer = 0.0f;
    while (!op.isDone)
    {
        yield return null;
        timer += Time.deltaTime;
        if (op.progress < 0.9f)
        {
            _loadingBar.value = Mathf.Lerp(_loadingBar.value, op.progress, timer);
            if (_loadingBar.value >= op.progress)
            {
                timer = 0f;
            }
        }
        else
        {
            _loadingBar.value = Mathf.Lerp(_loadingBar.value, 1f, timer);
            if (_loadingBar.value == 1.0f)
            {
                op.allowSceneActivation = true;
                yield break;
            }
        }
    }
}
```

- 리소스 로딩이 끝나기 전에 씬 로딩 되는 것을 막기 위해 allowSceneActivation을 false로 설정
- allowSceneActivation을 false로 90% 로드 한 상태로 대기하고, true 변경 시, 남은 부분을 로드하고 씬 이동
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
  
