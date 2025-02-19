Shader "Custom/DARK"
{
    Properties
    {
        _DarkColor ("Dark Color", Color) = (0,0,0,1)
        _Intensity ("Intensity", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend DstColor Zero  // Makes it darken the scene
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            fixed4 _DarkColor;
            float _Intensity;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _DarkColor * _Intensity;
            }
            ENDCG
        }
    }
}
