Shader "SkyArena/Enemies" {
	Properties {
		_Color ("Ship Secondary Color", Color) = (1,0,0)
		_Color1 ("Ship Primary Color", Color) = (1,0,0)
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
		float3 _Color1;

		Input vert( appdata_full v) 
		{

			Input o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			
			float ambientLight = dot( normalize( mul( _Object2World,  v.vertex)), half3(0,1,0)) * 0.5 + 0.5;
			float directLight = dot( normalize( mul( _Object2World,  float4(v.normal.xyz,0) )), half3(0,1,0) ) * 0.5 + 0.5;
			
			 
			o.ramps.y = ambientLight;
			o.ramps.z = 0.5;//v.texcoord.y;
			o.ramps.w = ( o.pos.z / _FogDistance );
			#if IOS_FIX_ON
			o.ramps.w *= 0.05;
			#endif
			o.ramps.w *= o.ramps.w;
			o.ramps.w = saturate( o.ramps.w);

			o.ramps.x = lerp(ambientLight, directLight, o.ramps.z );
			//o.ramps.x = ambientLight;
			//if ( v.color.a > 0.5)
			//{
				//o.col = v.color * 2;
			//}
			//else
			//{
			//	o.col = _Color * 2;
			//}	
			
			//o.col = v.color * v.color.a +  _Color * ( 1 - v.color.a) ;
			//o.col = v.color *   lerp( _Color, half3(1,1,1), v.color.a)  ;
			o.col = lerp( _Color, v.color * _Color1, v.color.a)  ;

			o.ramps.z =abs( pow(1- dot ( normalize(ObjSpaceViewDir(v.vertex)), v.normal.xyz), 3) );
			o.ramps.z *= ambientLight;
			o.ramps.z *= 5;
			o.ramps.z *= v.color.r;
			return o;
		}
		

		
		sampler2D _Gradient;
		sampler2D _GradientSky;
		
		fixed _Fog; 
		
		
		half4 frag ( Input IN ) :COLOR
		{
			half3 light = tex2D( _Gradient, half2(0, IN.ramps.x )).rgb * 2.5h;
			half3 fog = tex2D( _GradientSky, half2(0, IN.ramps.y)).rgb;
			
			//return half4( IN.col ,1);
			
			half3 color = light * IN.col ;//+ fog * IN.ramps.z * _Fog;
			color = lerp( color, fog, IN.ramps.w);
			color = color * 0.8h + IN.col * 0.2h;
			
			color += half3(1,1,1) * IN.ramps.z * fog ;
			
			return half4( color, 1) ;
		}
		
		ENDCG 

	}
}
}