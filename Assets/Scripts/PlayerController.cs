using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RabbitLabirint
{
    public class PlayerController : MonoSingleton<PlayerController>
    {
        public float speed = 5f;
        public int steps = 5;

        public Vector3Int Coordinate
        {
            get
            {
                return Vector3Int.FloorToInt(gameObject.transform.position);
            }
        }

        public bool IsMoving { get; private set; }

        private Vector2 position;
        private Vector2 target;
        private Camera cam;
        private int currentPoints;
        private Tilemap tilemap;        

        // Start is called before the first frame update
        void Start()
        {
            position = gameObject.transform.position;
            target = position;

            cam = Camera.main;

            currentPoints = 0;

            tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
            gameObject.transform.position = tilemap.GetCellCenterWorld(Coordinate);

            IsMoving = false;
            UIStep.Instance.SetValue(steps); // transfer to start level function
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                IsMoving = true;

                Vector3 targetWorldPos = Highlight.Instance.Target;
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
            if (IsMoving)
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

        void FinishMoving()
        {
            IsMoving = false;
            steps -= 1;
            UIStep.Instance.SetValue(steps);
        }
    }

}
