using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
/// <summary>
/// Pools all enemy tiers in one place and lets me grab an inactive enemy from whatever tier I need.
/// </summary>
public class EnemySpawnPool : MonoBehaviour
{
    private List<GameObject> pooledEnemies;
    private int[] tierCounts;
    private int[] tierStarts; // Since the indices are stored in an array, this variable is used to calculate which tiers correspond with each numbers

    [Header("Tier 1")]
    [SerializeField] private GameObject[] t1enemyPrefab;
    [SerializeField] private int t1ToPool;

    [Header("Tier 2")]
    [SerializeField] private GameObject[] t2enemyPrefab;
    [SerializeField] private int t2ToPool;

    [Header("Tier 3")]
    [SerializeField] private GameObject[] t3enemyPrefab;
    [SerializeField] private int t3ToPool;

    [Header("Boss")]
    // Typically 1, but I want to allow the opportunity to add in mini-bosses or multiple bosses at once.
    [SerializeField] private GameObject[] bossPrefab;
    [SerializeField] private int bossToPool;
    private void Start()
    {
        pooledEnemies = new List<GameObject>();

        AddToPool(t1enemyPrefab, t1ToPool);
        AddToPool(t2enemyPrefab, t2ToPool);
        AddToPool(t3enemyPrefab, t3ToPool);
        AddToPool(bossPrefab, bossToPool);

        // Stores tier sizes in array 
        tierCounts = new int[] { t1ToPool, t2ToPool, t3ToPool, bossToPool };
        // Gets tier start indices (this is important when working with multi-object pools)
        tierStarts = new int[tierCounts.Length];

        // Calculates which index is where for index counting purposes,, this is what separates the tiers
        int runningIndex = 0;
        for (int i = 0; i < tierCounts.Length; i++)
        {
            tierStarts[i] = runningIndex;
            runningIndex += tierCounts[i];
        }
    }
    private void AddToPool(GameObject[] prefabs, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject enemy = Instantiate(prefabs[Random.Range(0, prefabs.Length)]);
            enemy.SetActive(false);
            pooledEnemies.Add(enemy);
        }
    }
    internal GameObject GetPooledEnemy(int tierIndex)
    {
        int start = tierStarts[tierIndex];
        int count = tierCounts[tierIndex];

        for (int i = start; i < start + count; i++)
        {
            if (!pooledEnemies[i].activeInHierarchy)
                return pooledEnemies[i];
        }
        return null; // Called when pool is empty
    }
}
