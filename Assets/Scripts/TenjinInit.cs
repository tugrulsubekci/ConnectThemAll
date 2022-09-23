using UnityEngine;
using System.Collections;

public class TenjinInit : MonoBehaviour
{
    void Start()
    {
        SetGooglePlay();
        TenjinConnect();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            TenjinConnect();
        }
    }

    public void TenjinConnect()
    {
        BaseTenjin instance = Tenjin.getInstance("FCMSCTCWXB5PT2TMGJGHAROBCK8QE6BY");

        // Sends install/open event to Tenjin
        instance.Connect();
    }
    private void SetGooglePlay()
    {
        BaseTenjin instance = Tenjin.getInstance("FCMSCTCWXB5PT2TMGJGHAROBCK8QE6BY");

        instance.SetAppStoreType(AppStoreType.googleplay);
    }
}