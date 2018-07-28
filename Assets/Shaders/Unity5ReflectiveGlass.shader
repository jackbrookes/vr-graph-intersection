Shader "Custom/Unity5ReflectiveGlass" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Shininess("Shininess", Range(0.01, 1)) = 0.078125
		_ReflectColor("Reflection Color", Color) = (1,1,1,0.5)
		_Cube("Reflection Cubemap", Cube) = "black" { TexGen CubeReflect }
	_FresnelPower("FresnelPower", Range(0.01, 1)) = 0.078125
	}
		SubShader{
		Tags{
		"Queue" = "Transparent+1"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
	}
		LOD 200
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf StandardSpecular decal:add nolightmap
		//#pragma surface surf BlinnPhong decal:add nolightmap
		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0
		sampler2D _MainTex;
	samplerCUBE _Cube;
	fixed4 _ReflectColor;
	half _Shininess;
	half _FresnelPower;
	struct Input {
		float2 uv_MainTex;
		float3 worldRefl;
		float3 viewDir;
	};
	half _Glossiness;
	fixed4 _Color;
	void surf(Input IN, inout SurfaceOutputStandardSpecular o) {
		// Albedo comes from a texture tinted by color
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		// Metallic and smoothness come from slider variables
		//o.Metallic = _Metallic;
		o.Smoothness = _Glossiness;
		o.Alpha = c.a;
		//o.Albedo = 0;
		//o.Gloss = 1;
		o.Specular = _Shininess;
		//  Fresnel calc: http://answers.unity3d.com/questions/38138/fresnelrim-reflective-shader.html
		// FRESNEL CALCS float fcbias = 0.20373;
		float fcbias = 0.20373;
		float facing = saturate(1.0 - max(dot(normalize(IN.viewDir.xyz), normalize(o.Normal)), 0.0));
		float refl2Refr = max(fcbias + (1.0 - fcbias) * pow(facing, _FresnelPower), 0);
		fixed4 reflcol = texCUBE(_Cube, IN.worldRefl);
		o.Emission = reflcol.rgb * _ReflectColor.rgb * refl2Refr;
		o.Alpha = reflcol.a * _ReflectColor.a * c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}

