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
    public static WaveSpawnPool Instance;

    [Header("Prefabs and Spawn Delay")]
    [SerializeField] private GameObject[] tier1Prefabs;
    [SerializeField] private GameObject[] tier2Prefabs;
    [SerializeField] private GameObject[] tier3Prefabs;
    [SerializeField] private float spawnDelay;


    [Header("Amount To Spawn (Validate rounds down if value is not divisible by 3.)")]
    [Tooltip("This is corrected on validate if the divident is not evenly divisible by 3.")]

    [Header("Spawners")]


    [Header("Tier 1 Pooling")]
    private ObjectPool<GameObject> tier1Pool;
    [SerializeField] private int tier1ToPool;      // Amount of spawns we expect to see

    [Header("Tier 2 Pooling")]
    private ObjectPool<GameObject> tier2Pool;
    [SerializeField] private int tier2ToPool;

    [Header("Tier 3 Pooling")]
    private ObjectPool<GameObject> tier3Pool;
    [SerializeField] private int tier3ToPool;

    public enum TierLevel
    {
        Tier1 = 1,
        Tier2 = 2,
        Tier3 = 3,
    }
    private Dictionary<TierLevel, ObjectPool<GameObject>> pools;


    private void OnValidate()
    {
        // This basically autocorrects if the int given in inspector is not evenly divisible by three.
        // This is because there are 3 spawners.
        int[] poolSizes = { tier1ToPool, tier2ToPool, tier3ToPool };

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
    }

    private void Awake()
    {
        Instance = this;

        tier1Pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(tier1Prefabs[Random.Range(0, tier1Prefabs.Length)]),
            actionOnGet: (enemy) => enemy.SetActive(true),
            actionOnRelease: (enemy) => enemy.SetActive(false),
            defaultCapacity: tier1ToPool, maxSize: tier1ToPool
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

        // Pairs the object pools with the tiers
        pools = new Dictionary<TierLevel, ObjectPool<GameObject>>
        {
            { TierLevel.Tier1, tier1Pool },
            { TierLevel.Tier2, tier2Pool },
            { TierLevel.Tier3, tier3Pool }
        };

    }
    public GameObject GetEnemy(TierLevel tier)
    {
        return pools[tier].Get();
    }

    // Called by TierManager.cs to determine how many enemies should be spawning
    public int GetAmountToSpawn(int tier)          
    {
        switch (tier)
        {
            case 1:
                return tier1ToPool;
            case 2:
                return tier2ToPool;
            case 3:
                return tier3ToPool;
            default:
                Debug.Log("GetAmountToSpawn returned an invalid wave number.");
                return 0;
        }
    }
}


