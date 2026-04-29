using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tower Spawners are tiled in such a way so as to keep the player from double building, messing up their geometry, miscounting tiles they can build, etc.
/// Instantiation was chosen over deactivated towers or object pooling because the player will realistically only build a small percentage of what they can per level.
/// This was considered the most efficient way over other options for this reason since player income throttles tower spam.
/// 
/// Consider Raycast that checks collision with player to avoid constant checks on all squares.
/// </summary>
public class BuildingSquare : MonoBehaviour
{
    public enum TowerType
    {
        None = 0,   // Intentionally blank in order to keep towers consistent with keybinds
        Shooter = 1,
        AOE = 2,
        Gold = 3
        // Add more tower types here as needed
    }

    [System.Serializable]
    public struct TowerPrefabEntry
    {
        public TowerType type;      // Which tower this entry represents
        public GameObject prefab;   // The prefab to spawn for that tower
        public int cost;
    }

    [Header("Towers (Type -> Prefab pairs)")]
    [SerializeField] private TowerPrefabEntry[] towerPrefabs;

    private Dictionary<TowerType, TowerPrefabEntry> prefabLookup;

    private bool isOccupied;

    // The player should be able to clearly see which tile they are considered to be on
    private Renderer towerSR;
    private Color startColor;
    private bool playerPresent;

    private void Start()
    {
        isOccupied = false;

        towerSR = GetComponent<Renderer>();
        startColor = towerSR.material.color;

        // Build a lookup so we can safely get prefabs by TowerType and include cost
        prefabLookup = new Dictionary<TowerType, TowerPrefabEntry>();

        foreach (var tower in towerPrefabs)
        {
            if (tower.type == TowerType.None)
                continue;

            prefabLookup[tower.type] = tower;
        }
    }

    // Called when the player chooses to build a tower on this tile
    public void SpawnTower(TowerType type)
    {
        if (isOccupied) return;

        var tower = prefabLookup[type];
        if (GoldManager.Instance.CurrentGold < tower.cost)
        {
            Debug.Log("Not enough gold to build this tower.");
            return;
        }

        GoldManager.Instance.DecreaseGold(tower.cost);
        Instantiate(tower.prefab, transform.position + new Vector3(0f, 2f, 0f), transform.rotation);

        isOccupied = true;  // Prevents more than one tower being built on this tile
    }
    private void OnTriggerEnter(Collider other)
    {
        // Allows player to see what square they are on
        if (other.gameObject.CompareTag("Player"))
        {
            towerSR.material.color = Color.green;
            playerPresent = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            towerSR.material.color = startColor;
            playerPresent = false;
        }
    }
    private void Update()
    {
        // The player has a tiny collider so it only targets the tile directly below the center
        if (playerPresent)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SpawnTower(TowerType.Shooter);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SpawnTower(TowerType.AOE);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SpawnTower(TowerType.Gold);
            }
        }
        // More keybinds will be added as the scope and tower amount grows, but this is enough for this level of scope
    }
}
