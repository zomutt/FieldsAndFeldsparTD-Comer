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
        // IsReadyToAttacks ensures tower is not on cooldown and that target is in range
        if (IsReadyToAttack())
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
        GameObject projectile = DamagePool.SharedInstance.GetProjectile(DamagePool.DamageType.Shooter);

        projectile.transform.position = transform.position;
        projectile.SetActive(true);

        // Gives the projectile its target
        ShooterProjectile proj = projectile.GetComponent<ShooterProjectile>();
        proj.SetTarget(currentTarget);

        ResetCooldown();
    }
    private bool IsReadyToAttack()
    {
        // As in, not on CD
        return cooldown <= 0f;
    }
    private void ResetCooldown()
    {
        cooldown = timeBetweenAttacks;
    }
}
