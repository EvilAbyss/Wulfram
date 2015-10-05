// Unlit shader. Simplest possible textured shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "SkyArena/FX_Ship_Trail" 
{

	Properties 
	{
		_MainTex ("MainTex", 2D)= "grey" {}
		_ColorTop ("Top Color", Color) = (1,1,1,0.5)
		_ColorBottom ("Bottom Color", Color) = (1,1,1,0.5)
	}
	
SubShader
{
	
	
	Pass 
	{
		//Tags { "Queue" = "Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"} 
		//Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Tags { "Queue"="Geometry-1" "RenderType" = "Opaque" }
		
		//ZWrite off
        //Blend One One 	
        //Fog { Mode Off }
		//Lighting Off 
        
		CGPROGRAM 

		#pragma vertex vert
		#pragma fragment frag
	  	#include "UnityCG.cginc" 
	  	
		struct Input {
			float4 pos: SV_POSITION;
			float2 uv: TEXCOORD0;
			float blendValue: TEXCOORD1;
		};
		
		fixed3 _ColorTop;
		fixed3 _ColorBottom;
		sampler2D _MainTex;
		
		Input vert( appdata_base v)  
		{
			Input o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			o.uv = v.texcoord.xy;
			o.blendValue = v.texcoord.x;
			return o;
		}
		
		half4 frag ( Input IN ) :COLOR
		{
		half3 gradient = lerp( _ColorTop, _ColorBottom, IN.blendValue) * saturate(1- pow(IN.blendValue,2));
		half4 tex = tex2D( _MainTex, IN.uv * half2(0.25,1) - half2(0.75,0) * _Time.w);

		
		return half4( tex.rgb * gradient, 1);
		//	return half4(lerp( _ColorTop, _ColorBottom, IN.blendValue).rgb, 1);
		}
		
		ENDCG 
	}
}
}
