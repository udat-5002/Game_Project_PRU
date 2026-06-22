using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float rotationSmoothTime = 0.1f;
    
    [Tooltip("Điều chỉnh góc này (thường là 90, -90, 180) nếu nhân vật đi ngang")]
    public float modelRotationOffset = -90f;

    [Header("Animation")]
    public Animator animator;
    public float speedAnimBlend = 10f; // Tốc độ chuyển đổi animation

    [Header("Jumping & Gravity")]
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;
    public float groundedOffset = -0.14f;
    public float groundedRadius = 0.28f;
    public LayerMask groundLayers;

    private CharacterController controller;
    private Transform mainCamera;

    private float rotationVelocity;
    private float verticalVelocity;
    private float animationBlend;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    private bool isGrounded;

    void Start()
    {
        // Khóa con trỏ chuột vào giữa màn hình và ẩn nó đi
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        controller = GetComponent<CharacterController>();
        if (Camera.main != null)
        {
            mainCamera = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Main Camera! Nhân vật sẽ không di chuyển theo hướng nhìn của camera.");
        }

        // Setup Input Actions trực tiếp bằng code để không cần cấu hình trên Inspector
        moveAction = new InputAction("Move", binding: "<Gamepad>/leftStick");
        moveAction.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/s")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/a")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/d")
            .With("Right", "<Keyboard>/rightArrow");
            
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

    void Update()
    {
        CheckGrounded();
        JumpAndGravity();
        Move();
    }

    private void CheckGrounded()
    {
        // Tính toán vị trí chân của nhân vật dựa vào CharacterController thay vì transform.position
        float feetY = transform.position.y + controller.center.y - (controller.height / 2f);
        Vector3 spherePosition = new Vector3(transform.position.x, feetY - groundedOffset, transform.position.z);
        
        // Lưu ý: Nhớ gán groundLayers trong Inspector (VD: Everything)
        // Kết hợp kiểm tra bằng Layer và thuộc tính isGrounded có sẵn của CharacterController
        isGrounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore) || controller.isGrounded;

        // Cập nhật trạng thái chạm đất cho Animator
        if (animator != null)
        {
            animator.SetBool("Grounded", isGrounded);
        }
    }

private void Move()
{
    Vector2 input = moveAction.ReadValue<Vector2>();
    Vector3 targetDirection = Vector3.zero;
    
    // Nhận biết ngay lập tức việc người chơi có bấm phím hay không
    bool isMoving = input.sqrMagnitude > 0.01f;

    if (isMoving)
    {
        // Tính toán góc xoay... (giữ nguyên code cũ của bạn)
        Vector3 inputDirection = new Vector3(input.x, 0.0f, input.y).normalized;
        
        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        if (mainCamera != null)
        {
            targetAngle += mainCamera.eulerAngles.y;
        }
        
        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle + modelRotationOffset, ref rotationVelocity, rotationSmoothTime);
        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

        targetDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
    }

    // Tính toán tốc độ hiện tại (đi bộ hay chạy)
    float currentSpeed = sprintAction.IsPressed() ? runSpeed : moveSpeed;

    // Áp dụng di chuyển
    Vector3 movement = targetDirection.normalized * currentSpeed + Vector3.up * verticalVelocity;
    controller.Move(movement * Time.deltaTime);

    // --- CẬP NHẬT ANIMATION CHO BLEND TREE ---
    if (animator != null)
    {
        // Tốc độ mục tiêu: Bấm phím thì = currentSpeed, nhả phím = 0
        float targetSpeed = isMoving ? currentSpeed : 0f;
        
        // Cốt lõi chống trượt đà: 
        // Nếu nhả phím (targetSpeed == 0), tăng tốc độ blend lên rất cao (vd: 20f) để về Idle ngay lập tức.
        // Nếu đang di chuyển, dùng tốc độ speedAnimBlend bình thường (10f) cho mượt.
        float currentBlendSpeed = (targetSpeed == 0) ? 20f : speedAnimBlend; 

        // Dùng MoveTowards thay vì Lerp để kiểm soát tốc độ chính xác
        animationBlend = Mathf.MoveTowards(animationBlend, targetSpeed, Time.deltaTime * currentBlendSpeed);
        
        // Truyền giá trị vào Parameter "Speed" trong Animator (Giá trị sẽ chạy từ 0 đến 5)
        animator.SetFloat("Speed", animationBlend);
    }
}

    private void JumpAndGravity()
    {
        if (isGrounded)
        {
            if (animator != null) animator.SetBool("FreeFall", false);

            // Giữ nhân vật áp sát mặt đất khi không nhảy
            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -2f; 
            }

            // Xử lý nhảy
            if (jumpAction.triggered)
            {
                // Công thức tính lực nhảy: V = sqrt(H * -2 * G)
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                
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

            // Áp dụng trọng lực khi đang trên không
            verticalVelocity += gravity * Time.deltaTime;
        }
    }
}
