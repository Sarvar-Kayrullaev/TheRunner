Shader "Hyck/Water Waves" {
	Properties {
		_MainTex ("MainTex", 2D) = "white" {}
		_NoiseTex ("NoiseTex", 2D) = "white" {}
		[HDR] _Color ("Color", Vector) = (1,1,1,1)
		[HDR] _DepthColor ("DepthColor", Vector) = (1,1,1,1)
		[HDR] _ShoreColor ("ShoreColor", Vector) = (1,1,1,1)
		_Absorption ("Absorption", Float) = 1
		_ShoreDistance ("ShoreDistance", Float) = 1
		_NoiseSpeed ("NoiseSpeed", Float) = 0.1
		_WaveSpeed ("WaveSpeed", Float) = 0.1
		_Distortion ("Distortion", Float) = 0.1
		_FoamSpeed ("FoamSpeed", Float) = 0.1
		_FoamSize ("FoamSize", Float) = 1
		_FoamDistance ("FoamDistance", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}