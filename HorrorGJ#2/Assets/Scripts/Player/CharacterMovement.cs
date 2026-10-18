using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class CharacterMovement : MonoBehaviour
{

    public IInteractable Interactable {get; set;}
    static readonly int IsRunningHash = Animator.StringToHash("IsRunning");
    static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    static readonly int IsIdleHash = Animator.StringToHash("IsIdle");
    [SerializeField]public Light2D light;

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
    [Tooltip("Spot Light2D points along its local Y, so -90 aligns it with the aim angle.")]
    [SerializeField] float lightAngleOffset = -90f;

    [Header("Movement")]
    [SerializeField] public float moveSpeed = 3f;
    [SerializeField] public float runSpeed = 3f;
    [Tooltip("Vertical scale for 2:1 iso tiles. 0.5 keeps ground speed equal in every direction and makes diagonals follow tile edges.")]
    [SerializeField] float isoYScale = 0.5f;
 
    [Header("Animation (optional)")]
    [SerializeField] Animator animator;
 
    static readonly int FacingXHash = Animator.StringToHash("FacingX");
    static readonly int FacingYHash = Animator.StringToHash("FacingY");
    static readonly int SpeedHash   = Animator.StringToHash("Speed");

    static readonly int DirectionHash   = Animator.StringToHash("Direction");
    
    static readonly int DirectionLightHash   = Animator.StringToHash("DirectionLight");
    static readonly int hasLightHash   = Animator.StringToHash("HasLight");


 
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
        light.enabled = false;
    }
 
    void OnEnable()
    {
        if (moveAction) moveAction.action.Enable();
        if (lockFacingAction) lockFacingAction.action.Enable();
        if (runAction) runAction.action.Enable();
    }
 
    void OnDisable()
    {
        if (moveAction) moveAction.action.Disable();
        if (lockFacingAction) lockFacingAction.action.Disable();
        if (runAction) runAction.action.Disable();
        if (aimPointAction) aimPointAction.action.Enable();
    }
 

    private float lightTimer = 3f;
    private float lightCooldown = 0.5f; // Cooldown in seconds
    private float lightElpsed = 0f; // Threshold for light intensity
    private float speedMultiplier = 1f;
    public void PlayerUpdate()
    {
        moveInput = moveAction ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        IsFacingLocked = lockFacingAction && lockFacingAction.action.IsPressed();
        IsRunning = runAction.action.IsPressed();


        if (IsMoving)
        {
            animator.SetBool(IsIdleHash, false);
            Vector2 dir = moveInput.normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            FacingIndex = ((Mathf.RoundToInt(angle / 45f) % 8) + 8) % 8;
            //float angle = Mathf.Atan2(dir.y, dir.x);
            //float t = (angle / (2f * Mathf.PI) + 1f) % 1f;
            animator.SetFloat(DirectionHash, FacingIndex);

            float speed = IsRunning? runSpeed : moveSpeed;

            velocity = new Vector2(dir.x, dir.y * isoYScale) * speed;
            //if (!IsFacingLocked) SetFacing(dir);

            //animator.SetBool(IsWalkingHash, true);
        }
        else
        {
            velocity = Vector2.zero;
            animator.SetBool(IsWalkingHash, false);
            animator.SetBool(IsIdleHash, true);
        }
 
        if (animator)
        {
//            animator.SetFloat(FacingXHash, Facing8.x);
//            animator.SetFloat(FacingYHash, Facing8.y);
//            animator.SetFloat(SpeedHash, velocity.magnitude);
        }


        if(!hasLight) return;

        if(switchLight.action.WasPressedThisFrame())
        {
           SwitchLight = !SwitchLight;
        }

        if(SwitchLight)
        {
            lightElpsed += Time.deltaTime;
            if(lightElpsed >= lightTimer)
            {
                StartCoroutine(LightCooldown());
                lightElpsed = 0f;
            }
        }

        animator.SetFloat(DirectionLightHash, FacingIndex);
        animator.SetBool(hasLightHash, SwitchLight);

        // Only show the light once the animator has fully reached a holding-light state,
        // so it doesn't visibly slide into place during the transition.
        bool inLightState = animator.GetCurrentAnimatorStateInfo(0).IsTag("Light") && !animator.IsInTransition(0);
        light.enabled = SwitchLight && inLightState;
        
    }
 
    private IEnumerator LightCooldown()
    {
        yield return new WaitForSeconds(Random.Range(lightCooldown, lightCooldown * 2f));
        SwitchLight = false;
    }

    void LateUpdate()
    {
        // UpdateAim();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = velocity * speedMultiplier;;
    }
 
    void UpdateAim()
    {
        if (!cam || !light) return;
 
        Vector2 screenPos;
        if (aimPointAction) screenPos = aimPointAction.action.ReadValue<Vector2>();
        else if (Mouse.current != null) screenPos = Mouse.current.position.ReadValue();
        else return;
 
        Vector3 origin = light.transform.position;
        Vector3 world = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, origin.z - cam.transform.position.z));
        Vector2 toCursor = (Vector2)(world - origin);
 
        // Cursor right on top of the light: keep the last direction
        if (toCursor.sqrMagnitude < 0.0001f) return;
 
        AimDirection = toCursor.normalized;
 
        float angle = Mathf.Atan2(AimDirection.y, AimDirection.x) * Mathf.Rad2Deg;
        light.transform.rotation = Quaternion.Euler(0f, 0f, angle + lightAngleOffset);
    }

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
        speedMultiplier = 0.3f;
    }

    public void ResetSpeed()
    {
        speedMultiplier = 1f;
    }

}
