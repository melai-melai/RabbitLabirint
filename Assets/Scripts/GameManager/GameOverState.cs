using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RabbitLabirint
{
    /// <summary>
    /// State pushed on top of the GameManager when the player dies
    /// </summary>
    public class GameOverState : State
    {
        [SerializeField]
        private Canvas canvas;

        [SerializeField]
        private AudioClip gameOverMusic;

        #region State
        /// <summary>
        /// Enter the state
        /// </summary>
        /// <param name="from">Previous state</param>
        public override void Enter(State from)
        {
            canvas.gameObject.SetActive(true);

            // add music for game over (play)
        }

        /// <summary>
        /// Exit from the state
        /// </summary>
        /// <param name="to">Next state</param>
        public override void Exit(State to)
        {
            canvas.gameObject.SetActive(false);
        }

        /// <summary>
        /// Get name of the state
        /// </summary>
        /// <returns>string name</returns>
        public override string GetName()
        {
            return "GameOver";
        }

        /// <summary>
        /// Execute every frame (in update function of game manager)
        /// </summary>
        public override void Tick()
        {
           // some code
        }
        #endregion

        /// <summary>
        /// Go to the loadout state
        /// </summary>
        public void GoToLoadout()
        {
            gameManager.SwitchState("Loadout");
        }

        //public void PlayAgain()
        //{
        //    gameManager.SwitchState("Game"); // TODO: start current level again
        //}
    }
}
