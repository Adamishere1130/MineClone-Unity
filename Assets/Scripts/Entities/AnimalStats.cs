using UnityEngine;

[CreateAssetMenu(menuName = "AI/AnimalStats", fileName = "NewAnimalStats")]
public class AnimalStats : ScriptableObject
{
    [Header("Movement")]
    public float walkSpeed = 2.0f;
    public float runSpeed  = 4.0f;

    [Header("Perception")]
    [Tooltip("Max distance to detect targets")]
    public float viewRadius = 12f;
    [Tooltip("Horizontal viewing angle in degrees")]
    public float viewAngle  = 120f;

    [Header("Idle/Wander/Graze")]
    [Tooltip("Seconds")]
    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;

    [Tooltip("Probability to choose Graze instead of Wander after Idle (0~1)")]
    [Range(0f, 1f)] public float grazeChance = 0.4f;

    [Header("Turning")]
    public float turnSpeedDegPerSec = 360f;

    [Header("Flee")]
    [Tooltip("How long a flee burst lasts (seconds)")]
    public Vector2 fleeDurationRange = new Vector2(2f, 3f);
}
