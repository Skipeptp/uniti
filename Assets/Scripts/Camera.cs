using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target; // Лучше перетащи Player сюда
    private Vector3 offset;

    void Start()
    {
        // Автономайзинг, если не перетащил
        if (target == null)
        {
            GameObject playerObj = GameObject.Find("Aj"); // По имени, без тега
            if (playerObj == null)
                playerObj = GameObject.FindGameObjectWithTag("Aj");

            if (playerObj != null)
                target = playerObj.transform;
            else
                Debug.LogError("Player объект не найден! Назови объект 'Player' или перетащи в поле Target.");
        }

        if (target != null)
            offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, offset.z + target.position.z);
        transform.position = Vector3.Lerp(transform.position, newPosition, 0.6f);
    }
}