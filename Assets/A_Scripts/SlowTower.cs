using UnityEngine;

public class BasicSlowTower : TowerBase
{
    /// <summary>
    /// This is a single target slow tower that operates the same way as others (FIFO targeting) with the exception being that it targets and hits once, then moves to another target.
    /// This is to prevent slow stacking, and the slow is not intended to stack -- attacking twice does nothing.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        timeBetweenAttacks = TowerStats.Instance.StSlowCD;      // Functions as attack speed
    }
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
        GameObject projectile = DamagePool.Instance.GetProjectile(DamagePool.DamageType.Slow);
        projectile.transform.position = transform.position;

        // Gives the projectile its target
        SlowProjectile proj = projectile.GetComponent<SlowProjectile>();
        proj.SetTarget(currentTarget);
        ResetCooldown();
        currentTarget = null;
    }
    protected override void GetCurrentTarget()
    {
        // Null-check is edgecase-safe, checking for activeinHierarchy ensures we can find another target if the first target dies
        static bool IsInvalidEnemy(EnemyBase enemy)
        {
            return enemy == null || !enemy.gameObject.activeInHierarchy;
        }
        targetsInRange.RemoveAll(IsInvalidEnemy);

        // Checks to make sure nothing is slowed. If something is slowed, don't target it.
        foreach (EnemyBase target in targetsInRange)
        {
            EnemyMovement movement = target.GetComponent<EnemyMovement>();
            if (movement != null && !movement.isSlowed)
            {
                currentTarget = target;
                break;
            }
        }
    }
}
