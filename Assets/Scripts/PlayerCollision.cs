using UnityEngine;
using TMPro; // ← вместо UnityEngine.UI

public class PlayerCollision : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI coinsText; // ← вместо Text

    public int coins = 0;
    public bool hitObstacle = false;

    private void Start()
    {
        UpdateCoinsUI();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Obstacle"))
        {
            hitObstacle = true;
            Debug.Log("Удар: " + hit.collider.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
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