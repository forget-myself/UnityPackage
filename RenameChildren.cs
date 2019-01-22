/*
 * description :
 * author:nuts 
 * CreateTime:  #CREATETIME#
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
 
 
public class RenameChildren : Editor
{
    static  string AddStr = "_Music";

    [MenuItem("Tools/名字添加后缀")]
    public static void NameAddSuffix()
    {
        Object[] objects = Selection.objects;// (UnityEditor.SelectionMode.Assets);
        //GameObject objects = Selection.activeGameObject;
        if (objects.Length>0)
        {
            foreach (var item in objects)
            {
                //item.name = item.name + AddStr;
                string path = AssetDatabase.GetAssetPath(item);
                AssetDatabase.RenameAsset(path, item.name + AddStr);
                //Debug.Log(item.name);
               // EditorUtility.SetDirty(item);                
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        foreach (var item in objects)
        {
            //item.name = item.name + AddStr;
             Debug.Log(item.name);
           // EditorUtility.SetDirty(item);
        }

    }

    [MenuItem("Tools/名字移除后缀")]
    public static void NameRemoveSuffix()
    {
        Object[] objects = Selection.objects;// (UnityEditor.SelectionMode.Assets);
        //GameObject objects = Selection.activeGameObject;
        if (objects.Length > 0)
        {
            foreach (var item in objects)
            {
                //item.name = item.name + AddStr;
                string path = AssetDatabase.GetAssetPath(item);
                string newName = item.name.Replace(AddStr, "");
                AssetDatabase.RenameAsset(path, newName);
                //Debug.Log(item.name);
                // EditorUtility.SetDirty(item);                
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        foreach (var item in objects)
        {
            //item.name = item.name + AddStr;
            Debug.Log(item.name);
            // EditorUtility.SetDirty(item);
        }

    }
}
