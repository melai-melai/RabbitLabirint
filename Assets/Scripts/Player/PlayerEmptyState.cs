using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RabbitLabirint
{
    public class PlayerEmptyState : PlayerBaseState
    {
        public PlayerEmptyState(string name) : base(name) {}

        /// <summary>
        /// Enter the state
        /// </summary>
        /// <param name="prevState">Previous state</param>
        public override void Enter(PlayerBaseState prevState)
        {
            Debug.Log("Enter Player Empty State");
            PlayerController.Instance.RemoveShield();
            PlayerController.Instance.ResetPlayerData();
        }

        /// <summary>
        /// Exit from the state
        /// </summary>
        /// <param name="nextState">Next state</param>
        public override void Exit(PlayerBaseState nextState)
        {
            Debug.Log("Exit Player Empty State");
            PlayerController.Instance.SetPlayerData();
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
