using System.Collections.Generic;
using UnityEngine;

/// Put this on any object whose SpriteRenderer should cast a shadow.
/// The sprite's pivot must be at its feet (ground contact point).
/// Spawns child SpriteRenderers (one for the sun + up to N for point lights)
/// that draw the same sprite with the IsoSpriteShadow shader.
[RequireComponent(typeof(SpriteRenderer))]
public class IsoSpriteShadow : MonoBehaviour
{
    public Material shadowMaterial;

    [Header("Sources")]
    public bool castSunShadow = true;
    [Min(0)] public int maxPointShadows = 2;

    [Header("Sorting")]
    [Tooltip("Recommended: a dedicated 'Shadows' layer between ground and objects. Empty = same layer as the sprite.")]
    public string shadowSortingLayer = "";
    [Tooltip("Used only when shadowSortingLayer is empty.")]
    public int sortingOrderOffset = -1;

    static readonly int PivotID       = Shader.PropertyToID("_PivotWS");
    static readonly int ModeID        = Shader.PropertyToID("_Mode");
    static readonly int SunDirID      = Shader.PropertyToID("_SunDir");
    static readonly int SunLengthID   = Shader.PropertyToID("_SunLength");
    static readonly int LightPosID    = Shader.PropertyToID("_LightPosWS");
    static readonly int LightHeightID = Shader.PropertyToID("_LightHeight");
    static readonly int OpacityID     = Shader.PropertyToID("_Opacity");

    SpriteRenderer src;
    MaterialPropertyBlock mpb;
    readonly List<SpriteRenderer> pool = new();
    readonly List<(ShadowLight2D light, float weight)> candidates = new();

    void Awake()
    {
        src = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
    }

    void OnDisable()
    {
        foreach (var r in pool) if (r) r.enabled = false;
    }

    void LateUpdate()
    {
        int used = 0;

        if (src.sprite && src.enabled)
        {
            Vector2 pivot = transform.position;
            float spriteHeight = Mathf.Max(0.01f, src.bounds.max.y - pivot.y);

            // Sun
            var sun = ShadowSun.Instance;
            if (castSunShadow && sun && sun.strength > 0.001f)
            {
                var r = Prepare(used++);
                mpb.SetFloat(ModeID, 0f);
                mpb.SetVector(SunDirID, sun.ScreenDir);
                mpb.SetFloat(SunLengthID, sun.LengthPerHeight);
                Apply(r, pivot, spriteHeight, sun.strength);
            }

            // Point lights: pick the strongest nearby ones
            candidates.Clear();
            foreach (var l in ShadowLight2D.All)
            {
                if (!l.Light.enabled) continue;
                float dist = Vector2.Distance(l.GroundPosition, pivot);
                float w = (1f - dist / Mathf.Max(l.Range, 0.01f)) * l.shadowStrength * Mathf.Clamp01(l.Light.intensity);
                if (w > 0.01f) candidates.Add((l, w));
            }
            candidates.Sort((a, b) => b.weight.CompareTo(a.weight));

            for (int i = 0; i < candidates.Count && i < maxPointShadows; i++)
            {
                var (l, w) = candidates[i];
                var r = Prepare(used++);
                mpb.SetFloat(ModeID, 1f);
                mpb.SetVector(LightPosID, l.GroundPosition);
                mpb.SetFloat(LightHeightID, l.height);
                Apply(r, pivot, spriteHeight, w);
            }
        }

        for (int i = used; i < pool.Count; i++) pool[i].enabled = false;
    }

    SpriteRenderer Prepare(int index)
    {
        while (pool.Count <= index)
        {
            var go = new GameObject("Shadow_" + pool.Count);
            go.transform.SetParent(transform, false);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sharedMaterial = shadowMaterial;
            pool.Add(sr);
        }

        var r = pool[index];
        r.enabled = true;
        r.sprite = src.sprite;
        // Mirror via scale instead of flipX so the pivot stays put on every URP version.
        r.transform.localScale = new Vector3(src.flipX ? -1f : 1f, 1f, 1f);

        if (string.IsNullOrEmpty(shadowSortingLayer))
        {
            r.sortingLayerID = src.sortingLayerID;
            r.sortingOrder = src.sortingOrder + sortingOrderOffset;
        }
        else
        {
            r.sortingLayerName = shadowSortingLayer;
            r.sortingOrder = 0;
        }

        r.GetPropertyBlock(mpb);
        return r;
    }

    void Apply(SpriteRenderer r, Vector2 pivot, float spriteHeight, float opacity)
    {
        mpb.SetVector(PivotID, new Vector4(pivot.x, pivot.y, spriteHeight, 0f));
        mpb.SetFloat(OpacityID, opacity);
        r.SetPropertyBlock(mpb);
    }
}
