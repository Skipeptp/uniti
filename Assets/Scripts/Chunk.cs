using UnityEngine;

public class Chunk : MonoBehaviour
{
    [Header("Chunk bounds")]
    public Transform Begin;
    public Transform End;

    [Header("Obstacles")]
    public GameObject[] barrierPrefabs;
    public GameObject[] fencePrefabs;
    public GameObject[] carPrefabs;
    [Range(0f, 1f)] public float obstacleChance = 0.5f;

    [Header("Coins")]
    public GameObject coinPrefab;
    [Range(0f, 1f)] public float coinChance = 0.7f;

    [Header("Lanes")]
    public float leftLaneX = -3f;
    public float centerLaneX = 0f;
    public float rightLaneX = 3f;

    [Header("Z settings")]
    public float spawnStep = 5f;
    public float startOffsetZ = 5f;
    public float endOffsetZ = 5f;

    private float[] lanes;

    private void Awake()
    {
        lanes = new float[] { leftLaneX, centerLaneX, rightLaneX };
    }

    public void GenerateContent()
    {
        float beginZ = Begin.position.z;
        float endZ = End.position.z;

        // Берём min/max чтобы не зависеть от ориентации чанка
        float startZ = Mathf.Min(beginZ, endZ) + startOffsetZ;
        float stopZ = Mathf.Max(beginZ, endZ) - endOffsetZ;

        Debug.Log($"[Chunk] GenerateContent: beginZ={beginZ:F1} endZ={endZ:F1} startZ={startZ:F1} stopZ={stopZ:F1}");

        if (startZ >= stopZ)
        {
            Debug.LogWarning("[Chunk] Чанк слишком короткий или offsets слишком большие — ничего не генерируется!");
            return;
        }

        int totalSpawned = 0;

        for (float z = startZ; z < stopZ; z += spawnStep)
        {
            int obstacleLaneIndex = -1;

            if (Random.value < obstacleChance)
            {
                obstacleLaneIndex = Random.Range(0, lanes.Length);
                SpawnRandomObstacle(lanes[obstacleLaneIndex], z);
                totalSpawned++;
            }

            for (int i = 0; i < lanes.Length; i++)
            {
                if (i == obstacleLaneIndex) continue;

                if (coinPrefab != null && Random.value < coinChance)
                {
                    SpawnCoin(lanes[i], z);
                    totalSpawned++;
                }
            }
        }

        Debug.Log($"[Chunk] Заспавнено объектов: {totalSpawned}");
    }

    private void SpawnRandomObstacle(float x, float z)
    {
        GameObject prefab = null;
        int type = Random.Range(0, 3);

        if (type == 0 && barrierPrefabs.Length > 0)
            prefab = barrierPrefabs[Random.Range(0, barrierPrefabs.Length)];
        else if (type == 1 && fencePrefabs.Length > 0)
            prefab = fencePrefabs[Random.Range(0, fencePrefabs.Length)];
        else if (type == 2 && carPrefabs.Length > 0)
            prefab = carPrefabs[Random.Range(0, carPrefabs.Length)];

        if (prefab == null)
        {
            Debug.LogWarning($"[Chunk] Нет префаба для типа {type}!");
            return;
        }

        Vector3 pos = new Vector3(x, 0f, z);
        Instantiate(prefab, pos, prefab.transform.rotation, transform);
    }

    private void SpawnCoin(float x, float z)
    {
        if (coinPrefab == null) return;

        Vector3 pos = new Vector3(x, 1f, z);
        Instantiate(coinPrefab, pos, coinPrefab.transform.rotation, transform);
    }
}