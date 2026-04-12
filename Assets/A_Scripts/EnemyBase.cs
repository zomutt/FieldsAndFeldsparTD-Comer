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

    [Tooltip("Must be in decimal form. i.e. .05")]
    [SerializeField] protected float armor;    // Operates as percentage based damage reduction

    [Header("Combat Timers")]
    [SerializeField] protected float attackCD;
    protected bool canAttack;

    [Header("Visuals")]
    // The player deserves to see it actually die instead of just disappearing so now particle effects will spawn.
    [SerializeField] protected GameObject deathFeedback;

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
        canAttack = true;
    }
    internal virtual void TakeDamage(float rawDamage)
    {
        float armorReduction = (1 - armor);    // Ex: 1.00 - .1 = 90% damage taken
        float incomingDamage = rawDamage * armorReduction;
        currentHealth -= incomingDamage;
        if (currentHealth <= 0)
        {
            DisableEnemy();
        }
    }
    internal virtual void DisableEnemy()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
    // Once the enemy reaches the castle, that is where they stay. There's no moving away, there's just being there until they kill or be killed.
    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Castle"))
        {
            DealDamage();
        }
    }
    internal virtual void DealDamage()
    {
        if (!canAttack) return;
        CastleStats.Instance.TakeDamage(damage);
        StartCoroutine(DamageCD());
    }
    internal virtual IEnumerator DamageCD()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCD);
        canAttack = true;
    }
}
