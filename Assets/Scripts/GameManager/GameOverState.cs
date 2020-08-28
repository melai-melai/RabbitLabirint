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

        public override void Enter(State from)
        {
            canvas.gameObject.SetActive(true);

            // add music for game over (play)
        }

        public override void Exit(State to)
        {
            canvas.gameObject.SetActive(false);
        }

        public override string GetName()
        {
            return "GameOver";
        }

        public override void Tick()
        {
           // ??? 
        }

        public void GoToLoadout()
        {
            gameManager.SwitchState("Loadout");
        }

        public void PlayAgain()
        {
            gameManager.SwitchState("Game");
        }
    }
}
