

Shader "SkyArena/SkyGradient_Ouya" 
{

	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "black" {}
		_Dot ("Dot", 2D) = "black" {}
	}
	
SubShader
{
	
	
	Pass 
	{
		Tags { "Queue"="Background" "RenderType" = "Background" }
		Fog { Mode Off }
		Lighting Off
		ZWrite Off 
		//ZTest Always 
		
		CGPROGRAM 
		
		#pragma vertex vert
		#pragma fragment frag
	  	#include "UnityCG.cginc" 
	  	
		struct Input {
			float4 pos: SV_POSITION;
			float2 uv: TEXCOORD0; 
			float atmo: TEXCOORD1;
		};
		
		sampler2D _MainTex;
		sampler2D _Dot;
	
		Input vert( appdata_base v) 
		{
			//v.vertex.xyz -= v.normal * 1 * v.color.r; 
			Input o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			o.uv = v.texcoord.xy;

			o.atmo = dot ( normalize( ObjSpaceViewDir (v.vertex) ), v.normal ) ;
			
			o.atmo -= 0.215;
			o.atmo *= 1 - ((o.pos.y+30) / 100 );
			
			return o;
		}
		
		half4 frag ( Input IN ) :COLOR
		{	
			half4 color;
			color.a = 1;
			color.rgb = tex2D( _MainTex, IN.uv).rgb;
			float atmo = tex2D( _Dot, float2(IN.atmo, 0) ).rgb;
			
			color.rgb *= 1 + atmo * 2;
			
			return color;
		}
		
		ENDCG 
	}
}
}
