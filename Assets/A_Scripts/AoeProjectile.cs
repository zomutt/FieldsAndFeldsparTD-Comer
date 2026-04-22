using System.Collections;
using UnityEngine;

/// <summary>
/// The AoE projectile is meant to be a DoT fireball. 
/// When it lands, the mr is disabled, the fire remains, and it continuously deals damage to units inside the radius. 
/// </summary>
public class AoeProjectile : ProjectileBase
{
    private MeshRenderer mr;
    private bool canDamage;
    private bool isBurning;
    private bool burnRoutineStarted;


    private void OnEnable()
    {
        mr = GetComponent<MeshRenderer>();

        mr.enabled = true;
        canDamage = true;
        isBurning = false;
        burnRoutineStarted = false;
    }
    protected override void OnDisable()
    {
        base.OnDisable();

        mr.enabled = true;     // Fail-safe
        canDamage = true;
        isBurning = false;
        burnRoutineStarted = false;
    }
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !isBurning)
        {
            // Stop moving, instead of being a projectile now it's a stationary fire
            mr.enabled = false;
            enemyTarget = null;       // Now it can't chase anything
            isBurning = true;

            if (!burnRoutineStarted)
            {
                burnRoutineStarted = true;
                StartCoroutine(BurnDoT());
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!isBurning) return;
        if (other.CompareTag("Enemy") && canDamage)
        {
            var enemy = other.GetComponent<EnemyBase>();
            if (enemy != null)
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
            yield return new WaitForSeconds(1f / 3f);
        }
        gameObject.SetActive(false);
    }
}

