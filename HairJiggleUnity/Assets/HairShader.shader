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

            StructuredBuffer<float3> _HairPosition;

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
                float3 hairWorldPos = _HairPosition[v.index];
                if (length(hairWorldPos) > 0)
                {
                    float3 objPos = mul(unity_WorldToObject, float4(hairWorldPos, 1)); // TODO: collapse this line and the line below
                    return UnityObjectToClipPos(float4(objPos, 1));
                }
                return UnityObjectToClipPos(v.vertex);
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
