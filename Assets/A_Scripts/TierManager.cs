using UnityEngine;

public class TierManager : MonoBehaviour
{
    // Amount of mobs that correspond to each mob
    // This is used for tracking and knowing when to activate the next tier
    private int tier1Mobs;
    private int tier2Mobs;
    private int tier3Mobs;
    private int bossMobs;     // Typically 1, but this allows for flexibility for having 3 weaker bosses as a boss wave
}
