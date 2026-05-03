using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform firstWaypoint; // assign per lane
    //[SerializeField] private GameObject bossPrefab;
    
    public void SpawnEnemy(int tier)
    {
        // Called by TierManager.cs to spawn an enemy of the given tier.
        // The spawner is randomly picked from the three lane-specific spawners, and the enemy is spawned at the position of that spawner.
        GameObject enemy = WaveSpawnPool.Instance.GetEnemy((WaveSpawnPool.TierLevel)tier);
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();

        // Turns off agent so that it can properly spawn in correct spawner
        agent.enabled = false;

        // Sets enemy to the position of the spawner now that the agent is off. Otherwise, it just favors mid since it's the shortest path.
        enemy.transform.position = transform.position;
        agent.enabled = true;

        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        movement.SetFirstDestination(firstWaypoint);
    }
}
