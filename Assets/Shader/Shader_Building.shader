Shader "SkyArena/Building" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1)
		_Fog ( "Fog Intensity", float) = 0.75
		_FogDistance ( "Fog Distance", float) = 65
		_Gradient ("Gradient", 2D)= "grey" {}
		_GradientSky ("Sky Gradient", 2D)= "grey" {}
	}
SubShader 
{
	Pass 
	{	
		Tags { "Queue"="Geometry-1" "RenderType" = "Opaque" }
		ZWrite on
		Fog { Mode Off }
		CGPROGRAM  

		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile IOS_FIX_OFF IOS_FIX_ON
		#include "UnityCG.cginc" 

	  
		struct Input {
			float4 pos			: SV_POSITION;
			float4 ramps		: TEXCOORD0;
			float3 col			: TEXCOORD1;
		};
		
		fixed _FogDistance;
		float3 _Color;
	
		Input vert( appdata_full v) 
		{
			//v.vertex.xyz -= v.normal * 1 * v.color.r; 
			Input o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			
			float ambientLight = dot( normalize( mul( _Object2World,  v.vertex)), half3(0,1,0)) * 0.5 + 0.5;
			float directLight = dot( normalize( mul( _Object2World,  float4(v.normal.xyz,0) )), half3(0,1,0) ) * 0.5 + 0.5;
			
			 
			o.ramps.y = ambientLight;
			o.ramps.z = v.texcoord.y;
			o.ramps.w = ( o.pos.z / _FogDistance );
			#if IOS_FIX_ON
			o.ramps.w *= 0.05;
			#endif
			o.ramps.w *= o.ramps.w;
			o.ramps.w = saturate( o.ramps.w);

			o.ramps.x = lerp(ambientLight, directLight, max(o.ramps.z, 0.3) );
			
			if( v.color.r < 0.3 )
			{
				o.col = _Color;
			}
			else
			{
				o.col = v.color;
			}
			
			//half3 light = tex2D( _Gradient, half2(0, dot(IN.SphHarDir, half3(0,1,0))* 0.5 + 0.5)   ).rgb * _Fog;
			
			return o;
		}
		

		
		sampler2D _Gradient;
		sampler2D _GradientSky;
		
		fixed _Fog; 
		
		half4 frag ( Input IN ) :COLOR
		{
			half3 light = tex2D( _Gradient, half2(0, IN.ramps.x )).rgb * 1.5;
			half3 fog = tex2D( _GradientSky, half2(0, IN.ramps.y)).rgb ;
			
			
			half3 color = light * IN.col + fog * IN.ramps.z * _Fog;
			color = lerp( color, fog, IN.ramps.w);
			
			return half4( color, 1) ;
		}
		
		ENDCG 

	}
}
}
