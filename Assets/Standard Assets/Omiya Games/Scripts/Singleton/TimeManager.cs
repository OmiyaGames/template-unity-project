using UnityEngine;
using System.Collections;

namespace OmiyaGames
{
    public class TimeManager : ISingletonScript
    {
        public event System.Action<TimeManager> OnManuallyPausedChanged;

        float originalTimeScale = 1f,
            timeScale = 1f,
            timeScaleChangedFor = -1f,
            slowDownDuration = 1f;
        bool isManuallyPaused = false,
            isTimeScaleTemporarilyChanged = false;

        public float OriginalTimeScale
        {
            get
            {
                return originalTimeScale;
            }
        }

        public float TimeScale
        {
            get
            {
                return timeScale;
            }
            set
            {
                if (Mathf.Approximately(timeScale, value) == false)
                {
                    timeScale = value;
                    if (IsManuallyPaused == false)
                    {
                        Time.timeScale = timeScale;
                    }
                }
            }
        }

        public bool IsManuallyPaused
        {
            get
            {
                return isManuallyPaused;
            }
            set
            {
                if (isManuallyPaused != value)
                {
                    // Change value
                    isManuallyPaused = value;

                    // Change time scale
                    if (isManuallyPaused == true)
                    {
                        Time.timeScale = 0;
                    }
                    else
                    {
                        Time.timeScale = TimeScale;
                    }

                    // Shoot the pause event
                    if(OnManuallyPausedChanged != null)
                    {
                        OnManuallyPausedChanged(this);
                    }
                }
            }
        }

        public override void SingletonAwake(Singleton instance)
        {
            originalTimeScale = Time.timeScale;
            timeScale = Time.timeScale;
            instance.OnRealTimeUpdate += UpdateRealtime;
        }

        public override void SceneAwake(Singleton instance)
        {
            // Do nothing
        }

        public void RevertToOriginalTime()
        {
            TimeScale = OriginalTimeScale;
        }

        public void PauseFor(float durationSeconds)
        {
            TemporarilyChangeTimeScaleFor(0f, durationSeconds);
        }

        public void TemporarilyChangeTimeScaleFor(float timeScale, float durationSeconds)
        {
            // Change the time scale immediately
            Time.timeScale = timeScale;

            // Store how long it's going to change the time scale
            slowDownDuration = durationSeconds;

            // Update flags to revert the time scale later
            timeScaleChangedFor = 0f;
            isTimeScaleTemporarilyChanged = true;
        }

        void UpdateRealtime(float unscaledDeltaTime)
        {
            // Check to see if we're not paused, and changed the time scale temporarily
            if ((IsManuallyPaused == false) && (isTimeScaleTemporarilyChanged == true))
            {
                // Increment the duration we've changed the time scale
                timeScaleChangedFor += unscaledDeltaTime;

                // Check to see if enough time has passed to revert the time scale
                if (timeScaleChangedFor > slowDownDuration)
                {
                    // Revert the time scale
                    Time.timeScale = TimeScale;

                    // Flag the 
                    isTimeScaleTemporarilyChanged = false;
                }
            }
        }
    }
}
