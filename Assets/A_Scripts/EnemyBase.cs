using System.Collections;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Base Enemy Stats")]
    [SerializeField] protected int baseHealth;
    [SerializeField] protected int currentHealth;   // Serialized for testing purposes

    [SerializeField] protected int damage;
    [SerializeField] protected float attackDelay;   // Seconds between each shot
    protected float cooldown;
    [SerializeField] protected int goldYield;   // How much gold the player gets for killing this enemy
    protected bool hasCounted;     // If the kill has been logged or not
    [SerializeField] protected float timeBeforeDespawn = 1.5f;   // Time before the enemy despawns after death animation plays

    // ANIMATIONS
    private Animation anim;
    private const string ATTACK = "Anim_Attack";
    private const string DEATH = "Anim_Death";
    private EnemyMovement movement;
    
    protected virtual void OnEnable()
    {
        currentHealth = (baseHealth * GameManager.Instance.CurrentLevel);
        cooldown = attackDelay;
        gameObject.tag = "Enemy";
        hasCounted = false;

        anim = GetComponentInChildren<Animation>();
        movement = GetComponent<EnemyMovement>();
    }
    protected virtual void Update()
    {
        cooldown -= Time.deltaTime;
    }
    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            DisableEnemy();
        }
    }
    public virtual void DisableEnemy()
    {
        if (!hasCounted)
        {
            TierManager.Instance.RecordKill();     // Tracks how many kills have occurred vs. how many mobs spawn in the tier

            ParticlePool.Instance.SpawnDeathEffect(transform.position);     

            hasCounted = true;    // Ensures no double-counting edge case
            GoldManager.Instance.GiveGold(goldYield);
        }
        StopAllCoroutines();
        StartCoroutine(DespawnDelay());
    }
    protected virtual void OnTriggerStay(Collider other)
    {
        // Once the enemy reaches the castle, that is where they stay. There's no moving away, there's just being there until they kill or be killed.
        if (other.gameObject.CompareTag("Castle"))
        {
            DealDamage();
        }
    }
    protected virtual void DealDamage()
    {
        if (cooldown > 0f) return;
        CastleStats.Instance.TakeDamage(damage);      // Castle is the only thing enemy can attack, so I'm not going to over-engineer this one.
        cooldown = attackDelay;      // CD reset

        movement.isAttacking = true;
        anim.CrossFade(ATTACK);
        StartCoroutine(ResetAttackFlag(anim[ATTACK].length));
    }
    protected virtual IEnumerator ResetAttackFlag(float duration)
    {
        yield return new WaitForSeconds(duration);
        movement.isAttacking = false;
    }

    protected virtual IEnumerator DespawnDelay()
    {
        movement.isAttacking = true;
        movement.Agent.isStopped = true;

        anim.CrossFade(DEATH);

        yield return new WaitForSeconds(timeBeforeDespawn);

        movement.Agent.isStopped = false;
        gameObject.SetActive(false);
    }
}
