using RabbitLabirint;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Animator animator;

    [Header("Particles")]
    public ParticleSystem jumpEffect;

    private AudioSource audioSource;
    [Header("Sound")]
    public AudioClip wakeClip;
    public AudioClip idleClip;
    public AudioClip attackClip;
    public AudioClip jumpClip;

    private void OnEnable()
    {
        PlayerController.onHappenedFinish += StopSound;
        PlayerController.onHappenedFinish += StopAllCoroutines;
    }

    private void OnDisable()
    {
        PlayerController.onHappenedFinish -= StopSound;
        PlayerController.onHappenedFinish -= StopAllCoroutines;
    }

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = wakeClip;

        float randomTime = Random.Range(0.5f, 1.0f);
        StartCoroutine("StartWake", randomTime);
    }

    /// <summary>
    /// Start wake with delay
    /// </summary>
    /// <param name="time">Delay</param>
    /// <returns></returns>
    IEnumerator StartWake(float time)
    {
        yield return new WaitForSeconds(time);
        animator.SetTrigger("isReady");
        audioSource.Play();

        yield return new WaitForSeconds(2.0f);
        audioSource.Stop();
        audioSource.clip = idleClip;
        audioSource.Play();        
    }

    /// <summary>
    /// Attack player
    /// </summary>
    /// <returns></returns>
    public IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.2f);
        animator.SetTrigger("Attack");
        PlayerController.Instance.SwitchState("Attacked");
        jumpEffect.Play();
        audioSource.PlayOneShot(attackClip);
        yield return new WaitForSeconds(0.8f);
        audioSource.PlayOneShot(jumpClip);
        yield return new WaitForSeconds(3.0f);
        GameManager.Instance.SwitchState("GameOver");
    }

    public IEnumerator HideAway()
    {
        yield return new WaitForSeconds(0.2f);
        animator.SetTrigger("HideAway");
        jumpEffect.Play();
        yield return new WaitForSeconds(0.8f);
        audioSource.PlayOneShot(jumpClip);

        animator.SetTrigger("isAppeared");
        StartCoroutine("StartWake", 0.7f);
    }

    /// <summary>
    /// Stop enemy's sound
    /// </summary>
    public void StopSound()
    {
        audioSource.Stop();
    }
}
