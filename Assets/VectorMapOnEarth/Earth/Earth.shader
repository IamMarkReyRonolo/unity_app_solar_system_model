// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Earth"
{
	Properties 
	{
		_AtmosphereColor ("Atmosphere Color", Color) = (0.1, 0.35, 1.0, 1.0)
		_AtmospherePow ("Atmosphere Power", Range(1.5, 8)) = 2
		_AtmosphereMultiply ("Atmosphere Multiply", Range(1, 3)) = 1.5

		_DiffuseTex("Diffuse", 2D) = "white" {}
		
		_CloudAndNightTex("Cloud And Night", 2D) = "black" {}

		_LightDir("Light Dir", Vector) = (-1,0,0,1)

		_CloudMap("Cloud Map", 2D) = "black" {}
		_CloudSpeed("Cloud Speed", Range(-1, 1)) = -0.04
		_CloudAlpha("Cloud Alpha", Range(0, 1)) = 1
		_CloudShadowStrength("Cloud Shadow Strength", Range(0, 1)) = 0.2
		_CloudElevation("Cloud Elevation", Range(0.001, 0.1)) = 0.003
	}

	SubShader 
	{
		ZWrite On
		ZTest LEqual

		pass
		{
		CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert 
			#pragma fragment frag
			
			sampler2D _DiffuseTex;
			sampler2D _CloudAndNightTex;
			sampler2D _CloudMap;

			float4 _AtmosphereColor;
			float _AtmospherePow;
			float _AtmosphereMultiply;
			float _CloudSpeed;
			float _CloudAlpha;
			float _CloudShadowStrength;
			float _CloudElevation;

			float4 _LightDir;

			struct vertexInput 
			{
				float4 pos				: POSITION;
				float3 normal			: NORMAL;
				float2 uv				: TEXCOORD0;
			};

			struct vertexOutput 
			{
				float4 pos			: POSITION;
				float2 uv			: TEXCOORD0;
				half diffuse		: TEXCOORD1;
				half night			: TEXCOORD2;
				half3 atmosphere	: TEXCOORD3;
			};
			
			vertexOutput vert(vertexInput input) 
			{
				vertexOutput output;
				output.pos = UnityObjectToClipPos(input.pos);
				output.uv = input.uv;

				output.diffuse = saturate(dot(_LightDir.xyz, input.normal) * 1.2);
				output.night = 1 - saturate(output.diffuse * 2);

				half3 viewDir = normalize(ObjSpaceViewDir(input.pos));
				half3 normalDir = input.normal;
				output.atmosphere = output.diffuse * _AtmosphereColor.rgb * pow(1 - saturate(dot(viewDir, normalDir)), _AtmospherePow) * _AtmosphereMultiply;

				return output;
			}

			half4 frag(vertexOutput input) : Color
			{
				half3 colorSample = tex2D(_DiffuseTex, input.uv).rgb;

				half3 cloudAndNightSample = tex2D(_CloudAndNightTex, input.uv).rgb;
				half3 nightSample = cloudAndNightSample.ggb;
				half cloudSample = cloudAndNightSample.r;

				half4 result;
				result.rgb = (colorSample + cloudSample) * input.diffuse + nightSample * input.night + input.atmosphere;

				//half3 viewDir = normalize(ObjSpaceViewDir(input.pos));

				//float2 t = float2(_Time[0] * _CloudSpeed, 0);
				//float2 disp = -viewDir * _CloudElevation;

				//half3 cloud = tex2D(_CloudMap, input.diffuse + t - disp);
				//half3 shadows = tex2D(_CloudMap, input.diffuse + t + float2(0.998, 0) + disp) * _CloudShadowStrength;

				//result.rgb = (colorSample + cloudSample ) * input.diffuse + cloud.r + nightSample * input.night + input.atmosphere; // +(cloud.rgb -clamp(shadows.rgb, shadows.rgb, 1 - cloud.rgb));

				result.a = 1;

				return result;
			}

			
				
		ENDCG
		}
	}
	
	Fallback "Diffuse"
}