using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RabbitLabirint
{
    /// <summary>
    /// Pushed on top of the GameManager during gameplay. Takes care of initializing all the UI and start the TrackManager
    /// Also will take care of cleaning when leaving that state.
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

        protected bool isFinished;



        public override void Enter(State from)
        {
            StartGame();
        }

        private void StartGame()
        {
            canvas.gameObject.SetActive(true);
            pauseMenu.gameObject.SetActive(false);
            wholeUI.gameObject.SetActive(true);
            pauseButton.gameObject.SetActive(true);

            isFinished = false;

            UIStep.Instance.SetValue(PlayerController.Instance.steps);
            UICarrot.Instance.SetValue(0);
        }

        public override void Exit(State to)
        {
            canvas.gameObject.SetActive(false);

            // clear all collected points and steps
        }

        public override string GetName()
        {
            return "Game";
        }

        public override void Tick()
        {
            
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause) Pause();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus) Pause();
        }

        public void Pause(bool displayMenu = true)
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

        public void Resume()
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

            gameManager.SwitchState("Loadout");
        }

        IEnumerator WaitForGameOver()
        {
            isFinished = true;

            yield return new WaitForSeconds(2.0f);

            gameManager.SwitchState("GameOver");
        }

        public void GameOver()
        {
            gameManager.SwitchState("GameOver");
        }
    }
}
