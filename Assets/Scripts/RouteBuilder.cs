using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RabbitLabirint
{
    public class RouteBuilder : MonoBehaviour
    {
        [SerializeField]
        private GameObject target;
        private Tilemap tilemap;
        private Vector3 targetCoordinate;

        public Vector3 Target
        {
            get
            {
                return GetCoordinateTarget();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            tilemap = gameObject.GetComponent<Tilemap>();
        }

        private void Update()
        {
           
        }

        /// <summary>
        /// Draw target
        /// </summary>
        public void DrawTarget()
        {
            if (target == null)
            {
                Debug.Log("target is null");
                return;
            }
            target.transform.position = new Vector3(Target.x, Target.y, 0);

            target.gameObject.SetActive(true);
        }

        /// <summary>
        /// Get the coordinates of the center of the tile clicked on
        /// </summary>
        /// <returns></returns>
        Vector3 GetCoordinateUnderMouse()
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 coordinate = tilemap.GetCellCenterLocal(Vector3Int.FloorToInt(mouseWorldPos));
            return coordinate;
        }

        /// <summary>
        /// Get target coordinates
        /// </summary>
        /// <returns></returns>
        Vector3 GetCoordinateTarget()
        {
            Vector3 mouseTileCoordinate = GetCoordinateUnderMouse();
            Vector3 playerTileCoordinate = PlayerController.Instance.Coordinate;
            Vector3 targetCoordinate = Vector3.zero;

            float deltaX = Math.Abs(mouseTileCoordinate.x - playerTileCoordinate.x);
            float deltaY = Math.Abs(mouseTileCoordinate.y - playerTileCoordinate.y);

            if (deltaX <= deltaY)
            {
                targetCoordinate.x = playerTileCoordinate.x;
                targetCoordinate.y = mouseTileCoordinate.y;
            }
            else if (deltaX > deltaY)
            {
                targetCoordinate.x = mouseTileCoordinate.x;
                targetCoordinate.y = playerTileCoordinate.y;
            }

            return targetCoordinate;
        }

    }
}
