//#define CLEAR_AFTER_BUILD

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

namespace Community.UI
{
    /// <summary>
    /// Intended to fix the Sprite-packing error that occurs on Android builds.
    /// Code taken from http://forum.unity3d.com/threads/spritepacker-cannot-load-atlas-atlasname-during-build-enter-game-please-rebuild-sprite-atlas.386524/
    /// </summary>
    public class PostBuildAtlasCacheClear : MonoBehaviour
    {
#if CLEAR_AFTER_BUILD
        [PostProcessBuild]
        private static void DeleteAtlasCache(BuildTarget target, string pathToBuiltProject)
        {
            DeleteAtlasCache();
        }
#endif
        [MenuItem("Tools/Omiya Games/Delete Sprite Packing Cache")]
        public static void DeleteAtlasCache()
        {
            string projectPath = Application.dataPath; //Asset path
            string atlasCachePath = Path.GetFullPath(Path.Combine(projectPath, Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "Library" + Path.DirectorySeparatorChar + "AtlasCache"));
            if (Directory.Exists(atlasCachePath) == true)
            {
                Directory.Delete(atlasCachePath, true);
                Debug.Log("Deleted atlas cache folder.");
            }
        }
    }
}
#endif
