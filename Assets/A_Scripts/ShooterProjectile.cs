using Unity.VisualScripting;
using UnityEditor.AdaptivePerformance.Editor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ShooterProjectile : MonoBehaviour
{
    private Vector3 dir;
    // EnemyBase is stored opposed to transform because I'd love to add different types of targeting down the line (i.e. prioritizing low hp, etc.)
    private EnemyBase enemyTarget;
    [SerializeField] float speed;     // The speed at which the projectile flies, not to be confused with the speed at which the tower shoots. That's in TowerStats.cs
    [SerializeField] private TowerStats towerStats;
    [SerializeField] private float lifeTime;      // For testing
    private float currentLifetime;
    
    private void Update()
    {
        // Called each frame so it can chase a moving enemy
        SeekTarget();
        transform.position += dir * speed * Time.deltaTime;
        currentLifetime -= Time.deltaTime;

        if (currentLifetime <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    private void OnDisable()
    {
        enemyTarget = null;
    }
    private void OnEnable()
    {
        currentLifetime = lifeTime;
    }
    public void SetTarget(EnemyBase target)
    {
        // Set by the tower when the projectile is activated
        enemyTarget = target;
        Debug.Log($"Project found enemy:{target.gameObject.name}");

        // Calculates the distance between the target and the starting point of the projectile to determine how long it should be active for
        float distance = Vector3.Distance(transform.position, target.transform.position);
    }
    public void SeekTarget()
    {
        if (enemyTarget == null)
        {
            // Makes sure it doesn't float around if the target dies
            gameObject.SetActive(false);
            return;
        }
        // Follows the target enemy around when they're moving
        dir = (enemyTarget.transform.position - transform.position).normalized;
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
