using System;
using HyperCasualTemplate;

public class Config
{
#if UNITY_ANDROID
    public const string package_name = "com.supergem.giant.sniper3D";
#else
    public const string package_name = "com.supergem.giant.sniper3D";
#endif
    
    public const string FLAG_API = "https://flagcdn.com/w{0}/{1}.png"; //https://flagcdn.com/w80/vn.png
}