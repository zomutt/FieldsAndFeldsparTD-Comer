using UnityEngine;

/// <summary>
/// Script specifically for the Slowing (Amber) projectiles. This slows enemies on hit and once slowed, will not slow them again.
/// This is designed so that more than one enemy can be slowed at once, because the slows should not stack and it would be inefficient and counterproductive to only focus on one.
/// </summary>
public class SlowProjectile : ProjectileBase
{
    protected override void Update()
    {
        base.Update();

        float distance = Vector3.Distance(transform.position, enemyTarget.transform.position);
        if (distance <= hitRadius)
        {
            gameObject.SetActive(false);
        }
    }
    public override void SetTarget(EnemyBase target)
    {
        enemyTarget = target;

        // Ensures that no enemy can outrun a projectile
        var enemyMovement = enemyTarget.GetComponent<EnemyMovement>();
        speed = Mathf.Max(speed, enemyMovement.Speed + .5f);
    }
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.gameObject.GetComponent<EnemyMovement>();
            enemy.StartCoroutine(enemy.SlowEnemy(TowerStats.Instance.StSlowAmount, TowerStats.Instance.StSlowTime));
        }
    }
}
