using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;

public class ReferenceHelperrWindow : EditorWindow
{
    private static Object referencedObject;
    private List<Object> findResult = new List<Object>();
    private List<GameObject> gameObjectList = new List<GameObject>();
    private List<string> nodeHierarchy = new List<string>();
    private List<bool> checkButtonRecord;
    private Vector2 scrollPosition = Vector2.zero;
    private string[] typeList = { "Prefab", "Mesh", "Material", "Scene", "AnimationClip" };

    [MenuItem("DGH/SearchReference")]
    static void SearchRef()
    {
        ReferenceHelperrWindow win = GetWindow<ReferenceHelperrWindow>();
        win.titleContent = new GUIContent("SearchReference");
        win.minSize = new Vector2(500, 400);
        win.maxSize = new Vector2(500, 400);
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

    private void OnGUI()
    {
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
        GUILayout.Label("ѡ��Ҫ���ҵ���Դ���ͣ�", GUILayout.Width(150));
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

    private void DrawSelectReferencedObject()
    {
        EditorGUILayout.BeginHorizontal();
        referencedObject = EditorGUILayout.ObjectField(referencedObject, typeof(Object), true, GUILayout.Width(200));
        if (GUILayout.Button("Search", GUILayout.Width(100)))
        {
            findResult.Clear();
            if (referencedObject == null)
            {
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(referencedObject); // ��ȡ��Դ��·��
            string assetGuid = AssetDatabase.AssetPathToGUID(assetPath); // ��ȡ��Դ��GUID

            // / ���������е�����Object,��һ�����������������ڶ��������������ļ���Ŀ¼
            string targetType = GenFindType();
            string[] guids = AssetDatabase.FindAssets(targetType, new[] { "Assets" });
            int length = guids.Length;

            for (int i = 0; i < length; i++)
            {
                string filePath = AssetDatabase.GUIDToAssetPath(guids[i]); // ��ȡÿ��Prefab·��
                EditorUtility.DisplayCancelableProgressBar("Checking", filePath, i / length * 1.0f);
                string content = File.ReadAllText(filePath); // ����prefab�ļ��е�����
                if (content.Contains(assetGuid))
                {
                    Object fileObject = AssetDatabase.LoadAssetAtPath(filePath, typeof(Object)); // ��Prefab���ص�ObjectField��
                    findResult.Add(fileObject);
                }

                EditorUtility.ClearProgressBar();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawFindResult()
    {
        if(findResult.Count > 5)
        {
            // Begin the scroll view
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(470), GUILayout.Height(110));
            DoDrawFindResult();
            // End the scroll view
            GUILayout.EndScrollView();
        }
        else
        {
            GUILayout.BeginVertical();
            DoDrawFindResult();
            GUILayout.EndVertical();
        }
    }

    private void DoDrawFindResult()
    {
        foreach (var obj in findResult)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(obj, typeof(Object), true, GUILayout.Width(392));
            var type = obj.GetType();
            if (type == typeof(GameObject))
            {
                if (GUILayout.Button("Detial", GUILayout.Width(50)))
                {
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

    //ֻ�ҳ���һ��(�����Ǹ��ڵ㣩
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

    //�ҳ�����
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

        //Ҳ����ѡ��Project�µ�Object
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