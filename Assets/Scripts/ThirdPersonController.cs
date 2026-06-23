using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float rotationSmoothTime = 0.1f;
    public float modelRotationOffset = -90f;

    [Header("Animation")]
    public Animator animator;
    public float speedAnimBlend = 10f;

    [Header("Jumping & Gravity")]
    public float jumpHeight = 1.2f;
    public float gravity = -15f;
    public float groundedOffset = -0.14f;
    public float groundedRadius = 0.28f;
    public LayerMask groundLayers;

    CharacterController controller;
    Transform mainCamera;

    float rotationVelocity;
    float verticalVelocity;
    float animationBlend;
    float maxUpVelocity;

    InputAction moveAction;
    InputAction jumpAction;
    InputAction sprintAction;

    bool isGrounded;

    const float FootToPivot = 1.5f;
    const float MaxFeetAboveGround = 1.6f;

    void Start()
    {
        CharacterMaterialFixer.ApplyTo(transform);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        controller = GetComponent<CharacterController>();
        controller.stepOffset = 0f;
        controller.slopeLimit = 40f;
        groundLayers = LayerMask.GetMask("Ground", "Default");

        maxUpVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        if (Camera.main != null)
            mainCamera = Camera.main.transform;

        moveAction = new InputAction("Move", binding: "<Gamepad>/leftStick");
        moveAction.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w").With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/s").With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/a").With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/d").With("Right", "<Keyboard>/rightArrow");
        jumpAction = new InputAction("Jump", binding: "<Keyboard>/space");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");
        sprintAction = new InputAction("Sprint", binding: "<Keyboard>/leftShift");
        sprintAction.AddBinding("<Gamepad>/rightTrigger");

        moveAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();
    }

    void OnDestroy()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
        sprintAction?.Disable();
    }

    public void ResetVerticalVelocity() => verticalVelocity = -2f;

    void Update()
    {
        CheckGrounded();
        JumpAndGravity();
        if (GameManager.Instance != null && GameManager.Instance.InputLocked)
            return;
        Move();
        EnforceGroundHeight();
    }

    float GetFeetY() => transform.position.y - FootToPivot;

    void CheckGrounded()
    {
        if (!GroundSnap.TryGetGroundY(transform.position, out float groundY))
        {
            isGrounded = false;
            if (animator != null) animator.SetBool("Grounded", false);
            return;
        }

        float feetY = GetFeetY();
        float feetAbove = feetY - groundY;
        var spherePos = new Vector3(transform.position.x, feetY - groundedOffset, transform.position.z);
        bool nearSurface = Physics.CheckSphere(spherePos, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);

        isGrounded = nearSurface && feetAbove <= 0.45f && verticalVelocity <= 0.1f;

        if (animator != null)
            animator.SetBool("Grounded", isGrounded);
    }

    void Move()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 targetDirection = Vector3.zero;
        bool isMoving = input.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            var inputDirection = new Vector3(input.x, 0f, input.y).normalized;
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
            if (mainCamera != null)
                targetAngle += mainCamera.eulerAngles.y;

            float rotation = Mathf.SmoothDampAngle(
                transform.eulerAngles.y, targetAngle + modelRotationOffset, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, rotation, 0f);
            targetDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        float currentSpeed = sprintAction.IsPressed() ? runSpeed : moveSpeed;
        float yBefore = transform.position.y;
        var movement = targetDirection.normalized * currentSpeed + Vector3.up * verticalVelocity;
        controller.Move(movement * Time.deltaTime);

        float yGain = transform.position.y - yBefore;
        if (yGain > 0.25f && verticalVelocity <= 0.5f)
        {
            var p = transform.position;
            p.y = yBefore;
            controller.enabled = false;
            transform.position = p;
            controller.enabled = true;
            verticalVelocity = 0f;
        }

        if (animator != null)
        {
            float targetSpeed = isMoving ? currentSpeed : 0f;
            float blendSpeed = targetSpeed == 0f ? 20f : speedAnimBlend;
            animationBlend = Mathf.MoveTowards(animationBlend, targetSpeed, Time.deltaTime * blendSpeed);
            animator.SetFloat("Speed", animationBlend);
        }
    }

    void EnforceGroundHeight()
    {
        if (!GroundSnap.TryGetGroundY(transform.position, out float groundY))
            return;

        float feetY = GetFeetY();
        if (feetY <= groundY + MaxFeetAboveGround)
            return;

        SnapFeetTo(groundY);
    }

    void SnapFeetTo(float groundY)
    {
        var pos = transform.position;
        pos.y = groundY + FootToPivot + 0.08f;
        controller.enabled = false;
        transform.position = pos;
        controller.enabled = true;
        verticalVelocity = -2f;
    }

    void JumpAndGravity()
    {
        verticalVelocity = Mathf.Min(verticalVelocity, maxUpVelocity);

        if (isGrounded)
        {
            if (animator != null) animator.SetBool("FreeFall", false);
            if (verticalVelocity < 0f) verticalVelocity = -2f;

            bool dialogueOpen = DialogueManager.Instance != null && DialogueManager.Instance.IsShowing;
            if (jumpAction.triggered && !dialogueOpen &&
                (GameManager.Instance == null || !GameManager.Instance.InputLocked))
            {
                verticalVelocity = maxUpVelocity;
                if (animator != null) animator.SetBool("Jump", true);
            }
        }
        else
        {
            if (animator != null)
            {
                animator.SetBool("FreeFall", true);
                animator.SetBool("Jump", false);
            }
            verticalVelocity += gravity * Time.deltaTime;
        }
    }
}
