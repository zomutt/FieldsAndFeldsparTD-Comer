using System.Collections;
using UnityEngine;

public class TierManager : MonoBehaviour
{
    /// <summary>
    /// This initiates new waves and keeps track of how many mobs have been killed.
    /// Killed mobs are compared against expected mobs to track when a wave should end.
    /// After the wave ends, the player has a grace period to prepare for the next wave.
    /// </summary>
    public static TierManager Instance;

    // Tier 1, 2, and 3
    [SerializeField] private GameObject[] waveSpawners;
    // Starts on tier 1
    private int waveIndex = 0;
    private int expectedMobs;
    // Increased each time a mob is killed from mob script
    private int mobsKilled;
    [SerializeField] private float waveSpawnDelay;

    private void Awake()
    {
        Instance = this;
    }
    internal void StartWave()
    {
        GameObject spawnerGO = waveSpawners[waveIndex];
        WaveSpawnPool activePool = spawnerGO.GetComponent<WaveSpawnPool>();

        // Gets the amount pooled from the GO the script is on
        // This is because it's assigned in inspector because it's 1 script for 3 waves
        expectedMobs = activePool.amountToPool;
        mobsKilled = 0;
        spawnerGO.SetActive(true);

        Debug.Log($"Wave: {waveIndex + 1}, Expected mobs: {expectedMobs}");
    }
    internal void RecordKill()
    {
        // Called by enemy base script anytime a mob is destroyed
        mobsKilled++;

        if (mobsKilled >=  expectedMobs)
        {
            NextWave();
        }
    }
    private void NextWave()
    {
        waveIndex++;

        if (waveIndex < waveSpawners.Length)
        {
            StartCoroutine(WaveCooldown());
        }
        else
        {
            // Spawn boss logic goes here. Boss is it's own prefab with it's own logic, plus it's instantiated since that is lightweight.
            Debug.Log("All waves finished.");
        }
    }
    private IEnumerator WaveCooldown()
    {
        // Timer display logic will go here.
        yield return new WaitForSeconds(waveSpawnDelay);
        StartWave();
    }
}
