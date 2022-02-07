// Inspired by https://www.youtube.com/watch?v=tUdrYpU30Qw

Shader "LightningShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_Color("Lightning Color", Color) = (0,0,0,0)
		_Color2("Lightning Color2", Color) = (1,1,1,1)

		_Scale ("Scale", Vector) = (1,3,0)
		_TimeSeed("TimeSeed", Float) = 0.2
		_Intensity("Intensity", Float) = 1
		_FlashingSpeed("FlashingSpeed", Vector) = (1,2,3,4)
		_PanSpeed("PanSpeed", Vector) = (10,11,12,13)	
		_BeamSize("BeamSize", Float) = 1
		_BeamPower("BeamPower", Float) = 2
		_ScaleXPerLength("ScaleXPerLength", Float) = 1
	}
		SubShader
	{
		Tags { "RenderType" = "Transparent" }
		LOD 100
		ZWrite Off
		ZTest Always
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha


		Pass
		{
			 HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 screenPosition : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _Noise;
		
			float4 _Color;
			float4 _Color2;
		
			float _Speed;

			float4 _Scale ;
			float _ScaleXPerLength = 1;
	
			float _Intensity;
			float4 _FlashingSpeed;
			float4 _PanSpeed;
			float _BeamSize;
			float _BeamPower;

			v2f vert(appdata v)
			{
				v2f o;
			    o.vertex    = TransformObjectToHClip(v.vertex);
				o.screenPosition = ComputeScreenPos(o.vertex);
				o.uv = v.uv; 
				o.uv.y /= 2;
				return o;
			}

			float beam(v2f i,  int _invert, float _flashingSpeed, float _panSpeed)
			{
				float	speed = (_Time.y ) * _panSpeed;

				float scaleY = lerp(0.5,1,1/i.uv.x/_Scale.y);
				scaleY *= lerp(0.5,1,1/(1-i.uv.x)/_Scale.y);

				float uvy = i.uv.y*scaleY-scaleY/4+0.25*_invert;
				float uvx = i.uv.x*_Scale.x/_ScaleXPerLength + speed;
				float2 uv = float2(uvx  ,uvy);			

				float color = tex2D(_MainTex, uv).r/_BeamSize;
				color = pow(color,_BeamPower);
				float flash = tex2D(_Noise,_Time.y*_flashingSpeed).r;

				float l = (1/(scaleY*2))/2;
				float c = 0.25f;
				if ( i.uv.y>(c+l) || i.uv.y<(c-l) ) discard;
		
				color = color * flash;
				return color;
            }

			float4 frag(v2f i) : SV_Target
			{

				float color = 0;

				color += beam(i,-1,_FlashingSpeed.x, _PanSpeed.x);
				color += beam(i,1,_FlashingSpeed.y, _PanSpeed.y);
				color += beam(i,-1,_FlashingSpeed.z, _PanSpeed.z);
				color += beam(i,1,_FlashingSpeed.w, _PanSpeed.w);

				float a = color*_Intensity;
				float4 fragColor = lerp(_Color,_Color2,a);
				
				fragColor.w = a;
				return fragColor;
				
			}
		ENDHLSL
	}
	}
}
