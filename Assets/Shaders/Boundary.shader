Shader "Custom/Boundary"
{
    Properties
    {
        _MainTex("Texture", 2D) = "texture" {}

        // Increase this value to have the effect begin at a greater distance
        _StartDistOffset("Float with range", Range(0.0, 1.0)) = 0.0
    }
        SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _StartDistOffset;

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float camDist = length(i.worldPos.xyz - _WorldSpaceCameraPos.xyz);

                return half4(col.xyz, (col.a / camDist) + _StartDistOffset);
            }

            ENDCG
        }
    }
}
