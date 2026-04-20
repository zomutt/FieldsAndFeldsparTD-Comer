using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This is a simple script that controls the AI movement of the enemies. 
/// Enemies spawn -> Enemies follow nav mesh -> Enemies hit castle (handled separately) -> Castle loses hp -> GG.
/// Three waypoints were added in (one per lane) to allow for better flow and to prevent the enemies from favoring the middle lane.
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    NavMeshAgent agent;
    // The castle, aka the enemies objective
    private Transform castleTransform;
    private Transform currentTarget;
    private bool headingToCastle;
    [SerializeField] private float speed;
    internal float Speed => speed;          // Needed so that enemy can never out-run projectiles


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        GameObject castleGO = GameObject.FindGameObjectWithTag("Castle");
        castleTransform = castleGO.transform;

        agent.speed = speed;

        // This makes it less likely for enemies to stack on top of each other
        agent.avoidancePriority = Random.Range(30, 70);

        // This can go in start opposed to OnEnable because an enemy is only used once -- pooling was done for optimization, not reuse
        headingToCastle = false;
    }
    private void Update()
    {
        if (headingToCastle)
            return;
        if (currentTarget == null)
            return;

        float distance = Vector3.Distance(transform.position, currentTarget.position);

        // If the enemy is close enough to its waypoint, then it can start moving towards the castle
        // Waypoints were needed due to the lanes being uneven in length and causing routing issues
        if (distance < 1)
        {
            headingToCastle = true;
            currentTarget = castleTransform;
            agent.SetDestination(castleTransform.position);
        }
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
}
