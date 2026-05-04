using UnityEngine;

public class DevCheats : MonoBehaviour
{
    [SerializeField] private GameObject devWindow;
    private bool isDevOpen;
    private bool onWarpSpeed;
    private void Start()
    {
        devWindow.SetActive(false);
        isDevOpen = false;
        onWarpSpeed = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (!isDevOpen)
            {
                devWindow.SetActive(true);
                isDevOpen = true;
            }
            else
            {
                devWindow.SetActive(false);
                isDevOpen = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            GoldManager.Instance.GiveGold(99999);
            UIController.Instance.UpdateUI();
        }
        if (Input.GetKeyDown (KeyCode.F2))
        {
            if (!onWarpSpeed)
            {
                Time.timeScale = 4f;
                onWarpSpeed = true;
            }
            else if (onWarpSpeed)
            {
                Time.timeScale = 4f;
                onWarpSpeed = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            CastleStats.Instance.Repair(CastleStats.Instance.MaxHealth);
        }
        if (Input.GetKeyDown(KeyCode.F12))
        {
            GameManager.Instance.AdvanceLevel();
        }
    }
}
