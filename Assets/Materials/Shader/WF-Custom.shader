//This file contains code originally belonging to https://github.com/insominx/unity-wireframe
// and has been modified under the MIT License.
// Wireframe shader based on the the following
// http://developer.download.nvidia.com/SDK/10/direct3d/Source/SolidWireframe/Doc/SolidWireframe.pdf

//Additionally, contains code from https://docs.unity3d.com/Manual/built-in-shader-examples-diffuse-lighting-with-ambient-light.html

Shader "Custom/WireframeLit"
{
    Properties
	{
		_WireThickness ("Wire Thickness", RANGE(0, 800)) = 100
		_WireSmoothness ("Wire Smoothness", RANGE(0, 20)) = 3
		_WireColor ("Wire Color", Color) = (0.0, 1.0, 0.0, 1.0)
		_BaseColor ("Base Color", Color) = (0.0, 0.0, 0.0, 1.0)
		_MaxTriSize ("Max Tri Size", RANGE(0, 200)) = 25
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
	}

    SubShader{
        
        Tags
        {
            "RenderType"="Opaque"
            "LightMode"="ForwardBase"
        }
        Pass
        {
            Name "WireframeInitial"
           
            
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Wireframe.cginc"
            #include "UnityLightingCommon.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 diff : COLOR0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0;

                // the only difference from previous shader:
                // in addition to the diffuse lighting from the main light,
                // add illumination from ambient or light probes
                // ShadeSH9 function from UnityCG.cginc evaluates it,
                // using world space normal
                o.diff.rgb += ShadeSH9(half4(worldNormal,1));
                return o;
            }
            
            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= i.diff * 0.2;
                return col;
            }
            ENDCG
        }
        
    }
}