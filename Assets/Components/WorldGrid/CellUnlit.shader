Shader "Unlit/CellUnlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,.5,.3,.5)
        _BorderSize("BorderSize", Range(0, .2)) = .1
        _BorderSmoothness("BorderSmoothness", Range(0,.2)) = .1
    }
    SubShader
    {
       Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
     LOD 100
     
     ZWrite Off
     Blend SrcAlpha OneMinusSrcAlpha 

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _BorderSize;
            float _BorderSmoothness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                // apply border
                float thickness = .1;
                float borderSize = .1;
                float m = smoothstep(_BorderSize, _BorderSize + _BorderSmoothness, i.uv.x);
                m = min(m, smoothstep(_BorderSize, _BorderSize + _BorderSmoothness, i.uv.y));
                m = min(m, smoothstep(1-_BorderSize, 1-(_BorderSize + _BorderSmoothness), i.uv.y));
                m = min(m, smoothstep(1-_BorderSize, 1-(_BorderSize + _BorderSmoothness), i.uv.x));
                col.a = (1-m) * _Color.a;
                col.rgb = _Color.rgb;

                return col;
            }
            ENDCG
        }
    }
}
