using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// This is a simple script that controls the AI movement of the enemies. 
/// Enemies spawn -> Enemies follow nav mesh to waypoint -> Enemies move to tower -> Enemies hit castle (handled separately) -> Castle loses hp -> GG.
/// Three waypoints were added in (one per lane) to allow for better flow and to prevent the enemies from favoring the middle lane. Otherwise, they just go to mid.
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    NavMeshAgent agent;
    // The castle, aka the enemies objective
    private Transform castleTransform;
    private Transform currentTarget;
    private bool headingToCastle;
    [SerializeField] private float speed;
    public float Speed => speed;          // Needed so that enemy can never out-run projectiles
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void OnEnable()
    {
        GameObject castleGO = GameObject.FindGameObjectWithTag("Castle");
        castleTransform = castleGO.transform;
        agent.speed = speed;

        // This makes it less likely for enemies to stack on top of each other
        agent.avoidancePriority = Random.Range(30, 70);

        // Pooling was done for optimization and placing the load at the beginning of the game opposed to throughout the game, not for reuse
        headingToCastle = false;
    }
    private void Update()
    {
        if (headingToCastle)
        {
            return;
        }
        if (currentTarget == null)
        {
            return;
        }
        float distance = Vector3.Distance(transform.position, currentTarget.position);
        // If the enemy is close enough to its waypoint, then it can start moving towards the castle
        // Waypoints were needed due to the lanes being uneven in length and causing routing issues
        if (distance < 8)
        {
            headingToCastle = true;
            currentTarget = castleTransform;
            agent.SetDestination(castleTransform.position);
        }
    }
    private void OnDisable()
    {
        // Ensures that the enemies start with a clean slate for the next level. Fail-safe.
        castleTransform = null;
        currentTarget = null;
        headingToCastle = false;
    }
    public void SetFirstDestination(Transform firstWaypoint)
    {
        if (firstWaypoint != null)
        {
            currentTarget = firstWaypoint;
            agent.SetDestination(firstWaypoint.position);
        }
        else
        {
            // If, for whatever reason, there's no waypoint, just head straight to the castle. Edge case protection.
            currentTarget = castleTransform;
            agent.SetDestination(castleTransform.position);
            headingToCastle = true;
        }
    }
    public void SetCastle(Transform castle)
    {
        // Called by EnemySpawner.cs to pass the current scene's castle reference.
        // This is needed because EnemyMovement persists across scenes via the object pool,
        // and Start() only fires once on instantiation, not on reuse.
        castleTransform = castle;
        headingToCastle = false;
    }
}