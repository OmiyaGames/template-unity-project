using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System;

public class DomainListAssetBundleGenerator : EditorWindow
{
    const float VerticalMargin = 2;

    string nameOfFile = "domains";
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
        EditorGUILayout.HelpBox("What is a Domain List Asset?\nIt's an asset containing a list of domains the WebLocationChecker script uses to check if the domain of the website the WebGL build is running in is an accepted host or not. Using this method vs. the normal text-based method makes the asset serialized, and as such, harder to change.", MessageType.Info);

        // Ask the list of layouts
        allDomainsField.DoLayoutList();

        // Ask the name of the file this will generate
        GUILayout.Label("Name of asset to generate");
        nameOfFile = GUILayout.TextField(nameOfFile);

        // Generate the asset
        if (GUILayout.Button("Generate Domain Asset") == true)
        {
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
