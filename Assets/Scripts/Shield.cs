using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RabbitLabirint
{
    public class Shield : MonoBehaviour
    {
        public float time;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                PlayerController.Instance.SetShield(time);
                Destroy(gameObject);
            }
        }
    }
}
