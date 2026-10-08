using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Camera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow")]
    [Tooltip("Damping of the horizontal follow")]
    [SerializeField] private float followSmoothTime = 0.15f;

    [Header("Look-ahead")]
    [Tooltip("How far ahead of the player the camera shifts")]
    [SerializeField] private float lookAheadDistance = 3f;
    [Tooltip("How long the look-ahead takes to swap sides")]
    [SerializeField] float lookAheadSmooth = 0.5f;
    [Tooltip("Minimum horizontal speed")]
    [SerializeField] private float directionThreshold = 0.5f;
    [Tooltip("Initial facing: 1 = right, -1 = left")]
    [SerializeField] private float startFacing = 1f;

    [Header("Vertical (locked)")]
    [Tooltip("If true, use cameras Y at the start of the scene")]
    [SerializeField] private bool useStartY = true;
    [SerializeField] private float fixedY = 0f;

    [Header("Level bounds (world x)")]
    [SerializeField] private bool clampToBounds = true;
    [SerializeField] private float levelMinX = -50f;
    [SerializeField] private float levelMaxX = 50f;

    private UnityEngine.Camera cam;

    private float facing;
    private float currentLookAhead;
    private float lookAheadVelocity;
    private float followVelocity;
    private float lastTargetX;


    private void Awake()
    {
        cam = GetComponent<UnityEngine.Camera>();
        if(useStartY) fixedY = transform.position.y;
        facing = Mathf.Sign(startFacing);
    }

    private void Start()
    {
        if(target == null) return;

        lastTargetX = target.position.x;
        currentLookAhead = facing * lookAheadDistance;
        SnapToTarget();
    }

    private void LateUpdate()
    {
        if(target == null) return;

        UpdateFacing();

        float targetLookAhead = facing * lookAheadDistance;
        currentLookAhead = Mathf.SmoothDamp(currentLookAhead, targetLookAhead, ref lookAheadVelocity, lookAheadSmooth);

        float desiredX = target.position.x + currentLookAhead;
        float newX = Mathf.SmoothDamp(transform.position.x, desiredX, ref followVelocity, followSmoothTime);

        newX = ClampX(newX);
        transform.position = new Vector3(newX, fixedY, transform.position.z);

    }

    private void UpdateFacing()
    {
        float dt = Time.deltaTime;
        if(dt<=0f)return;

        float speedX = (target.position.x - lastTargetX) / dt;
        lastTargetX = target.position.x;

        if(Mathf.Abs(speedX) > directionThreshold)
            facing = Mathf.Sign(speedX);

    }

    private float ClampX(float x)
    {
        if(!clampToBounds || !cam.orthographic) return x;

        float halfWidth = cam.orthographicSize * cam.aspect;
        float min = levelMinX + halfWidth;
        float max = levelMaxX - halfWidth;

        if(min > max) return (levelMinX + levelMaxX) * 0.5f;

        return Mathf.Clamp(x, min, max);
    }

    public void SnapToTarget()
    {
        if(target == null) return;

        lastTargetX = target.position.x;
        lookAheadVelocity = 0f;
        followVelocity = 0f;

        float x = ClampX(target.position.x + currentLookAhead);
        transform.position = new Vector3(x, fixedY, transform.position.z);
    }

    public void SetTarget(Transform newTarget, bool snap = true)
    {
        target = newTarget;
        if(snap) SnapToTarget();
    }

    private void OnDrawGizmosSelected()
    {
        if(!clampToBounds) return;

        float y = Application.isPlaying ? fixedY : transform.position.y;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(levelMinX, y - 20f, 0f), new Vector3(levelMinX, y + 20f, 0f));
        Gizmos.DrawLine(new Vector3(levelMaxX, y - 20f, 0f), new Vector3(levelMaxX, y + 20f, 0f));

    }

}
