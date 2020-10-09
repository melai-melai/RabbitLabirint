using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RabbitLabirint
{
    /// <summary>
    /// The state of the player upon successful completion of the level
    /// </summary>
    public class PlayerFinishedState : PlayerBaseState
    {
        public PlayerFinishedState(string name) : base(name) {}

        /// <summary>
        /// Enter the state
        /// </summary>
        /// <param name="prevState">Previous state</param>
        public override void Enter(PlayerBaseState prevState)
        {
            Debug.Log("Enter Player Finished State");

            PlayerController.Instance.CallOnFinished();
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
