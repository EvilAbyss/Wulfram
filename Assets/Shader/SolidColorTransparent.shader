Shader "Solid Color Transparent" {

Properties {
	_Color ("Color", Color) = (1,1,1)
}

SubShader {
	Color [_Color]
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	Fog { Mode Off }

	ZWrite On
	Blend SrcAlpha OneMinusSrcAlpha 

	Pass {}
} 

}
