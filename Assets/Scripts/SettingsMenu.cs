using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

namespace RabbitLabirint
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField]
        private AudioMixer audioMixer;

        [SerializeField]
        private TMP_Dropdown resolutionDropdown;

        Resolution[] resolutions;

        void Start()
        {
            SetListForResolutionDropdown();
        }

        /// <summary>
        /// Get all resolution for screen and set list for resolution dropdown
        /// </summary>
        private void SetListForResolutionDropdown()
        {
            resolutions = Screen.resolutions;

            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();

            int currentResolutionIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        /// <summary>
        /// Set game screen resolution
        /// </summary>
        /// <param name="resolutionIndex"></param>
        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        /// <summary>
        /// Set master volume
        /// </summary>
        /// <param name="volume"></param>
        public void SetVolume(float volume)
        {
            audioMixer.SetFloat("Volume", volume);
        }

        /// <summary>
        /// Set graphics quality
        /// </summary>
        /// <param name="qualityIndex"></param>
        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex + 1);
        }

        /// <summary>
        /// Set full screen
        /// </summary>
        /// <param name="isFullScreen"></param>
        public void SetFullScreen(bool isFullScreen)
        {
            Screen.fullScreen = isFullScreen;
        }
    }
}
