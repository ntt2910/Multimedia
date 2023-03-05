using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;

#endif

public class EditorHelper : MonoBehaviour
{
    const string MENU_NAME = "●▬▬๑۩۩๑▬▬●";
    private const string ALT = "&";

#if UNITY_EDITOR
    static void LoadSceneByName(string _nameScene)
    {
        EditorApplication.SaveCurrentSceneIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/_Fish-io/Scenes/" + _nameScene + ".unity");
    }
    
    [MenuItem(MENU_NAME + "/-> PLAY GAME <-")]
    static void PlayGame()
    {
        EditorSceneManager.OpenScene("Assets/_Fish-io/Scenes/Start.unity");
        EditorApplication.ExecuteMenuItem("Edit/Play");
    }
    
    [MenuItem(MENU_NAME + "/Scenes/Start " + ALT + "0")]
    static void OpenLogo()
    {
        LoadSceneByName("Start");
    }
    
    [MenuItem(MENU_NAME + "/Scenes/Game " + ALT + "1")]
    static void OpenMenu()
    {
        LoadSceneByName("Game");
    }

    [MenuItem(MENU_NAME + "/Clear All PlayerPrefs")]
    static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
    
    [MenuItem(MENU_NAME + "/-> TEST <-")]
    static void Test()
    {
        
        var lsRandomInt = new List<int>();
        for (int i = 0; i < 10; i++)
        {
            lsRandomInt.Add(i);
        }

        var lsFishPlayerIndex = Helper.DisruptiveListObject(lsRandomInt);
        for (int i = 0; i < lsFishPlayerIndex.Count; i++)
        {
            Debug.Log(lsFishPlayerIndex[i]);
        }
        
        // var textFile = Resources.Load<TextAsset>("PlayerName");
        // Debug.Log(textFile.text);
        //
        // Debug.Log(Time.time);
        // var datas = new List<string>();
        // var  lineSeperater = '\n';
        // var  fieldSeperator = ',';
        // string[] records = textFile.text.Split (lineSeperater);
        // for (int i = 0; i < records.Length; i++)
        // {
        //     var record = records[i];
        //     string[] fields = record.Split(fieldSeperator);
        //     foreach(string field in fields)
        //     {
        //         //Data in 1 row
        //         datas.Add(field);
        //     }
        // }
        // Debug.Log(Time.time);
        // Debug.Log(datas.Count);
        // Debug.Log(datas[0]);
        // Debug.Log(datas[datas.Count-1]);
        
        var textFile = Resources.Load<TextAsset>("country");
        Debug.Log(textFile.text);
        
        var datas = new List<string>();
        var  lineSeperater = '\n';
        var  fieldSeperator = ',';
        string[] records = textFile.text.Split (lineSeperater);
        for (int i = 0; i < records.Length; i++)
        {
            var record = records[i];
            string[] fields = record.Split(fieldSeperator);
            foreach(string field in fields)
            {
                //Data in 1 row
                datas.Add(field);
            }
        }

        Debug.Log(datas.Count);
        Debug.Log(datas[0]);
        Debug.Log(datas[datas.Count-1]);

        for (int i = 0; i < datas.Count; i++)
        {
            var code = datas[i].ToLower();
            var url = $"https://ipdata.co/flags/{code}.png";
            ObservableWWW.GetWWW(url).Do(_ =>
            {
                if (string.IsNullOrEmpty(_.error))
                {
                    Debug.LogError($"{code} === {_.error}");
                }
                else
                {
                    Debug.Log($"Success: {code}");
                }
            }).CatchIgnore().Subscribe(_ =>
            {
                if (string.IsNullOrEmpty(_.error))
                {
                    Debug.LogError($"{code} === {_.error}");
                }
                else
                {
                    Debug.Log($"Success: {code}");
                }
            });
        }
    }
    
    public static void AddDirective(string pSymbol, BuildTargetGroup pTarget = BuildTargetGroup.Unknown)
    {
        var taget = pTarget == BuildTargetGroup.Unknown ? EditorUserBuildSettings.selectedBuildTargetGroup : pTarget;
        string directivesStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(taget);
        string[] directives = directivesStr.Split(';');
        for (int j = 0; j < directives.Length; j++)
            if (directives[j] == pSymbol)
                return;

        if (string.IsNullOrEmpty(directivesStr))
            directivesStr += pSymbol;
        else
            directivesStr += ";" + pSymbol;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(taget, directivesStr);
    }
    
    public static void RemoveDirective(string pSymbol, BuildTargetGroup pTarget = BuildTargetGroup.Unknown)
    {
        var taget = pTarget == BuildTargetGroup.Unknown ? EditorUserBuildSettings.selectedBuildTargetGroup : pTarget;
        string directives = PlayerSettings.GetScriptingDefineSymbolsForGroup(taget);
        directives = directives.Replace(pSymbol, "");
        if (directives.Length > 1 && directives[directives.Length - 1] == ';')
            directives = directives.Remove(directives.Length - 1, 1);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(taget, directives);
    }
    
#endif
}