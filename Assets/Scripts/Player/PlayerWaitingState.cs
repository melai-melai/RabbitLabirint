using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RabbitLabirint
{
    public class PlayerWaitingState : PlayerBaseState
    {
        /// <summary>
        /// Enter the state
        /// </summary>
        /// <param name="prevState">Previous state</param>
        public override void Enter(PlayerBaseState prevState)
        {
            Debug.Log("Enter Player Waiting State");
            PlayerController.Instance.RouteBuilder.DrawPossiblePaths();
        }

        /// <summary>
        /// Exit from the state
        /// </summary>
        /// <param name="nextState">Next state</param>
        public override void Exit(PlayerBaseState nextState)
        {
            Debug.Log("Exit Player Waiting State");
        }

        /// <summary>
        /// Get name of the state
        /// </summary>
        /// <returns>string name</returns>
        public override string GetName()
        {
            return "Waiting";
        }

        /// <summary>
        /// Execute every frame (in update function of player controller)
        /// </summary>
        public override void Tick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                PlayerController.Instance.SwitchState("Moving");
            }
        }

        /// <summary>
        /// Execute each fixed frame (in fixedUpdate function player controller)
        /// </summary>
        public override void FixedTick()
        {

        }
    }
}
