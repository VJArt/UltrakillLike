Shader "Unlit/WorldSpaceCheckerboard"
{
    Properties
    {
        _Size ("Square Size", Float) = 1.0
        _ColorA ("Color A", Color) = (1,1,1,1)
        _ColorB ("Color B", Color) = (0.5,0.5,0.5,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM // Changed to CGPROGRAM for better compatibility with UnityCG.cginc
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            float _Size;
            fixed4 _ColorA;
            fixed4 _ColorB;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                // Get the position of the pixel in the 3D world
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                // Get the direction the surface is facing
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 1. Scale the coordinates
                float3 p = i.worldPos / _Size;
                
                // 2. Calculate checkers for all 3 sides (Triplanar)
                float3 check = floor(p);
                float x = frac((check.y + check.z) * 0.5) * 2.0;
                float y = frac((check.x + check.z) * 0.5) * 2.0;
                float z = frac((check.x + check.y) * 0.5) * 2.0;

                // 3. Figure out which side of the cube we are looking at
                float3 weights = abs(i.worldNormal);
                // Make the transition sharp
                weights = pow(weights, 10); 
                weights /= (weights.x + weights.y + weights.z);

                // 4. Combine them based on the surface direction
                float finalCheck = x * weights.x + y * weights.y + z * weights.z;

                return lerp(_ColorA, _ColorB, finalCheck);
            }
            ENDCG
        }
    }
}