using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This is a universal spawner script for all 3 enemy tiers.
/// Spawns enemies from three lane-specific spawners that are randomly picked from the given prefabs.
/// Each tier has it's own GameObject where the specifics of what to spawn and how many to spawn are placed.
/// The TierManager.cs controls which wave-specific spawner is active or inactive.
/// </summary>

public class WaveSpawnPool : MonoBehaviour
{
    [Header("Prefabs and Spawn Delay")]
    [SerializeField] private GameObject[] tierPrefabs;
    [SerializeField] private float spawnDelay;


    [Header("Amount To Spawn (Corrected in Validate if not divisible by 3.)")]
    [Tooltip("This is corrected on validate if the divident is not evenly divisible by 3.")]
    // Public because TierManager needs to know how many mobs are supposed to be in each tier
    public int amountToPool;

    [Header("Spawners")]
    // These are what the mobs spawn from, the left/right is from perspective of the castle
    [SerializeField] private Transform spawnerMid;
    [SerializeField] private Transform spawnerLeft;
    [SerializeField] private Transform spawnerRight;

    private int amountSpawned;
    private List<GameObject> pooledObjects;

    private void OnValidate()
    {
        if (amountToPool % 3 != 0)
        {
            Debug.LogWarning($"{name}: amountToPool must be divisible by 3. Adjusting automatically.");
            amountToPool -= amountToPool % 3;
        }
    }
    private void Start()
    {
        pooledObjects = new List<GameObject>();
        GameObject newMob;

        for (int i = 0; i < amountToPool; i++)
        {
            newMob = Instantiate(tierPrefabs[Random.Range(0, tierPrefabs.Length)]);
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
            Debug.Log("All enemies for this wave spawned.");
            StopAllCoroutines();
            gameObject.SetActive(false);
        }
    }
    private GameObject GetPooledObject()
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
            yield return new WaitForSeconds(spawnDelay);
        }
    }
    private void SpawnMob(Transform spawner)
    {
        GameObject mob = GetPooledObject();
        if (mob == null) return;
        mob.transform.position = spawner.position;
        mob.SetActive(true);
        amountSpawned++;
    }
}


