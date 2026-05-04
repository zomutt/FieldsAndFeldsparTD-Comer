using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Input and decisions are made by PlayerController.cs and this executes them out. The player is operating off a raycast that detects what tile they are on and then tells the tile what to build.
/// This was done to reduce overhead cost and increase game performance, especially given ~200 or so exist per scene.
/// </summary>
public class BuildingSquare : MonoBehaviour
{
    public enum TowerType
    {
        None = 0,   // Remove tower will go here when that is implemented.
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

    [Header("Towers (Type to Prefab pairs)")]
    [SerializeField] private TowerPrefabEntry[] towerPrefabs;

    private Dictionary<TowerType, TowerPrefabEntry> prefabLookup;

    private bool isOccupied;

    // The player should be able to clearly see which tile they are considered to be on
    private Renderer towerSR;
    private Color startColor;
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
            {
                continue;
            }
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
    public void OnPlayerEnter()
    {
        towerSR.material.color = Color.green;
    }
    public void OnPlayerExit()
    {
        towerSR.material.color = startColor;
    }
}
