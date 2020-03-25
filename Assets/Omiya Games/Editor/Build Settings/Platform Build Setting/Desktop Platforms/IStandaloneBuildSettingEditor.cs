using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using OmiyaGames.Builds;
using OmiyaGames.Common.Editor;

namespace OmiyaGames.UI.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="IStandaloneBuildSettingEditor.cs" company="Omiya Games">
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
    /// <date>11/16/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Helper script for <code>IPlatformBuildSetting</code>
    /// </summary>
    /// <seealso cref="IPlatformBuildSetting"/>
    [CustomEditor(typeof(IStandaloneBuildSetting))]
    public abstract class IStandaloneBuildSettingEditor : IPlatformBuildSettingEditor
    {
        protected SerializedProperty architecture;
        private SerializedProperty compression;
        private SerializedProperty scriptingBackend;

        public override void OnEnable()
        {
            base.OnEnable();
            architecture = serializedObject.FindProperty("architecture");
            compression = serializedObject.FindProperty("compression");
            scriptingBackend = serializedObject.FindProperty("scriptingBackend");
        }

        protected override void DrawPlatformSpecificSettings()
        {
            IStandaloneBuildSetting targetSetting = (IStandaloneBuildSetting)target;
            EditorUiUtility.DrawEnum(architecture, targetSetting.SupportedArchitectures, targetSetting.DefaultArchitecture, targetSetting.ArchitectureToBuild, "\"{0}\" is not supported for this build platform; \"{1}\" will be used instead.");
            EditorGUILayout.PropertyField(compression);
            EditorUiUtility.DrawEnum(scriptingBackend, targetSetting.SupportedScriptingBackends, targetSetting.DefaultScriptingBackend, targetSetting.ScriptingBackend, "\"{0}\" is not supported for this build platform, on this editor; \"{1}\" will be used instead.");
        }
    }
}
