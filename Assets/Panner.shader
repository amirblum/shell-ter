Shader "Sprites/Panner"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        _Offset ("Offset", Float) = 0
        _Speed ("Speed", Float) = 1
        _SwirlSpeed ("Swirl Speed", Float) = 1
        _SwirlAmount ("Swirl Amount", Float) = 0.05
        _FlowMap ("Flow Map", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"

            float _Offset;
            float _Speed;
            float _SwirlSpeed;
            float _SwirlAmount;
            sampler2D _FlowMap;

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 offset;
                offset.x = _Time * _Speed + _Offset;
                offset.y = 0;
                fixed4 sampled = tex2D (_FlowMap, IN.texcoord);
                float2 mixed = lerp(IN.texcoord, sampled.rg, (1 + sin(_Time * _SwirlSpeed)) / 2 * _SwirlAmount);
                float2 uv = (mixed + offset) % 1;
                fixed4 c = SampleSpriteTexture (uv) * IN.color;
                c.a *= clamp(IN.texcoord.y * 5, 0, 1);
                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}