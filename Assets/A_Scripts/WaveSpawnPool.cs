using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// This is a universal spawner script for all 3 enemy tiers.
/// Spawns enemies from three lane-specific spawners that are randomly picked from the given prefabs.
/// Each tier has it's own GameObject where the specifics of what to spawn and how many to spawn are placed.
/// The TierManager.cs controls which wave-specific spawner is active or inactive.
/// </summary>

public class WaveSpawnPool : MonoBehaviour
{
    public static WaveSpawnPool Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private GameObject[] tier1Prefabs;
    [SerializeField] private GameObject[] tier2Prefabs;
    [SerializeField] private GameObject[] tier3Prefabs;
    [SerializeField] private GameObject[] tier4Prefabs;   // BOSS

    [Header("Amount To Spawn (Validate rounds down if value is not divisible by 3.)")]
    [Tooltip("This is corrected on validate if the divident is not evenly divisible by 3.")]

    // All pool sizes are determined via inspector.
    [Header("Tier 1 Pooling")]
    private ObjectPool<GameObject> tier1Pool;
    [SerializeField] private int tier1ToPool;      // Amount of spawns we expect to see

    [Header("Tier 2 Pooling")]
    private ObjectPool<GameObject> tier2Pool;
    [SerializeField] private int tier2ToPool;

    [Header("Tier 3 Pooling")]
    private ObjectPool<GameObject> tier3Pool;
    [SerializeField] private int tier3ToPool;

    [Header("Tier 4 Pooling")]
    private ObjectPool<GameObject> tier4Pool;
    [SerializeField] private int tier4ToPool;

    public enum TierLevel
    {
        Tier1 = 1,
        Tier2 = 2,
        Tier3 = 3,
        Tier4 = 4
    }
    private Dictionary<TierLevel, ObjectPool<GameObject>> pools;
    private void OnValidate()
    {
        // This basically autocorrects if the int given in inspector is not evenly divisible by three.
        // This is because there are 3 spawners.
        int[] poolSizes = { tier1ToPool, tier2ToPool, tier3ToPool, tier4ToPool };

        for (int i = 0; i < poolSizes.Length; i++)
        {
            int remainder = poolSizes[i] % 3;
            if (remainder != 0)
            {
                poolSizes[i] -= remainder;
            }
        }
        tier1ToPool = poolSizes[0];
        tier2ToPool = poolSizes[1];
        tier3ToPool = poolSizes[2];
        tier4ToPool = poolSizes[3];
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        tier1Pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(tier1Prefabs[Random.Range(0, tier1Prefabs.Length)]),      // Random enemy from the prefab array -- this is to add more variety to the level
            actionOnGet: (enemy) => enemy.SetActive(true),
            actionOnRelease: (enemy) => enemy.SetActive(false),
            defaultCapacity: tier1ToPool, maxSize: tier1ToPool            // These are the same because there should only be an exact number of spawns -- no more, no less.
        );

        tier2Pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(tier2Prefabs[Random.Range(0, tier2Prefabs.Length)]),
            actionOnGet: (enemy) => enemy.SetActive(true),
            actionOnRelease: (enemy) => enemy.SetActive(false),
            defaultCapacity: tier2ToPool, maxSize: tier2ToPool
        );

        tier3Pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(tier3Prefabs[Random.Range(0, tier3Prefabs.Length)]),
            actionOnGet: (enemy) => enemy.SetActive(true),
            actionOnRelease: (enemy) => enemy.SetActive(false),
            defaultCapacity: tier3ToPool, maxSize: tier3ToPool
        );

        tier4Pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(tier4Prefabs[Random.Range(0, tier4Prefabs.Length)]),
            actionOnGet: (enemy) => enemy.SetActive(true),
            actionOnRelease: (enemy) => enemy.SetActive(false),
            defaultCapacity: tier4ToPool, maxSize: tier4ToPool
        );

        // Pairs the object pools with the tiers
        pools = new Dictionary<TierLevel, ObjectPool<GameObject>>
        {
            { TierLevel.Tier1, tier1Pool },
            { TierLevel.Tier2, tier2Pool },
            { TierLevel.Tier3, tier3Pool },
            { TierLevel.Tier4, tier4Pool }
        };
    }
    public GameObject GetEnemy(TierLevel tier)
    {
        // Called by EnemySpawner.cs to determine what to spawn
        return pools[tier].Get();
    }
    public int GetAmountToSpawn(int tier)   // Called by TierManager.cs to determine how many enemies should be spawning
    {
        // Called by TierManager.cs to determine how many enemies should be spawning
        switch (tier)
        {
            case 1:
                return tier1ToPool;
            case 2:
                return tier2ToPool;
            case 3:
                return tier3ToPool;
            case 4:
                return tier4ToPool;
            default:
                Debug.Log("GetAmountToSpawn returned an invalid wave number.");
                return 0;
        }
    }
}


