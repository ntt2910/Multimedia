using UnityEngine;
using UnityEditor;
using BW.Inspector;

[CustomPropertyDrawer(typeof(LocalizationKey))]
public class LocalizationKeyDrawer : BaseKeywordDrawer
{

	public LocalizationKeyDrawer()
	{
		config = LocalizationKeys.Config.keyWordCategories;

		base.Init();
	}
}

public static class CreateLocalizationKeys
{
	[MenuItem("Assets/ScriptableObjects/Create New LocalizationKeys")]
	public static void CreateLocalizationKeysConfig()
	{
		KeywordsConfig config = ScriptableObject.CreateInstance<KeywordsConfig>();

		if(!System.IO.Directory.Exists(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Assets/Resources/Config/")))
			System.IO.Directory.CreateDirectory(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Assets/Resources/Config/"));
		
		AssetDatabase.CreateAsset(config, "Assets/Resources/Config/LocalizationKeysConfig.asset");
		AssetDatabase.SaveAssets();
		
		EditorUtility.FocusProjectWindow();
  		Selection.activeObject = config;
	}
}
