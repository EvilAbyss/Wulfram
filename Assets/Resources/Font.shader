Shader "GUI/Text Shader Z" {
Properties {
	_MainTex ("Font Texture", 2D) = "white" {}
	_Color ("Text Color", Color) = (1,1,1,1)
}

SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Lighting Off Cull Off ZTest Always ZWrite Off Fog { Mode Off }
	Blend SrcAlpha OneMinusSrcAlpha
	Pass {
		ZTest LEqual 
		Color [_Color]
		SetTexture [_MainTex] {
			combine primary, texture * primary
		}
	}
}
}
