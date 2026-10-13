// Unlit projected sprite shadow for URP 2D Renderer (isometric).
// The sprite is re-drawn as a flat silhouette and its vertices are "laid down"
// on the ground: each vertex is pushed along the shadow direction proportionally
// to its height above the pivot (the sprite's feet).
//
// Per-renderer values (_PivotWS, _Mode, _LightPosWS, ...) are set by IsoSpriteShadow.cs
// through a MaterialPropertyBlock.

Shader "Custom/IsoSpriteShadow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite", 2D) = "white" {}
        _ShadowColor ("Shadow Color", Color) = (0.05, 0.05, 0.12, 0.55)
        _MaxLength ("Max Shadow Length (units)", Float) = 6
        _TipFade ("Fade Towards Tip", Range(0, 1)) = 0.5

        // NotEqual (6) = overlapping shadows don't stack darker. Always (8) = they stack.
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Overlap Stencil Comp", Float) = 6
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Tags { "LightMode" = "Universal2D" }

            // Uses only the top stencil bit to avoid clashing with Sprite Masks.
            Stencil
            {
                Ref 128
                ReadMask 128
                WriteMask 128
                Comp [_StencilComp]
                Pass Replace
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                half4  _ShadowColor;
                float  _MaxLength;
                half   _TipFade;
                float  _StencilComp;

                // Set per renderer by IsoSpriteShadow.cs
                float4 _PivotWS;     // xy = pivot (feet) in world space, z = sprite height above pivot
                float  _Mode;        // 0 = sun (directional), 1 = point light
                float4 _SunDir;      // xy = screen-space ground direction of the shadow (iso foreshortened)
                float  _SunLength;   // shadow length per unit of height (1 / tan(elevation))
                float4 _LightPosWS;  // xy = light's ground position in world space
                float  _LightHeight; // light height above ground (world units, screen-up)
                float  _Opacity;
            CBUFFER_END

            struct Attributes
            {
                float3 positionOS : POSITION;
                half4  color      : COLOR;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half4  color      : COLOR;
                float2 uv         : TEXCOORD0;
                half   height01   : TEXCOORD1; // 0 at the feet, 1 at the top of the sprite
            };

            Varyings vert (Attributes v)
            {
                Varyings o;

                float3 posWS = TransformObjectToWorld(v.positionOS);
                float2 pivot = _PivotWS.xy;

                // In iso, screen-up from the feet is height.
                float h = max(0.0, posWS.y - pivot.y);

                // Keep the horizontal width, drop the vertex onto the ground line of the pivot.
                float2 basePos = float2(posWS.x, pivot.y);

                float2 offset;
                if (_Mode < 0.5)
                {
                    // Sun: parallel projection.
                    offset = _SunDir.xy * (h * _SunLength);
                }
                else
                {
                    // Point light: similar triangles from the light's ground point.
                    // ground = feet + (feet - light) * h / (lightHeight - h)
                    float2 away  = pivot - _LightPosWS.xy;
                    float  denom = max(_LightHeight - h, 0.01);
                    offset = away * (h / denom);
                }

                // Clamp so tall objects near a low light don't stretch to infinity.
                float len = length(offset);
                offset *= min(1.0, _MaxLength / max(len, 1e-4));

                posWS.xy = basePos + offset;

                o.positionCS = TransformWorldToHClip(posWS);
                o.color      = v.color;
                o.uv         = v.uv;
                o.height01   = saturate(h / max(_PivotWS.z, 1e-4));
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                half a = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).a * i.color.a;
                clip(a - 0.01); // transparent pixels must not write stencil

                half fade = 1.0 - i.height01 * _TipFade;
                return half4(_ShadowColor.rgb, a * _ShadowColor.a * _Opacity * fade);
            }
            ENDHLSL
        }
    }
}
