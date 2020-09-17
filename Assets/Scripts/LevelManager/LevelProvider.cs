using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RabbitLabirint
{
    public class LevelProvider : MonoBehaviour
    {
        [SerializeField]
        private Grid grid;
        [SerializeField]
        private Transform playerStartPosition;
        [SerializeField]
        private Tilemap tilemap;
        [SerializeField]
        private RouteBuilder routeBuilder;
        [SerializeField]
        private int maxSteps;

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

        public Tilemap Tilemap
        {
            get
            {
                return tilemap;
            }
        }

        public RouteBuilder RouteBuilder
        {
            get
            {
                return routeBuilder;
            }
        }

        public int MaxSteps
        {
            get
            {
                return maxSteps;
            }
        }
    }
}
