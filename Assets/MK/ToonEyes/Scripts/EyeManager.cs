using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MK.Eyes
{
    public struct EyeData
    {
        public float eyeLidSizeTop;
        public float eyeLidSizeBot;

        public float defaultPupilSize;
        public float defaultEyeLidRecede;
        public float defaultBlinkSpeed;
        public Color defaultColor;

        public void SetEyeLids(float value)
        {
            eyeLidSizeTop = value;
            eyeLidSizeBot = value;
        }

        public void SetEyeLids(float top, float bot)
        {
            eyeLidSizeTop = top;
            eyeLidSizeBot = bot;
        }

        public void GetDefaultValeus(Renderer renderer)
        {
            defaultPupilSize = renderer.material.GetFloat("_PupilSize");
            defaultColor = renderer.material.GetColor("_Color");

            defaultEyeLidRecede = renderer.material.GetFloat("_EyeLidRecede");
            defaultBlinkSpeed = renderer.material.GetFloat("_BlinkSpeed");
        }

        public void ResetToDefault(Renderer renderer)
        {
            renderer.material.SetFloat("_PupilSize", defaultPupilSize);
            renderer.material.SetColor("_Color", defaultColor);
        }

        public void SetBlinkSpeed(Renderer renderer, float blinkSpeed)
        {
            renderer.material.SetFloat("_EyeLidRecede", blinkSpeed != 0 ? defaultEyeLidRecede / blinkSpeed : 0);
            renderer.material.SetFloat("_BlinkSpeed", defaultBlinkSpeed * blinkSpeed);
        }
    }

    public enum Emotion { Neutral, Excited, Angry, Bored, Tired, Confused, Suspicious, Trance, Intoxicated }

    public class EyeManager : MonoBehaviour
    {
        [SerializeField] Renderer eyeL;
        [SerializeField] Renderer eyeR;

        [Tooltip("The offset of the _Time variable in the eye shader. If -1 it will be generated on Awake.")]
        public float timeOffset = -1;
        public float blinkSpeed = 1f;

        [SerializeField] Emotion emotion;

        EyeData eyeDataL;
        EyeData eyeDataR;

        bool hasInitialized = false;

        private void Awake()
        {
            if (timeOffset < 0)
            {
                timeOffset = (transform.position.x + transform.position.y + transform.position.z) / 3;
            }

            eyeL.material.SetFloat("_TimeOffset", timeOffset);
            eyeR.material.SetFloat("_TimeOffset", timeOffset);

            eyeDataL.GetDefaultValeus(eyeL);
            eyeDataR.GetDefaultValeus(eyeR);

            SetBlinkSpeed(blinkSpeed);

            hasInitialized = true;

            SetEmotion(emotion);
            SetBlinkSpeed(blinkSpeed);
        }

        public void SetEmotion(Emotion emotion)
        {
            this.emotion = emotion;

            eyeDataL.ResetToDefault(eyeL);
            eyeDataR.ResetToDefault(eyeR);

            switch (emotion)
            {
                case Emotion.Neutral:
                    SetAllLidData(0);
                    break;
                case Emotion.Excited:
                    SetAllLidData(0, 0.3f);
                    break;
                case Emotion.Angry:
                    SetAllLidData(0.4f, 0.4f);
                    break;
                case Emotion.Tired:
                    SetAllLidData(0.4f, 0.2f);
                    break;
                case Emotion.Bored:
                    SetAllLidData(0.3f, 0f);
                    break;
                case Emotion.Confused:
                    eyeDataL.SetEyeLids(0.1f, 0.2f);
                    eyeDataR.SetEyeLids(0.3f, 0.25f);
                    break;
                case Emotion.Suspicious:
                    SetAllLidData(0.3f, 0.3f);
                    break;
                case Emotion.Trance:
                    SetAllLidData(0);
                    SetFloatValues("_PupilSize", 0.4f, 0.4f);
                    break;
                case Emotion.Intoxicated:
                    eyeDataL.SetEyeLids(0.1f, 0.2f);
                    eyeDataR.SetEyeLids(0.3f, 0.25f);

                    Color brightRed = new Color(1, 0.7f, 0.7f);
                    SetColorValues("_Color", brightRed, brightRed);
                    SetFloatValues("_PupilSize", 0.1f, 0.1f);

                    break;
            }

            UpdateEyeLidValues();
        }

        public void UpdateEyeLidValues()
        {
            SetEyeLidValues(eyeL, eyeDataL.eyeLidSizeTop, eyeDataL.eyeLidSizeBot);
            SetEyeLidValues(eyeR, eyeDataR.eyeLidSizeTop, eyeDataR.eyeLidSizeBot);
        }

        private void SetEyeLidValues(Renderer renderer, float top, float bot)
        {
            renderer.material.SetFloat("_EyeLidSizeTop", top);
            renderer.material.SetFloat("_EyeLidSizeBot", bot);
        }

        private void SetFloatValues(string variableName, float left, float right)
        {
            eyeL.material.SetFloat(variableName, left);
            eyeR.material.SetFloat(variableName, right);
        }

        private void SetColorValues(string variableName, Color left, Color right)
        {
            eyeL.material.SetColor(variableName, left);
            eyeR.material.SetColor(variableName, right);
        }

        private void SetAllLidData(float value)
        {
            eyeDataL.SetEyeLids(value);
            eyeDataR.SetEyeLids(value);
        }

        private void SetAllLidData(float top, float bot)
        {
            eyeDataL.SetEyeLids(top, bot);
            eyeDataR.SetEyeLids(top, bot);
        }

        private void SetBlinkSpeed(float blinkSpeed)
        {
            eyeDataL.SetBlinkSpeed(eyeL, blinkSpeed);
            eyeDataR.SetBlinkSpeed(eyeR, blinkSpeed);
        }

        private void OnValidate()
        {
            if (hasInitialized)
            {
                SetEmotion(emotion);
                SetBlinkSpeed(blinkSpeed);
            }          
        }
    }
}
