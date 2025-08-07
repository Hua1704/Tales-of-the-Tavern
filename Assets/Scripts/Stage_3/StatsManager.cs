using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;
    PlayerControllerStage2 playerControllerStage=null;//spped
    PlayerController playerController = null;//speed
    PlayerAttack attackScript;//attack
    BaseUnit unit;//health

    [Header("Combat Stats")]
    public float attack;

    [Header("Movement Stats")]
    public float speed;

    [Header("Health Stats")]
    public float maxHealth;
    public float currentHealth;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerControllerStage = player.GetComponent<PlayerControllerStage2>();
        playerController = player.GetComponent<PlayerController>();
        attackScript = player.GetComponentInChildren<PlayerAttack>(true);
        unit = player.GetComponent<BaseUnit>();
        if (playerController == null)
        {
            
        }
    }
    private void Update()
    {
        attack = attackScript.attackDamage;
        
        maxHealth = unit.MAX_HEALTH;
        currentHealth = unit.health;
        if (playerController==null)
        {
            speed = playerControllerStage.moveSpeed;
        }
        else
        {
            speed = playerController.moveSpeed;
        }
    }
}
