using RabbitLabirint;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RabbitLabirint
{
    /// <summary>
    /// State pushed on the GameManager during the Loadout
    /// </summary>
    public class LoadoutState : State
    {
        [SerializeField]
        private Canvas inventoryCanvas;

        [SerializeField]
        private Button playButton;

        [SerializeField]
        private AudioClip menuMusic;

        #region State
        /// <summary>
        /// Enter the state
        /// </summary>
        /// <param name="from">Previous state</param>
        public override void Enter(State from)
        {
            inventoryCanvas.gameObject.SetActive(true);

            // set menu music (play)

            playButton.interactable = false;
            playButton.GetComponentInChildren<TextMeshProUGUI>().text = "Loading...";
        }

        /// <summary>
        /// Exit from the state
        /// </summary>
        /// <param name="to">Next state</param>
        public override void Exit(State to)
        {
            inventoryCanvas.gameObject.SetActive(false);
        }

        /// <summary>
        /// Get name of the state
        /// </summary>
        /// <returns>string name</returns>
        public override string GetName()
        {
            return "Loadout";
        }

        /// <summary>
        /// Execute every frame (in update function of game manager)
        /// </summary>
        public override void Tick()
        {
            if (!playButton.interactable)
            {
                playButton.interactable = true;
                playButton.GetComponentInChildren<TextMeshProUGUI>().text = "Play!";
            }
        }
        #endregion

        /// <summary>
        /// Start game (example: click on "Play" button on the loadout screen)
        /// </summary>
        public void StartGame()
        {
            LevelManager.Instance.SetCurrentLevelFromSave(); // TODO: needs refactoring
            gameManager.SwitchState("Game");
        }
    }
}
