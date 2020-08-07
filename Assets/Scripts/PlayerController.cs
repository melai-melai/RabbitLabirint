using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RabbitLabirint
{
    public class PlayerController : MonoBehaviour
    {
        public float speed = 5f;

        private Vector2 position;
        private Vector2 target;
        private Camera cam;
        private int currentPoints;

        // Start is called before the first frame update
        void Start()
        {
            position = gameObject.transform.position;
            target = position;

            cam = Camera.main;

            currentPoints = 0;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void FixedUpdate()
        {
            // move sprite towards the target location
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }

        private void OnGUI()
        {
            Event currentEvent = Event.current;
            Vector2 mousePos = new Vector2();
            Vector2 point = new Vector2();

            // compute where the mouse is in world space
            mousePos.x = currentEvent.mousePosition.x;
            mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;
            point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0.0f));

            if (Input.GetMouseButtonDown(0))
            {
                // set the target to the mouse click location
                target = point;
            }
        }

        public void ChangePoints(int amount)
        {
            currentPoints += amount;
        }
    }

}
