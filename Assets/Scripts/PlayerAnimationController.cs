using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController controller;

    [Header("Movement")]
    [SerializeField] private float laneDistance = 3f;
    [SerializeField] private float laneChangeSpeed = 8f;

    [Header("Speed Progression")]
    [SerializeField] private float minSpeed = 6f;
    [SerializeField] private float maxSpeed = 12f;
    [SerializeField] private float timeToMaxSpeed = 120f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private int maxJumps = 2;

    [Header("Gravity")]
    [SerializeField] private float gravity = -20f;
    [SerializeField][Range(1.5f, 4f)] private float fastFallMultiplier = 2.5f;

    [Header("Slide")]
    [SerializeField] private float slideDuration = 0.6f;

    [Header("Collider sizes")]
    [SerializeField] private float normalHeight = 1.8f;
    [SerializeField] private float normalCenterY = 0.9f;
    [SerializeField] private float slideHeight = 0.9f;
    [SerializeField] private float slideCenterY = 0.45f;

    [Header("Player Collision Reference")]
    [SerializeField] private PlayerCollision playerCollision;

    private const string SPEED_PARAM = "speed";
    private const string JUMP_TRIG = "jump_trig";
    private const string SLIDE_TRIG = "slide_trig";
    private const string DIE_TRIG = "die_trig";

    private float forwardSpeed;
    private float elapsedTime = 0f;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isFastFalling;
    private bool isSliding;
    private float slideTimer;
    private bool isDead = false;

    private int currentLane = 0;
    private int jumpCount = 0;

    private void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (controller == null) controller = GetComponent<CharacterController>();
        if (playerCollision == null) playerCollision = GetComponent<PlayerCollision>();

        forwardSpeed = minSpeed;
        animator.SetFloat(SPEED_PARAM, 1f);

        if (normalHeight == 0f) normalHeight = controller.height;
        if (normalCenterY == 0f) normalCenterY = controller.center.y;
    }

    private void Update()
    {
        if (isDead) return;

        // Нарастание скорости со временем
        elapsedTime += Time.deltaTime;
        forwardSpeed = Mathf.Lerp(minSpeed, maxSpeed, elapsedTime / timeToMaxSpeed);

        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
            isFastFalling = false;
            jumpCount = 0;
        }

        HandleLaneInput();
        HandleJumpInput();
        HandleSlideInput();
        UpdateSlideTimer();
        ApplyGravity();
        MoveCharacter();
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        velocity = Vector3.zero;

        Debug.Log("Die() called!");

        animator.ResetTrigger(JUMP_TRIG);
        animator.ResetTrigger(SLIDE_TRIG);
        animator.SetTrigger(DIE_TRIG);

        StartCoroutine(GoToMenuAfterDeath());
    }

    private IEnumerator GoToMenuAfterDeath()
    {
        yield return null;
        yield return null;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float deathAnimLength = stateInfo.length;
        Debug.Log("Death anim length: " + deathAnimLength);

        yield return new WaitForSeconds(deathAnimLength + 1.5f);

        int coins = 0;
        if (playerCollision != null)
        {
            coins = playerCollision.coins;
            Debug.Log("Coins collected: " + coins);
        }
        else
        {
            Debug.LogError("playerCollision is NULL! Assign it in Inspector.");
        }

        PlayerPrefs.SetInt("LastCoins", coins);
        int record = PlayerPrefs.GetInt("RecordCoins", 0);
        if (coins > record)
            PlayerPrefs.SetInt("RecordCoins", coins);
        PlayerPrefs.Save();

        Debug.Log("Saved LastCoins: " + coins + " | Record: " + PlayerPrefs.GetInt("RecordCoins", 0));

        SceneManager.LoadScene("MainMenu");
    }

    private void HandleLaneInput()
    {
        if (Input.GetKeyDown(KeyCode.A)) ChangeLane(-1);
        if (Input.GetKeyDown(KeyCode.D)) ChangeLane(1);
    }

    private void ChangeLane(int direction)
    {
        currentLane = Mathf.Clamp(currentLane + direction, -1, 1);
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpCount++;

            if (isSliding)
            {
                isSliding = false;
                slideTimer = 0f;
                SetColliderNormal();
            }

            isFastFalling = false;
            animator.ResetTrigger(SLIDE_TRIG);
            animator.SetTrigger(JUMP_TRIG);
        }
    }

    private void HandleSlideInput()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            animator.ResetTrigger(JUMP_TRIG);
            animator.SetTrigger(SLIDE_TRIG);

            isSliding = true;
            slideTimer = 0f;
            SetColliderSlide();

            if (!isGrounded) isFastFalling = true;
        }
    }

    private void UpdateSlideTimer()
    {
        if (!isSliding) return;
        slideTimer += Time.deltaTime;
        if (slideTimer >= slideDuration)
        {
            isSliding = false;
            SetColliderNormal();
        }
    }

    private void SetColliderSlide()
    {
        controller.height = slideHeight;
        controller.center = new Vector3(0f, slideCenterY, 0f);
    }

    private void SetColliderNormal()
    {
        controller.height = normalHeight;
        controller.center = new Vector3(0f, normalCenterY, 0f);
    }

    private void ApplyGravity()
    {
        float currentGravity = gravity;
        if (isFastFalling && velocity.y < 0f && !isGrounded)
            currentGravity *= fastFallMultiplier;
        velocity.y += currentGravity * Time.deltaTime;
    }

    private void MoveCharacter()
    {
        float targetX = currentLane * laneDistance;
        float smoothX = Mathf.Lerp(transform.position.x, targetX, laneChangeSpeed * Time.deltaTime);
        float moveX = (smoothX - transform.position.x) / Time.deltaTime;

        Vector3 horizontalMove = new Vector3(moveX, 0f, forwardSpeed);
        controller.Move((horizontalMove + velocity) * Time.deltaTime);
    }
}