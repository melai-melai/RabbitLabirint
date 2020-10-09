using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace RabbitLabirint
{
    public class CarrotCollectible : MonoBehaviour
    {
        private readonly int point = 1;
        private Animator animator;

        private void Start()
        {
            animator = gameObject.GetComponent<Animator>();

            float randomTime = Random.Range(0.1f, 0.5f);
            StartCoroutine("WaitBeforeStartAnimation", randomTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                PlayerController.Instance.EatCarrot(point);
                gameObject.SetActive(false);
            }
        }

        IEnumerator WaitBeforeStartAnimation(float time)
        {
            yield return new WaitForSeconds(time);
            animator.SetBool("isMoving", true);
        }
    }
}

