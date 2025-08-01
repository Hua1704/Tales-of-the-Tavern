Shader "Unlit/TwoLightsDarkness"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white" {}
        _Color("Overlay Color", Color) = (0,0,0,1)
        _PlayerWorldPos("Player Pos", Vector) = (0,0,0,0)
        _StartWorldPos("Start Pos",  Vector) = (0,0,0,0)
        _LightRadius("Player Radius", Float) = 1.5
        _StartRadius("Start Radius",  Float) = 2.0
        _SmoothEdge("Smooth Edge",  Float) = 1.0
    }

        SubShader
        {
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                fixed4 _Color;
                float4 _PlayerWorldPos, _StartWorldPos;
                float _LightRadius, _StartRadius, _SmoothEdge;

                struct appdata { float4 vertex : POSITION; };
                struct v2f { float4 pos : SV_POSITION; float2 worldXY : TEXCOORD0; };

                v2f vert(appdata v) {
                    v2f o;
                    float4 world = mul(unity_ObjectToWorld, v.vertex);
                    o.worldXY = world.xy;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float dPlayer = distance(i.worldXY, _PlayerWorldPos.xy);
                    float dStart = distance(i.worldXY, _StartWorldPos.xy);

                    // correct order: edge0 < edge1
                    float a1 = smoothstep(_LightRadius - _SmoothEdge, _LightRadius, dPlayer);
                    float a2 = smoothstep(_StartRadius - _SmoothEdge, _StartRadius, dStart);

                    float alpha = a1 * a2; // 0 in holes, 1 outside

                    return fixed4(_Color.rgb, _Color.a * alpha);
                }
                ENDCG
            }
        }
}