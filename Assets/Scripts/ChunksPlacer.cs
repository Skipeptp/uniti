using UnityEngine;

public class ChunksPlacer : MonoBehaviour
{
    [Header("Ссылки")]
    public Transform player;
    public Chunk chunkPrefab;
    public Chunk firstChunk;

    [Header("Параметры")]
    public bool spawnOnZ = true;

    private Chunk currentChunk;
    private Chunk previousChunk;
    private bool spawned = false;

    private void Start()
    {
        currentChunk = firstChunk;
        previousChunk = null;
        spawned = false;

        if (firstChunk == null)
        {
            Debug.LogError("[ChunksPlacer] First Chunk не назначен!");
            return;
        }

        if (chunkPrefab == null)
        {
            Debug.LogError("[ChunksPlacer] Chunk Prefab не назначен!");
            return;
        }

        firstChunk.GenerateContent();
    }

    private void Update()
    {
        if (currentChunk == null) return;
        CheckMiddleAndSpawn();
    }

    private void CheckMiddleAndSpawn()
    {
        float beginPos = spawnOnZ
            ? currentChunk.Begin.position.z
            : currentChunk.Begin.position.x;

        float endPos = spawnOnZ
            ? currentChunk.End.position.z
            : currentChunk.End.position.x;

        float middlePos = (beginPos + endPos) * 0.5f;

        float playerPos = spawnOnZ
            ? player.position.z
            : player.position.x;

        if (!spawned && playerPos >= middlePos)
        {
            spawned = true;
            SpawnNextChunk();
        }
    }

    private void SpawnNextChunk()
    {
        Chunk newChunk = Instantiate(chunkPrefab);

        // Сначала выставляем позицию
        Vector3 offset = newChunk.Begin.localPosition;
        Vector3 targetPos = currentChunk.End.position - offset;
        newChunk.transform.position = targetPos;

        Debug.Log($"[ChunksPlacer] Новый чанк на позиции {targetPos}");

        // Только после позиционирования генерируем контент
        newChunk.GenerateContent();

        if (previousChunk != null)
            Destroy(previousChunk.gameObject);

        previousChunk = currentChunk;
        currentChunk = newChunk;
        spawned = false;
    }
}