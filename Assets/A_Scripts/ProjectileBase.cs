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
        dir = (enemyTarget.transform.position - transform.position).normalized; 
        transform.position += dir * speed * Time.deltaTime;
    }
    protected virtual void OnDisable()
    {
        enemyTarget = null;
        StopAllCoroutines();
    }
    public virtual void SetTarget(EnemyBase target)
    {
        // Called and set by ShooterTower.cs or AoeTower.cs, so on and so forth.
        enemyTarget = target;

        // Ensures that no enemy can outrun a projectile
        var enemyMovement = enemyTarget.GetComponent<EnemyMovement>();
        speed = Mathf.Max(speed, enemyMovement.Speed + .5f);
    }

    protected abstract void OnTriggerEnter(Collider other);
}
