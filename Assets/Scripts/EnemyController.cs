using RabbitLabirint;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("You are died!");
            GameManager.Instance.SwitchState("GameOver");
        }
    }
}
