using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace RabbitLabirint
{
    public class CarrotCollectible : MonoBehaviour
    {
        private int point = 1;

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.ChangePoints(point);
                Destroy(gameObject);
            }
        }
    }
}

