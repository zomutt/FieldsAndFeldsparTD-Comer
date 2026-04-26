using UnityEngine;

public class AoeTower : TowerBase
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
        GameObject projectile = DamagePool.Instance.GetProjectile(DamagePool.DamageType.AOE);

        projectile.transform.position = transform.position;

        // Gives the projectile its target and gets the actual projectile script
        AoeProjectile proj = projectile.GetComponent<AoeProjectile>();
        proj.SetTarget(currentTarget);

        ResetCooldown();
    }
}
