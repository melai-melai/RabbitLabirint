using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private Dictionary<string, Vector3> directions = new Dictionary<string, Vector3>();

        private List<Vector3> pathPoints = new List<Vector3>();

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

            directions.Add("top", PlayerController.Instance.Coordinate);
            directions.Add("bottom", PlayerController.Instance.Coordinate);
            directions.Add("left", PlayerController.Instance.Coordinate);
            directions.Add("right", PlayerController.Instance.Coordinate);
        }

        private void Update()
        {
           
        }

        /// <summary>
        /// Draw target
        /// </summary>
        /*public void DrawTarget()
        {
            if (target == null)
            {
                Debug.Log("target is null");
                return;
            }
            target.transform.position = new Vector3(Target.x, Target.y, 0);

            target.gameObject.SetActive(true);
        }*/

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
        /// Get target coordinates (the closest to mouse point and belonging to path point)
        /// </summary>
        /// <returns></returns>
        Vector3 GetCoordinateTarget()
        {
            Vector3 mouseTileCoordinate = GetCoordinateUnderMouse();
            Vector3 playerTileCoordinate = PlayerController.Instance.Coordinate;

            Vector3 targetCoordinate = playerTileCoordinate;
            float minDistance = Vector3.Distance(playerTileCoordinate, mouseTileCoordinate);

            foreach (Vector3 point in pathPoints)
            {
                float distance = Vector3.Distance(point, mouseTileCoordinate);

                if (distance < minDistance)
                {
                    targetCoordinate = point;
                    minDistance = distance;
                }
            }

            Debug.Log("Mouse coord: " + mouseTileCoordinate);
            Debug.Log("Target coord: " + targetCoordinate);
            Debug.Log("Min distance: " + minDistance);
            return targetCoordinate;
        }


        /// <summary>
        /// Draw possible paths the player can follow at this step
        /// </summary>
        public void DrawPossiblePaths()
        {
            foreach (string key in directions.Keys.ToList())
            {
                directions[key] = BuildPathTowards(key, directions[key]);
                Debug.Log("Конечная точка направления " + key + " равна " + directions[key]);
            }
        }

        /// <summary>
        /// Build a path towards 
        /// </summary>
        /// <param name="direction">The direction along which the path is built</param>
        /// <param name="startPoint">Starting point of the path</param>
        /// <returns></returns>
        private Vector3 BuildPathTowards(string direction, Vector3 startPoint)
        {
            Vector3 step = Vector3.zero;

            switch (direction)
            {
                case "top":
                    step = Vector3.up;
                    break;
                case "bottom":
                    step = Vector3.down;
                    break;
                case "left":
                    step = Vector3.left;
                    break;
                case "right":
                    step = Vector3.right;
                    break;
            }
            Vector3 newPoint = startPoint;

            while (CheckIfRoadTile(newPoint + step))
            {
                newPoint += step;
                pathPoints.Add(newPoint);
                ChangeColorForRoadTile(newPoint, Color.cyan);
            }

            return newPoint;
        }

        /// <summary>
        /// Checks if a tile belongs to road tiles
        /// </summary>
        /// <param name="tileCoordinate">Tile coordinates to check</param>
        /// <returns></returns>
        private bool CheckIfRoadTile(Vector3 tileCoordinate)
        {
            TileBase t = tilemap.GetTile<RoadRuleTile>(Vector3Int.FloorToInt(tileCoordinate));
            return t != null;
        }

        /// <summary>
        /// Change road tile color
        /// </summary>
        /// <param name="tileCoordinate">Tile coordinates</param>
        /// <param name="color">Color</param>
        private void ChangeColorForRoadTile(Vector3 tileCoordinate, Color color)
        {
            tilemap.SetColor(Vector3Int.FloorToInt(tileCoordinate), color);
        }

        /// <summary>
        /// Clear possible built paths (color and data)
        /// </summary>
        public void ClearBuiltPaths()
        {
            foreach (var item in pathPoints)
            {
                ChangeColorForRoadTile(item, Color.white);
            }

            directions["top"] = PlayerController.Instance.Coordinate;
            directions["bottom"] = PlayerController.Instance.Coordinate;
            directions["left"] = PlayerController.Instance.Coordinate;
            directions["right"] = PlayerController.Instance.Coordinate;

            pathPoints.Clear();
        }

    }
}
