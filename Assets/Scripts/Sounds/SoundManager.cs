using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace RabbitLabirint
{
    public class SoundManager : MonoSingleton<SoundManager>
    {
        private const string volumeParamName = "Volume";
        private const string volumeMusicParamName = "VolumeMusic";
        private const string volumeSFXParamName = "VolumeSoundEffect";

        [SerializeField]
        private AudioMixer audioMixer;

        public override void Init()
        {
            base.Init();
        }

        // Start is called before the first frame update
        void Start()
        {
            PlayerData.Create();

            if (PlayerData.Instance.masterVolume > float.MinValue)
            {
                SetSavedVolumes();
            }
            else
            {
                SaveVolumes();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        #region Music management
        /// <summary>
        /// Set audio mixer volume from player data
        /// </summary>
        public void SetSavedVolumes()
        {
            audioMixer.SetFloat(volumeParamName, PlayerData.Instance.masterVolume);
            audioMixer.SetFloat(volumeMusicParamName, PlayerData.Instance.musicVolume);
            audioMixer.SetFloat(volumeSFXParamName, PlayerData.Instance.masterSFXVolume);
        }

        /// <summary>
        /// Save audio mixer volume in player data
        /// </summary>
        public void SaveVolumes()
        {
            audioMixer.GetFloat(volumeParamName, out PlayerData.Instance.masterVolume);
            audioMixer.GetFloat(volumeMusicParamName, out PlayerData.Instance.musicVolume);
            audioMixer.GetFloat(volumeSFXParamName, out PlayerData.Instance.masterSFXVolume);

            PlayerData.Instance.Save();
        }
        #endregion
    }
}
