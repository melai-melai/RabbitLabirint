using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RabbitLabirint
{
    public class PlayerMovingState : PlayerBaseState
    {
        private float speed = 5f;
        private Vector3 target;
        private bool hasTarget ;

        /// <summary>
        /// Enter the state
        /// </summary>
        /// <param name="prevState">Previous state</param>
        public override void Enter(PlayerBaseState prevState)
        {
            Debug.Log("Enter Player Moving State");
            target = PlayerController.Instance.RouteBuilder.Target;
            hasTarget = true;
        }

        /// <summary>
        /// Exit from the state
        /// </summary>
        /// <param name="nextState">Next state</param>
        public override void Exit(PlayerBaseState nextState)
        {
            hasTarget = false;
            PlayerController.Instance.ChangeSteps(1);
            PlayerController.Instance.RouteBuilder.ClearBuiltPaths();
            Debug.Log("Exit Player Moving State");
        }

        /// <summary>
        /// Get name of the state
        /// </summary>
        /// <returns>string name</returns>
        public override string GetName()
        {
            return "Moving";
        }

        /// <summary>
        /// Execute every frame (in update function of player controller)
        /// </summary>
        public override void Tick()
        {
                       
        }

        /// <summary>
        /// Moves the player to the target
        /// </summary>
        void Move()
        {
            // move sprite towards the target location
            PlayerController.Instance.transform.position = Vector2.MoveTowards(PlayerController.Instance.transform.position, target, speed * Time.deltaTime);
            if (PlayerController.Instance.transform.position.Equals(target))
            {
                PlayerController.Instance.SwitchState("Finishing");
            }
        }

        /// <summary>
        /// Execute each fixed frame (in fixedUpdate function player controller)
        /// </summary>
        public override void FixedTick()
        {
            if (hasTarget)
            {
                Move();
            }
        }
    }
}
