using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RabbitLabirint
{
    /// <summary>
    /// Provides data about the current level, within which the player is currently playing 
    /// (objects on the level, data for calculating the result of passing and restrictions)
    /// </summary>
    public class LevelProvider : MonoBehaviour
    {
        [Header("Level restrictions")]
        [SerializeField]
        private int maxSteps;

        [Header("Level Game Objects")]
        [SerializeField]
        private Grid grid;
        [SerializeField]
        private Transform playerStartPosition;
        [SerializeField]
        private Tilemap tilemap;
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

        public int Carrots
        {
            get
            {
                return GetCarrotsLevel();
            }
        }

        /// <summary>
        /// Get the number of carrots at this level
        /// </summary>
        /// <returns>The number of carrots</returns>
        private int GetCarrotsLevel()
        {
            int carrots = 0;

            CarrotCollectible[] carrotArray = tilemap.gameObject.GetComponentsInChildren<CarrotCollectible>(true);

            if (carrotArray.Length > 0)
            {
                carrots = carrotArray.Length;
            }

            return carrots;
        }
    }
}
