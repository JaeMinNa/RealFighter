public class User
{
    #region Property

    #region User Data
    // 유저의 기본 정보
    private static UserData_Common m_UserCommonData;
    #endregion User Data

    #endregion

    #region Get Set Method

    #region User - Set
    public static void SetUserCommonData(UserData_Common data)
    {
        m_UserCommonData = data;
    }

    //public static void SetUserGoodsData(UserData_Goods Goods)
    //{
    //    m_UserGoodsData = Goods;
    //}  
    #endregion User Data Set Method

    #region User - Get
    public static UserData_Common UserCommonData
    {
        get
        {
            return m_UserCommonData;
        }
    }
    #endregion User Data Get Method

    #endregion Get Set Method
}