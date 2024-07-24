using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;

public class ReferenceHelperrWindow2 : EditorWindow
{
    public static List<Object> referencedObjects = new List<Object>();
    private static Object referencedObject;
    private Dictionary<int, List<Object>> findResult = new Dictionary<int, List<Object>>();
    private List<GameObject> gameObjectList = new List<GameObject>();
    private List<string> nodeHierarchy = new List<string>();
    private List<bool> checkButtonRecord;
    private Vector2 scrollPosition = Vector2.zero;
    private string[] typeList = { "Prefab", "Mesh", "Material", "Scene", "AnimationClip" };

    static ReferenceHelperrWindow2 win;
    [MenuItem("DGH/SearchReference2")]
    static void SearchRef()
    {
        win = GetWindow<ReferenceHelperrWindow2>();
        win.titleContent = new GUIContent("SearchReference");
        win.minSize = new Vector2(500, 400);
       // win.maxSize = new Vector2(500, 400);
        win.Show();
        /*
        SearchReferenceEditorWindow window =
            (SearchReferenceEditorWindow)EditorWindow.GetWindow(typeof(SearchReferenceEditorWindow), false, "Searching", true);
        */
    }

    private void OnEnable()
    {
        LoadOptRecord();
    }

    private void OnDisable()
    {
        SaveOptRecord();
    }

    private void LoadOptRecord()
    {
        checkButtonRecord = new List<bool>();
        bool check1 = EditorPrefs.GetBool("AssetHelperCheck1", false);
        bool check2 = EditorPrefs.GetBool("AssetHelperCheck2", false);
        bool check3 = EditorPrefs.GetBool("AssetHelperCheck3", false);
        bool check4 = EditorPrefs.GetBool("AssetHelperCheck4", false);
        bool check5 = EditorPrefs.GetBool("AssetHelperCheck5", false);
        bool check6 = EditorPrefs.GetBool("AssetHelperCheck6", false);
        checkButtonRecord.Add(check1);
        checkButtonRecord.Add(check2);
        checkButtonRecord.Add(check3);
        checkButtonRecord.Add(check4);
        checkButtonRecord.Add(check5);
        checkButtonRecord.Add(check6);
    }

    private void SaveOptRecord()
    {
        EditorPrefs.SetBool("AssetHelperCheck1", checkButtonRecord[0]);
        EditorPrefs.SetBool("AssetHelperCheck2", checkButtonRecord[1]);
        EditorPrefs.SetBool("AssetHelperCheck3", checkButtonRecord[2]);
        EditorPrefs.SetBool("AssetHelperCheck4", checkButtonRecord[3]);
        EditorPrefs.SetBool("AssetHelperCheck5", checkButtonRecord[4]);
        EditorPrefs.SetBool("AssetHelperCheck6", checkButtonRecord[5]);
    }

    private float windowWidth = 300;
    private float windowHeight = 300;

    private void OnGUI()
    {
        windowWidth = EditorGUILayout.FloatField("窗口宽度", windowWidth);
        windowHeight = EditorGUILayout.FloatField("窗口高度", windowHeight);
        if (GUILayout.Button("Repaint", GUILayout.Width(100)) )
        {
            position = new Rect(win.position.x, win.position.y, windowWidth, windowHeight);
            Repaint();
        }        

        GUILayout.Space(10);
        DrawTypeCheckButtons();
        GUILayout.Space(10);
        DrawSelectReferencedObject();
        GUILayout.Space(20);
        DrawFindResult();
        GUILayout.Space(20);
        DrawNodeHierarchy();


       

     

    }

    private void DrawTypeCheckButtons()
    {
        GUILayout.Label("选择要查找的资源类型：", GUILayout.Width(150));
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        checkButtonRecord[0] = GUILayout.Toggle(checkButtonRecord[0], "Prefab", GUILayout.Width(120));
        checkButtonRecord[1] = GUILayout.Toggle(checkButtonRecord[1], "Mesh", GUILayout.Width(120));
        checkButtonRecord[2] = GUILayout.Toggle(checkButtonRecord[2], "Material", GUILayout.Width(120));
        checkButtonRecord[3] = GUILayout.Toggle(checkButtonRecord[3], "Scene", GUILayout.Width(120));
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        checkButtonRecord[4] = GUILayout.Toggle(checkButtonRecord[4], "AnimationClip", GUILayout.Width(120));
        checkButtonRecord[5] = GUILayout.Toggle(checkButtonRecord[5], "AllCanUseSprite", GUILayout.Width(120));
        GUILayout.EndHorizontal();
    }

