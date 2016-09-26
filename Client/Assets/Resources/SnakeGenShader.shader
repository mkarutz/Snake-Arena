Shader "Unlit/SnakeGenShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_BackboneTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma glsl

			#include "UnityCG.cginc"

			// Main snake skin texture
			uniform sampler2D _MainTex;

			// Texture with passed in coords
			uniform sampler2D _BackboneTex;

			// Snake parameters
			uniform int _BackboneTexDim;
			uniform int _BackboneLength;
			uniform float _SnakeLength;
			uniform float _SnakeRadius;

			// Input structure
			struct vertIn
			{
				float3 params : POSITION;
			};

			// VS Output/PS Input
			struct vertOut
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			// Helper to retrieve a backbone point given an index
			// Samples the backbone texture by scaling the index
			float2 getBackbonePoint(uint idx)
			{
				float x = idx % _BackboneTexDim;
				float y = floor(idx / _BackboneTexDim);
				float2 pt = tex2Dlod(_BackboneTex, 
					float4(((x + 0.5f) / (float)_BackboneTexDim), 
						   ((y + 0.5f) / (float)_BackboneTexDim), 0, 0));
				return pt;
			}

			// Calculate a position and normal given a parametized
			// distance from the head of the snake.
			// Encoded as:
			// x,y - Pos
			// z,w - Norm
			float4 calcParametizedPosNorm(float distance)
			{
				if (_BackboneLength == 0)
					return float4(0.0f, 0.0f, 0.0f, 0.0f);

				float accDistance = 0.0f;
				float2 prev = getBackbonePoint(0);
				for (int i = 0; i < _BackboneLength; i++)
				{
					float2 curr = getBackbonePoint(i);
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

				// Get info from current vertex
				float distance = v.params.x;
				float parity = v.params.y;
				float texh = v.params.z;

				// Scale vertex based on distance (larger snakes should not have the same density)
				distance *= _SnakeRadius * 2.0f;

				// Short-circuit unnecessary vertices
				if (distance > _SnakeLength + 1.0f)
				{
					o.vertex = float4(0.0f, 0.0f, 0.0f, 0.0f);
					o.uv = float2(0.0f, 0.0f);
					return o;
				}

				// Ensure distance is capped
				distance = saturate(distance / _SnakeLength) * _SnakeLength;

				float4 posNorm = calcParametizedPosNorm(distance); // Expensive, possibly optimise
				float2 pos = float2(posNorm.x, posNorm.y);
				float2 norm = float2(posNorm.z, posNorm.w);

				float4 localPos = float4(0.0f, 0.0f, 0.0f, 1.0f);

				// Tail taper
				float fct = saturate((_SnakeLength - distance) / (_SnakeLength * 0.1f));
				if (distance < _SnakeRadius * 2.0f)
				{
					// Head taper
					fct = saturate((distance / (_SnakeRadius * 2.0f)) + 0.15f);
				}

				// Calculate world-space position
				localPos.xy = pos + (norm * _SnakeRadius * parity * fct);
				localPos.z = distance; // Increase z to achieve ortho depth effect

				// Calculate texture coordinate
				float2 uv = float2(texh, distance / (_SnakeRadius * 2.0f));

				// Convert to screen space and pass through tex coord
				o.vertex = mul(UNITY_MATRIX_VP, localPos);
				o.uv = uv;
				
				return o;
			}

			// Implementation of the fragment shader
			fixed4 frag(vertOut v) : SV_Target
			{
				return tex2D(_MainTex, v.uv);
			}

			
			ENDCG
		}
	}
}
