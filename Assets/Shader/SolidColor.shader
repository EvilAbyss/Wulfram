Shader "Solid Color" {

Properties {
	_Color ("Color", Color) = (1,1,1)
}

SubShader {
	Color [_Color]
	Fog { Mode Off }
	Pass {}
} 

}
