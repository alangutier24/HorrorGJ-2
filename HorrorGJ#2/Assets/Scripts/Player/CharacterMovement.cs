using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{

    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float sprintSpeed = 7f;

    static readonly int SpeedHash = Animator.StringToHash("Speed");
    static readonly int IsRunningHash = Animator.StringToHash("IsRunning");

    Rigidbody2D rb;
    SpriteRenderer sr;
    InputAction moveAction;
    InputAction sprintAction;
    Animator anim;

    float moveX;
    bool sprinting;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        moveAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint");
    }
        
    void Update()
    {
        moveX = moveAction.ReadValue<Vector2>().x;
        sprinting = sprintAction.IsPressed();

        if(sr != null && moveX != 0f)
            sr.flipX = moveX < 0f;

        float speed = Mathf.Abs(rb.linearVelocity.x);
        anim.SetFloat(SpeedHash, speed);
        anim.SetBool(IsRunningHash, sprinting && speed > 0.1f);
        
    }

    void FixedUpdate()
    {
        float speed = sprinting ? sprintSpeed : walkSpeed;
        rb.linearVelocity = new Vector2(moveX * speed, rb.linearVelocity.y);
    }

}
