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

        //private Vector2 position;
        private Vector2 target;
        private Camera cam;
        private int currentPoints;
        //private Tilemap tilemap;
        private Highlight highlightMap;

        public Vector3Int Coordinate
        {
            get
            {
                return Vector3Int.FloorToInt(gameObject.transform.position);
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
            if (Input.GetMouseButtonDown(0) && GameManager.Instance.topState.GetName() == "Game")
            {
                IsMoving = true;

                if (highlightMap == null)
                {
                    highlightMap = GameObject.FindGameObjectWithTag("HighlightTilemap").GetComponent<Highlight>();
                    Debug.Log("highlight reinit");
                }

                Vector3 targetWorldPos = highlightMap.Target;
                Tilemap tilemap = GameObject.FindGameObjectWithTag("Tilemap").GetComponentInChildren<Tilemap>();
                target = tilemap.GetCellCenterWorld(Vector3Int.FloorToInt(targetWorldPos));
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.name == "Tilemap")
            {
                FinishMoving();
            }            
        }

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
            Steps -= 1;
            UIStep.Instance.SetValue(Steps);
        }

        /// <summary>
        /// Prepare the player for the level
        /// </summary>
        public void PreparePlayerForLevel()
        {
            Grid grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
            Transform playerStartPosition = GameObject.FindGameObjectWithTag("PlayerStartPosition").GetComponent<Transform>();
            highlightMap = GameObject.FindGameObjectWithTag("HighlightTilemap").GetComponent<Highlight>();
            Vector3Int cellPosition = grid.LocalToCell(playerStartPosition.position);
            transform.localPosition = grid.GetCellCenterWorld(cellPosition);
            Destroy(playerStartPosition.gameObject);

            target = transform.localPosition;

            currentPoints = 0;
            Steps = maxSteps;

            IsMoving = false;
            IsFinished = false;

            ResetPlayer();
            currentPlayerGO = Instantiate(playerPrefab, gameObject.transform, false) as GameObject;
        }

        public void ResetPlayer()
        {
            if (currentPlayerGO != null)
            {
                Destroy(currentPlayerGO);
                Debug.Log("Reset player " + currentPlayerGO.ToString());
            }            
        }
    }

}
