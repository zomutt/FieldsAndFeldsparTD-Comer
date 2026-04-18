using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
        timeBetweenAttacks = TowerStats.Instance.ShooterCD;
    }
    protected virtual void Update()
    {
        GetCurrentTarget();

        if (cooldown > 0f)
        {
            cooldown -= Time.deltaTime;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // Each tower has a large spherical collider that serves to detect when a tagged Enemy is in range
        if (other.CompareTag("Enemy"))
        {
            // Adds enemy to in range target list
            AddTargetToInRangeList(other.GetComponent<EnemyBase>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            RemoveTargetFromInRangeList(other.GetComponent<EnemyBase>());
        }
    }
    public void AddTargetToInRangeList(EnemyBase target)
    {
        if (!targetsInRange.Contains(target))
        {
            targetsInRange.Add(target);
        }
    }
    public void RemoveTargetFromInRangeList(EnemyBase target)
    {
        targetsInRange.Remove(target);

        if (currentTarget == target)
        {
            currentTarget = null;
        }
    }
    private void GetCurrentTarget()
    {
        // All enemies also have the same movement speed, so there won't be any racing ahead of others that could cause this to break (this will get refactored later as various enemies are added)
        // This is meant to target the first enemy that enters the radius, that being the enemy closest to the tower (FIFO)
        if (targetsInRange.Count <= 0)
        {
            currentTarget = null;
            return;
        }
        currentTarget = targetsInRange[0];
    }
}
