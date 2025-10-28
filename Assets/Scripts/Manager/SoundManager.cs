using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    private AudioSource m_BGMAudioSource = null;
    private AudioSource m_PlayerSFXAuidoSource = null;
    private AudioSource[] m_EtcSFXAudioSources = new AudioSource[5];

    private Dictionary<string, AudioClip> m_BgmDic = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> m_SfxDic = new Dictionary<string, AudioClip>();
    private int m_SoundNum;
    private float m_MaxDistance = 50f;
    private float m_StartVolume = 0.5f;

    public static SoundManager Instance
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

                GameObject managerObj = GameObject.Find("[Managers]/SoundManager");
                if (managerObj == null)
                {
                    managerObj = new GameObject("SoundManager");
                    managerObj.transform.SetParent(Obj.transform);
                }

                m_Instance = managerObj.GetComponent<SoundManager>();
                if (m_Instance == null)
                {
                    m_Instance = managerObj.AddComponent<SoundManager>();
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
        //var mainCamera = Camera.main;
        transform.AddComponent<AudioListener>();
        m_BGMAudioSource = transform.AddComponent<AudioSource>();
        m_PlayerSFXAuidoSource = transform.AddComponent<AudioSource>();
        for (int index = 0; index < m_EtcSFXAudioSources.Length; ++index)
            m_EtcSFXAudioSources[index] = transform.AddComponent<AudioSource>();

        // BGM
        m_BGMAudioSource.loop = true;
        m_BGMAudioSource.volume = m_StartVolume;

        // SFX (Player)
        m_PlayerSFXAuidoSource.playOnAwake = false;
        m_PlayerSFXAuidoSource.volume = m_StartVolume;

        // SFX (Etc)
        for (int index = 0; index < m_EtcSFXAudioSources.Length; ++index)
        {
            m_EtcSFXAudioSources[index].playOnAwake = false;
            m_EtcSFXAudioSources[index].volume = m_StartVolume;
        }

        // BGM
        m_BgmDic.Add("BGM_Title", ResourceLoader.LoadAssetResources<AudioClip>("Sound/BGM/6. Throne of the Fjords"));
        m_BgmDic.Add("BGM_Lobby", ResourceLoader.LoadAssetResources<AudioClip>("Sound/BGM/5. Odin's Whisper"));
        m_BgmDic.Add("BGM_Battle", ResourceLoader.LoadAssetResources<AudioClip>("Sound/BGM/4. Frostbound Horizons"));

        // SFX
        m_SfxDic.Add("ButtonClick", ResourceLoader.LoadAssetResources<AudioClip>("Sound/SFX/UI/ButtonClick"));
        m_SfxDic.Add("Punch0", ResourceLoader.LoadAssetResources<AudioClip>("Sound/SFX/Ingame/Punch0"));
        m_SfxDic.Add("Punch1", ResourceLoader.LoadAssetResources<AudioClip>("Sound/SFX/Ingame/Punch1"));
        m_SfxDic.Add("Punch2", ResourceLoader.LoadAssetResources<AudioClip>("Sound/SFX/Ingame/Punch2"));
        m_SfxDic.Add("StartSkill", ResourceLoader.LoadAssetResources<AudioClip>("Sound/SFX/Ingame/Skill"));
        m_SfxDic.Add("Hit", ResourceLoader.LoadAssetResources<AudioClip>("Sound/SFX/Ingame/Hit"));
        m_SfxDic.Add("Win", ResourceLoader.LoadAssetResources<AudioClip>("Sound/SFX/Ingame/Win"));
        m_SfxDic.Add("Lose", ResourceLoader.LoadAssetResources<AudioClip>("Sound/SFX/Ingame/Lose"));
        m_SfxDic.Add("StartGame", ResourceLoader.LoadAssetResources<AudioClip>("Sound/SFX/Ingame/StartGame"));
    }
    #endregion

    #region Public Method
    // 사운드가 거리에 따라 볼륨 조절이 필요할 때
    public void StartSFX(string name, Vector3 position)
    {
        var MyCharacter = GameObject.Find($"{DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.HeroName}(Clone)");
        if (MyCharacter == null)
            return;

        m_SoundNum = m_SoundNum % m_EtcSFXAudioSources.Length;

        float distance = Vector3.Distance(position, MyCharacter.transform.position);
        float volume = 1f - (distance / m_MaxDistance);
        m_EtcSFXAudioSources[m_SoundNum].volume = Mathf.Clamp01(volume) * m_StartVolume;
        m_EtcSFXAudioSources[m_SoundNum].PlayOneShot(m_SfxDic[name]);

        m_SoundNum++;
    }

    // Player에서 출력되는 사운드
    public void StartSFX(string name)
    {
        m_PlayerSFXAuidoSource.PlayOneShot(m_SfxDic[name]);
    }

    public void StartBGM(string name)
    {
        m_BGMAudioSource.Stop();
        m_BGMAudioSource.clip = m_BgmDic[name];
        m_BGMAudioSource.Play();
    }

    public void StopBGM()
    {
        if (m_BGMAudioSource != null)
            m_BGMAudioSource.Stop();
    }

    public void StartSFX_Punch()
    {
        List<AudioClip> sources = new List<AudioClip>();
        sources.Add(m_SfxDic["Punch0"]);
        sources.Add(m_SfxDic["Punch1"]);
        sources.Add(m_SfxDic["Punch2"]);

        int value = RandomUtil.GetRandomIndex(0, sources.Count - 1);

        m_PlayerSFXAuidoSource.PlayOneShot(sources[value]);
    }
    #endregion
}
