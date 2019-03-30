using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using OmiyaGames.Translations;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="SceneInfo.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2018 Omiya Games
    /// 
    /// Permission is hereby granted, free of charge, to any person obtaining a copy
    /// of this software and associated documentation files (the "Software"), to deal
    /// in the Software without restriction, including without limitation the rights
    /// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    /// copies of the Software, and to permit persons to whom the Software is
    /// furnished to do so, subject to the following conditions:
    /// 
    /// The above copyright notice and this permission notice shall be included in
    /// all copies or substantial portions of the Software.
    /// 
    /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    /// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    /// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    /// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    /// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    /// THE SOFTWARE.
    /// </copyright>
    /// <author>Taro Omiya</author>
    /// <date>5/18/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A container of scene-related information. Most used by
    /// <code>SceneManager</code>.
    /// </summary>
    /// <seealso cref="SceneManager"/>
    [System.Serializable]
    public class SceneInfo
    {
        public const string SceneFileExtension = ".unity";

        [SerializeField]
        [ScenePath]
        string scenePath = "";
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("translatedDisplayName")]
        TranslatedString displayName = null;
        [SerializeField]
        CursorLockMode cursorMode = CursorLockMode.None;
        [SerializeField]
        [Tooltip("See TimeManager to set the scene's timescale.")]
        bool revertTimeScale = true;

        //TranslatedString translatedDisplayName = null;
        Scene? reference = null;
        string sceneName = null;
        string loadName = null;
        int ordinal = 0;

        public SceneInfo(string scene, string displayNameTranslationKey, bool revertTime = true, CursorLockMode lockMode = CursorLockMode.None, int index = 0)
        {
            // Setup all member variables
            scenePath = scene;
            revertTimeScale = revertTime;
            cursorMode = lockMode;
            ordinal = index;

            // Setup translation variable
            displayName = new TranslatedString(displayNameTranslationKey, (Ordinal + 1));
        }

        public string ScenePath
        {
            get
            {
                return scenePath;
            }
        }

        public Scene Reference
        {
            get
            {
                if (reference.HasValue == false)
                {
                    reference = SceneManager.GetSceneByPath(ScenePath);
                }
                return reference.Value;
            }
        }

        public string SceneFileName
        {
            get
            {
                if (sceneName == null)
                {
                    sceneName = Path.GetFileNameWithoutExtension(ScenePath);
                }
                return sceneName;
            }
        }

        public string SceneLoadName
        {
            get
            {
                //loadName = ScenePath;
                if (loadName == null)
                {
                    // Load the full directory, minus the file extension
                    System.Text.StringBuilder builder = new System.Text.StringBuilder();
                    builder.Append(ScenePath);

                    // Check if the StringBuilder starts with "Assets"
                    if (StartsWith(builder, FolderPathAttribute.DefaultLocalPath) == true)
                    {
                        // Remove "Assets"
                        builder.Remove(0, (FolderPathAttribute.DefaultLocalPath.Length + 1));
                    }

                    // Check if the StringBuilder ends with ".unity"
                    if (EndsWith(builder, SceneFileExtension) == true)
                    {
                        // Remove "Assets"
                        builder.Remove((builder.Length - SceneFileExtension.Length), SceneFileExtension.Length);
                    }
                    loadName = builder.ToString();
                }
                return loadName;
            }
        }

        public TranslatedString DisplayName
        {
            get
            {
                return displayName;
            }
        }

        public int Ordinal
        {
            get
            {
                return ordinal;
            }
            internal set
            {
                ordinal = value;
                DisplayName.SetValues(Ordinal + 1);
            }
        }

        public CursorLockMode LockMode
        {
            get
            {
                return cursorMode;
            }
        }

        public bool RevertTimeScale
        {
            get
            {
                return revertTimeScale;
            }
        }

        private static bool StartsWith(System.Text.StringBuilder builder, string compare)
        {
            bool isDefaultLocalPath = true;
            for (int i = 0; i < compare.Length; ++i)
            {
                if (char.ToLower(builder[i]) != char.ToLower(compare[i]))
                {
                    isDefaultLocalPath = false;
                    break;
                }
            }
            return isDefaultLocalPath;
        }

        private static bool EndsWith(System.Text.StringBuilder builder, string compare)
        {
            bool isDefaultLocalPath = true;
            for (int i = 0, j = (builder.Length - compare.Length); i < compare.Length; ++i, ++j)
            {
                if (char.ToLower(builder[j]) != char.ToLower(compare[i]))
                {
                    isDefaultLocalPath = false;
                    break;
                }
            }
            return isDefaultLocalPath;
        }
    }
}
