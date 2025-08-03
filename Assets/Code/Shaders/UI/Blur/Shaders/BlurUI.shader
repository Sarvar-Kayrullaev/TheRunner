
    Shader "UI/Custom/BlurUI" {
        Properties {
            _Color ("Main Color", Color) = (1,1,1,1)
            //_MainTex ("Tint Color (RGB)", 2D) = "white" {}
            _Distortion ("Distortion", Range(0, 20)) = 1
            _Saturation ("Saturation", Range(0, 1)) = 0
            _Bloom ("Bloom", Range(0, 2)) = 0
            _CornerRadius ("Corner Radius", Vector) = (0.1, 0.1, 0, 0)
            //AspectRatio ("_AspectRatio", Range(0, 10)) = 0
            //_Size ("_Size", Range(0, 2)) = 0
        }
     
        Category {
     
            // We must be transparent, so other objects are drawn before this one.
            Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque" }
     
     
            SubShader {
         
                // Horizontal blur
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
                        float2 uvmain : TEXCOORD2;
                    };

                    float4 _MainTex_ST;
                    sampler2D _MainTex;
                    sampler2D _GrabTexture;
                    float4 _GrabTexture_TexelSize;
                    float _Distortion;
                    float _Bloom;
                    float2 _CornerRadius;
                 
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
                        o.uvmain = TRANSFORM_TEX( v.texcoord, _MainTex );
                        return o;
                    }
                 
                    half4 frag( v2f i ) : COLOR {
                        /// Mask
                        float2 trueSize = float2(1 - _CornerRadius.x, 1 - _CornerRadius.y);
                        float2 d = abs(i.uvmain - float2(0.5, 0.5)) * 2.0 - trueSize;
                        float cornerDistance = length(max(float2(0.0, 0.0), d) / _CornerRadius);
                        half mask = smoothstep(1.0, 1.0 - fwidth(cornerDistance) * 2.0, cornerDistance);
                        /// Mask
                        half4 sum = half4(0,0,0,0);
                        #define GRABPIXEL(weight,kernelx) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x + _GrabTexture_TexelSize.x * kernelx * _Distortion * mask, i.uvgrab.y, i.uvgrab.z, i.uvgrab.w))) * weight
                        
                        sum += GRABPIXEL(0.05*_Bloom, -4.0);
                        sum += GRABPIXEL(0.09*_Bloom, -3.0);
                        sum += GRABPIXEL(0.12*_Bloom, -2.0);
                        sum += GRABPIXEL(0.15*_Bloom, -1.0);
                        sum += GRABPIXEL(0.18*_Bloom,  0.0);
                        sum += GRABPIXEL(0.15*_Bloom, +1.0);
                        sum += GRABPIXEL(0.12*_Bloom, +2.0);
                        sum += GRABPIXEL(0.09*_Bloom, +3.0);
                        sum += GRABPIXEL(0.05*_Bloom, +4.0);
                     
                        return sum;
                    }
                    ENDCG
                }
                // Vertical blur
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
                        float2 uvmain : TEXCOORD2;
                    };

                    float4 _MainTex_ST;
                    sampler2D _MainTex;
                    sampler2D _GrabTexture;
                    float4 _GrabTexture_TexelSize;
                    float _Distortion;
                    float _Bloom;
                    float2 _CornerRadius;
                 
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
                        o.uvmain = TRANSFORM_TEX( v.texcoord, _MainTex );
                        return o;
                    }
                 
                    half4 frag( v2f i ) : COLOR {
                        /// Mask
                        float2 trueSize = float2(1 - _CornerRadius.x, 1 - _CornerRadius.y);
                        float2 d = abs(i.uvmain - float2(0.5, 0.5)) * 2.0 - trueSize;
                        float cornerDistance = length(max(float2(0.0, 0.0), d) / _CornerRadius);
                        half mask = smoothstep(1.0, 1.0 - fwidth(cornerDistance) * 2.0, cornerDistance);
                        /// Mask

                        half4 sum = half4(0,0,0,0);
                        #define GRABPIXEL(weight,kernely) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x, i.uvgrab.y + _GrabTexture_TexelSize.y * kernely * _Distortion * mask, i.uvgrab.z, i.uvgrab.w))) * weight
                        //G(X) = (1/(sqrt(2*PI*deviation*deviation))) * exp(-(x*x / (2*deviation*deviation)))

                        sum += GRABPIXEL(0.05*_Bloom, -4.0);
                        sum += GRABPIXEL(0.09*_Bloom, -3.0);
                        sum += GRABPIXEL(0.12*_Bloom, -2.0);
                        sum += GRABPIXEL(0.15*_Bloom, -1.0);
                        sum += GRABPIXEL(0.18*_Bloom,  0.0);
                        sum += GRABPIXEL(0.15*_Bloom, +1.0);
                        sum += GRABPIXEL(0.12*_Bloom, +2.0);
                        sum += GRABPIXEL(0.09*_Bloom, +3.0);
                        sum += GRABPIXEL(0.05*_Bloom, +4.0);
                     
                        return sum;
                    }
                    ENDCG
                }
             
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
                    float _Saturation;
                    float2 _CornerRadius;
                    float _AspectRatio;
                    float _Size;
                    float4 _MainTex_ST;
                    sampler2D _MainTex;
                 
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
                        o.uvmain = TRANSFORM_TEX( v.texcoord, _MainTex);
                        return o;
                    }
                 
                    fixed4 _Color;
                    sampler2D _GrabTexture;
                    float4 _GrabTexture_TexelSize;
                    //sampler2D _BumpMap;
                 
                    half4 frag( v2f i ) : COLOR {
                        /// Mask
                        float2 trueSize = float2(1 - _CornerRadius.x, 1 - _CornerRadius.y);
                        float2 d = abs(i.uvmain - float2(0.5, 0.5)) * 2.0 - trueSize;
                        float cornerDistance = length(max(float2(0.0, 0.0), d) / _CornerRadius);
                        half mask = smoothstep(1.0, 1.0 - fwidth(cornerDistance) * 2.0, cornerDistance);
                        /// Mask

                        float2 offset = _BumpAmt * _GrabTexture_TexelSize.xy;
                        i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
                        half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
                        col = lerp(col, _Color, _Color.a * mask);

                        float3 intensity = dot(col.rgb, float3 (0.212, 0.715, 0.072));
                        col.rgb = lerp(col.rgb,intensity ,_Saturation * mask);
                        return col;
                    }
                    ENDCG
                }
            }
        }
    }
