using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Manages Tier 1 spawns using object pooling.
/// Spawns enemies from three lane-specific spawners, each with its own lane target.
/// These tier scripts are kept separate for clarity and debugging at my current skill level.
/// I plan to consolidate all tiers into a single system for my portfolio once I have more experience with multi-tier structures.
/// </summary>

public class WaveSpawnPool : MonoBehaviour
{
    [Header("Prefabs and Spawn Delay")]
    [SerializeField] private GameObject[] tier1Prefabs;
    [SerializeField] private float spawnDelay;


    [Header("Amount To Spawn (Must be divisible by 3)")]
    [Tooltip("Since there are 3 spawners, anything not divisible by 3 will cause overshoots and bugs.")]
    // Public because TierManager needs to know how many mobs are supposed to be in each tier
    internal int amountToPool;


    [Header("Spawners")]
    // These are what the mobs spawn from, the left/right is from perspective of the castle
    [SerializeField] private Transform spawnerMid;
    [SerializeField] private Transform spawnerLeft;
    [SerializeField] private Transform spawnerRight;


    [Header("Lane Targets")]
    // The lane targets are used to prevent funneling and leaking to one side
    [SerializeField] private Transform laneTargetMid;
    [SerializeField] private Transform laneTargetLeft;
    [SerializeField] private Transform laneTargetRight;


    private int amountSpawned;
    private List<GameObject> pooledObjects;

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
        amountSpawned = 0;
        StartCoroutine(CycleMobs());
    }
    private void Update()
    {
        // Disables the spawner once all mobs are on the map
        if (amountSpawned >= amountToPool)
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
            SpawnMob(spawnerMid, laneTargetMid );
            SpawnMob(spawnerLeft, laneTargetLeft);
            SpawnMob(spawnerRight, laneTargetRight);

            Debug.Log($"Tier 1 enemies spawned: {amountSpawned}/{amountToPool}");
            yield return new WaitForSeconds(spawnDelay);
        }
    }
    private void SpawnMob(Transform spawner, Transform laneTarget)
    {
        GameObject tier1Mob = GetPooledT1Object();
        if (tier1Mob == null) return;
        tier1Mob.transform.position = spawner.position;
        NavMeshAgent agent = tier1Mob.GetComponent<NavMeshAgent>();

        // Send mob to its lane target (mid lane, left lane, right lane)
        agent.SetDestination(laneTarget.position);

        // Activate and count so the TierManager knows when to end the tier
        tier1Mob.SetActive(true);
        amountSpawned++;
    }
}


