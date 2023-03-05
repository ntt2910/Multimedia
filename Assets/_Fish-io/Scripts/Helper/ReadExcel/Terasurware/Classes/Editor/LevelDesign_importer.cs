using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class LevelDesign_importer : AssetPostprocessor
{
    // private static readonly string filePath = "Assets/Resources/monster_merge.xlsx";
    // private static readonly string[] sheetNames = { "LevelDesignDev", };
    //
    // static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    // {
    //     foreach (string asset in importedAssets)
    //     {
    //         if (!filePath.Equals(asset))
    //             continue;
    //
    //         using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    //         {
    //             IWorkbook book = null;
    //             if (Path.GetExtension(filePath) == ".xls")
    //             {
    //                 book = new HSSFWorkbook(stream);
    //             }
    //             else
    //             {
    //                 book = new XSSFWorkbook(stream);
    //             }
    //
    //             foreach (string sheetName in sheetNames)
    //             {
    //                 // check sheet
    //                 var sheet = book.GetSheet(sheetName);
    //                 if (sheet == null)
    //                 {
    //                     Debug.LogError("[QuestData] sheet not found:" + sheetName);
    //                     continue;
    //                 }
    //                 
    //                 var exportPath = "Assets/_MergeMonster/Scripts/ScriptableObject/LevelDesignDatabase.asset";
    //                 if (!Directory.Exists("Assets/_MergeMonster/Scripts/ScriptableObject/"))
    //                 {
    //                     Directory.CreateDirectory("Assets/_MergeMonster/Scripts/ScriptableObject/");
    //                 }
    //                 // check scriptable object
    //                 var data = (LevelDatabase)AssetDatabase.LoadAssetAtPath(exportPath, typeof(LevelDatabase));
    //                 if (data == null)
    //                 {
    //                     data = ScriptableObject.CreateInstance<LevelDatabase>();
    //                     AssetDatabase.CreateAsset((ScriptableObject)data, exportPath);
    //                 }
    //
    //                 data.hideFlags = HideFlags.None;
    //                 data.enemyDatabaseObjects = new List<LevelDatabaseObject>();
    //                 //data.enemyDatabaseObjects?.Clear();
    //
    //                 // add infomation
    //                 for (int i = 1; i <= sheet.GetSheetLength(); i++)
    //                 {
    //                     IRow row = sheet.GetRow(i);
    //                     ICell cell = null;
    //                     //cell = row.GetCell(1);
    //                     // string _name = cell.ToString();
    //                     // if (string.IsNullOrEmpty(_name))
    //                     // {
    //                     //     _name = sheetName + i;
    //                     // }
    //                     
    //                     var level = new LevelDatabaseObject();
    //                     level.ID = i;
    //                     var earning = row.GetCell(20);
    //                     level.earningConfig = earning.TryGetCell<int>();
    //                     var dmg = row.GetCell(21);
    //                     var hp = row.GetCell(22);
    //                     level.dmg = dmg.TryGetCell<float>();
    //                     level.hp = hp.TryGetCell<float>();
    //                     level.enemyInLevelObjects = new List<EnemyInLevelObject>();
    //
    //                     Debug.Log("cell " + i);
    //                     for (int j = 0; j < 20; j++)
    //                     {
    //                         cell = row.GetCell(j);
    //                         // Debug.Log(cell);
    //                         // Debug.Log(cell.CellType);
    //                         // Debug.Log(cell.NumericCellValue);
    //                         // Debug.Log(cell.ToString());
    //                         if(cell.CellType != CellType.Numeric || string.IsNullOrEmpty(cell.ToString())) continue;
    //                         
    //                         var cellGetInt = cell.TryGetCell<int>();
    //                         if (cellGetInt > 0)
    //                         {
    //                             var enemyInLevelObject = new EnemyInLevelObject {iDEnemy = j, count = cellGetInt};
    //                             level.enemyInLevelObjects.Add(enemyInLevelObject);
    //                         }
    //                     }
    //                     Debug.Log("cell " + i);
    //                     data.enemyDatabaseObjects.Add(level);
    //                 }
    //
    //                 Debug.Log("Done!!!");
    //
    //                 ScriptableObject obj = AssetDatabase.LoadAssetAtPath(exportPath, typeof(ScriptableObject)) as ScriptableObject;
    //                 EditorUtility.SetDirty(obj);
    //             }
    //         }
    //
    //     }
    // }
}