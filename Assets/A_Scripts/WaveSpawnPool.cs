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


    [Header("Amount To Spawn (Must be divisible by 3")]
    [Tooltip("Since there are 3 spawners, anything not divisible by 3 will cause overshoots and bugs.")]
    [SerializeField] private int amountToPool;


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


    [Header("Lane Balancing")]
    // Bag for mid lane distribution
    private List<Transform> midLaneBag = new List<Transform>();

    [Tooltip("MUST be one third of what is chosen for amountToPool")]
    // This must be 1/3 because 1/3 of all pooled enemies go to mid -- thus 1/3
    // Tracking this is necessary because I need to set half of mid spawns to go left
    // This is because of imba issues with them all pooling right
    [Header("1/3 of amount to pool")]
    [SerializeField] private int midSpawnCount;


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
        // Build the shuffle bag for mid lane before spawning begins.
        // Again, this is to keep everything balanced and store mid spawns.
        RefillMidLaneBag();
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
            SpawnMob(spawnerMid, laneTargetMid);
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

        // Randomly redirect half of mid spawns to right lane, this is to eliminate lane imba
        if (spawner == spawnerMid)
        {
            // If empty, rebuild bag (failsafe)
            if (midLaneBag.Count == 0)
            {
                RefillMidLaneBag();
            }
            // Pull next lane destination from bag
            Transform chosenTarget = midLaneBag[0];
            midLaneBag.RemoveAt(0);
            agent.SetDestination(chosenTarget.position);
        }
        else
        {
            agent.SetDestination(laneTarget.position);
        }
        tier1Mob.SetActive(true);
        amountSpawned++;
    }
    private void RefillMidLaneBag()
    {
        // midLaneBag acts like a shuffled deck of lane choices.
        // Each "card" is either laneTargetMid or laneTargetRight.
        // Every mid spawn draws the next card, guaranteeing a perfect 50/50 split.

        midLaneBag.Clear();
        int half = midSpawnCount / 2;
        for (int i = 0; i < half; i++)
        {
            midLaneBag.Add(laneTargetMid);
            midLaneBag.Add(laneTargetRight);
        }
        // Shuffle so that mob targets get randomized
        for (int i = midLaneBag.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (midLaneBag[i], midLaneBag[j]) = (midLaneBag[j], midLaneBag[i]);
        }
    }
}

