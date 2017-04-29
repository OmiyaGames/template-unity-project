using UnityEngine;
using UnityEngine.SceneManagement;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="SceneInfo.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2016 Omiya Games
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
        [SerializeField]
        string scenePath = "";
        [SerializeField]
        string displayName = "";
        [SerializeField]
        CursorLockMode cursorMode = CursorLockMode.None;
        [SerializeField]
        CursorLockMode cursorModeWeb = CursorLockMode.None;
        [SerializeField]
        bool revertTimeScale = true;

        TranslatedString? translatedDisplayName = null;
        Scene? reference = null;
        string sceneName = null;
        int ordinal = 0;

        public SceneInfo(string scene, string displayNameTranslationKey, bool revertTime = true, CursorLockMode lockMode = CursorLockMode.None, int index = 0)
        {
            // Setup all member variables
            scenePath = scene;
            displayName = displayNameTranslationKey;
            revertTimeScale = revertTime;
            cursorMode = lockMode;
            cursorModeWeb = lockMode;
            ordinal = index;

            // Setup translation variable
            translatedDisplayName = new TranslatedString(displayName, (Ordinal + 1));
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
                if(reference.HasValue == false)
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
                if(sceneName == null)
                {
                    sceneName = System.IO.Path.GetFileNameWithoutExtension(ScenePath);
                }
                return sceneName;
            }
        }

        public TranslatedString DisplayName
        {
            get
            {
                if(translatedDisplayName.HasValue == false)
                {
                    translatedDisplayName = new TranslatedString(displayName, (Ordinal + 1));
                }
                return translatedDisplayName.Value;
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

        public CursorLockMode LockModeWeb
        {
            get
            {
                return cursorModeWeb;
            }
        }

        public bool RevertTimeScale
        {
            get
            {
                return revertTimeScale;
            }
        }
    }
}
