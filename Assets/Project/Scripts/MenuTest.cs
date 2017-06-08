using UnityEngine;
using System.Collections.Generic;
using OmiyaGames;
using OmiyaGames.Menu;

///-----------------------------------------------------------------------
/// <copyright file="MenuTest.cs" company="Omiya Games">
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
/// <date>8/21/2015</date>
///-----------------------------------------------------------------------
/// <summary>A simple test script: displays the <code>PauseMenu</code>, <code>LevelFailedMenu</code>, or <code>LevelCompleteMenu</code></summary>
/// <seealso cref="PauseMenu"/>
/// <seealso cref="LevelFailedMenu"/>
/// <seealso cref="LevelCompleteMenu"/>
public class MenuTest : MonoBehaviour
{
    [SerializeField]
    string[] popUpTexts;

    int popUpTextsIndex = 0;
    readonly List<KeyValuePair<ulong, string>> allPopUpTexts = new List<KeyValuePair<ulong, string>>();

    public void OnPopUpClicked()
    {
        // Show a new dialog
        MenuManager manager = Singleton.Get<MenuManager>();
        ulong id = manager.PopUps.ShowNewDialog(popUpTexts[popUpTextsIndex]);

        // Store information
        allPopUpTexts.Add(new KeyValuePair<ulong, string>(id, popUpTexts[popUpTextsIndex]));

        // Get next index
        ++popUpTextsIndex;
        if(popUpTextsIndex >= popUpTexts.Length)
        {
            popUpTextsIndex = 0;
        }
    }

    public void OnPopUpHideTopClicked()
    {
        if(allPopUpTexts.Count > 0)
        {
            MenuManager manager = Singleton.Get<MenuManager>();
            manager.PopUps.RemoveDialog(allPopUpTexts[allPopUpTexts.Count - 1].Key);
            allPopUpTexts.RemoveAt(allPopUpTexts.Count - 1);
        }
    }

    public void OnPopUpHideBottomClicked()
    {
        if (allPopUpTexts.Count > 0)
        {
            MenuManager manager = Singleton.Get<MenuManager>();
            ulong id = manager.PopUps.RemoveLastVisibleDialog();
            for (int index = (allPopUpTexts.Count - 1); index >= 0; --index)
            {
                if (allPopUpTexts[index].Key == id)
                {
                    allPopUpTexts.RemoveAt(index);
                    break;
                }
            }
        }
    }

    public void OnPopUpHideRandomClicked()
    {
        if (allPopUpTexts.Count > 0)
        {
            MenuManager manager = Singleton.Get<MenuManager>();
            int randomIndex = (allPopUpTexts.Count - manager.PopUps.MaximumNumberOfDialogs);
            if(randomIndex < 0)
            {
                randomIndex = 0;
            }
            randomIndex = Random.Range(randomIndex, allPopUpTexts.Count);

            manager.PopUps.RemoveDialog(allPopUpTexts[randomIndex].Key);
            allPopUpTexts.RemoveAt(randomIndex);
        }
    }

    public void OnPauseClicked()
    {
        MenuManager manager = Singleton.Get<MenuManager>();
        manager.Show<PauseMenu>();
        manager.ButtonClick.Play();
    }

    public void OnFailedClicked()
    {
        MenuManager manager = Singleton.Get<MenuManager>();
        manager.Show<LevelFailedMenu>();
        manager.ButtonClick.Play();
    }

    public void OnCompleteClicked()
    {
        MenuManager manager = Singleton.Get<MenuManager>();
        manager.Show<LevelCompleteMenu>();
        manager.ButtonClick.Play();
    }

    public void OnSoundClicked()
    {
        SoundEffect sound = GetComponent<SoundEffect>();
        if(sound != null)
        {
            sound.Play();
        }
    }
}
