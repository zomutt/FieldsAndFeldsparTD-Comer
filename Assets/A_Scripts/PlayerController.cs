using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;


    private void Awake()
    {
        
    }
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
        if (other.gameObject.CompareTag("BuildArea"))
        {
            TowerSpawners towerSpawner = other.gameObject.GetComponent<TowerSpawners>();

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("1 pressed");
                towerSpawner.SpawnTower(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                towerSpawner.SpawnTower(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                towerSpawner.SpawnTower(2);
            }
        }
    }
}
