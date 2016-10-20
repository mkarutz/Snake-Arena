// Simplified Additive Particle shader. Differences from regular Additive Particle one:
// - no Tint color
// - no Smooth particle support
// - no AlphaTest
// - no ColorMask

// Credit : https://gist.github.com/keijiro/b91fd4c1a711a0fd3295

Shader "Mobile/Particles/Additive" {
	Properties
	{
		_TintColor("Tint Color", Color) = (0.5, 0.5, 0.5, 0.5)
		_MainTex("Particle Texture", 2D) = "white" {}
	}

		CGINCLUDE

#include "UnityCG.cginc"

		sampler2D _MainTex;
	float4 _MainTex_ST;
	fixed4 _TintColor;

	struct appdata_t
	{
		float4 position : POSITION;
		float4 texcoord : TEXCOORD0;
		fixed4 color : COLOR;
	};

	struct v2f
	{
		float4 position : SV_POSITION;
		float2 texcoord : TEXCOORD0;
		fixed4 color : COLOR;
		UNITY_FOG_COORDS(1)
	};

	v2f vert(appdata_t v)
	{
		v2f o;
		o.position = mul(UNITY_MATRIX_MVP, v.position);
		o.color = v.color;//HUEtoRGB(o.position.z);
		//o.position.z = 0.0f;
		o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
		//o.color = HUEtoRGB(random(mul(unity_ObjectToWorld, v.position).xy));
		

		UNITY_TRANSFER_FOG(o, o.vertex);
		return o;
	}


	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 col = 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord);
	UNITY_APPLY_FOG_COLOR(i.fogCoord, col, (fixed4)0);
	return col;
	}

		ENDCG

		SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

			Blend SrcAlpha One
			Cull Off Lighting Off ZWrite Off Fog{ Mode Off }

			Pass
		{
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_particles
#pragma multi_compile_fog
			ENDCG
		}
	}
}