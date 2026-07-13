using TMPro;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This script was created to avoid overflowing the main UIController.cs script since this has a large amount of specific job duties.
/// HelpUI.cs handles all of the behaviour that lives inside of the help screen -- the help screen is still toggled on and off by UIManager.cs while the pausing is handled by GameManager.cs
/// </summary>
public class HelpUI : MonoBehaviour
{
    public static HelpUI Instance;

    // AESTHETICS
    [SerializeField] private GameObject horizontalPebbles;         // Needed for enemies and towers info

    // TABS
    [SerializeField] private GameObject[] tabs;

    // TOWER HELP
    [SerializeField] private GameObject towerInfoPanel;
    [SerializeField] private GameObject[] towerInfoPrefabs;        // Prefabs include text and image
    private int towerInt;

    [Header("Upgrade Info")]
    [SerializeField] private GameObject mainMenuPanel;      // Needed so the upgrades can't be toggled while in the main menu 
    [SerializeField] private UnityEngine.UI.Button upgradeButton;
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private TextMeshProUGUI upgradeLevelText;

    [Header("Amethyst")]
    [SerializeField] private TextMeshProUGUI amethystDmg;
    [SerializeField] private TextMeshProUGUI amethystCost;

    [Header("Magma")]
    [SerializeField] private TextMeshProUGUI aoeDmg;
    [SerializeField] private TextMeshProUGUI aoeCost;

    [Header("Slow")]
    [SerializeField] private TextMeshProUGUI slowAmt;
    [SerializeField] private TextMeshProUGUI slowCost;

    [Header("Gold")]
    [SerializeField] private TextMeshProUGUI goldAmt;
    [SerializeField] private TextMeshProUGUI goldCost; 
    // HOW TO PLAY
    [SerializeField] private GameObject howToPlay;

    private void Start()
    {
        Instance = this;
        // Game should default to How to Play tab, but stay on whatever tab the player closes
        // ex: if they close while looking at towers, they should be able to open it and see towers still
        ClearAllTabs();
        howToPlay.SetActive(true);
        towerInt = 0;

        upgradeCostText.text = string.Empty;
        UpdateTowerHelpUI();
    }
    public void UpdateTowerHelpUI()
    {
        amethystDmg.text = $"Damage: {TowerStats.Instance.ShooterDamage}";
        amethystCost.text = $"Cost: {TowerStats.Instance.ShooterCost}";

        aoeDmg.text = $"Damage/sec: {TowerStats.Instance.AoeDamage}";
        aoeCost.text = $"Cost: {TowerStats.Instance.AoeCost}";

        slowAmt.text = $"Slow: {TowerStats.Instance.StSlowAmount}%";
        slowCost.text = $"Cost: {TowerStats.Instance.StSlowCost}";

        goldAmt.text = $"Gold/sec: {GoldManager.Instance.GoldFarmYield}";
        goldCost.text = $"Cost: {GoldManager.Instance.GoldFarmCost}";
        UpdateUpgradeInfoText();
    }
    public void UpdateUpgradeInfoText()
    {
        switch(towerInt)
        {
            case 0:
                upgradeCostText.text = $"Upgrade Cost: {UpgradeManager.Instance.ShooterUpgradeCost}";
                upgradeLevelText.text = $"Level: {UpgradeManager.Instance.ShooterUpgradeLevel}";
                break;
            case 1:
                upgradeCostText.text = $"Upgrade Cost: {UpgradeManager.Instance.AoeUpgradeCost}";
                upgradeLevelText.text = $"Level: {UpgradeManager.Instance.AoeUpgradeLevel}";
                break;
            case 2:
                if (!UpgradeManager.Instance.IsStSlowMaxed())
                {
                    upgradeCostText.text = $"Upgrade Cost: {UpgradeManager.Instance.StSlowUpgradeCost}";
                    upgradeLevelText.text = $"Level: {UpgradeManager.Instance.StSlowUpgradeLevel}";
                }
                else
                {
                    upgradeCostText.text = string.Empty;
                    upgradeLevelText.text = "Max level!";
                }
                break;
            case 3:
                {
                    upgradeCostText.text = $"Upgrade Cost: {UpgradeManager.Instance.MineUpgradeCost}";
                    upgradeLevelText.text = $"Level: {UpgradeManager.Instance.MineUpgradeLevel}";
                    break;
                }
        }
    }
    private void ClearAllTabs()
    {
        foreach (GameObject go in tabs)
        {
            go.SetActive(false);
        }
        horizontalPebbles.SetActive(false);
        towerInfoPanel.SetActive(false);
    }
    public void OnClickHowToPlay()
    {
        ClearAllTabs();
        howToPlay.SetActive(true);
    }
    public void OnClickTowersTab()
    {
        ClearAllTabs();
        towerInfoPanel.SetActive(true);
        horizontalPebbles.SetActive(true);
        foreach (GameObject tower in towerInfoPrefabs)
        {
            tower.SetActive(false);
        }
        towerInfoPrefabs[towerInt].SetActive(true);

        if (!mainMenuPanel.activeSelf)
        {
            upgradeButton.interactable = true;
        }
        else
        {
            upgradeButton.interactable = false;
        }
        UpdateUpgradeInfoText();
    }
    public void OnClickNextTower()
    {
        towerInfoPrefabs[(towerInt)].SetActive(false);
        towerInt++;
        if (towerInt >=  towerInfoPrefabs.Length)
        {
            towerInt = 0;
        }
        towerInfoPrefabs[(towerInt)].SetActive(true);
        UpdateUpgradeInfoText();
    }
    public void OnClickPrevTower()
    {
        towerInfoPrefabs[(towerInt)].SetActive(false);
        towerInt--;
        if(towerInt < 0)
        {
            towerInt = towerInfoPrefabs.Length - 1;
        }
        towerInfoPrefabs[(towerInt)].SetActive(true);
        UpdateUpgradeInfoText();
    }
    public void OnClickUpgradeTower()
    {
        switch (towerInt)
        {
            case 0:
            {
                UpgradeManager.Instance.UpgradeShooter();
                break;
            }
            case 1: 
            {
                UpgradeManager.Instance.UpgradeAoe();
                break;
            }
            case 2:
            {
                UpgradeManager.Instance.UpgradeStSlow();
                break;
            }
            case 3:
            {
                UpgradeManager.Instance.UpgradeMine();
                break;
            }
        }
        UpdateTowerHelpUI();
        UpdateUpgradeInfoText();
    }
}
