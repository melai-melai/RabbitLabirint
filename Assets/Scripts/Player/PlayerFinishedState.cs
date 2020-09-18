using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RabbitLabirint
{
    public class PlayerFinishedState : PlayerBaseState
    {
        /// <summary>
        /// Enter the state
        /// </summary>
        /// <param name="prevState">Previous state</param>
        public override void Enter(PlayerBaseState prevState)
        {
            // animation
            Debug.Log("Reached the finish");
            Debug.Log("Enter Player Finished State");

            PlayerController.CallOnFinished(); // TODO: need refactoring
        }

        /// <summary>
        /// Exit from the state
        /// </summary>
        /// <param name="nextState">Next state</param>
        public override void Exit(PlayerBaseState nextState)
        {
            Debug.Log("Exit Player Finished State");
        }

        /// <summary>
        /// Get name of the state
        /// </summary>
        /// <returns>string name</returns>
        public override string GetName()
        {
            return "Finished";
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
    }
}
