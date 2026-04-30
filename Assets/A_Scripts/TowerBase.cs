using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This holds all the behaviour that any tower should need for targeting.
/// Since attacks differ by tower, the individual towers inherit targeting and attack speed, but change actual attack logic as needed.
/// </summary>
public class TowerBase : MonoBehaviour
{
    protected List<EnemyBase> targetsInRange = new();
    protected EnemyBase currentTarget;

    protected float damage;
    protected float timeBetweenAttacks;
    protected float cooldown;

    protected Vector3 towerLocation;

    protected virtual void Start()
    {
        towerLocation = transform.position;
        timeBetweenAttacks = TowerStats.Instance.ShooterCD;      // Functions as attack speed
    }
    protected virtual void Update()
    {
        GetCurrentTarget();

        if (cooldown > 0f)
        {
            cooldown -= Time.deltaTime;
        }
    }
    protected bool IsReadyToAttack()
    {
        return cooldown <= 0f;
    }

    protected void ResetCooldown()
    {
        cooldown = timeBetweenAttacks;
    }
    private void OnTriggerEnter(Collider other)
    {
        // Each tower has a large spherical collider that serves to detect when a tagged Enemy is in range
        if (other.CompareTag("Enemy"))
        {
            // Adds enemy to in range target list
            AddToRangeList(other.GetComponent<EnemyBase>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            RemoveFromRangeList(other.GetComponent<EnemyBase>());
        }
    }
    public void AddToRangeList(EnemyBase target)
    {
        if (!targetsInRange.Contains(target))
        {
            targetsInRange.Add(target);
        }
    }
    public void RemoveFromRangeList(EnemyBase target)
    {
        targetsInRange.Remove(target);

        if (currentTarget == target)
        {
            currentTarget = null;
        }
    }
    private void GetCurrentTarget()
    {
        // Null-check is edgecase-safe, checking for activeinHierarchy ensures we can find another target if the first target dies
        static bool IsInvalidEnemy(EnemyBase enemy)
        {
            return enemy == null || !enemy.gameObject.activeInHierarchy;
        }
        targetsInRange.RemoveAll(IsInvalidEnemy);

        // FIFO targeting: first enemy that entered the radius = first enemy that gets attacked
        // This will be replaced with more advanced targeting logic once enemy variety increases.
        if (targetsInRange.Count <= 0)
        {
            currentTarget = null;
            return;
        }
        currentTarget = targetsInRange[0];
    }
}
