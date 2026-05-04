using UnityEngine;

public class DevCheats : MonoBehaviour
{
    [SerializeField] private GameObject devWindow;
    private bool isDevOpen;
    private void Start()
    {
        devWindow.SetActive(false);
        isDevOpen = false;
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
            if (Time.timeScale == 1f)
            {
                Time.timeScale = 3f;
            }
            else if (Time.timeScale == 3f)
            {
                Time.timeScale = 1f;
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
