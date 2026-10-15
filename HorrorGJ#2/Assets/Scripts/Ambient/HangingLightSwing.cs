using UnityEngine;

/// <summary>
/// Pendulum swing for a hanging light in a 2D / isometric scene.
/// Put this on the anchor (ceiling attachment point). Assign a child as swingTarget
/// (holding the Light2D, bulb sprite, FlickerLight2D, etc.).
/// </summary>
public class HangingLightSwing : MonoBehaviour
{
    private const float Gravity = 9.81f;

    [Header("References")]
    [Tooltip("Child that moves with the swing (Light2D, bulb sprite...).")]
    [SerializeField] private Transform swingTarget;
    [Tooltip("Optional sprite that tilts (cable + lamp). Its pivot should be at the top.")]
    [SerializeField] private Transform tiltVisual;

    [Header("Pendulum")]
    [Tooltip("Longer rope = slower swing.")]
    [SerializeField, Min(0.05f)] private float ropeLength = 1.5f;
    [Tooltip("On-screen direction of the swing. For isometric, e.g. (1, 0.5).")]
    [SerializeField] private Vector2 swingDirection = new Vector2(1f, 0.5f);
    [Tooltip("On-screen displacement (world units) at a 90° swing.")]
    [SerializeField] private float swingDistance = 1f;
    [SerializeField, Range(1f, 89f)] private float maxAngle = 60f;

    [Header("Behaviour")]
    [Tooltip("ON: always swings, returns to constantAmplitude after pushes. OFF: rests and only moves when pushed.")]
    [SerializeField] private bool constantSwing = true;
    [SerializeField, Range(0f, 60f)] private float constantAmplitude = 15f;
    [Tooltip("How fast it settles (to rest, or back to constantAmplitude).")]
    [SerializeField, Min(0f)] private float damping = 0.5f;
    [Tooltip("Degrees of sprite tilt per degree of swing. Use a negative value to flip the side.")]
    [SerializeField] private float tiltFactor = 1f;
    [SerializeField] private bool randomizeStartPhase = true;

    private float angle;       // radians
    private float angularVel;  // radians/s
    private Vector3 restLocalPos;
    private Quaternion restTiltRot;

    /// <summary>Current swing angle in degrees.</summary>
    public float AngleDegrees => angle * Mathf.Rad2Deg;

    private void Awake()
    {
        if (swingTarget == null)
        {
            Debug.LogWarning($"{name}: HangingLightSwing needs a swingTarget.", this);
            enabled = false;
            return;
        }

        restLocalPos = swingTarget.localPosition;
        if (tiltVisual != null) restTiltRot = tiltVisual.localRotation;

        if (constantSwing && constantAmplitude > 0f)
        {
            float amp = constantAmplitude * Mathf.Deg2Rad;
            float phase = randomizeStartPhase ? Random.Range(0f, Mathf.PI * 2f) : Mathf.PI * 0.5f;
            float w = Mathf.Sqrt(Gravity / ropeLength);
            angle = amp * Mathf.Sin(phase);
            angularVel = amp * w * Mathf.Cos(phase);
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        float w2 = Gravity / ropeLength;
        float acc = -w2 * Mathf.Sin(angle);

        if (constantSwing && constantAmplitude > 0f)
        {
            // Pulls the amplitude toward the target: damps if above, drives if below.
            float target = constantAmplitude * Mathf.Deg2Rad;
            float error = (CurrentAmplitude(w2) - target) / target;
            acc += -damping * error * angularVel;

            // Kick it if it ever stops completely
            if (Mathf.Abs(angle) < 0.0001f && Mathf.Abs(angularVel) < 0.0001f)
                angularVel = target * Mathf.Sqrt(w2);
        }
        else
        {
            acc += -damping * angularVel;
        }

        angularVel += acc * dt;
        angle += angularVel * dt;

        float maxRad = maxAngle * Mathf.Deg2Rad;
        if (Mathf.Abs(angle) > maxRad)
        {
            angle = Mathf.Sign(angle) * maxRad;
            if (Mathf.Sign(angularVel) == Mathf.Sign(angle)) angularVel = 0f;
        }

        Apply();
    }

    private float CurrentAmplitude(float w2)
    {
        // Energy-based amplitude estimate
        float cosMax = 1f - (0.5f * angularVel * angularVel / w2 + (1f - Mathf.Cos(angle)));
        return Mathf.Acos(Mathf.Clamp(cosMax, -1f, 1f));
    }

    private void Apply()
    {
        Vector2 dir = swingDirection.sqrMagnitude > 0f ? swingDirection.normalized : Vector2.right;
        float offset = Mathf.Sin(angle) * swingDistance;
        swingTarget.localPosition = restLocalPos + (Vector3)(dir * offset);

        if (tiltVisual != null)
            tiltVisual.localRotation = restTiltRot * Quaternion.Euler(0f, 0f, -angle * Mathf.Rad2Deg * tiltFactor);
    }

    /// <summary>Adds angular velocity (degrees/s). Positive = toward swingDirection.</summary>
    public void Push(float strength)
    {
        angularVel += strength * Mathf.Deg2Rad;
    }

    /// <summary>Push from a world direction (e.g. player/enemy bumping into it, explosion).</summary>
    public void PushFrom(Vector2 worldDirection, float strength)
    {
        Vector2 dir = transform.TransformDirection(swingDirection).normalized;
        float alignment = Vector2.Dot(worldDirection.normalized, dir);
        Push(alignment * strength);
    }
}
