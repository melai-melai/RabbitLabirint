using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RabbitLabirint
{
    /// <summary>
    /// Pushed on top of the GameManager during gameplay. Takes care of initializing all the UI.
    /// Also will take care of cleaning when leaving that state
    /// </summary>
    public class GameState : State
    {
        [SerializeField]
        private Canvas canvas;

        [SerializeField]
        private AudioClip gameMusic;

        [Header("UI")]
        public TMP_Text carrotText;
        public TMP_Text stepText;

        public RectTransform wholeUI;
        public RectTransform pauseMenu;
        public Button pauseButton;
        public RectTransform totalInfoPopup;

        protected bool isFinished;

        #region State
        /// <summary>
        /// Enter the state
        /// </summary>
        /// <param name="from">Previous state</param>
        public override void Enter(State from)
        {
            StartGame();
        }

        /// <summary>
        /// Exit from the state
        /// </summary>
        /// <param name="to">Next state</param>
        public override void Exit(State to)
        {
            canvas.gameObject.SetActive(false);
            pauseMenu.gameObject.SetActive(false);
            totalInfoPopup.gameObject.SetActive(false);
            wholeUI.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(false);

            LevelManager.Instance.ToggleLevelSelectPopup(false);
            LevelManager.Instance.DeleteLevel();

            PlayerController.Instance.ResetPlayer();

            // clear all collected points and steps
        }

        /// <summary>
        /// Get name of the state
        /// </summary>
        /// <returns>string name</returns>
        public override string GetName()
        {
            return "Game";
        }

        /// <summary>
        /// Execute every frame (in update function of game manager)
        /// </summary>
        public override void Tick()
        {
            if (PlayerController.Instance.IsFinished)
            {
                //    StartCoroutine("WaitForLevelGameOver");
                OpenTotalInfoPopup();
            }
        }
        #endregion

        #region Unity methods
        private void OnApplicationPause(bool pause)
        {
            if (pause) Pause();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus) Pause();
        }
        #endregion

        /// <summary>
        /// Prepare game for playing
        /// </summary>
        private void StartGame()
        {
            canvas.gameObject.SetActive(true);
            pauseMenu.gameObject.SetActive(false);
            totalInfoPopup.gameObject.SetActive(false);
            wholeUI.gameObject.SetActive(true);
            pauseButton.gameObject.SetActive(true);

            isFinished = false;

            // load current level or selected level
            LevelManager.Instance.LoadLevel();
        }

        /// <summary>
        /// Pause the game
        /// </summary>
        /// <param name="displayMenu"></param>
        public override void Pause(bool displayMenu = true)
        {
            //check if we aren't finished OR if we aren't already in pause (as that would mess states)
            if (isFinished || AudioListener.pause == true)
            {
                return;
            }

            AudioListener.pause = true;
            Time.timeScale = 0;

            pauseButton.gameObject.SetActive(false);
            pauseMenu.gameObject.SetActive(displayMenu);
            wholeUI.gameObject.SetActive(false);
        }

        /// <summary>
        /// Resume the game
        /// </summary>
        public override void Resume()
        {
            Time.timeScale = 1.0f;

            pauseButton.gameObject.SetActive(true);
            pauseMenu.gameObject.SetActive(false);
            wholeUI.gameObject.SetActive(true);

            AudioListener.pause = false;
        }

        /// <summary>
        /// Used by the pause menu to return 
        /// immediately to loadout, (not now????) canceling everything.
        /// </summary>
        public void QuitToLoadout()
        {
            Time.timeScale = 1.0f;
            AudioListener.pause = false;
            PlayerData.Instance.Save();

            gameManager.SwitchState("Loadout");
        }

        //IEnumerator WaitForLevelGameOver()
        //{
        //    isFinished = true;
        //    yield return null;
            // count level points (example 2 from 3 stars)
        //    OpenTotalInfoPopup();
        //}

        /// <summary>
        /// Game over with changing state
        /// </summary>
        public void GameOver()
        {
            gameManager.SwitchState("GameOver");
        }

        /// <summary>
        /// Open popup with level results
        /// </summary>
        private void OpenTotalInfoPopup()
        {
            totalInfoPopup.gameObject.SetActive(true);
        }

        /// <summary>
        /// Load selected level (example: select from game state)
        /// </summary>
        public override void LoadSelectedLevel()
        {
            StartGame();
        }

        /// <summary>
        /// Repeat level
        /// </summary>
        public void RepeatLevel()
        {
            LevelManager.Instance.RepeatLevel();
            totalInfoPopup.gameObject.SetActive(false);
        }

        /// <summary>
        /// Load next level
        /// </summary>
        public void GoNextLevel()
        {
            LevelManager.Instance.LoadNextLevel();
            totalInfoPopup.gameObject.SetActive(false);
        }
    }
}
