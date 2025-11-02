using Photon.Pun;
using UnityEngine;

public class PhotonController : MonoBehaviour, IPunObservable
{
    public PhotonView PhotonView = null;
    public string MyNickName { get; private set; } = string.Empty;
    public int MyScore { get; private set; } = 0;
    public string MyImage { get; private set; } = string.Empty;
    public string MyHeroName { get; private set; } = string.Empty;
    public int[] MyHeroSkillproficiencies { get; private set; } = new int[3];
    public int MyHeroLevel { get; private set; } = 0;
    public int MyHeroExp { get; private set; } = 0;
    public int MyHeroGrade { get; private set; } = 0;
    public int MyHeroGradeExp { get; private set; } = 0;

    private PVPModule m_pvpModule = null;

    private void Awake()
    {
        m_pvpModule = BattleModule.Instance as PVPModule;
    }

    #region Photon
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (m_pvpModule == null)
        {
            m_pvpModule = BattleModule.Instance as PVPModule;
            if (m_pvpModule == null)
                return;
        }

        // 내가 보낼 때
        if (stream.IsWriting)
        {
            // Ingame Data
            stream.SendNext(m_pvpModule.IsMyReady);
            stream.SendNext(m_pvpModule.MySelectBtnNum);
            stream.SendNext(PhotonNetwork.IsMasterClient? m_pvpModule.CurTime : 0);
        }
        // 내가 받을 때
        else
        {
            // Ingame Data
            m_pvpModule.IsEnemyReady = (bool)stream.ReceiveNext();
            m_pvpModule.EnemySelectBtnNum = (int)stream.ReceiveNext();
            float curTime = (float)stream.ReceiveNext();

            // 마스터가 아닌 쪽만 CurTime 반영
            if (!PhotonNetwork.IsMasterClient)
                m_pvpModule.CurTime = curTime;
        }
    }

    [PunRPC]
    public void RPCSetMyData(string nick, int score, string image, string heroName,
                          int skill0, int skill1, int skill2, int level, int exp, int grade, int gradeExp)
    {
        MyNickName = nick;
        MyScore = score;
        MyImage = image;
        MyHeroName = heroName;
        MyHeroSkillproficiencies[0] = skill0;
        MyHeroSkillproficiencies[1] = skill1;
        MyHeroSkillproficiencies[2] = skill2;
        MyHeroLevel = level;
        MyHeroExp = exp;
        MyHeroGrade = grade;
        MyHeroGradeExp = gradeExp;
    }
    #endregion
}
