Shader "Unlit/OutLine"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_OutLineFactor("OutLine Factor", Range(0, 0.2)) = 0.1
		_OutLineColor("OutLine Color", Color) = (0,0,0,1)
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
			
			#include "UnityCG.cginc"


			fixed4 _Color;
			
			float4 vert (appdata_base v) : SV_POSITION
			{
				return UnityObjectToClipPos(v.vertex);
			}
			
			fixed4 frag () : SV_Target
			{
				return _Color;
			}
			ENDCG
		}

		Pass
		{
			Cull Front
			
			CGPROGRAM
            #pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"


			float _OutLineFactor;
			fixed4	_OutLineColor;

			float4 vert(appdata_base v) : SV_POSITION
			{
				float4 pos = UnityObjectToClipPos(v.vertex);
				float3 normal = mul((float3x3)UNITY_MATRIX_MVP, v.normal);
				pos.xy += normal.xy* _OutLineFactor;
				return pos;
			}

			fixed4 frag() : SV_Target
			{
				return _OutLineColor;
			}

			ENDCG
		}
	}
}