    private string GenFindType()
    {
        string targetType = "";
        if (checkButtonRecord[5])
        {
            targetType = "t:Prefab t:Material t:Mesh t:AnimationClip t:Scene";
        }
        else
        {
            int count = 0;
            for(int i = 0; i < typeList.Length; i++)
            {
                if (checkButtonRecord[i])
                {
                    if(count > 0)
                    {
                        targetType += " t:" + typeList[i];
                    }
                    else
                    {
                        targetType = "t:" + typeList[i];
                    }
                    count++;
                }
            }
        }

        return targetType;
    }
    private Vector3 scroll = Vector3.zero;
    private void DrawSelectReferencedObject()
    {
       // if (Selection.gameObjects.Length > 0)
        {
           // for (int k = 0; k < Selection.gameObjects.Length; k++)
            {

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Search", GUILayout.Width(100)) && Selection.objects.Length>0)
                {
                    findResult = new Dictionary<int, List<Object>>();
                    referencedObjects = new List<Object>();
                    // / 遍历工程中的所有Object,第一个参数搜索变量，第二个参数，搜索文件夹目录
                    string targetType = GenFindType();
                    string[] guids = AssetDatabase.FindAssets(targetType, new[] { "Assets" });
                    int length = guids.Length;
                    //scroll = EditorGUILayout.BeginScrollView(scroll);
                    for (int k = 0; k < Selection.objects.Length; k++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        referencedObjects.Add(EditorGUILayout.ObjectField(Selection.objects[k], typeof(Object), true, GUILayout.Width(200)));
                        findResult[k] = new List<Object>();
                        if (referencedObjects[k] == null)
                        {
                            continue;
                        }
                        referencedObject = referencedObjects[k];
                        string assetPath = AssetDatabase.GetAssetPath(referencedObject); // 获取资源的路径
                        string assetGuid = AssetDatabase.AssetPathToGUID(assetPath); // 获取资源的GUID
                        
                        for (int i = 0; i < length; i++)
                        {
                            string filePath = AssetDatabase.GUIDToAssetPath(guids[i]); // 获取每个Prefab路径
                            EditorUtility.DisplayCancelableProgressBar("Checking", filePath, i / length * 1.0f);
                            string content = File.ReadAllText(filePath); // 查找prefab文件中的引用
                            if (content.Contains(assetGuid))
                            {
                                Object fileObject = AssetDatabase.LoadAssetAtPath(filePath, typeof(Object)); // 将Prefab加载到ObjectField上
                                findResult[k].Add(fileObject);
                            }

                            EditorUtility.ClearProgressBar();
                        }
                        EditorGUILayout.EndHorizontal();

                    }
                   // EditorGUILayout.EndScrollView();
                }

                if (GUILayout.Button("Print", GUILayout.Width(100)) && findResult.Count> 0)
                {
                    if (findResult != null && findResult.Count > 0)
                    {
                        var str = "";
                        foreach (var item in findResult)
                        {
                            str += referencedObjects[item.Key].name + "  :   ";
                            foreach (var obj in item.Value)
                            {
                                EditorGUILayout.ObjectField(obj, typeof(Object), true, GUILayout.Width(392));
                                str += obj.name+"   +   ";
                                var type = obj.GetType();
                                if (type == typeof(GameObject))
                                {
                                    if (GUILayout.Button("Detail", GUILayout.Width(50)))
                                    {
                                        referencedObject = referencedObjects[item.Key];
                                        Transform parent = (obj as GameObject).transform;
                                        string assetPath = AssetDatabase.GetAssetPath(referencedObject);
                                        Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
                                        Texture2D image = asset as Texture2D;
                                        nodeHierarchy.Clear();
                                        gameObjectList.Clear();
                                        FindNodeUsingImageV2(parent, image, gameObjectList);
                                    }
                                }
                                GUILayout.EndHorizontal();
                            }
                            str += "   \r\n   ";
                            GUILayout.EndVertical();
                        }
                        Debug.Log(str);
                        // EditorGUILayout.EndScrollView();
                    }

                }
                EditorGUILayout.EndHorizontal();


            }
        }
        
    }

