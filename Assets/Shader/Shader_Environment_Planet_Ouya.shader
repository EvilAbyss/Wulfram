Shader "SkyArena/Planet_Ouya" {
	Properties {
		//_Color ("Main Color", Color) = (1,1,1)
		_Base ("Base", 2D)= "white" {} 
		_Overlay ("Overlay", 2D)= "white" {} 
		_Fresnel ("Fresnel", 2D)= "white" {} 
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
			float4 col			: TEXCOORD1;
			float2 uv			: TEXCOORD2;
		};
		
		fixed _FogDistance;
		
	
		Input vert( appdata_full v) 
		{
			
			Input o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			
			//float ambientLight = dot( normalize( mul( _Object2World,  v.vertex)), half3(0,1,0)) * 0.5 + 0.5; 
			float ambientLight = dot( v.normal , half3(0,1,0)) * 0.5 + 0.5;
	
			
			o.ramps.x = ambientLight;
			 
			o.ramps.y = ambientLight;
			
			o.ramps.w = ( o.pos.z / _FogDistance );
			#if IOS_FIX_ON
			o.ramps.w *= 0.05;
			#endif
			o.ramps.w *= o.ramps.w;
			o.ramps.w = saturate( o.ramps.w);

			o.ramps.z = 1-dot ( normalize( ObjSpaceViewDir (v.vertex) ), v.normal ) ;
			
			o.col = v.color;
			
			o.uv = v.texcoord.xy;
			
			
			return o;
		}
		

		sampler2D _Base;
		sampler2D _Overlay;
		sampler2D _Fresnel; 
		
		sampler2D _Gradient;
		sampler2D _GradientSky;
		
		
		fixed _Fog;  
		
		
		half4 frag ( Input IN ) :COLOR
		{
			half3 light = tex2D( _Gradient, half2(0, IN.ramps.x )).rgb * 2.5;
			half3 fog = tex2D( _GradientSky, half2(0, IN.ramps.y)).rgb;
			  
			half4 map = tex2D( _Overlay , IN.uv * 8 );  
 
			if ( IN.col.a + (map.a-0.5) > 0.5)
			{ 
				map = tex2D( _Base, IN.uv * 8); 
			}
			 
			
			
			light = light * map;
			
			float fresnel = tex2D( _Fresnel, half2(IN.ramps.z,0) );
			
			half3 color = light * IN.col.rgb ;
			
			
			color = lerp( color, fog, IN.ramps.w );
			color *= (1+ fresnel *2);
			return half4( color  , 1) ;
		}
		
		ENDCG 

	}
}
}
