using UnityEngine;
using UnityEditor;

public class ResetPlayerPrefsContextMenu
{
    [MenuItem ("Omiya Games/Reset Stored Settings")]
    public static void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
