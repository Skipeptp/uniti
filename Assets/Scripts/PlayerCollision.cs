using UnityEngine;
using TMPro;

public class PlayerCollision : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI coinsText;

    [Header("References")]
    [SerializeField] private PlayerAnimationController animationController;

    public int coins = 0;
    public bool hitObstacle = false;

    private void Start()
    {
        UpdateCoinsUI();
    }

    // Для препятствий с Is Trigger = OFF (физический удар)
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Obstacle"))
        {
            if (hitObstacle) return;
            hitObstacle = true;
            Debug.Log("Физический удар: " + hit.collider.name);

            if (animationController != null)
                animationController.Die();
            else
                Debug.LogError("animationController = NULL!");
        }
    }

    // Для препятствий с Is Trigger = ON
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            if (hitObstacle) return;
            hitObstacle = true;
            Debug.Log("Триггер удар: " + other.name);

            if (animationController != null)
                animationController.Die();
            else
                Debug.LogError("animationController = NULL!");
        }

        if (other.CompareTag("Coin"))
        {
            coins++;
            UpdateCoinsUI();
            Destroy(other.gameObject);
        }
    }

    private void UpdateCoinsUI()
    {
        if (coinsText != null)
            coinsText.text = "Монеты: " + coins;
    }
}