using UnityEngine;

[CreateAssetMenu(menuName = "AI/AnimalStats", fileName = "NewAnimalStats")]
public class AnimalStats : ScriptableObject
{
    [Header("Movement")]
    public float walkSpeed = 2.2f;
    public float runSpeed = 4.5f;

    [Header("Perception")]
    [Tooltip("Max distance to detect targets")]
    public float viewRadius = 12f;

    [Tooltip("Horizontal viewing angle in degrees")]
    public float viewAngle = 120f;

    [Header("Wander")]
    [Tooltip("Radius used when selecting random wander points")]
    public float wanderRadius = 8f;
    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;

    [Header("Misc")]
    public float turnSpeedDegPerSec = 360f;
}
