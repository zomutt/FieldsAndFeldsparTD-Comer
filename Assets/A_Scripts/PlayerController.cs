using UnityEngine;
/// <summary>
/// Handles player movement, tile detection, and tower placement input.
/// Raycasting downward replaces per-tile Update() checks, significantly reducing overhead since there's 200 or so tiles
/// The player is whats responsible for which tile is active -- tiles just respond when told.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private BuildingSquare currentSquare;  // Tracks which square the player is currently on

    void Update()
    {
        HandleMovement();
        HandleRaycast();
        HandleInput();
    }

    private void HandleMovement()
    {
        // These are backwards because of how the level was set up vs. how the camera was set up
        float x = -Input.GetAxisRaw("Vertical");
        float z = Input.GetAxisRaw("Horizontal");
        Vector3 dir = new Vector3(x, 0f, z).normalized;
        transform.Translate(moveSpeed * Time.deltaTime * dir, Space.World);
    }

    private void HandleRaycast()
    {
        // Shoots a ray downward to find which tile the player is standing on
        // This replaces Update() checks on the tiles to significantly reduce overhead

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 5f))
        {
            BuildingSquare square = hit.collider.GetComponent<BuildingSquare>();
            if (square != currentSquare)
            {
                if (currentSquare != null)
                {
                    currentSquare.OnPlayerExit();
                }
                currentSquare = square;
                if (currentSquare != null)
                {
                    currentSquare.OnPlayerEnter();
                }
            }
        }
        else
        {
            if (currentSquare != null)
            {
                currentSquare.OnPlayerExit();
            }
            currentSquare = null;
        }
    }

    private void HandleInput()
    {
        if (currentSquare == null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentSquare.SpawnTower(BuildingSquare.TowerType.Shooter);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentSquare.SpawnTower(BuildingSquare.TowerType.AOE);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentSquare.SpawnTower(BuildingSquare.TowerType.Gold);
        }
    }
}