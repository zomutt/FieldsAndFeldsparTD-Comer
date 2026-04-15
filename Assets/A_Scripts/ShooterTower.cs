using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class ShooterTower : TowerBase
{
    [Header("Shooter Tower Stats")]
    [SerializeField] private float damage;
    [SerializeField] private float range;
    [SerializeField] private float cost;
    [SerializeField] private float timeBetweenAttacks;
    private float cooldown;

    [Header("Attacks")]
    [SerializeField] private GameObject shooterPrefab;

    // Defaults to attacking the first enemy to enter radius
    private void OnEnable()
    {
        cooldown = timeBetweenAttacks;   // Begin ready to attack
    }

    private void Update()
    {
        cooldown -= Time.deltaTime;
    }
    private void GetCurrentTarget()
    {
        if (targetsInRange.Count <= 0)
        {
            currentTarget = null;
            return;
        }
    }
    private void ShootProjectile()
    {
        if (cooldown > 0) return;
        cooldown = timeBetweenAttacks;
    }
}
