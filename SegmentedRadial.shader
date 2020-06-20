Shader "Custom/SegmentedRadialFill" {
	Properties {
		[PerRendererData]
		_MainTex ("Texture", 2D) = "white" {
		}
		_Start("Start", Range(0, 360)) = 0
		_Fill("Fill", Range(0, 1)) = 1
		_Segments("Segments", Range(0, 6)) = 3
	}
	
	SubShader {
		Tags {
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"CanUseSpriteAtlas" = "True"
			"PreviewType" = "Plane"
		}
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			sampler2D _MainTex;
			float _Start;
			float _Fill;
			
			struct appdata {
				float4 position : POSITION;
				float4 color : COLOR;
				float2 texCoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 position : SV_POSITION;
				fixed4 color : COLOR;
				float2 texCoord : TEXCOORD0;
			};
			
			v2f vert(appdata IN) {
				v2f OUT;
				OUT.position = UnityObjectToClipPos(IN.position);
				OUT.texCoord = IN.texCoord;
				OUT.color = IN.color;
				return OUT;
			}
			
			fixed4 frag(v2f IN) : SV_Target {
				fixed4 c = tex2D(_MainTex, IN.texCoord) * IN.color;
	 			c.rgb *= c.a;
				
				float startAngle = _Start;
				float endAngle = (1 - _Fill) * 360 + startAngle;
				float offset0 = clamp(0, 360, startAngle + 360);
				float offset360 = clamp(0, 360, endAngle - 360);
				
				float angle = atan2(lerp(-1, 1, IN.texCoord.y), lerp(-1, 1, IN.texCoord.x));
				angle *= -57.2957795;
				angle += 90;
				
				if(angle < 0) angle = 360 + angle;
				if(angle >= startAngle && angle <= endAngle) discard;
				if(angle <= offset360) discard;
				if(angle >= offset0) discard;
				return c;
			}
			ENDCG
		}
	}
}
