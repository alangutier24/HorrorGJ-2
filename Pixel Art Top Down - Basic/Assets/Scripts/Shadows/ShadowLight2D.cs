using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// Add to a Point Light 2D (torch, lamp) to make it cast sprite shadows.
/// Place the light where it visually is; 'height' says how far above the ground that is.
[RequireComponent(typeof(Light2D))]
public class ShadowLight2D : MonoBehaviour
{
    public static readonly List<ShadowLight2D> All = new();

    [Tooltip("Height above the ground in world units (screen-up in iso).")]
    [Min(0.01f)] public float height = 1.5f;
    [Range(0f, 1f)] public float shadowStrength = 1f;

    public Light2D Light { get; private set; }

    /// Point on the ground directly below the light.
    public Vector2 GroundPosition => (Vector2)transform.position - Vector2.up * height;

    public float Range => Light.pointLightOuterRadius;

    void Awake() => Light = GetComponent<Light2D>();
    void OnEnable() => All.Add(this);
    void OnDisable() => All.Remove(this);
}
