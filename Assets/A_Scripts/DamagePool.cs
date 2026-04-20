using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

/// <summary>
/// This handles all of the spawning of all damage types when called by the tower scripts.
/// This pool has intentionally been designed to be able to dynamically grow later down the line as more towers are added.
/// </summary>
public class DamagePool : MonoBehaviour
{
    public static DamagePool SharedInstance;

    public static DamagePool Instance;

    [Header("Prefabs")]
    [SerializeField] private GameObject shooterPrefab;
    [SerializeField] private GameObject aoePrefab;

    [Header("Pool Sizes")]
    [SerializeField] private int shooterPoolSize = 20;
    [SerializeField] private int aoePoolSize = 20;

    // I opted to use an enum despite there only being two towers at the moment for scope scaling purposes
    public enum DamageType
    {
        Shooter,
        AOE
    }
    private Dictionary<DamageType, GameObject> prefabPairs;      // Links the damage types to their pool limit
    private Dictionary<DamageType, ObjectPool<GameObject>> pools; // Maps each damage type to its ObjectPool

    private void Awake()
    {
        SharedInstance = this;

        // Links the prefabs to its type
        prefabPairs = new Dictionary<DamageType, GameObject>
        {
            { DamageType.Shooter, shooterPrefab },
            { DamageType.AOE, aoePrefab }
        };

        // Then links the pool to it's type too
        pools = new Dictionary<DamageType, ObjectPool<GameObject>>
        {
            { DamageType.Shooter, CreatePool(DamageType.Shooter, shooterPoolSize) },
            { DamageType.AOE, CreatePool(DamageType.AOE, aoePoolSize) }
        };
    }

    private ObjectPool<GameObject> CreatePool(DamageType type, int initialSize)
    {
        GameObject prefab = prefabPairs[type];

        return new ObjectPool<GameObject>(
            createFunc: () => Instantiate(prefab),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            defaultCapacity: initialSize,
            maxSize: initialSize * 3
        );
    }

    public GameObject GetProjectile(DamageType type)
    {
        return pools[type].Get();
    }

    public void ReturnProjectile(DamageType type, GameObject projectile)
    {
        pools[type].Release(projectile);
    }
}
