using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class BaseUnit : MonoBehaviour
{
    [Header("Health Variables")]
    [SerializeField] public int MAX_HEALTH = 5;
    [SerializeField] public int health = 5;
    protected bool isAlive = true;
    private bool _isInitialized = false; // Flag to prevent re-initialization

    [Header("HealthBar")]
    [SerializeField] protected GameObject healthBarParent;
    [SerializeField] protected GameObject healthBarSlider;

    [Header("Hit Effect")]
    [SerializeField] protected Material flashMaterial;
    [SerializeField] private float duration = 0.1f;
    protected SpriteRenderer spriteRenderer;
    protected Material originalMaterial;
    protected Coroutine flashRoutine;

    public void Start()
    {
        if (!_isInitialized)
        {
            InitializeUnit(MAX_HEALTH, MAX_HEALTH);
        }
    }
    public virtual void InitializeUnit(int newMaxHealth, int newHealth)
    {
        MAX_HEALTH = newMaxHealth;
        health = newHealth;
        isAlive = true;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            originalMaterial = spriteRenderer.material;
        }

        UpdateHealthBar();
        _isInitialized = true; // Mark as initialized
    }
    public void OnTakeDamage(int damage)
    {
        if (!isAlive) return; // Don't take damage if not alive

        health -= damage;
        Flash();

        if (health < 1)
        {
            health = 0;
            isAlive = false; // Set isAlive to false before calling Die()
            Die();
        }

        UpdateHealthBar();
    }

    public void OnHeal(int heal)
    {
        health += heal;
        health = Math.Min(MAX_HEALTH,health);

        UpdateHealthBar();
    }

    protected void UpdateHealthBar()
    {
        if (healthBarParent.activeInHierarchy == false)
        {
            healthBarParent.SetActive(true);
        }

        healthBarSlider.transform.localScale = new Vector3(Mathf.Clamp01((float)health / (float)MAX_HEALTH), 1, 1);
    }

    protected virtual void Die()
    {
        StopCoroutine(flashRoutine);
        gameObject.SetActive(false);
        isAlive = false;
    }

    public void Flash()
    {
        if (isAlive)
        {
            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
            }
            flashRoutine = StartCoroutine(FlashRoutine());
        }
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(duration);
        spriteRenderer.material = originalMaterial;
        flashRoutine = null;
    }
}
