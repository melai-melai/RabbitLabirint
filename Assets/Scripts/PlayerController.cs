using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RabbitLabirint
{
    public class PlayerController : MonoSingleton<PlayerController>
    {
        public float speed = 5f;
        private int maxSteps = 5;
        [SerializeField]
        private GameObject playerPrefab;
        private GameObject currentPlayerGO;

        private Vector2 target;
        private Camera cam;
        private int currentPoints;
        private RouteBuilder routeBuilder;
        private Grid grid;
        private Tilemap tilemap;

        public Vector3 Coordinate
        {
            get
            {
                return gameObject.transform.position;
            }
        }

        public int Steps { get; private set; }

        public bool IsMoving { get; private set; }
        public bool IsFinished { get; private set; }


        // Start is called before the first frame update
        void Start()
        {
            cam = Camera.main;

            target = gameObject.transform.position;

            currentPoints = 0;
            Steps = maxSteps;

            IsMoving = false;
            IsFinished = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(1) && GameManager.Instance.topState.GetName() == "Game")
            {
                DrawWays();
            }

            if (Input.GetMouseButtonDown(0) && GameManager.Instance.topState.GetName() == "Game")
            {

                /*if (routeBuilder == GameObject.FindGameObjectWithTag("Level").GetComponent<LevelProvider>().RouteBuilder)
                {
                    Debug.Log("route builder equals!!!!");
                }
                routeBuilder.DrawTarget();
                */

                target = routeBuilder.Target;
                IsMoving = true;
                //Tilemap tilemap = GameObject.FindGameObjectWithTag("Tilemap").GetComponentInChildren<Tilemap>();
                //tilemap.GetCellCenterWorld(Vector3Int.FloorToInt(targetWorldPos));
            }
        }

        void FixedUpdate()
        {
            Move();
        }

        //private void OnGUI()
        //{
        //    Event currentEvent = Event.current;
        //    Vector2 mousePos = new Vector2();
        //    Vector2 point = new Vector2();

        //    // compute where the mouse is in world space
        //    mousePos.x = currentEvent.mousePosition.x;
        //    mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;
        //    point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0.0f));

        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        // set the target to the mouse click location
        //        target = point;
        //    }
        //}

        public void ChangePoints(int amount)
        {
            currentPoints += amount;
            UICarrot.Instance.SetValue(currentPoints);
        }

        void Move()
        {
            if (IsMoving && GameManager.Instance.topState.GetName() == "Game")
            {
                // move sprite towards the target location
                transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
                if (transform.position.Equals(target))
                {
                    FinishMoving();
                }
            }
        }

        /*private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.name == "Tilemap")
            {
                FinishMoving();
            }
        }*/

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.name == "RabbitHole")
            {
                IsFinished = true;
            }
        }

        void FinishMoving()
        {
            IsMoving = false;
            Steps = Mathf.Clamp(Steps - 1, 0, maxSteps);
            UIStep.Instance.SetValue(Steps);

            if (Steps == 0)
            {
                IsFinished = true;
            }

            ClearWays();
        }

        /// <summary>
        /// Prepare the player for the level
        /// </summary>
        public void PreparePlayerForLevel(LevelProvider lvl)
        {
            if (lvl == null)
            {
                Debug.Log("Level is not available!");
                GameManager.Instance.SwitchState("Loadout");
                return;
            }

            //Vector3Int cellPosition = lvl.Grid.LocalToCell(lvl.PlayerStartPosition.position);
            //transform.localPosition = lvl.Grid.GetCellCenterWorld(cellPosition);
            transform.position = lvl.PlayerStartPosition.position;
            //Destroy(lvl.PlayerStartPosition.gameObject);

            target = transform.localPosition;

            currentPoints = 0;
            Steps = maxSteps;

            IsMoving = false;
            IsFinished = false;

            grid = lvl.Grid;
            tilemap = lvl.Tilemap;
            routeBuilder = lvl.RouteBuilder;

            ResetPlayer();
            currentPlayerGO = Instantiate(playerPrefab, gameObject.transform, false) as GameObject;

        }

        /// <summary>
        /// Reset player (reset attached game object and etc.)
        /// </summary>
        public void ResetPlayer()
        {
            if (currentPlayerGO != null)
            {
                Destroy(currentPlayerGO);
                Debug.Log("Reset player " + currentPlayerGO.ToString());
            }
        }

        public void DrawWays() // TODO: Add states for player (and replace it)
        {
            routeBuilder.DrawPossiblePaths();
        }

        public void ClearWays() // TODO: Add states for player (and replace it)
        {
            routeBuilder.ClearBuiltPaths();
        }
    }

}
