using UnityEngine;
/// <summary>
/// This script calls heavily on it's base class, but has it's own unique damage patterns.
/// It also serves to pass in the target and the lifetime of projectiles.
/// </summary>
public class ShooterTower : TowerBase
{
    protected override void Update()
    {
        base.Update();
        if (IsReadyToAttack())      // Not on cooldown and has a target
        {
            Shoot();
        }
    }
    private void Shoot()
    {
        if (currentTarget == null || !currentTarget.gameObject.activeSelf)
        {
            currentTarget = null;
            return;
        }
        // Calls to get the projectile from spawner script
        GameObject projectile = DamagePool.Instance.GetProjectile(DamagePool.DamageType.Shooter);
        projectile.transform.position = transform.position;

        // Gives the projectile its target
        ShooterProjectile proj = projectile.GetComponent<ShooterProjectile>();
        proj.SetTarget(currentTarget);
        ResetCooldown();
    }
}
