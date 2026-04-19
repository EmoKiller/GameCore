
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputAdapter : MonoBehaviour , ICharacterInput
{
    /// <summary>
    /// PlayerInput component được attach vào Player prefab, reference đến Input System action map,
    /// được sử dụng để lấy các InputAction cụ thể và đăng ký callback.
    /// </summary>
    [SerializeField] private PlayerInput playerInput;

    /// <summary>
    /// InputBuffer component để lưu trữ trạng thái input hiện tại, được attach vào Player prefab,
    /// được sử dụng để lưu trữ trạng thái input hiện tại cho di chuyển, tấn công, nhảy, và cung cấp các phương thức để tiêu thụ input này trong PlayerController.
    /// </summary>
    //[SerializeField] private InputBuffer inputBuffer;

    /// <summary>
    /// Các InputAction cụ thể cho di chuyển, tấn công, nhảy, được lấy từ player action map trong Awake(),
    /// và được sử dụng để đăng ký callback trong OnEnable() và hủy đăng ký trong OnDisable().
    /// </summary>
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction attackAction;
    private InputAction jumpAction;

    /// <summary>
    /// Biến lưu trữ input hiện tại cho di chuyển và ngắm, được cập nhật trong callback của các actions,
    /// </summary>
    private Vector2 moveInput;
    
    public Vector2 MoveInput => moveInput;

    private bool sprintHeld;

    public bool SprintHeld => sprintHeld;

    private bool jumpHeld;
    public bool JumpHeld => jumpHeld;

    //private Vector2 aimInput;

    /// <summary>
    /// Các sự kiện public để các hệ thống khác (như PlayerController) có thể đăng ký và phản hồi khi có input mới,
    /// OnMove sẽ truyền Vector2 direction, OnJump và OnAttack sẽ chỉ đơn giản là thông báo rằng hành động đã được thực hiện mà không cần dữ liệu bổ sung.
    /// </summary>
    public event System.Action OnJump;
    public event System.Action OnAttack;

    public void Initialize()
    {
        playerInput = FindAnyObjectByType<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("Player Input component not found on " + gameObject.name);
            return;
        }
        
        // Lấy các action cụ thể
        moveAction = playerInput.actions["Move"];
        attackAction = playerInput.actions["Attack"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];

        if (moveAction == null || attackAction == null || jumpAction == null || sprintAction == null)
        {
            Debug.LogError("InputActions not found in PlayerInput");
        }
        OnEnable();
    }
    private void OnEnable()
    {
        if (moveAction == null) return;
        Debug.Log("OnEnable");
        moveAction.performed += HandleMove;
        moveAction.canceled += HandleMove;

        sprintAction.performed += HandlePerformedSprint;
        sprintAction.canceled += HandleCanceledSprint;

        
        jumpAction.performed += HandlePerformedJump;
        jumpAction.canceled += HandleCanceledJump;

        attackAction.performed += HandleAttack;
    }
    
    private void OnDisable()
    {
        if (moveAction == null) return;

        moveAction.performed -= HandleMove;
        moveAction.canceled -= HandleMove;

        sprintAction.performed -= HandlePerformedSprint;
        sprintAction.canceled -= HandleCanceledSprint;

        jumpAction.performed -= HandlePerformedJump;
        jumpAction.canceled -= HandleCanceledJump;

        attackAction.performed -= HandleAttack;
        
    }
    

    private void HandleMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
        moveInput = new Vector2(moveInput.x, 0f);
    }
    
    private void HandlePerformedSprint(InputAction.CallbackContext ctx)
    {
        sprintHeld = true;
    }

    private void HandleCanceledSprint(InputAction.CallbackContext ctx)
    {
        sprintHeld = false;
    }

    private void HandlePerformedJump(InputAction.CallbackContext context)
    {
        jumpHeld = true;
        OnJump?.Invoke();
    }

    private void HandleCanceledJump(InputAction.CallbackContext context)
    {
        jumpHeld = false;
    }

    private void HandleAttack(InputAction.CallbackContext context)
    {
        OnAttack?.Invoke();
    }
    // public bool ConsumeAttack()
    // {
    //     return inputBuffer.ConsumeAttack();
    // }

    // public bool ConsumeJump()
    // {
    //     return inputBuffer.ConsumeJump();
    // }

    
}
