Shader "Cardboard/Sphere/Show Inside/Transparent" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Opacity("Opacity", Range(0,1)) = 1.0
	}
		SubShader{

		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }

		Cull Front

		CGPROGRAM

#pragma surface surf Lambert alpha vertex:vert
		sampler2D _MainTex;
		float _Opacity;

	struct Input {
		float2 uv_MainTex;
		float4 color : COLOR;
	};


	void vert(inout appdata_full v)
	{
		v.normal.xyz = v.normal * -1;
	}

	void surf(Input IN, inout SurfaceOutput o) {
		fixed3 result = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = result.rgb;
		o.Alpha = tex2D(_MainTex, IN.uv_MainTex).a * _Opacity;
	}

	ENDCG

	}

		Fallback "Diffuse"
}

