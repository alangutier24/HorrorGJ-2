using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using System.Collections;

public class CharacterMovement : MonoBehaviour
{

    public IInteractable Interactable {get; set;}
    static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    static readonly int IsIdleHash = Animator.StringToHash("IsIdle");
    static readonly int IsIdleLightHash = Animator.StringToHash("IsIdleLight");
    static readonly int IsIdleLightDirectionHash = Animator.StringToHash("IdleLightDirection");
    static readonly int DirectionHash   = Animator.StringToHash("Direction");
    static readonly int hasLightHash   = Animator.StringToHash("HasLight");

    [FormerlySerializedAs("light")]
    [SerializeField] public Light2D playerLight;

    [Header("Input")]
    [Tooltip("Value / Vector2 (WASD composite)")]
    [SerializeField] InputActionReference moveAction;
    [Tooltip("Button () - hold to keep current facing")]
    [SerializeField] InputActionReference lockFacingAction;
    [Tooltip("Button (Left Shift) - hold left ship to sprint")]
    [SerializeField] InputActionReference runAction;

    [Tooltip("Button (Left Shift) - hold left ship to sprint")]
    [SerializeField] InputActionReference switchLight;
 
    [Header("Aim")]
    [Tooltip("Optional. Value / Vector2 bound to <Pointer>/position. If empty, reads Mouse.current directly.")]
    [SerializeField] InputActionReference aimPointAction;
    [Tooltip("Camera used to convert the cursor to world space. Defaults to Camera.main.")]
    [SerializeField] UnityEngine.Camera cam;

    [Header("Movement")]
    [SerializeField] public float moveSpeed = 3f;
    [SerializeField] public float runSpeed = 3f;
    [Tooltip("Vertical scale for 2:1 iso tiles. 0.5 keeps ground speed equal in every direction and makes diagonals follow tile edges.")]
    [SerializeField] float isoYScale = 0.5f;
 
    [Header("Animation (optional)")]
    [SerializeField] Animator animator;
 
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

    public bool IsRunning;

    public bool SwitchLight;

    public bool hasLight = false;
 
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        SetFacing(Facing8);
        playerLight.enabled = false;

        animator.SetBool(IsIdleHash, true);
        animator.SetBool(hasLightHash, false);
    }
 
    void OnEnable()
    {
        if (moveAction) moveAction.action.Enable();
        if (runAction) runAction.action.Enable();
        if (switchLight) switchLight.action.Enable();
    }
 
    void OnDisable()
    {
        if (moveAction) moveAction.action.Disable();
        if (runAction) runAction.action.Disable();
        if (switchLight) switchLight.action.Disable();
    }
 
    void Start()
    {
        playerLight.enabled = false;
    }

    private float lightTimer = 3f;
    private float lightCooldown = 0.5f; // Cooldown in seconds
    private float lightElapsed = 0f; // Threshold for light intensity
    private float speedMultiplier = 1f;
    Vector2 dir = Vector2.zero;

    private bool lightRequested;
    private bool isLightCoolingDown;
    public void PlayerInput()
    {
        moveInput = moveAction ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        IsRunning = runAction.action.IsPressed();
        SwitchLight = switchLight.action.WasPressedThisFrame();
    }
 
    public void UpdateMovement()
    {       
        if (IsMoving)
        {
            float speed = IsRunning? runSpeed : moveSpeed;
            velocity = new Vector2(dir.x, dir.y * isoYScale) * speed;
       }
        else
        {
            velocity = Vector2.zero;
        }
    }

    public void UpdateLight()
    {
        if (!hasLight) return;

        if (SwitchLight && !playerLight.enabled && !isLightCoolingDown)
        {
            lightRequested = true;
        }

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        if (lightRequested && !animator.IsInTransition(0) && state.IsTag("Light"))
        {
            playerLight.enabled = true;
            lightRequested = false;
            lightElapsed = 0f;
        }

        if (playerLight.enabled)
        {
            lightElapsed += Time.deltaTime;
            if (lightElapsed >= lightTimer && !isLightCoolingDown)
            {
                isLightCoolingDown = true;
                StartCoroutine(LightCooldown());
            }
        }

    }

    public void UpdateAnimations()
    {
        if (IsMoving)
        {
            dir = moveInput.normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            FacingIndex = ((Mathf.RoundToInt(angle / 45f) % 8) + 8) % 8;
        }

        animator.SetBool(IsIdleHash, !IsMoving);
        animator.SetBool(IsWalkingHash, IsMoving);
        animator.SetBool(hasLightHash, hasLight);
        animator.SetBool(IsIdleLightHash, !IsMoving && hasLight);
        animator.SetFloat(DirectionHash, FacingIndex);
        animator.SetFloat(IsIdleLightDirectionHash, FacingIndex / 8f);
    }
    
    public void PlayerUpdate()
    {
        PlayerInput();
        UpdateMovement();
        UpdateLight();
        UpdateAnimations();
    }
    private IEnumerator LightCooldown()
    {
        yield return new WaitForSeconds(Random.Range(lightCooldown, lightCooldown * 2f));
        playerLight.enabled = false;
        isLightCoolingDown = false;
        lightElapsed = 0f;
    }

    void FixedUpdate()
    {
        if (knockbackTimeLeft > 0f)
        {
            knockbackTimeLeft -= Time.fixedDeltaTime;
            rb.linearVelocity = knockbackVelocity;
            return;
        }
        rb.linearVelocity = velocity * speedMultiplier;
    }

    Vector2 knockbackVelocity;
    float knockbackTimeLeft;
    [SerializeField] float knockbackDuration = 2f;

    [SerializeField] float knockbackForce = 1f;

    void SetFacing(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        FacingIndex = ((Mathf.RoundToInt(angle / 45f) % 8) + 8) % 8;
 
        float rad = FacingIndex * 45f * Mathf.Deg2Rad;
        Facing8 = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        AimDirection = new Vector2(Facing8.x, Facing8.y * isoYScale).normalized;
    }

    public void StopMovement()
    {
        velocity = Vector2.zero;
    }

    public void RecieveDamage()
    {
        speedMultiplier = 0.7f;

        Vector2 away = velocity.sqrMagnitude > 0.0001f ? velocity : new Vector2(dir.x, dir.y * isoYScale);
        knockbackVelocity = -away * knockbackForce;
        knockbackTimeLeft = knockbackDuration;
    }

    public void ResetSpeed()
    {
        speedMultiplier = 1f;
    }

}
