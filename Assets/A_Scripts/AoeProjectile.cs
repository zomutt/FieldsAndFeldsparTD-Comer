using System.Collections;
using UnityEngine;

/// <summary>
/// The AoE projectile is meant to be a DoT fireball. 
/// When it lands, the mr is disabled, the fire remains, and it continuously deals damage to units inside the radius. 
/// Damage can be adjusted in the TowerStats scriptable object.
/// </summary>
public class AoeProjectile : ProjectileBase
{
    private bool canDamage;
    private bool isBurning;
    private bool burnRoutineStarted;
    [SerializeField] private MeshRenderer orbRenderer;

    private void OnEnable()
    {
        orbRenderer.enabled = true;
        canDamage = true;
        isBurning = false;
        burnRoutineStarted = false;
        StartCoroutine(FizzleOut());

        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            // Ensures the flames on the projectile start playing upon reuse
            ps.Play();
        }
    }
    protected override void Update()
    {
        if (isBurning)
        {
            return;
        }
        base.Update();
        // Tracks distance between projectile and target, and if it's close enough, applies damage.
        // This is needed because the projectile can move fast enough to skip over the target's collider.
        if (enemyTarget == null)
        {
            return;
        }
        float distance = Vector3.Distance(transform.position, enemyTarget.transform.position);
        if (distance <= hitRadius)
        {
            enemyTarget.TakeDamage(towerStats.AoeDamage);
        }
    }
    protected override void OnDisable()
    {
        base.OnDisable();

        // Reset state so the next pooled activation starts clean
        canDamage = true;
        isBurning = false;
        burnRoutineStarted = false;

        // Re-enable the orb mesh for next time
        if (orbRenderer != null)
        {
            orbRenderer.enabled = true;
        }

        // Stop all particle systems on disable so they don't linger
        foreach (ParticleSystem particles in GetComponentsInChildren<ParticleSystem>())
        {
            particles.Stop();
            particles.Clear();
        }
    }
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !isBurning)
        {
            // Stop moving, instead of being a projectile now it's a stationary fire
            orbRenderer.enabled = false;     // Hides orb and makes it look like it's just a fire on the ground, but the collider is still active and can damage
            enemyTarget = null;       // Now it can't chase anything
            isBurning = true;

            if (!burnRoutineStarted)
            {
                burnRoutineStarted = true;
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(BurnDoT());
                }
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!isBurning)
        {
            return;
        }
        if (other.CompareTag("Enemy") && canDamage)
        {
            if (other.TryGetComponent<EnemyBase>(out EnemyBase enemy))
            {
                enemy.TakeDamage(towerStats.AoeDamage);
            }
        }
    }
    private IEnumerator BurnDoT()
    {
        float totalDuration = TowerStats.Instance.AoeDuration;
        int totalTicks = Mathf.RoundToInt(totalDuration * 3);   // 3 ticks per second

        for (int i = 0; i < totalTicks; i++)
        {
            canDamage = true;
            yield return null;     // Hits for 1 frame

            canDamage = false;

            // Wait until next tick
            yield return new WaitForSeconds(.33f);
        }
        EndBurn();
    }
    private void EndBurn()
    {
        canDamage = false;
        isBurning = false;
        burnRoutineStarted = false;
        gameObject.SetActive(false);
    }
    private IEnumerator FizzleOut()
    {
        // Since it's fire, the fire should fizzle out if it doesn't land fast enough.
        // This helps balance the tower compared to the amethyst shooter.
        yield return new WaitForSeconds(3f);
        if (!isBurning)
        {
            gameObject.SetActive(false);
        }
    }
}

