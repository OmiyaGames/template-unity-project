using UnityEngine;
using UnityEngine.UI;
using TMPro;
using OmiyaGames.Settings;
using OmiyaGames.Translations;

namespace OmiyaGames.Menus
{
    ///-----------------------------------------------------------------------
    /// <copyright file="NewHighScoreMenu.cs" company="Omiya Games">
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
    /// <date>7/12/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A script to setup menu to enter new high score.
    /// </summary>
    /// <remarks>
    /// Revision History:
    /// <list type="table">
    /// <listheader>
    /// <description>Date</description>
    /// <description>Name</description>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <description>8/17/2018</description>
    /// <description>Taro</description>
    /// <description>Initial verison</description>
    /// </item>
    /// </list>
    /// </remarks>
    [DisallowMultipleComponent]
    public class NewHighScoreMenu : IMenu
    {
        [SerializeField]
        MenuNavigator navigator;

        [Header("Labels")]
        [SerializeField]
        TMP_InputField nameField;
        [SerializeField]
        TranslatedTextMeshPro scorePlacementLabel;
        [SerializeField]
        TranslatedTextMeshPro scoreLabel;

        #region Non-abstract Properties
        public override BackgroundMenu.BackgroundType Background
        {
            get
            {
                return BackgroundMenu.BackgroundType.SolidColor;
            }
        }

        public override Type MenuType
        {
            get
            {
                return Type.ManagedMenu;
            }
        }

        public override Selectable DefaultUi
        {
            get
            {
                return nameField;
            }
        }

        public override MenuNavigator Navigator
        {
            get
            {
                return navigator;
            }
        }

        public string EnteredName
        {
            get
            {
                return nameField.text;
            }
        }

        public IRecord<int> NewScore
        {
            get;
            set;
        } = null;
        #endregion

        public void Setup(int highScorePlacement, IRecord<int> score)
        {
            NewScore = score;
            scorePlacementLabel.gameObject.SetActive(NewScore != null);
            scoreLabel.gameObject.SetActive(NewScore != null);
            if (NewScore != null)
            {
                scorePlacementLabel.SetArguments(highScorePlacement + 1);
                scoreLabel.SetArguments(NewScore.Record);
            }
        }

        public void OnRestartClicked()
        {
            if (IsListeningToEvents == true)
            {
                RecordNewHighScore();

                // Transition to the current level
                SceneChanger.ReloadCurrentScene();
            }
        }

        protected override void OnStateChanged(VisibilityState from, VisibilityState to)
        {
            // Check if this menu is going from hidden to visible
            if (to == VisibilityState.Visible)
            {
                // Use the last stored name
                nameField.text = Settings.LastEnteredName;
            }
            else if (to == VisibilityState.Hidden)
            {
                RecordNewHighScore();
            }

            // Call base method
            base.OnStateChanged(from, to);
        }

        private void RecordNewHighScore()
        {
            // Record the new name!
            if (NewScore != null)
            {
                NewScore.Name = nameField.text;
            }

            // Store this name for the next gameplay as well
            Settings.LastEnteredName = nameField.text;
            Settings.SaveSettings();
        }
    }
}