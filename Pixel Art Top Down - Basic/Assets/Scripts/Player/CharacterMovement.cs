using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class CharacterMovement : MonoBehaviour
{

    public IInteractable Interactable {get; set;}
    static readonly int IsRunningHash = Animator.StringToHash("IsRunning");
    static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float sprintSpeed = 7f;
    public bool medCamIsActive = false;
    [SerializeField] Light2D light;

    [Header("Input")]
    [Tooltip("Value / Vector2 (WASD composite)")]
    [SerializeField] InputActionReference moveAction;
    [Tooltip("Button (e.g. Left Shift) - hold to keep current facing")]
    [SerializeField] InputActionReference lockFacingAction;
 
    [Header("Movement")]
    [SerializeField] float moveSpeed = 3f;
    [Tooltip("Vertical scale for 2:1 iso tiles. 0.5 keeps ground speed equal in every direction and makes diagonals follow tile edges.")]
    [SerializeField] float isoYScale = 0.5f;
 
    [Header("Animation (optional)")]
    [SerializeField] Animator animator;
 
    static readonly int FacingXHash = Animator.StringToHash("FacingX");
    static readonly int FacingYHash = Animator.StringToHash("FacingY");
    static readonly int SpeedHash   = Animator.StringToHash("Speed");
 
    Rigidbody2D rb;
    Vector2 moveInput;
    Vector2 velocity;
 
    /// Snapped 8-dir facing in input space (use for sprites / blend tree).
    public Vector2 Facing8 { get; private set; } = Vector2.down;
    /// 0 = E, 1 = NE, 2 = N, 3 = NW, 4 = W, 5 = SW, 6 = S, 7 = SE.
    public int FacingIndex { get; private set; } = 6;
    /// Iso-corrected, normalized facing (use to point the light).
    public Vector2 AimDirection { get; private set; }
    public bool IsFacingLocked { get; private set; }
    public bool IsMoving => moveInput.sqrMagnitude > 0.01f;
 
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        SetFacing(Facing8);
    }
 
    void OnEnable()
    {
        if (moveAction) moveAction.action.Enable();
        if (lockFacingAction) lockFacingAction.action.Enable();
    }
 
    void OnDisable()
    {
        if (moveAction) moveAction.action.Disable();
        if (lockFacingAction) lockFacingAction.action.Disable();
    }
 
    void Update()
    {
        moveInput = moveAction ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        IsFacingLocked = lockFacingAction && lockFacingAction.action.IsPressed();
 
        if (IsMoving)
        {
            Vector2 dir = moveInput.normalized;
            velocity = new Vector2(dir.x, dir.y * isoYScale) * moveSpeed;
            if (!IsFacingLocked) SetFacing(dir);
        }
        else
        {
            velocity = Vector2.zero;
        }
 
        if (animator)
        {
            animator.SetFloat(FacingXHash, Facing8.x);
            animator.SetFloat(FacingYHash, Facing8.y);
            animator.SetFloat(SpeedHash, velocity.magnitude);
        }
    }
 
    void FixedUpdate()
    {
        rb.linearVelocity = velocity;
    }
 
    void SetFacing(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        FacingIndex = ((Mathf.RoundToInt(angle / 45f) % 8) + 8) % 8;
 
        float rad = FacingIndex * 45f * Mathf.Deg2Rad;
        Facing8 = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        AimDirection = new Vector2(Facing8.x, Facing8.y * isoYScale).normalized;
    }
}
