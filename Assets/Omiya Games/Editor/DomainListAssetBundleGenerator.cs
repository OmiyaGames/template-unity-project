using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System;
using System.Text;

public class DomainListAssetBundleGenerator : EditorWindow
{
    const float VerticalMargin = 2;

    string nameOfFile = "domains";
    string nameOfFolder = "Assets";
    List<string> allDomains = new List<string> { "localhost", "build.cloud.unity3d.com" };
    ReorderableList allDomainsField = null;

    [MenuItem("Omiya Games/Domain List Asset Generator")]
    static void Initialize()
    {
        DomainListAssetBundleGenerator window = EditorWindow.GetWindow<DomainListAssetBundleGenerator>(title: "Domain List Asset Generator");
        window.Show();
    }

    void OnGUI()
    {
        // Explain what this dialog does
        EditorGUILayout.HelpBox("What is a Domain List Asset?\nIt's an asset containing a list of domains the WebLocationChecker script uses to check if the domain of the website the WebGL build is running in is an accepted host or not. Since Asset Bundles are serialized, and thus, harder to edit, it creates a more secure method for developers to change the list of approved domains on-the-fly while making it harder for others to change it.", MessageType.Info);

        // Ask the list of layouts
        allDomainsField.DoLayoutList();

        // Ask the path this will generate
        GUILayout.Label("Name of folder to generate this asset");
        nameOfFolder = GUILayout.TextField(nameOfFolder);

        // Ask the name of the file this will generate
        GUILayout.Label("Name of asset to generate");
        nameOfFile = GUILayout.TextField(nameOfFile);

        // Generate the asset
        if (GUILayout.Button("Generate Domain Asset") == true)
        {
            // Generate a path to create this asset
            StringBuilder builder = new StringBuilder();
            builder.Append(nameOfFolder);
            if(nameOfFolder.EndsWith("/") == false)
            {
                builder.Append('/');
            }
            builder.Append(nameOfFile);
            if(nameOfFolder.EndsWith(".asset") == false)
            {
                builder.Append(".asset");
            }
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (builder.ToString());
            
            // FIXME: remove this ScriptableObject generation, and just create an asset bundle straight from it
            // Create the asset at the assigned path
            AcceptedDomainList domainList = ScriptableObject.CreateInstance<AcceptedDomainList>();
            domainList.name = nameOfFile;
            AssetDatabase.CreateAsset (domainList, assetPathAndName);
            AssetDatabase.SaveAssets ();
            
            // Select this asset
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow ();
            Selection.activeObject = domainList;
            
            //FIXME: create an AssetBundle from this asset
            // FIXME: do something!
            Debug.Log("You pushed the button!");
            foreach(string domain in allDomains)
            {
                Debug.Log(domain);
            }
        }
    }

    void OnEnable()
    {
        allDomainsField = new ReorderableList(allDomains, typeof(string), true, true, true, true);
        allDomainsField.drawHeaderCallback = DrawLevelListHeader;
        allDomainsField.drawElementCallback = DrawLevelListElement;
        allDomainsField.onAddCallback = OnAddDomain;
        allDomainsField.elementHeight = (EditorGUIUtility.singleLineHeight + (VerticalMargin * 2));
    }

    private void OnAddDomain(ReorderableList list)
    {
        string toAdd = string.Empty;
        if(allDomains.Count > 0)
        {
            if((list.index >= 0) && (list.index < allDomains.Count))
            {
                toAdd = allDomains[list.index];
            }
            else
            {
                toAdd = allDomains[allDomains.Count - 1];
            }
        }
        allDomains.Add(toAdd);
    }

    void DrawLevelListHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, "All Accepted Domains");
    }

    void DrawLevelListElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        rect.y += VerticalMargin;
        rect.height = EditorGUIUtility.singleLineHeight;
        allDomains[index] = EditorGUI.TextField(rect, GUIContent.none, allDomains[index]);
    }
}
