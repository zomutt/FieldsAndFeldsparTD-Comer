using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    protected Vector3 dir;

    // EnemyBase is stored opposed to transform because I'd love to add different types of targeting down the line (i.e. prioritizing low hp, etc.)
    protected EnemyBase enemyTarget;

    [SerializeField] protected float speed = 3f;     // The speed at which the projectile flies, not to be confused with the speed at which the tower shoots. That's in TowerStats.cs
    [SerializeField] protected TowerStats towerStats;
    [SerializeField] protected float hitRadius = 0.2f;     // How close counts as a hit; protects against edge cases.
    protected virtual void Update()
    {

        if (enemyTarget == null)
        {
            // For example, if enemy dies from something else, we just need the projectile to go away.
            gameObject.SetActive(false);
            return;
        }

        // Move towards target
        dir = (enemyTarget.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        // EXAMPLE OF WHAT IS NEEDED:

        //// Check if we reached the enemy this frame
        //float distance = Vector3.Distance(transform.position, enemyTarget.transform.position);
        //if (distance <= hitRadius)
        //{
        //    enemyTarget.TakeDamage(towerStats.ShooterDamage);
        //    gameObject.SetActive(false);
        //}
    }
    protected virtual void OnDisable()
    {
        enemyTarget = null;
        StopAllCoroutines();
    }
    internal virtual void SetTarget(EnemyBase target)
    {
        // Called and set by ShooterTower.cs or AoeTower.cs, so on and so forth.
        enemyTarget = target;

        // Ensures that no enemy can outrun a projectile
        var enemyMovement = enemyTarget.GetComponent<EnemyMovement>();
        speed = Mathf.Max(speed, enemyMovement.Speed + .5f);
    }

    protected abstract void OnTriggerEnter(Collider other);
    // EXAMPLE OF WHAT IS NEEDED: ^^^

    //if (other.CompareTag("Enemy"))
    //{
    //    var enemy = other.gameObject.GetComponent<EnemyBase>();
    //    enemy.TakeDamage(towerStats.ShooterDamage);
    //}

}
