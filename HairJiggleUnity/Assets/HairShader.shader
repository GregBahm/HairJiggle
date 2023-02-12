Shader "Unlit/HairShader"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            StructuredBuffer<float3> _HairMomentum;

            struct appdata
            {
                uint index : SV_VertexID;
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
            };

            float4 GetVertex(appdata v)
            {
                float3 momentum = _HairMomentum[v.index];
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
                worldPos += momentum;
                float3 objPos = mul(unity_WorldToObject, float4(worldPos, 1));
                return UnityObjectToClipPos(float4(objPos, 1));
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = GetVertex(v);
                o.uv = v.uv;
                o.normal = v.normal;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return float4(i.normal, 1);
            }
            ENDCG
        }
    }
}
