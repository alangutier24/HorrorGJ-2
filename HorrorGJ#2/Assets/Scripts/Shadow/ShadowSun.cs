using UnityEngine;

/// One per scene. Drives the directional (sun/moon) shadow.
/// Animate azimuth/elevation/strength for a day-night cycle.
public class ShadowSun : MonoBehaviour
{
    public static ShadowSun Instance { get; private set; }

    [Tooltip("Iso Grid used to convert the ground direction to screen space. If empty, assumes a 2:1 iso.")]
    public Grid grid;

    [Tooltip("Direction the shadow points, in ground/cell space (0 = +X cell axis).")]
    [Range(0f, 360f)] public float azimuth = 45f;
    [Tooltip("Sun angle above the horizon. Low = long shadows.")]
    [Range(5f, 89f)] public float elevation = 40f;
    [Range(0f, 1f)] public float strength = 1f;

    public Vector2 ScreenDir { get; private set; }
    public float LengthPerHeight => 1f / Mathf.Tan(elevation * Mathf.Deg2Rad);

    void OnEnable() => Instance = this;
    void OnDisable() { if (Instance == this) Instance = null; }

    void Update()
    {
        float a = azimuth * Mathf.Deg2Rad;
        Vector2 g = new Vector2(Mathf.Cos(a), Mathf.Sin(a));

        if (grid)
        {
            // Ground vector -> screen vector, normalized so a screen-horizontal ground direction has length 1.
            Vector2 d = GroundToScreen(g);
            Vector2 horiz = GroundToScreen(new Vector2(1f, -1f).normalized);
            ScreenDir = d / Mathf.Max(horiz.magnitude, 1e-4f);
        }
        else
        {
            // Plain 2:1 isometric: rotate 45° and squash Y by half.
            ScreenDir = new Vector2((g.x - g.y) * 0.7071f, (g.x + g.y) * 0.7071f * 0.5f);
        }
    }

    Vector2 GroundToScreen(Vector2 v)
    {
        Vector3 o = grid.CellToLocalInterpolated(Vector3.zero);
        Vector3 p = grid.CellToLocalInterpolated(new Vector3(v.x, v.y, 0f));
        return grid.transform.TransformVector(p - o);
    }
}
