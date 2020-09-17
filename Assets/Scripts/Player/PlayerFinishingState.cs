using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RabbitLabirint
{
    public class PlayerFinishingState : PlayerBaseState
    {
        /// <summary>
        /// Enter the state
        /// </summary>
        /// <param name="prevState">Previous state</param>
        public override void Enter(PlayerBaseState prevState)
        {
            Debug.Log("Enter Player Finishing State");
            if (PlayerController.Instance.OnFinish)
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
            PlayerController.Instance.OnFinish = false;
        }

        /// <summary>
        /// Get name of the state
        /// </summary>
        /// <returns>string name</returns>
        public override string GetName()
        {
            return "Finishing";
        }

        /// <summary>
        /// Execute every frame (in update function of game manager)
        /// </summary>
        public override void Tick()
        {

        }
    }
}
