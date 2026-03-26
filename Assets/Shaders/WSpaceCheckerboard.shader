Shader "Unlit/WorldSpaceCheckerboard"
{
    Properties
    {
        _Size ("Square Size", Float) = 2.0 // Increase this value for bigger squares
        _ColorA ("Color A", Color) = (1,1,1,1)
        _ColorB ("Color B", Color) = (0.5,0.5,0.5,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
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
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 1. Scale coordinates. 
                // Larger _Size means the 'floor' changes less frequently over distance.
                float3 p = i.worldPos / _Size;
                
                // 2. Standard checkerboard logic: sum the coordinates and check parity
                float3 check = floor(p + 0.00001); // Small offset to prevent flickering at 0
                
                // Triplanar projection logic
                // We use different coordinate pairs depending on which axis the normal faces
                float xCheck = (int(check.y) + int(check.z)) % 2;
                float yCheck = (int(check.x) + int(check.z)) % 2;
                float zCheck = (int(check.x) + int(check.y)) % 2;

                // 3. Weighting based on normal direction
                float3 weights = abs(i.worldNormal);
                weights = pow(weights, 10); 
                weights /= (weights.x + weights.y + weights.z);

                // 4. Blend the results
                float finalCheck = abs(xCheck * weights.x + yCheck * weights.y + zCheck * weights.z);

                // If finalCheck is close to 1, use ColorB, if 0, use ColorA
                return lerp(_ColorA, _ColorB, step(0.5, finalCheck));
            }
            ENDCG
        }
    }
}