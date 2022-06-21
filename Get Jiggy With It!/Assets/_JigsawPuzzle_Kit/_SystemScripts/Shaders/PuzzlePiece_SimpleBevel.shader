Shader "Sprites/PuzzlePiece_SimpleBevel"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}

		_Color("Tint", Color) = (1,1,1,1)

		_BevelIntens("Bevel Intensivity", Range(0.0, 1.0)) = 0.5
		_BevelOffset("Bevel Offset", Range(-0.03, 0.03)) = 0.02
		_HiglightColor ("Bevel Higlight", Color) = (1,1,1,1)
		_ShadowColor("Bevel Blackout", Color) = (0,0,0,1)
	}


	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		//Blend One OneMinusSrcAlpha
		Blend SrcAlpha OneMinusSrcAlpha


		// draw bevel
		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float2 texcoord  : TEXCOORD0;
			};



			fixed4 _ShadowColor;			
			float _BevelOffset;
			float _BevelIntens;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex + float4(_BevelOffset, -_BevelOffset, 0, 0));
				OUT.texcoord = IN.texcoord;
				return OUT;
			}


			sampler2D _MainTex;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = _ShadowColor;
				c.rgba *= _BevelIntens * tex2D(_MainTex, IN.texcoord).a;
				return c;
			}
		ENDCG
		}


		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float2 texcoord  : TEXCOORD0;
			};
		


			fixed4 _HiglightColor;
			float _BevelOffset;
			float _BevelIntens;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex +float4(-_BevelOffset, _BevelOffset, 0, 0));
				OUT.texcoord = IN.texcoord;
				return OUT;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = _HiglightColor;
				c.rgba *= _BevelIntens * tex2D(_MainTex, IN.texcoord).a;
				return c;
			}
		ENDCG
		}
		

		// draw real sprite
		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;				
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;				
				float2 texcoord  : TEXCOORD0;
			};



			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _MaskTex;	
			float4 _MaskOffset;
			float4 _Color;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, IN.texcoord) * _Color;
				c.rgb *= c.a;
				return c;
			}

		ENDCG
		}
	}
}
