using UnityEngine;

public class ShooterTower : MonoBehaviour
{
    [Header("Shooter Tower Stats")]
    [SerializeField] private float damage;
    [SerializeField] private float range;
    [SerializeField] private float cost;
    [SerializeField] private float cooldown;
    [SerializeField] private float attackSpeed;
    private SphereCollider rangeRadius;

    [Header("Attacks")]
    [SerializeField] private GameObject shooterPrefab;

    private void OnEnable()
    {
        cooldown = attackSpeed;   // Begin ready to attack

        rangeRadius = GetComponent<SphereCollider>();
        // Sets how large the detection collider present on the tower should be
        rangeRadius.radius = range;
    }

    private void Update()
    {
        cooldown -= Time.deltaTime;
    }

    private void ShootProjectile()
    {
        if (cooldown > 0) return;
        cooldown = attackSpeed;
    }
}
