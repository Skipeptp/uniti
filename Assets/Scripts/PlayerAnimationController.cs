using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController controller;

    [Header("Movement")]
    [SerializeField] private float forwardSpeed = 7f;
    [SerializeField] private float laneDistance = 3f;
    [SerializeField] private float laneChangeSpeed = 10f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private int maxJumps = 2;

    [Header("Gravity")]
    [SerializeField] private float gravity = -20f;
    [SerializeField][Range(1.5f, 4f)] private float fastFallMultiplier = 2.5f;

    [Header("Slide")]
    [SerializeField] private float slideDuration = 0.6f;

    [Header("Collider размеры")]
    [SerializeField] private float normalHeight = 1.8f;
    [SerializeField] private float normalCenterY = 0.9f;
    [SerializeField] private float slideHeight = 0.9f;
    [SerializeField] private float slideCenterY = 0.45f;

    private const string SPEED_PARAM = "speed";
    private const string JUMP_TRIG = "jump_trig";
    private const string SLIDE_TRIG = "slide_trig";

    private Vector3 velocity;
    private bool isGrounded;
    private bool isFastFalling;
    private bool isSliding;
    private float slideTimer;

    private int currentLane = 0;
    private int jumpCount = 0;

    private void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (controller == null) controller = GetComponent<CharacterController>();

        animator.SetFloat(SPEED_PARAM, 1f);

        // Запоминаем нормальные размеры из инспектора, если не заданы вручную
        if (normalHeight == 0f) normalHeight = controller.height;
        if (normalCenterY == 0f) normalCenterY = controller.center.y;
    }

    private void Update()
    {
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

    private void HandleLaneInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
            ChangeLane(-1);

        if (Input.GetKeyDown(KeyCode.D))
            ChangeLane(1);
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

            // Прыжок прерывает подкат — возвращаем нормальный коллайдер
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

            // Сжимаем коллайдер
            SetColliderSlide();

            if (!isGrounded)
                isFastFalling = true;
        }
    }

    private void UpdateSlideTimer()
    {
        if (!isSliding) return;

        slideTimer += Time.deltaTime;
        if (slideTimer >= slideDuration)
        {
            isSliding = false;
            // Возвращаем нормальный коллайдер
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
        float diffX = targetX - transform.position.x;

        float moveX = diffX * laneChangeSpeed;

        if (Mathf.Abs(moveX * Time.deltaTime) > Mathf.Abs(diffX))
            moveX = diffX / Time.deltaTime;

        Vector3 horizontalMove = new Vector3(moveX, 0f, forwardSpeed);
        controller.Move((horizontalMove + velocity) * Time.deltaTime);
    }
}