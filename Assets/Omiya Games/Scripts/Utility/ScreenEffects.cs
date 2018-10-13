using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="RotateObject.cs" company="Omiya Games">
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
    /// <date>10/13/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>Controls animations on the camera</summary>
    [RequireComponent(typeof(Animator))]
    public class ScreenEffects : MonoBehaviour
    {
        const string ShakeOnceTrigger = "Shake Once";
        const string FlashOnceTrigger = "Flash Once";
        const string XOffset = "x-offset";
        const string YOffset = "y-offset";
        const string TiltOffset = "tilt-offset";
        const float DefaultShakePositionIntensity = 0.1f;
        const float DefaultShakeRotationIntensity = 0f;

        [SerializeField]
        UnityEngine.UI.Image flashImage;

        Animator animator = null;

        Animator Animator
        {
            get
            {
                if(animator == null)
                {
                    animator = GetComponent<Animator>();
                }
                return animator;
            }
        }

        public void ShakeOnce(float maxShakePositionIntensity = DefaultShakePositionIntensity, float maxShakeRotationIntensity = DefaultShakeRotationIntensity)
        {
            float shakeIntensity = Mathf.Clamp01(maxShakePositionIntensity);
            if (Mathf.Approximately(shakeIntensity, 0f) == false)
            {
                Vector2 position = Random.insideUnitCircle * shakeIntensity;
                Animator.SetFloat(XOffset, position.x);
                Animator.SetFloat(YOffset, position.y);
            }

            shakeIntensity = Mathf.Clamp01(maxShakeRotationIntensity);
            if (Mathf.Approximately(shakeIntensity, 0f) == false)
            {
                shakeIntensity = Random.Range(-shakeIntensity, shakeIntensity);
                Animator.SetFloat(TiltOffset, shakeIntensity);
            }
            Animator.SetTrigger(ShakeOnceTrigger);
        }

        public void FlashOnce(Color flashColor)
        {
            flashImage.color = flashColor;
            Animator.SetTrigger(FlashOnceTrigger);
        }

#if UNITY_EDITOR
        [SerializeField]
        bool shakePositionOnce = false;
        [SerializeField]
        bool shakeRotationOnce = false;
        [SerializeField]
        bool flashOnce = false;

        private void Update()
        {
            if(shakePositionOnce == true)
            {
                ShakeOnce();
                shakePositionOnce = false;
            }
            if (shakeRotationOnce == true)
            {
                ShakeOnce(0f, DefaultShakePositionIntensity);
                shakeRotationOnce = false;
            }
            if (flashOnce == true)
            {
                FlashOnce(Color.white);
                flashOnce = false;
            }
        }
#endif
    }
}
