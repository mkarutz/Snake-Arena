Shader "Unlit/SnakeGenShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			#define MAX_BACKBONE_POINTS 1000

			uniform sampler2D _MainTex;

			uniform float2 _Backbone[MAX_BACKBONE_POINTS];
			uniform int _BackboneLength;
			uniform float _SnakeLength;
			uniform float _SnakeRadius;

			struct vertIn
			{
				float3 params : POSITION;
			};

			struct vertOut
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			float4 calcParametizedPosNorm(float distance)
			{
				if (_BackboneLength == 0)
					return float4(0.0f, 0.0f, 0.0f, 0.0f);

				float accDistance = 0.0f;
				float2 prev = _Backbone[0];
				for (int i = 0; i < _BackboneLength; i++)
				{
					float2 curr = _Backbone[i];
					float2 segmentVector = curr - prev;
					float segmentMagnitude = length(segmentVector);
					if (accDistance + segmentMagnitude >= distance)
					{
						float t = distance - accDistance;

						float2 normalizedSegmentVector = normalize(segmentVector);
						float2 pos = prev + normalizedSegmentVector * t;
						float2 norm = float2(normalizedSegmentVector.y, -normalizedSegmentVector.x);

						// Return position and normal encoded in a float4 (two float2s)
						return float4(pos.x, pos.y, norm.x, norm.y);
					}
					else
					{
						accDistance += segmentMagnitude;
					}
					prev = curr;
				}
				return float4(0.0f, 0.0f, 0.0f, 0.0f);
			}

			// Implementation of the vertex shader
			vertOut vert(vertIn v)
			{
				vertOut o;

				float distance = v.params.x;
				float parity = v.params.y;
				float texh = v.params.z;

				distance *= _SnakeRadius * 2.0f;

				if (distance > _SnakeLength + 1.0f)
				{
					o.vertex = float4(0.0f, 0.0f, 0.0f, 0.0f);
					o.uv = float2(0.0f, 0.0f);
					return o;
				}

				distance = saturate(distance / _SnakeLength) * _SnakeLength;

				float4 posNorm = calcParametizedPosNorm(distance); // Expensive, possibly optimise
				float2 pos = float2(posNorm.x, posNorm.y);
				float2 norm = float2(posNorm.z, posNorm.w);

				float4 localPos = float4(0.0f, 0.0f, 0.0f, 1.0f);

				float fct = saturate((_SnakeLength - distance) / (_SnakeLength * 0.1f));
				if (distance < _SnakeRadius * 2.0f)
				{
					// Head taper
					fct = saturate((distance / (_SnakeRadius * 2.0f)) + 0.15f);
				}

				localPos.xy = pos + (norm * _SnakeRadius * parity * fct);
				localPos.z = distance;

				float2 uv = float2(texh, distance / (_SnakeRadius * 2.0f));

				o.vertex = mul(UNITY_MATRIX_MVP, localPos);
				o.uv = uv;
				
				return o;
			}

			// Implementation of the fragment shader
			fixed4 frag(vertOut v) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, v.uv);
				return col;
			}

			
			ENDCG
		}
	}
}
