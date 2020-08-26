﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RabbitLabirint
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField]
        private GameObject startGameScreen;
        [SerializeField]
        private GameObject gameScreen;
        [SerializeField]
        private GameObject pauseScreen;
        [SerializeField]
        private GameObject settingsScreen;
        [SerializeField]
        private GameObject gameOverScreen;
        [SerializeField]
        private GameObject winGameScreen;

        private AudioSource audioSource;
        [SerializeField]
        private AudioClip menuClip;
        [SerializeField]
        private AudioClip gameClip;
        [SerializeField]
        private AudioClip pauseClip;
        [SerializeField]
        private AudioClip gameOverClip;

        private bool isPause = false;

        public override void Init()
        {
            base.Init();

            audioSource = GetComponent<AudioSource>();
            //PlayNewBackgroundMusic(menuClip);
        }

        private void Update()
        {
            // keyword controls
        }

        #region Game states
        /// <summary>
        /// Start game function
        /// </summary>
        public void StartGame()
        {
            ChangeScreen(startGameScreen, gameScreen);
            PlayNewBackgroundMusic(gameClip);
        }

        /// <summary>
        /// Pause function
        /// </summary>
        public void PauseGame()
        {
            if (isPause)
            {
                Time.timeScale = 0;
                PlayNewBackgroundMusic(pauseClip);
            } 
            else
            {
                Time.timeScale = 1.0f;
                PlayNewBackgroundMusic(gameClip);
            }

            pauseScreen.gameObject.SetActive(isPause);
        }
        
        /// <summary>
        /// Game over function
        /// </summary>
        public void GameOver()
        {
            ChangeScreen(gameScreen, gameOverScreen);
            PlayNewBackgroundMusic(gameOverClip);
        }
        
        /// <summary>
        /// Quit game function
        /// </summary>
        public void QuitGame()
        {
            Application.Quit();
        }
        #endregion

        #region Settings
        public void ShowSettings()
        {
            ChangeScreen(startGameScreen, settingsScreen);
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Set new background audio clip and play it
        /// </summary>
        /// <param name="newClip"></param>
        public void PlayNewBackgroundMusic(AudioClip newClip)
        {
            audioSource.clip = newClip;
            audioSource.Play();
        }

        /// <summary>
        /// Change active screen
        /// </summary>
        /// <param name="oldScreen"></param>
        /// <param name="newScreen"></param>
        public void ChangeScreen(GameObject oldScreen, GameObject newScreen)
        {
            oldScreen.gameObject.SetActive(false);
            newScreen.gameObject.SetActive(true);
        }
        #endregion
    }
}
