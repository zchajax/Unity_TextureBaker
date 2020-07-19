Shader "Custom/PerlinNoise"
{
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float2 random2(float2 uv)
            {
                uv %= 10;
                uv = float2(dot(uv,float2(127.1,311.7)), dot(uv, float2(269.5,183.3)));

                return -1.0 + 2.0*frac(sin(uv)*43758.5453123);
            }

            float noise(float2 uv)
            {
    
                float2 ipos = floor(uv);
                float2 fpos = frac(uv);
    
                float2 lb = ipos + float2(0.0, 0.0);
                float2 rb = ipos + float2(1.0, 0.0);
                float2 lt = ipos + float2(0.0, 1.0);
                float2 rt = ipos + float2(1.0, 1.0);
    
                // smoothstep
                float2 u = fpos * fpos * (3.0 - 2.0 * fpos);
    
   	            return lerp(lerp(dot(random2(lb), fpos - float2(0., 0.)),
                               dot(random2(rb), fpos - float2(1., 0.)), u.x),
                            lerp(dot(random2(lt), fpos - float2(0., 1.)),
                               dot(random2(rt), fpos - float2(1., 1.)), u.x), u.y);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
               return noise(i.uv * 10) * 0.5 + 0.5;
            }
            ENDCG
        }
    }
}
