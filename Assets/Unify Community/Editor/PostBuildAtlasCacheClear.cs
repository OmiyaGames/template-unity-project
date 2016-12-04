#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

/// <summary>
/// Intended to fix the Sprite-packing error that occurs on Android builds.
/// Code taken from http://forum.unity3d.com/threads/spritepacker-cannot-load-atlas-atlasname-during-build-enter-game-please-rebuild-sprite-atlas.386524/
/// </summary>
public class PostBuildAtlasCacheClear : MonoBehaviour
{
    [PostProcessBuild]
    private static void DeleteAtlasCache(BuildTarget target, string pathToBuiltProject)
    {
        string projectPath = Application.dataPath; //Asset path
        string atlasCachePath = Path.GetFullPath(Path.Combine(projectPath, Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "Library" + Path.DirectorySeparatorChar + "AtlasCache"));
        if (Directory.Exists(atlasCachePath) == true)
        {
            Directory.Delete(atlasCachePath, true);
            Debug.Log("Deleted atlas cache folder.");
        }
    }

    [MenuItem("Build/Delete Sprite Packing Cache")]
    public static void DeleteAtlasCache()
    {
        DeleteAtlasCache(EditorUserBuildSettings.activeBuildTarget, null);
    }
}
#endif
