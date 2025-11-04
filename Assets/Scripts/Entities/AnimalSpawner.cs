using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject pigPrefab;        // Assign Pig Prefab here
    public int spawnCount = 10;         // how many pigs to spawn
    public Vector2 worldSize = new Vector2(50, 50); // X-Z range
    public float baseY = 10f;           // estimated ground Y (you can adjust)

    [Header("Y auto-correct")]
    public LayerMask groundMask;        // ground mask for raycast

    void Start()
    {
        SpawnAnimals();
    }

    void SpawnAnimals()
    {
        if (pigPrefab == null)
        {
            Debug.LogWarning("Pig Prefab not assigned in spawner!");
            return;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 pos = RandomXZ();

            // Try to ground-align
            pos = SnapToGround(pos);

            Instantiate(pigPrefab, pos, Quaternion.identity, transform);
        }
    }

    Vector3 RandomXZ()
    {
        float x = Random.Range(-worldSize.x, worldSize.x);
        float z = Random.Range(-worldSize.y, worldSize.y);
        return new Vector3(x, baseY, z);
    }

    Vector3 SnapToGround(Vector3 pos)
    {
        // cast down
        if (Physics.Raycast(pos + Vector3.up * 50f, Vector3.down, out var hit, 200f, groundMask))
        {
            return hit.point;
        }
        return pos;
    }
}
