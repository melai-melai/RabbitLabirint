using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RabbitLabirint
{
    public class LevelProvider : MonoBehaviour
    {
        [SerializeField]
        private Grid grid;
        [SerializeField]
        private Transform playerStartPosition;
        [SerializeField]
        private RouteBuilder routeBuilder;

        public Grid Grid
        {
            get
            {
                return grid;
            }
        }

        public Transform PlayerStartPosition
        {
            get
            {
                return playerStartPosition;
            }
        }

        public RouteBuilder RouteBuilder
        {
            get
            {
                return routeBuilder;
            }
        }
    }
}
