using UnityEngine;
   using UnityEngine.Tilemaps;

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
    [SerializeField] private Vector2 lookAheadDistance = new Vector2(2.5f, 1.23f);
    [Tooltip("How long the look-ahead takes to swap sides")]
    [SerializeField] float lookAheadSmooth = 0.5f;
    [Tooltip("Minimum horizontal speed")]
    [SerializeField] private float directionThreshold = 0.5f;

    [Header("Map bounds")]
    [SerializeField] private bool clampToBounds = true;
    [SerializeField] private Tilemap boundsTilemap;
    [SerializeField] private Vector2 boundsPadding = new Vector2(0f, 1f);
    [SerializeField] SpriteRenderer mapSprite;

    private UnityEngine.Camera cam;

    private Vector2 direction;
    private Vector2 currentLookAhead;
    private Vector2 lookAheadVelocity;
    private Vector2 followVelocity;
    private Vector2 lastTargetPos;
    private Bounds mapBounds;
    private bool hasBounds;


    private void Awake()
    {
        cam = GetComponent<UnityEngine.Camera>();
    }
    public void SetBoundsFromSprite(SpriteRenderer sr)
    {
        Bounds b = sr.bounds; // world-space, already includes position/scale
        b.Expand(new Vector3(boundsPadding.x * 2f, boundsPadding.y * 2f, 0f));
        SetBounds(b);
    }

    private void Start()
    {
        //if(boundsTilemap != null) SetBoundsFromTilemap(boundsTilemap);
        SnapToTarget();
        if (mapSprite) SetBoundsFromSprite(mapSprite);
    }

    private void LateUpdate()
    {
        if(target == null) return;

        UpdateDirection();

        Vector2 targetLookAhead = Vector2.Scale(direction, lookAheadDistance);
        currentLookAhead = Vector2.SmoothDamp(currentLookAhead, targetLookAhead, ref lookAheadVelocity, lookAheadSmooth);

        Vector2 desired = (Vector2)target.position + currentLookAhead;
        Vector2 pos = Vector2.SmoothDamp(transform.position, desired, ref followVelocity, followSmoothTime);

        pos = Clamp(pos);
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);

    }

    private void UpdateDirection()
    {
        float dt = Time.deltaTime;
        if(dt<=0f)return;

        Vector2 velocity = ((Vector2)target.position - lastTargetPos) / dt;
        lastTargetPos = target.position;

        if(velocity.magnitude > directionThreshold)
            direction = velocity.normalized;

    }

        public void SetBoundsFromTilemap(Tilemap tilemap)
    {
        tilemap.CompressBounds();
        BoundsInt cb = tilemap.cellBounds;
        if (cb.size.x == 0 || cb.size.y == 0) { hasBounds = false; return; }
 
        int zLow = cb.zMin;
        int zHigh = Mathf.Max(cb.zMin, cb.zMax - 1);
 
        // The 4 corner cells of the grid are the 4 tips of the iso diamond.
        Vector3Int[] corners =
        {
            new Vector3Int(cb.xMin,     cb.yMin,     0),
            new Vector3Int(cb.xMax - 1, cb.yMin,     0),
            new Vector3Int(cb.xMin,     cb.yMax - 1, 0),
            new Vector3Int(cb.xMax - 1, cb.yMax - 1, 0),
        };
 
        Bounds b = new Bounds(tilemap.GetCellCenterWorld(new Vector3Int(cb.xMin, cb.yMin, zLow)), Vector3.zero);
        foreach (Vector3Int c in corners)
        {
            b.Encapsulate(tilemap.GetCellCenterWorld(new Vector3Int(c.x, c.y, zLow)));
            b.Encapsulate(tilemap.GetCellCenterWorld(new Vector3Int(c.x, c.y, zHigh)));
        }
 
        // Grow from cell centers to cell edges, then add padding.
        Vector3 cell = Vector3.Scale(tilemap.layoutGrid.cellSize, tilemap.transform.lossyScale);
        b.Expand(new Vector3(cell.x, cell.y, 0f));
        b.Expand(new Vector3(boundsPadding.x * 2f, boundsPadding.y * 2f, 0f));
 
        SetBounds(b);
    }
 
    public void SetBounds(Bounds worldBounds)
    {
        mapBounds = worldBounds;
        hasBounds = true;
    }
 
    public void ClearBounds() => hasBounds = false;
 
    private Vector2 Clamp(Vector2 p)
    {
        if (!clampToBounds || !hasBounds || !cam.orthographic) return p;
 
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
 
        p.x = ClampAxis(p.x, mapBounds.min.x, mapBounds.max.x, halfW);
        p.y = ClampAxis(p.y, mapBounds.min.y, mapBounds.max.y, halfH);
        return p;
    }
 
    private static float ClampAxis(float v, float min, float max, float halfView)
    {
        float lo = min + halfView;
        float hi = max - halfView;
        // Map smaller than the view on this axis: center it.
        return lo > hi ? (min + max) * 0.5f : Mathf.Clamp(v, lo, hi);
    }

    public void SnapToTarget()
    {
        if(target == null) return;

        lastTargetPos = target.position;
        lookAheadVelocity = Vector2.zero;
        followVelocity = Vector2.zero;

        Vector2 pos = Clamp((Vector2)target.position + currentLookAhead);
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }

    public void SetTarget(Transform newTarget, bool snap = true)
    {
        target = newTarget;
        if(snap) SnapToTarget();
    }

    private void OnDrawGizmosSelected()
    {
        if(!hasBounds) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(mapBounds.center, mapBounds.size);

    }

}
