using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Base Enemy Stats")]
    // These are all floats because of how CastleStats.cs handles damage reduction, this reduces issues with conversions.
    [SerializeField] protected float maxHealth;
    protected float currentHealth;

    [SerializeField] protected float damage;
    [SerializeField] protected float attackSpeed;   // Seconds between each shot
    protected float cooldown;
    [SerializeField] protected float armor;    // Reduces damage by flat amount

    [Header("Visuals")]
    // Particle effects will spawn for visual feedback
    [SerializeField] protected GameObject deathFeedback;
    
    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
        cooldown = attackSpeed;    // Ready to attack
    }
    protected virtual void Update()
    {
        // Continuously updates cooldown
        cooldown -= Time.deltaTime;
    }
    internal virtual void TakeDamage(float rawDamage)
    {
        float incomingDamage = rawDamage - armor;

        currentHealth -= incomingDamage;
        if (currentHealth <= 0)
        {
            DisableEnemy();
        }
    }
    internal virtual void DisableEnemy()
    {
        TierManager.Instance.RecordKill();     // Tracks how many kills have occurred vs. how many mobs spawn in the tier
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
    protected virtual void OnTriggerStay(Collider other)
    {
        // Once the enemy reaches the castle, that is where they stay. There's no moving away, there's just being there until they kill or be killed.
        if (other.gameObject.CompareTag("Castle"))
        {
            DealDamage();
        }
    }
    internal virtual void DealDamage()
    {
        if (cooldown > 0f) return;
        CastleStats.Instance.TakeDamage(damage);
        cooldown = attackSpeed;      // CD reset
    }
}
