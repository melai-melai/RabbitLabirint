﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RabbitLabirint
{
    public class PlayerAttackedState : PlayerBaseState
    {
        public PlayerAttackedState(string name) : base(name) {}

        /// <summary>
        /// Enter the state
        /// </summary>
        /// <param name="prevState">Previous state</param>
        public override void Enter(PlayerBaseState prevState)
        {
            Debug.Log("Enter Player Attacked State");
            // attacked animation
            PlayerController.Instance.TriggerAttacked();
        }

        /// <summary>
        /// Exit from the state
        /// </summary>
        /// <param name="nextState">Next state</param>
        public override void Exit(PlayerBaseState nextState)
        {
            Debug.Log("Exit Player Attacked State");
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
