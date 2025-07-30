using UnityEngine;
using System.Collections;

public class SkeletonDeathTrigger : MonoBehaviour
{
    public Animator animator;
    public AudioSource bonesSound;
    public SpriteRenderer sprite;
    public Collider2D col;

    private bool triggered = false;

    void Start()
    {
        // Skeleton starts visible
        col.enabled = true;
        sprite.color = new Color(1f, 1f, 1f, 1f); // Ensure full opacity
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag("Player")) return;

        triggered = true;

        // Play death sequence
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        animator.SetTrigger("Freeze");
        if (bonesSound) bonesSound.Play();
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float duration = 1.5f;
        float t = 0f;
        Color c = sprite.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / duration);
            sprite.color = c;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}