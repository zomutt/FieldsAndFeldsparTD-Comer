using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEditorInternal;

/// <summary>
/// This script manages the Tier 1 spawns. After the game starts, there is a delay, then tier 1 will begin.
/// After tier 1 mobs have all been destroyed, the Tier1Spawnpool object is disabled, then a delay, then tier 2 begins, and so on.
/// For clarity and ease of debugging at my skill level, I have opted to have all tiers recycle this script but use their own GO instead of combining everything all into a single piece of logic.
/// Once I am more comfortable with data structures and multi-tier management, I intend to refactor all tiers into one script for my portfolio.
/// For now, all tiers live on their own GO with the exception of the boss due to that fight needing to be unique.
/// These are also scene specific to avoid edge-cases, bugs, etc. if reloading upon losing or advancing.
/// </summary>


public class WaveSpawnPool : MonoBehaviour
{
    [Header("Prefabs and Spawn Delay")]
    [SerializeField] private GameObject[] tier1Prefabs;
    [SerializeField] private float spawnDelay;

    private List<GameObject> pooledObjects;

    [Header("Amount To Spawn (Must be divisible by 3")]
    [Tooltip("Since there are 3 spawners, anything not divisible by 3 will cause overshoots and bugs.")]
    [SerializeField] private int amountToPool;

    private int amountSpawned;  // Tracks spawns for disabling purposes

    [Header("Spawners")]
    // These are what the mobs spawn from, the left/right is from perspective of the castle
    [SerializeField] private Transform spawnerMid;
    [SerializeField] private Transform spawnerLeft;
    [SerializeField] private Transform spawnerRight;

    private void Start()
    {
        pooledObjects = new List<GameObject>();
        GameObject newMob;
        for (int i = 0; i < amountToPool; i++)
        {
            newMob = Instantiate(tier1Prefabs[Random.Range(0, tier1Prefabs.Length)]);
            newMob.SetActive(false);
            pooledObjects.Add(newMob);
        }
        StartCoroutine(CycleMobs());
    }
    private void Update()
    {
        // Disables the spawner once all mobs are on the map
        if (amountSpawned >=  amountToPool)
        {
            // Coroutine should already be stopped, but this is a failsafe.
            Debug.Log("All Tier 1 enemies spawned.");
            StopAllCoroutines();
            gameObject.SetActive(false);
        }
    }
    private GameObject GetPooledT1Object()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }
    private IEnumerator CycleMobs()
    {
        while (true)
        {
            // Stops the coroutine so that mobs do not spawn infinitely -- this is meant to be wave-based, not endurance based.
            if (amountSpawned >= amountToPool)
            {
                yield break;
            }

            // The game loop has mobs coming from all 3 points slightly staggered out
            SpawnMob(spawnerMid);
            SpawnMob(spawnerLeft);
            SpawnMob(spawnerRight);
            Debug.Log($"Tier 1 enemies spawned:{amountSpawned}/{amountToPool}");
            yield return new WaitForSeconds(spawnDelay);
        }
    }
    private void SpawnMob(Transform spawner)
    {
        GameObject tier1Mob = GetPooledT1Object();
        if (tier1Mob == null) return;
        tier1Mob.transform.position = spawner.position;
        tier1Mob.SetActive(true);
        amountSpawned++;
    }
}
