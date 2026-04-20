using UnityEngine;

public class ShooterProjectile : MonoBehaviour
{
    private Vector3 dir;
    // EnemyBase is stored opposed to transform because I'd love to add different types of targeting down the line (i.e. prioritizing low hp, etc.)
    private EnemyBase enemyTarget;
    
    [SerializeField] float speed;     // The speed at which the projectile flies, not to be confused with the speed at which the tower shoots. That's in TowerStats.cs
    [SerializeField] private TowerStats towerStats;
    [SerializeField] private float hitRadius = 0.2f;     // How close counts as a hit
    private void Update()
    {
        if (enemyTarget == null)
        {
            // For example, if enemy dies from something else, we just need the projectile to go away
            gameObject.SetActive(false);
            return;
        }
        // Move towards target
        dir = (enemyTarget.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        // Check if we reached the enemy this frame
        float distance = Vector3.Distance(transform.position, enemyTarget.transform.position);
        if (distance <= hitRadius)
        {
            enemyTarget.TakeDamage(towerStats.ShooterDamage);
            gameObject.SetActive(false);
        }
    }
    private void OnDisable()
    {
        enemyTarget = null;
    }
    public void SetTarget(EnemyBase target)
    {
        // Called and set by ShooterTower.cs
        enemyTarget = target;
        // Ensures that no enemy can outrun a projectile
        var enemyMovement = enemyTarget.GetComponent<EnemyMovement>();
        speed = Mathf.Max(speed, enemyMovement.Speed + .5f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.gameObject.GetComponent<EnemyBase>();
            enemy.TakeDamage(towerStats.ShooterDamage);
        }
    }
}
