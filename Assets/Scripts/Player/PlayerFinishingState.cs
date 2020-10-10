using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RabbitLabirint
{
    public class PlayerFinishingState : PlayerBaseState
    {
        public PlayerFinishingState(string name) : base(name) {}

        /// <summary>
        /// Enter the state
        /// </summary>
        /// <param name="prevState">Previous state</param>
        public override void Enter(PlayerBaseState prevState)
        {
            Debug.Log("Enter Player Finishing State");

            if (PlayerController.Instance.OnFinish && IsGoalReached())
            {
                PlayerController.Instance.SwitchState("Finished");
            } 
            else
            {
                if (PlayerController.Instance.Steps == 0)
                {
                    GameManager.Instance.SwitchState("GameOver");
                }
                else
                {
                    PlayerController.Instance.SwitchState("Idle");
                }
            }            
        }

        /// <summary>
        /// Exit from the state
        /// </summary>
        /// <param name="nextState">Next state</param>
        public override void Exit(PlayerBaseState nextState)
        {
            Debug.Log("Exit Player Finishing State");
        }

        /// <summary>
        /// Execute every frame (in update function of player controller)
        /// </summary>
        public override void Tick()
        {

        }

        /// <summary>
        /// Execute each fixed frame (in fixedUpdate function player controller)
        /// </summary>
        public override void FixedTick()
        {

        }

        /// <summary>
        /// Checks if the goal has been achieved
        /// </summary>
        private bool IsGoalReached()
        {
            // the player has collected all the carrots in the level
            if (PlayerController.Instance.Points == LevelManager.Instance.LevelData.Carrots)
            {
                return true;
            }

            return false;
        }
    }
}
