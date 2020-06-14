Shader "Custom/Blur And Lerp"
{
	Properties
	{
		_BlurRadius("Blur Radius",float) = 1
		_LastTex("Last Texture", 2D) = "black" {}
		_MainTex("Texture", 2D) = "black" {}
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	struct v2f
	{
		float4 pos:SV_POSITION;
		float2 uv:TEXCOORD0;
	};
	float _BlurRadius;
	sampler2D _LastTex;
	sampler2D _MainTex;
	float4 _MainTex_TexelSize;
	v2f vert (appdata_img v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	}
	fixed4 fragBlur(v2f i) : SV_Target
	{
		fixed4 col = fixed4(0, 0, 0, 0);
		float2 offset = _BlurRadius * _MainTex_TexelSize;
		half gau[9] = {1, 2, 1, 2, 4, 2, 1, 2, 1};
		for (int x = 0; x < 3; ++x)
		{
			for (int y = 0; y < 3; ++y)
			{
				col += tex2D(_MainTex, fixed2(x - 1, y - 1) * offset + i.uv) * gau[x * 3 + y];
			}
		}
		return col / 16;
	}
	fixed4 fragLerp(v2f i) : SV_Target
	{
		half4 lastCol = tex2D(_LastTex, i.uv);
		half4 col = tex2D(_MainTex, i.uv);
		return lerp(lastCol, col, unity_DeltaTime.x * 5);
	} 
	ENDCG
	
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert	
			#pragma fragment fragBlur
			ENDCG
		}
		Pass
		{
			CGPROGRAM
			#pragma vertex vert	
			#pragma fragment fragLerp
			ENDCG
		}
	}
}