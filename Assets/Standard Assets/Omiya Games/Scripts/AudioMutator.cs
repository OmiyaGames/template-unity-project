using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioMutator : MonoBehaviour
{
    /// <summary>
    /// The center pitch of this audio.
    /// </summary>
    [Range(-3, 3)]
    public float centerPitch = 1;
    /// <summary>
    /// The allowed range the pitch can mutate from the center pitch
    /// </summary>
    public Vector2 pitchMutationRange = new Vector2(-0.5f, 0.5f);

    AudioSource audioCache = null;

    public AudioSource Audio
    {
        get
        {
            if(audioCache == null)
            {
                audioCache = audio;
            }
            return audioCache;
        }
    }

    public void Play()
    {
        // Stop the audio
        Audio.Stop();

        // Change the audio's pitch
        Audio.pitch = centerPitch + Random.Range(pitchMutationRange.x, pitchMutationRange.y);

        // Play the audio
        Audio.Play();
    }

    public void Play(float delaySeconds)
    {
        // Delay playing the audio
        StartCoroutine(DelayPlay(delaySeconds));
    }

    public void Stop()
    {
        Audio.Stop();
    }

    void Awake()
    {
        // Update the pithes first
        Audio.pitch = centerPitch;
    }

    IEnumerator DelayPlay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        Play();
    }
}
