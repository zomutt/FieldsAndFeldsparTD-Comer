using UnityEngine;

/// <summary>
/// Small script that goes on our basic projectiles. These are single target and do not apply any status effects (a planned future feature).
/// This inherits heavily from ProjectileBase.cs with changes being made for it to specifically grab shooter stats.
/// Damage can be adjusted in the TowerStats scriptable object.
/// </summary>
public class ShooterProjectile : ProjectileBase
{
    protected override void Update()
    {
        if (enemyTarget == null)
        {
            // For example, if enemy dies from something else, we just need the projectile to go away.
            gameObject.SetActive(false);
            return;
        }

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
    protected override void OnDisable()
    {
        base.OnDisable();
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
