using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TierManager : MonoBehaviour
{
    /// <summary>
    /// This initiates new waves and keeps track of how many mobs have been killed.
    /// Killed mobs are compared against expected mobs to track when a wave should end.
    /// After the wave ends, the player has a grace period to prepare for the next wave.
    /// </summary>
    public static TierManager Instance;

    // Starts on tier 1
    private int currentTier;

    // Grabbed from WaveSpawnPool.cs
    private int expectedSpawns;
    private int currentSpawns;

    // Increased each time a mob is killed from mob script
    private int mobsKilled;

    [SerializeField] private Transform[] waveSpawners;      // The red portals the enemies come from
    [SerializeField] private float waveSpawnDelay;
    [SerializeField] private float timeBetweenSpawns = .2f;
    private float spawnCD;
    private bool canSpawn;
    private void Awake()
    {
        Instance = this;

        spawnCD = timeBetweenSpawns;
    }
    public void StartLevel()
    {
        currentTier = 1;
        Debug.Log("TM: StartLevel called");
        StartCoroutine(WaveSpawnDelay());
    }
    public void RecordKill()
    {
        // Called by enemy base script anytime a mob is destroyed
        mobsKilled++;
        if (mobsKilled >= expectedSpawns)
        {
            Debug.Log($"Wave {currentTier} cleared!");
            currentTier++;
            if (currentTier < 3)
            {
                StartCoroutine(WaveSpawnDelay());
            }
            else Debug.Log("TM: All waves complete. Boss logic next.");
        }
    }
    private IEnumerator WaveSpawnDelay()
    {
        yield return new WaitForSeconds(waveSpawnDelay);
        Debug.Log("TM: WaveSpawnDelay called.");
        StartNewWave();
    }
    private void StartNewWave()
    {
        // Gets the amount pooled
        expectedSpawns = WaveSpawnPool.Instance.GetAmountToSpawn(currentTier);
        currentSpawns = 0;
        StartCoroutine(SpawnEnemyWithDelay());
    }

    // Calls 1 enemy per 3 spawners, tracks how many enemies were spawned, and gives a small buffer window before spawning the next
    private IEnumerator SpawnEnemyWithDelay()
    {
        while (currentSpawns < expectedSpawns)
        {
            foreach (Transform t in waveSpawners)
            {
                if (currentSpawns >= expectedSpawns)
                { 
                    break; 
                }
                GameObject enemy = WaveSpawnPool.Instance.GetEnemy((WaveSpawnPool.TierLevel)currentTier);
                enemy.transform.position = t.position;
                currentSpawns++;
            }
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}
