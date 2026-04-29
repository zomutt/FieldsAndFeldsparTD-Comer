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
    [SerializeField] protected float attackDelay;   // Seconds between each shot
    protected float cooldown;
    [SerializeField] protected float armor;    // Reduces damage by flat amount
    [SerializeField] protected int goldYield;   // How much gold the player gets for killing this enemy
    protected bool hasCounted;     // If the kill has been logged or not
    protected float incomingDamage;
    
    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
        cooldown = attackDelay;
        gameObject.tag = "Enemy";
        hasCounted = false;
    }
    protected virtual void Update()
    {
        cooldown -= Time.deltaTime;
    }
    internal virtual void TakeDamage(float rawDamage)
    {
        if (armor > rawDamage)
        {
            // Makes sure nothing can accidentally heal the enemy
            incomingDamage = 1;
        }
        else
        {
            incomingDamage = rawDamage - armor;
        }

        currentHealth -= incomingDamage;

        if (currentHealth <= 0)
        {
            DisableEnemy();
        }
    }
    internal virtual void DisableEnemy()
    {
        if (!hasCounted)
        {
            TierManager.Instance.RecordKill();     // Tracks how many kills have occurred vs. how many mobs spawn in the tier
            ParticlePool.Instance.SpawnDeathEffect(transform.position);
            hasCounted = true;    // Ensures no double-counting edge case
            GoldManager.Instance.GiveGold(goldYield);
        }
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
        cooldown = attackDelay;      // CD reset
    }
}
