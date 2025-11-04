using UnityEngine;
using System.Collections;

public class AutoAnimalSpawner : MonoBehaviour
{
    [Header("Resources path (no extension). Example: Entities/Pig")]
    public string pigResourcePath = "Entities/Pig";   // Resources/Entities/Pig.prefab

    [Header("Spawn Settings")]
    public int spawnCount = 6;
    public float radiusAroundPlayer = 20f;            // 先调小，刷在你身边更明显
    public float startDelaySeconds = 2.5f;            // 给世界一点时间加载

    [Header("Grounding")]
    public LayerMask groundMask = ~0;                 // Everything
    public float raycastUp = 80f;
    public float raycastDown = 200f;

    private Transform _player;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);                // survive Menu -> Game
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
            // ✅ 改1：没有 Player 时，以主相机为中心；再不行就用(0,0,0)
            Vector3 center = _player
                ? _player.position
                : (Camera.main ? Camera.main.transform.position : Vector3.zero);

            Vector3 pos = RandomOnXZCircle(center, radiusAroundPlayer, center.y + 1f);
            pos = SnapToGround(pos);

            var go = Instantiate(pigPrefab, pos, Quaternion.identity);
            Debug.Log($"[Spawner] Spawned pig at {pos} -> {go.name}");   // ✅ 改2：日志确认确实刷了

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
            return hit.point + Vector3.up * 0.3f;  // 小抬高，避免卡地面

        // ✅ 改3：兜底。如果没打到地面，就用主相机高度附近，避免藏在地下/天外
        float y = Camera.main ? Camera.main.transform.position.y + 1f : pos.y;
        return new Vector3(pos.x, y, pos.z);
    }
}
