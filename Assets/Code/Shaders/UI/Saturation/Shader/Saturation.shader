
    Shader "UI/Custom/Saturation" {
        Properties {
            _Color ("Main Color", Color) = (1,1,1,1)
            _MainTex ("Tint Color (RGB)", 2D) = "white" {}
            _Saturation ("Saturation", Range(0, 1)) = 0
        }
     
        Category {
     
            // We must be transparent, so other objects are drawn before this one.
            Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque" }
     
     
            SubShader {
             
                // Distortion
                GrabPass {                        
                    Tags { "LightMode" = "Always" }
                }
                Pass {
                    Tags { "LightMode" = "Always" }
                 
                    CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    #pragma fragmentoption ARB_precision_hint_fastest
                    #include "UnityCG.cginc"
                 
                    struct appdata_t {
                        float4 vertex : POSITION;
                        float2 texcoord: TEXCOORD0;
                    };
                 
                    struct v2f {
                        float4 vertex : POSITION;
                        float4 uvgrab : TEXCOORD0;
                        float2 uvbump : TEXCOORD1;
                        float2 uvmain : TEXCOORD2;
                    };
                 
                    float _BumpAmt;
                    float4 _MainTex_ST;
                    float _Saturation;
                 
                    v2f vert (appdata_t v) {
                        v2f o;
                        o.vertex = UnityObjectToClipPos(v.vertex);
                        #if UNITY_UV_STARTS_AT_TOP
                        float scale = -1.0;
                        #else
                        float scale = 1.0;
                        #endif
                        o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
                        o.uvgrab.zw = o.vertex.zw;
                        //o.uvbump = TRANSFORM_TEX( v.texcoord, _BumpMap );
                        o.uvmain = TRANSFORM_TEX( v.texcoord, _MainTex );
                        return o;
                    }
                 
                    fixed4 _Color;
                    sampler2D _GrabTexture;
                    float4 _GrabTexture_TexelSize;
                    //sampler2D _BumpMap;
                    sampler2D _MainTex;
                 
                    half4 frag( v2f i ) : COLOR {
                        // calculate perturbed coordinates
                        //half2 bump = UnpackNormal(tex2D( _BumpMap, i.uvbump )).rg; // we could optimize this by just reading the x  y without reconstructing the Z
                        float2 offset = _BumpAmt * _GrabTexture_TexelSize.xy;
                        i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
                     
                        half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
                        half4 tint = tex2D( _MainTex, i.uvmain ) * _Color;

                        float3 intensity = dot(col.rgb, float3 (0.212, 0.715, 0.072));
                        col.rgb = lerp(col.rgb,intensity , _Saturation);
                     
                        return col * tint;
                    }
                    ENDCG
                }
            }
        }
    }
