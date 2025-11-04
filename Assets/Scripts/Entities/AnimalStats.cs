using UnityEngine;

[CreateAssetMenu(menuName = "AI/AnimalStats", fileName = "NewAnimalStats")]
public class AnimalStats : ScriptableObject
{
    [Header("Movement")]
    public float walkSpeed = 2.2f;
    public float runSpeed  = 4.5f;

    [Header("Perception")]
    [Tooltip("能看到目标的最大半径")]
    public float viewRadius = 12f;
    [Tooltip("水平视角（度）")]
    public float viewAngle  = 120f;

    [Header("Wander")]
    [Tooltip("随机游走的半径（用于选目标点等扩展）")]
    public float wanderRadius = 8f;
    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;

    [Header("Misc")]
    public float turnSpeedDegPerSec = 360f;
}
