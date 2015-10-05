Shader "SkyArena/SkyDome" {
	Properties {
		_Intensity ( "Intensity", float) = 1
		_Power ( "Power", float) = 1
		_AtmoColor ("Atmosphere Color", color)= (1,1,1)
	}
	SubShader {
 
		
		Tags { "Queue"="Geometry-1" "RenderType" = "Transparent" }
		Fog {Mode Off}
		Cull Back 
		CGPROGRAM 
		
		#pragma surface surf Lambert alpha 

		struct Input {
			float3 worldNormal; INTERNAL_DATA 
			float3 viewDir;
		};
		
		fixed _Intensity; 
		fixed _Power;
		fixed3 _AtmoColor;
		
		void surf ( Input IN, inout SurfaceOutput o)
		{
			
			
			//o.Normal =  float3(0,0,1);
			float atmo =  saturate( dot ( normalize(IN.viewDir), IN.worldNormal) *0.5 +0.5);
			
			o.Albedo = float3(0,0,0); 
			//o.Albedo += _AtmoColor * atmo;
			
			o.Emission = _AtmoColor;
			//o.Emission = dif * _Intensity;
			
			
			//o.Albedo = o.Emission;
			o.Alpha = pow( atmo, _Power) * _Intensity;
		}
		
		ENDCG 

	} 
	//FallBack Off//"Diffuse"
}
