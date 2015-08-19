using System.Collections;
using UnityEngine;

namespace OmiyaGames
{
    public abstract class IAudio : MonoBehaviour
    {
        public abstract AudioSource Audio
        {
            get;
        }

        public bool IsPlaying
        {
            get
            {
                return Audio.isPlaying;
            }
        }

        public virtual void Play()
        {
            Audio.Play();
        }

        public void Play(float delaySeconds)
        {
            if(delaySeconds > 0)
            {
                // Delay playing the audio
                StartCoroutine(DelayPlay(delaySeconds));
            }
            else
            {
                Play();
            }
        }

        public void Stop()
        {
            Audio.Stop();
        }

        public void Pause()
        {
            Audio.Pause();
        }

        public void UnPause()
        {
            Audio.UnPause();
        }

        IEnumerator DelayPlay(float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);
            Play();
        }
    }
}
