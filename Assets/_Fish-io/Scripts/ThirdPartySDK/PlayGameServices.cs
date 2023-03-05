using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
#if UNITY_ANDROID && GOOGLE_PLAY_GAMES
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

public class PlayGameServices : MonoBehaviour
{
    void Start()
    {
#if UNITY_ANDROID && GOOGLE_PLAY_GAMES && (!GLAY_PLUGIN)
        Debug.Log("PlayGameServices: Start()");
        // Select the Google Play Games platform as our social platform implementation
        GooglePlayGames.PlayGamesPlatform.Activate();
        Debug.Log("PlayGameServices: Activate()");
        Social.localUser.Authenticate((bool success) =>
        {
            Debug.Log("PlayGameServices: login: " + success);
            if (success)
            {
                //サインイン成功
            }
            else
            {
                //サインイン失敗
            }
        });
        Debug.Log("PlayGameServices: Authenticate()");
#endif
#if GLAY_PLUGIN
        //Login to Game Servicies
        GameServices.Instance.LogIn(LoginResult);
#endif
    }
#if GLAY_PLUGIN
    //Automatically called when Login is complete 
    private void LoginResult(bool success)
    {
        if (success == true)
        {
            //Login was successful
        }
        else
        {
            //Login failed
        }

        Debug.Log("Login success: " + success);
        GleyGameServices.ScreenWriter.Write("Login success: " + success);
    }
#endif
}
