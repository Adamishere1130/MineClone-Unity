using UnityEngine;
using System.Collections;

public class AutoAnimalSpawner : MonoBehaviour
{
    [Header("Resources path (no extension). Example: Entities/Pig")]
    public string pigResourcePath = "Entities/Pig";   // Resources/Entities/Pig.prefab

    [Header("Spawn Settings")]
    public int spawnCount = 6;
    public float radiusAroundPlayer = 40f;           // spawn around player
    public float startDelaySeconds = 1.5f;           // wait for world/chunks to load

    [Header("Grounding")]
    public LayerMask groundMask = ~0;                // Everything
    public float raycastUp = 80f;
    public float raycastDown = 200f;

    private Transform _player;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);               // survive Menu -> Game
    }

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        // small delay to allow scene/world to initialize
        yield return new WaitForSeconds(startDelaySeconds);

        // find player if exists
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        _player = playerGO ? playerGO.transform : null;

        // load pig prefab from Resources
        var pigPrefab = Resources.Load<GameObject>(pigResourcePath);
        if (pigPrefab == null)
        {
            Debug.LogWarning($"AutoAnimalSpawner: Resources.Load failed at '{pigResourcePath}'. " +
                             $"Ensure you have Resources/{pigResourcePath}.prefab");
            yield break;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 center = _player ? _player.position : Vector3.zero;
            Vector3 pos = RandomOnXZCircle(center, radiusAroundPlayer, 10f); // baseY 10
            pos = SnapToGround(pos);
            Instantiate(pigPrefab, pos, Quaternion.identity);
            yield return null; // small spread across frames
        }
    }

    Vector3 RandomOnXZCircle(Vector3 center, float r, float baseY)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float dist = Random.Range(r * 0.4f, r);
        float x = center.x + Mathf.Cos(angle) * dist;
        float z = center.z + Mathf.Sin(angle) * dist;
        return new Vector3(x, baseY, z);
    }

    Vector3 SnapToGround(Vector3 pos)
    {
        Vector3 start = pos + Vector3.up * raycastUp;
        if (Physics.Raycast(start, Vector3.down, out var hit, raycastUp + raycastDown, groundMask))
            return hit.point + Vector3.up * 0.05f; // small lift
        return pos;
    }
}
