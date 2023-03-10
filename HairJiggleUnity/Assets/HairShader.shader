Shader "Unlit/HairShader"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4x4 _RootBone;
            float3 _CraniumCenter;
            float _CraniumRadius;

            int _VertCount;

            struct appdata
            {
                uint index : SV_VertexID;
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float3 color : COLOR0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float3 color : COLOR0;
            };

            float4 GetVertPos(appdata v)
            {
                if (_CraniumRadius > 0)
                {
                    float3 meshPos = v.color;
                    
                    float3 unskinnedPos = mul(_RootBone, float4(meshPos, 1));
                    //return mul(UNITY_MATRIX_VP, float4(unskinnedPos, 1));

                    float3 unskinnedWorldPos = mul(unity_ObjectToWorld, unskinnedPos);
                    float3 skinnedWorldPos = mul(unity_ObjectToWorld, v.vertex);
                    
                    float distToCranium = length(_CraniumCenter - skinnedWorldPos);
                    //if (distToCranium < _CraniumRadius) // the vert is in the cranium
                    //{
                        float baseDist = length(_CraniumCenter - unskinnedPos);
                        float3 norm = normalize(skinnedWorldPos - _CraniumCenter);
                        float3 newPos = norm * baseDist + _CraniumCenter;

                        return mul(UNITY_MATRIX_VP, float4(newPos, 1));
                    //}

                }
                return UnityObjectToClipPos(v.vertex);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = GetVertPos(v);
                o.uv = v.uv;
                o.normal = v.normal* float3(-1, 1, 1);
                o.color = v.color;
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
