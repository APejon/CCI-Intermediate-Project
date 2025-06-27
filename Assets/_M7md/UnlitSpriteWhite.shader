Shader "Custom/UnlitSpriteWhite"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {} // still needed for alpha
        _Color   ("Tint", Color) = (1,1,1,1)         // leave at white
    }

    SubShader
    {
        // Same render settings Unity uses for sprites
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f      { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; };

            sampler2D _MainTex;
            fixed4 _MainTex_ST;
            fixed4 _Color;               // white by default

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv     = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample only the texture’s ALPHA
                fixed a = tex2D(_MainTex, i.uv).a;
                // return solid white (or _Color) with the sprite’s alpha
                return fixed4(_Color.rgb, a * _Color.a);
            }
            ENDCG
        }
    }
}