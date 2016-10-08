Shader "Unlit/SnakeShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom
			
			#include "UnityCG.cginc"

			struct VERT_IN
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct VERT_OUT
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			struct GEOM_OUT
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			VERT_OUT vert(VERT_IN v)
			{
				VERT_OUT o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			[maxvertexcount(4)]
			void geom(lineadj VERT_OUT v[4], uint primID : SV_PrimitiveID, inout TriangleStream<GEO_OUT> triStream)
			{
				float3 up = float3(0.0f, 1.0f, 0.0f);
				float3 look = gEyePosW - gin[0].CenterW;
				look.y = 0.0f; // y-axis aligned, so project to xz-plane
				look = normalize(look);
				float3 right = cross(up, look);

				//
				// Compute triangle strip vertices (quad) in world space.
				//
				float halfWidth = 0.5f*gin[0].SizeW.x;
				float halfHeight = 0.5f*gin[0].SizeW.y;

				float4 v[4];
				v[0] = float4(gin[0].CenterW + halfWidth*right - halfHeight*up, 1.0f);
				v[1] = float4(gin[0].CenterW + halfWidth*right + halfHeight*up, 1.0f);
				v[2] = float4(gin[0].CenterW - halfWidth*right - halfHeight*up, 1.0f);
				v[3] = float4(gin[0].CenterW - halfWidth*right + halfHeight*up, 1.0f);

				//
				// Transform quad vertices to world space and output 
				// them as a triangle strip.
				//
				GeoOut gout;
				[unroll]
				for (int i = 0; i < 4; ++i)
				{
					gout.PosH = mul(v[i], gViewProj);
					gout.PosW = v[i].xyz;
					gout.NormalW = look;
					gout.Tex = gTexC[i];
					gout.PrimID = primID;

					triStream.Append(gout);
				}
			}
			
			fixed4 frag(GEOM_OUT i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
