using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace RabbitLabirint
{
    public class CarrotCollectible : MonoBehaviour
    {
        private readonly int point = 1;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                PlayerController.Instance.ChangePoints(point);
                gameObject.SetActive(false);
            }
        }
    }
}

