using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// This script serves the purpose of handling player movement and feeding input into the TowerSpawner.cs scripts to tell the squares what to instantiate.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    void Update()
    {
        // These are backwards because of how the level was set up vs. how the camera was set up
        float x = -Input.GetAxisRaw("Vertical");
        float z = Input.GetAxisRaw("Horizontal");

        Vector3 dir = new Vector3(x, 0f, z).normalized;

        transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
    }
    private void OnTriggerStay(Collider other)
    {
        // The player itself has a TINY collider to prevent overlap between tiles; it can only target what is directly below the center of the players GO
        if (other.gameObject.CompareTag("BuildArea"))
        {
            var towerS = other.gameObject;
            if (towerS == null) return;     // Fail-safe

            var towerSquare = other.gameObject.GetComponent<TowerSpawners>();

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                // Shooter tower
                towerSquare.SpawnTower(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                // AOE tower
                towerSquare.SpawnTower(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                // Gold farm tower
                towerSquare.SpawnTower(3);
            }

            // More towers can be added as long as Enum and prefabs in TowerSpawner.cs are updated too
        }
    }
}
