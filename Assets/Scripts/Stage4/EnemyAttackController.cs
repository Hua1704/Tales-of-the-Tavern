using UnityEngine;
using System.Collections.Generic;

public class EnemyAtkController : MonoBehaviour
{
    // In the Inspector, drag all your PARENT hitbox containers here
    // e.g., Atk1_Down_Hitbox, Atk1_Right_Hitbox, etc.
    [SerializeField]
    public List<GameObject> hitboxContainers;

    // This list will track which colliders have been hit during a SINGLE attack animation.
    // This is the key to preventing multiple damage instances from one attack.
    private List<Collider2D> hitTargets;

    void Awake()
    {
        hitTargets = new List<Collider2D>();

        // Start with all hitbox containers disabled
        foreach (var container in hitboxContainers)
        {
            if (container != null)
            {
                container.SetActive(false);
            }
        }
    }

    // --- ANIMATION EVENT FUNCTIONS ---

    // Called by an Animation Event to turn on a specific attack's hitbox container.
    public void ActivateHitbox(string containerName)
    {
        // Clear the list of hit targets at the start of every new attack.
        hitTargets.Clear();

        foreach (var container in hitboxContainers)
        {
            if (container.name == containerName)
            {
                container.SetActive(true);
                return; // Exit after finding and activating the correct one
            }
        }
    }

    // Called by an Animation Event to turn off all hitbox containers.
    public void DeactivateAllHitboxes()
    {
        foreach (var container in hitboxContainers)
        {
            container.SetActive(false);
        }
    }

    // --- PUBLIC FUNCTION FOR HITBOXES ---

    // The individual hitbox scripts will call this function.
    public void RegisterHit(Collider2D playerCollider, int damage)
    {
        // If we have already hit this target during this attack, do nothing.
        if (hitTargets.Contains(playerCollider))
        {
            return;
        }

        // Add the player to the list of targets hit in this attack.
        hitTargets.Add(playerCollider);

        // Get the player's controller and deal damage.
        BaseUnit player = playerCollider.GetComponent<BaseUnit>();
        if (player != null)
        {
            player.OnTakeDamage(damage);
        }
    }
}