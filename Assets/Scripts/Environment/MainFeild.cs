using UnityEngine;

public class MainFeild : MonoBehaviour
{

    #region Cashed Object
    [Header("Player")]
    [SerializeField] private Transform Trs_Player_Left = null;
    [SerializeField] private Transform Trs_Player_Right = null;
    #endregion

    private void Start()
    {
        //altarScr.ToggleDemonicAltar();
        //HellGateScr.ToggleHellGate();
        //BloodPoolScr.F_ToggleBloodPool();
    }

    #region Public Method
    public Transform GetTransformPlayer(bool isLeft)
    {
        if(isLeft)
            return Trs_Player_Left;

        return Trs_Player_Right;
    }
    #endregion
}
