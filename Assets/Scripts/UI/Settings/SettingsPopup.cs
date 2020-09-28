using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

namespace RabbitLabirint
{
    public class SettingsPopup : MonoBehaviour
    {
        [SerializeField]
        private AudioMixer audioMixer;

        [SerializeField]
        private TMP_Dropdown qualityDropdown;

        [SerializeField]
        private Toggle fullScreenToggle;

        [SerializeField]
        private TMP_Dropdown resolutionDropdown;
        private Resolution[] resolutions;

        [SerializeField]
        private Slider volumeSlider;
        [SerializeField]
        private Slider volumeMusicSlider;
        [SerializeField]
        private Slider volumeSFXSlider;
        private const float minVolume = -80f;
        private const string volumeParamName = "Volume";
        private const string volumeMusicParamName = "VolumeMusic";
        private const string volumeSFXParamName = "VolumeSoundEffect";
        private float volume;
        private float volumeMusic;
        private float volumeSFX;

        private float maxValueVolumeSlider = 1.0f;

        public LoadoutState loadoutState;
        public DataDeleteConfirmation confirmationPopup;

        private void Start()
        {
            Open();
        }

        /// <summary>
        /// Get all quality level names and set list for quality dropdown
        /// </summary>
        private void SetListForQualityDropdown()
        {
            qualityDropdown.ClearOptions();
            List <string> names = new List<string>(QualitySettings.names);
            int qualityLevel = QualitySettings.GetQualityLevel();
            qualityDropdown.AddOptions(names);
            qualityDropdown.value = qualityLevel - 1;
            qualityDropdown.RefreshShownValue();
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

        public void Open()
        {
            gameObject.SetActive(true);
            UpdateUI();
        }

        public void Close()
        {
            PlayerData.Instance.Save();
            gameObject.SetActive(false);
        }

        private void UpdateUI()
        {
            SetListForQualityDropdown();
            SetListForResolutionDropdown();

            audioMixer.GetFloat(volumeParamName, out volume);
            audioMixer.GetFloat(volumeMusicParamName, out volumeMusic);
            audioMixer.GetFloat(volumeSFXParamName, out volumeSFX);

            volumeSlider.value = maxValueVolumeSlider - (volume / minVolume);
            volumeMusicSlider.value = maxValueVolumeSlider - (volumeMusic / minVolume);
            volumeSFXSlider.value = maxValueVolumeSlider - (volumeSFX / minVolume);

            fullScreenToggle.enabled = Screen.fullScreen;
        }

        /// <summary>
        /// Set master volume
        /// </summary>
        /// <param name="value">Received value from the slider</param>
        public void SetVolume(float value)
        {
            volume = minVolume * (maxValueVolumeSlider - value);
            audioMixer.SetFloat(volumeParamName, volume);
            PlayerData.Instance.masterVolume = volume;
        }

        /// <summary>
        /// Set music volume
        /// </summary>
        /// <param name="value">Received value from the slider</param>
        public void SetMusicVolume(float value)
        {
            volumeMusic = minVolume * (maxValueVolumeSlider - value);
            audioMixer.SetFloat(volumeMusicParamName, volumeMusic);
            PlayerData.Instance.musicVolume = volumeMusic;
        }

        /// <summary>
        /// Set sound effect volume
        /// </summary>
        /// <param name="value">Received value from the slider</param>
        public void SetSFXVolume(float value)
        {
            volumeSFX = minVolume * (maxValueVolumeSlider - value);
            audioMixer.SetFloat(volumeSFXParamName, volumeSFX);
            PlayerData.Instance.masterSFXVolume = volumeSFX;
        }

        /// <summary>
        /// Set game screen resolution
        /// </summary>
        /// <param name="resolutionIndex"></param>
        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            PlayerData.Instance.resolutionWidth = resolution.width;
            PlayerData.Instance.resolutionHeight = resolution.height;
        }

        /// <summary>
        /// Set graphics quality
        /// </summary>
        /// <param name="qualityIndex"></param>
        public void SetQuality(int qualityIndex)
        {
            int qualityLevel = qualityIndex + 1;
            QualitySettings.SetQualityLevel(qualityLevel);
            PlayerData.Instance.qualityLevel = qualityLevel;
        }

        /// <summary>
        /// Set full screen
        /// </summary>
        /// <param name="isFullScreen"></param>
        public void SetFullScreen(bool isFullScreen)
        {
            Screen.fullScreen = isFullScreen;
            PlayerData.Instance.isFullScreen = isFullScreen;
        }

        public void DeleteData()
        {
            confirmationPopup.Open(loadoutState);
        }
    }
}
