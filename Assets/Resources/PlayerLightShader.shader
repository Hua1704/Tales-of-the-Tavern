Shader "Unlit/Darkness"
{
    Properties
    {
        _PlayerWorldPos("Player World Position", Vector) = (0,0,0,0)
        _LightRadius("Light Radius", Float) = 1.5
        _SmoothEdge("Smooth Edge", Float) = 1
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            Lighting Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _PlayerWorldPos;
            float _LightRadius;
            float _SmoothEdge;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 worldPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xy;
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float dist = distance(i.worldPos, _PlayerWorldPos.xy);
                float darkness = smoothstep(_LightRadius - _SmoothEdge, _LightRadius, dist);
                return float4(0, 0, 0, darkness); // fully dark outside the circle
            }
            ENDCG
        }
    }
}