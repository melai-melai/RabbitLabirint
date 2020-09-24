using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RabbitLabirint
{
    public class PlayerWaitingState : PlayerBaseState
    {
        bool hasInput = false;
        Vector3 inputPosition = Vector3.zero;

        public PlayerWaitingState(string name) : base(name) {}

        public override Vector3 InputPosition
        {
            get { return Camera.main.ScreenToWorldPoint(inputPosition); }
        }

        /// <summary>
        /// Enter the state
        /// </summary>
        /// <param name="prevState">Previous state</param>
        public override void Enter(PlayerBaseState prevState)
        {
            Debug.Log("Enter Player Waiting State");
            PlayerController.Instance.RouteBuilder.DrawPossiblePaths();
            inputPosition = Vector3.zero;
        }

        /// <summary>
        /// Exit from the state
        /// </summary>
        /// <param name="nextState">Next state</param>
        public override void Exit(PlayerBaseState nextState)
        {
            hasInput = false;
            Debug.Log("Exit Player Waiting State");
        }

        /// <summary>
        /// Execute every frame (in update function of player controller)
        /// </summary>
        public override void Tick()
        {
#if UNITY_EDITOR || UNITY_STANDALONE

            //If the left mouse button is clicked.
            if (Input.GetMouseButtonDown(0) && EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject(-1))
            {
                hasInput = true;
                inputPosition = Input.mousePosition;
            }

#else
            // Use touch input on mobile
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject(touch.fingerId) && touch.phase == TouchPhase.Began)
                {
                    hasInput = true;
                    inputPosition = touch.position;
                }
            }
#endif
        }

        /// <summary>
        /// Execute each fixed frame (in fixedUpdate function player controller)
        /// </summary>
        public override void FixedTick()
        {
            if (hasInput)
            {
                hasInput = false;

                if (PlayerController.Instance.CheckHitTilemap(inputPosition))
                {
                    PlayerController.Instance.SwitchState("Moving");
                }
            }
        }
    }
}
