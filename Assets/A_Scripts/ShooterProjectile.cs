using UnityEngine;

/// <summary>
/// Small script that goes on our basic projectiles. These are single target and do not apply any status effects (a planned future feature).
/// This inherits heavily from ProjectileBase.cs with changes being made for it to specifically grab shooter stats.
/// </summary>
public class ShooterProjectile : ProjectileBase
{
    protected override void Update()
    {
        // Handles targeting, finding direction, and position
        base.Update();

        // Check if we reached the enemy this frame
        float distance = Vector3.Distance(transform.position, enemyTarget.transform.position);
        if (distance <= hitRadius)
        {
            enemyTarget.TakeDamage(towerStats.ShooterDamage);
            gameObject.SetActive(false);
        }
    }
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.gameObject.GetComponent<EnemyBase>();
            enemy.TakeDamage(towerStats.ShooterDamage);
        }
    }
}
