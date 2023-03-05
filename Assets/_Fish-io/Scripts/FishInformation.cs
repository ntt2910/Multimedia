using System;
using System.Runtime.Remoting.Contexts;
using HyperCasualTemplate;
using UniRx;
using UnityEngine;

public class FishInformation : MonoBehaviour
{
    public FishPlayerData fishPlayerData;
    
    private static Texture2D _myflag;
    public static Texture2D MyFlag
    {
        get { return _myflag == null ? defaultFlag : _myflag; }
    }
    
    private static Texture2D defaultFlag
    {
        get
        {
            return Resources.Load("flag") as Texture2D;
        }
    }
    
    public void LoadMyFlag(CountryCode countryCode)
    {
        if (_myflag != null) return;
        var url = string.Format(Config.FLAG_API, countryCode);
        LoadImageFromURL(url,
            $"flag_{countryCode}",
            txture2d => { _myflag = txture2d; },
            null);
    }

    public void LoadImageFromURL(CountryCode countryCode, Action<Texture2D> OnFlagComplete)
    {
        var url = Uri.EscapeUriString(string.Format(Config.FLAG_API, countryCode.ToString()));
        LoadImageFromURL(url, string.Format("flag_{0}", countryCode), OnFlagComplete, null);
    }

    public void LoadImageFromURL(string image_url, string file_name, Action<Texture2D> OnComplete, Action OnError)
    {
        var url = Uri.EscapeUriString(image_url);
        Helper.DownloadOrCache(url, file_name)
            .CatchIgnore()
            .Subscribe(_ =>
            {
                if (_ != null && _.texture != null)
                    OnComplete(_.texture);
            });
    }
}