using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    [SerializeField] private GameObject timerPanel;
    [SerializeField] private TextMeshProUGUI timerText;
    private PhaseTimer phaseTimer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        phaseTimer = FindFirstObjectByType<PhaseTimer>();    // Find the PhaseTimer in the newly loaded scene,, needed because this is a Singleton and Timer.cs is not
        if (phaseTimer == null)
        {
            Debug.LogWarning("UIController: No PhaseTimer found in this scene.");
        }
        else
        {
            Debug.Log("UIController: Found PhaseTimer in scene.");
        }
    }

    private void Start()
    {
        timerPanel.SetActive(true);
    }
    private void Update()
    {
        // Convert elapsed time into MM:SS format
        int minutes = Mathf.FloorToInt(Time.time/60f);
        int seconds = Mathf.FloorToInt(Time.time % 60f); // Seconds = remainder after dividing by 60
        timerText.text = $"Time Elapsed: {minutes:00}:{seconds:00}"; // The 00 ensures that 2 digits will always be shown
    }
}
