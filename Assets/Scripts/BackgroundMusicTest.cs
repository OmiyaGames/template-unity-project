using UnityEngine;
using System.Collections;
using OmiyaGames;

public class BackgroundMusicTest : MonoBehaviour
{
    [SerializeField]
    AudioClip backgroundMusic;

    // Use this for initialization
    void Start ()
    {
        if(backgroundMusic != null)
        {
            Singleton.Get<BackgroundMusic>().CurrentMusic = backgroundMusic;
        }
    }
}
