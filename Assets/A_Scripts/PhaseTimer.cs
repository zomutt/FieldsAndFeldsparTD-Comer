using UnityEngine;

public enum PhaseType
{
    Grace,  // No spawning,, player prepares and explores mechanics, map, and builds
    Tier,   // Enemies begin to spawn 
    Rest    // No spawning between tiers,, this gives player some breathing room
}
[System.Serializable]
public struct Phase
{
    public PhaseType type;   // Grace, Tier, Rest
    public float duration;   // How long this phase lasts
}
public class PhaseTimer : MonoBehaviour
{
    [Header("Phase Schedule")]
    public Phase[] phases;   // These are set in inspector,, this method allows for scalability between scenes without needing extra scripts
    public int CurrentPhaseIndex { get; private set; } = 0;
    public Phase CurrentPhase => phases[CurrentPhaseIndex];
    public float TimeInPhase { get; private set; } = 0f;
    public bool IsGrace => CurrentPhase.type == PhaseType.Grace;
    public bool IsTier => CurrentPhase.type == PhaseType.Tier;
    public bool IsRest => CurrentPhase.type == PhaseType.Rest;
    public int CurrentTier { get; private set; } = -1;  // Only increments during Tier phases,, tier 1 = game start

    private void Start()
    {
        TimeInPhase = 0f;
        CurrentPhaseIndex = 0;
        CurrentTier = -1; // No tier yet
    }
    private void Update()
    {
        TimeInPhase += Time.deltaTime;

        // If this phase is finished then we move to the next one
        if (TimeInPhase >= CurrentPhase.duration)
        {
            AdvancePhase();
        }

        // Dev cheat for testing purposes, will not be in final game.
        if (Input.GetKeyDown(KeyCode.P))
        {
            AdvancePhase();
        }

    }
    private void AdvancePhase()
    {
        TimeInPhase = 0f;
        CurrentPhaseIndex++;
        // Clamp to last phase so that we cannot go beyond the boss fight,, this is a fail safe
        if (CurrentPhaseIndex >= phases.Length)
        {
            CurrentPhaseIndex = phases.Length - 1;
        }
        // When we enter a new tier phase we increment the tier counter
        if (CurrentPhase.type == PhaseType.Tier)
        {
            CurrentTier++;
        }
    }
}