    private void DrawFindResult()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        DoDrawFindResult();
        // End the scroll view
        GUILayout.EndScrollView();
    }

    private void DoDrawFindResult()
    {
        if (findResult != null && findResult.Count > 0) 
        { 
            //scroll = EditorGUILayout.BeginScrollView(scroll);
            foreach (var item in findResult)
            {
                referencedObjects[item.Key] = EditorGUILayout.ObjectField(referencedObjects[item.Key], typeof(Object), true, GUILayout.Width(200));
                GUILayout.BeginVertical();
                foreach (var obj in item.Value)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(obj, typeof(Object), true, GUILayout.Width(392));
                    
                    var type = obj.GetType();
                    if (type == typeof(GameObject))
                    {
                        if (GUILayout.Button("Detail", GUILayout.Width(50)))
                        {
                            referencedObject = referencedObjects[item.Key];
                            Transform parent = (obj as GameObject).transform;
                            string assetPath = AssetDatabase.GetAssetPath(referencedObject);
                            Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
                            Texture2D image = asset as Texture2D;
                            nodeHierarchy.Clear();
                            gameObjectList.Clear();
                            FindNodeUsingImageV2(parent, image, gameObjectList);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
           // EditorGUILayout.EndScrollView();
        }      
        
    }

    private void DrawNodeHierarchy()
    {
        if(nodeHierarchy.Count > 5)
        {
            // Begin the scroll view
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(500), GUILayout.Height(110));
            DoDrawNodeHierarchy();
            // End the scroll view
            GUILayout.EndScrollView();
        }
        else
        {
            GUILayout.BeginVertical();
            DoDrawNodeHierarchy();
            GUILayout.EndVertical();
        }
    }

    private void DoDrawNodeHierarchy()
    {
        for (int i = 0; i < nodeHierarchy.Count; i++)
        {
            string hierarchy = nodeHierarchy[i];
            GameObject go = gameObjectList[i];

            GUILayout.BeginHorizontal();
            GUIStyle btnStyle = GUI.skin.button;
            btnStyle.alignment = TextAnchor.MiddleLeft;
            //GUILayout.Label(hierarchy, GUILayout.Width(430));
            if (GUILayout.Button(hierarchy, btnStyle, GUILayout.Width(500)))
            {
                EditorGUIUtility.PingObject(go);
                Selection.activeGameObject = go;
                var window = EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow"));
                window.Focus();
            }
            GUILayout.EndHorizontal();
        }
    }

    //只找出第一个(不能是根节点）
    private void FindNodeUsingImageV1(Transform parent, Texture2D image, List<GameObject> gameObjectList)
    {
        List < GameObject > aaa = new List<GameObject>();
        foreach (Transform child in parent)
        {
            // Check if the child has a texture component that references the image
            Renderer renderer = child.GetComponent<Renderer>();

            if (renderer != null && renderer.sharedMaterial != null && renderer.sharedMaterial.mainTexture == image)
            {
                gameObjectList.Add(child.gameObject);
            }

            Image img = child.GetComponent<Image>();
            
            if(img != null && img.mainTexture == image)
            {
                gameObjectList.Add(child.gameObject);
                string hierarchy = GenHierarchyStr(child.gameObject);
                nodeHierarchy.Add(hierarchy);
                Debug.Log("v1 the node hierarchy is " + hierarchy);
                EditorApplication.delayCall += () =>
                {
                    Selection.activeGameObject = child.gameObject;
                    EditorGUIUtility.PingObject(Selection.activeGameObject);
                    //SceneView.lastActiveSceneView.FrameSelected();
                };

                return;
            }

            // Recursively search the child's children
            FindNodeUsingImageV1(child, image, gameObjectList);
        }
    }

    //找出所有
    private void FindNodeUsingImageV2(Transform root, Texture2D image, List<GameObject> gameObjectList)
    {
        bool find = false;
        // Check if the child has a texture component that references the image
        Renderer renderer = root.GetComponent<Renderer>();
        if (renderer != null && renderer.sharedMaterial != null && renderer.sharedMaterial.mainTexture == image)
        {
            find = true;
        }

        Image uguiImage = root.GetComponent<Image>();
        if (uguiImage != null && uguiImage.mainTexture == image)
        {
            find = true;
        }
        if (find)
        {
            gameObjectList.Add(root.gameObject);
            string hierarchy = GenHierarchyStr(root.gameObject);
            nodeHierarchy.Add(hierarchy);
            Debug.Log("v2 the node hierarchy is " + hierarchy);
        }

        // Recursively search the child's children
        foreach (Transform child in root)
        {
            FindNodeUsingImageV2(child, image, gameObjectList);
        }
    }

    private string GenHierarchyStr(GameObject go)
    {
        Transform transform = go.transform;
        StringBuilder sb = new StringBuilder();
        while(transform.parent != null)
        {
            sb.Insert(0, transform.parent.name + "/");
            transform = transform.parent;
        }
        sb.Append(go.name);
        return sb.ToString();
    }


    [MenuItem("DGH/SelectNode-scene", false, 11)]
    static void Select1()
    {
        string goName = "background";
        GameObject go = GameObject.Find(goName);
        EditorGUIUtility.PingObject(go);
        Selection.activeGameObject = go;

        //也可以选择Project下的Object
        //Selection.activeObject  = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Cube.prefab");
    }

    //[MenuItem("DGH/SelectNode-prefab", false, 11)]
    static void Select2()
    {
        // Select the specified game object
        string goName = "b_click_03";
        GameObject go = GameObject.Find(goName);
        Selection.activeObject = go;

        // Highlight the selected game object in the Hierarchy view
        EditorApplication.delayCall += () =>
        {
            //EditorApplication.ExecuteMenuItem("Edit/Select All");
        };
    }
}