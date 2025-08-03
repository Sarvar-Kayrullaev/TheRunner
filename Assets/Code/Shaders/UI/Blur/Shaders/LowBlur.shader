
    Shader "UI/Custom/LowBlur" {
        Properties {
            [MainColor] _Color ("Main Color", Color) = (1,1,1,1)
            [MainTexture] _MainTex ("Tint Color (RGB)", 2D) = "white" {}
            //_BumpMap ("Normalmap", 2D) = "bump" {}
            _Distortion ("Distortion", Range(0, 20)) = 1
            _Saturation ("Saturation", Range(0, 1)) = 0
            _Bloom ("Bloom", Range(0, 2)) = 0
            _GrabTexture ("Grab Texture", 2D) = "" {}
            _BumpAmt ("Bump", Range(0,1)) = 1
        }
     
        Category {
     
            // We must be transparent, so other objects are drawn before this one.
            Tags {"RenderPipeline" = "UniversalPipeline" "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque" }

            // Grab
            SubShader {
         
                // Horizontal blur
                Pass {
                    Tags {"RenderPipeline" = "UniversalPipeline" "LightMode" = "Always" }
                 
                    HLSLPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    #pragma fragmentoption ARB_precision_hint_fastest
                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                    struct Attributes {
                        float4 vertex : POSITION;
                        float2 texcoord: TEXCOORD0;
                    };
                 
                    struct Varyings {
                        float4 vertex : SV_POSITION;
                        float4 uvgrab : TEXCOORD0;
                    };
                 
                    Varyings vert (Attributes v) {
                        Varyings o;
                        o.vertex = TransformObjectToHClip(v.vertex);
                        #if UNITY_UV_STARTS_AT_TOP
                        float scale = -1.0;
                        #else
                        float scale = 1.0;
                        #endif
                        o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
                        o.uvgrab.zw = o.vertex.zw;
                        return o;
                    }

                    SAMPLER(sampler_linear_clamp);
                    TEXTURE2D(_GrabTexture);
                    CBUFFER_START(UnityPerMaterial)

                    sampler2D _GrabTexture_ST;
                    float4 _GrabTexture_TexelSize;
                    float _BumpAmt;
                    sampler2D _MainTex_ST;
                    float _Saturation;
                    float _Distortion;
                    float _Bloom;
                    half4 _Color;
                    
                    CBUFFER_END
                 
                    half4 frag( Varyings i ) : COLOR {
    //                  half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
    //                  return col;
                     
                        half4 sum = half4(0,0,0,0);
                        #define GRABPIXEL(weight, kernelx) SAMPLE_TEXTURE2D(_GrabTexture, sampler_linear_clamp, float2((i.uvgrab.x + _GrabTexture_TexelSize.x * kernelx * _Distortion) / i.uvgrab.w, i.uvgrab.y / i.uvgrab.w)) * weight
                        
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
                    ENDHLSL
                }
                // Vertical blur
                Pass {
                    Tags {"RenderPipeline" = "UniversalPipeline" "LightMode" = "Always" }
                 
                    HLSLPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    #pragma fragmentoption ARB_precision_hint_fastest
                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                 
                    struct Attributes {
                        float4 vertex : POSITION;
                        float2 texcoord: TEXCOORD0;
                    };
                 
                    struct Varyings {
                        float4 vertex : SV_POSITION;
                        float4 uvgrab : TEXCOORD0;
                    };
                 
                    Varyings vert (Attributes v) {
                        Varyings o;
                        o.vertex = TransformObjectToHClip(v.vertex);
                        #if UNITY_UV_STARTS_AT_TOP
                        float scale = -1.0;
                        #else
                        float scale = 1.0;
                        #endif
                        o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
                        o.uvgrab.zw = o.vertex.zw;
                        return o;
                    }

                    SAMPLER(sampler_linear_clamp);
                    TEXTURE2D(_GrabTexture);
                    CBUFFER_START(UnityPerMaterial)

                    sampler2D _GrabTexture_ST;
                    float4 _GrabTexture_TexelSize;
                    float _BumpAmt;
                    sampler2D _MainTex_ST;
                    float _Saturation;
                    float _Distortion;
                    float _Bloom;
                    half4 _Color;
                    
                    CBUFFER_END
                 
                    half4 frag( Varyings i ) : COLOR {
    //                  half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
    //                  return col;
                     
                        half4 sum = half4(0,0,0,0);
                        #define GRABPIXEL(weight, kernelx) SAMPLE_TEXTURE2D(_GrabTexture, sampler_linear_clamp, float2((i.uvgrab.x + _GrabTexture_TexelSize.x * kernelx * _Distortion) / i.uvgrab.w, i.uvgrab.y / i.uvgrab.w)) * weight
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
                    ENDHLSL
                }
             
                // Distortion
                Pass {
                    Tags {"RenderPipeline" = "UniversalPipeline" "LightMode" = "Always" }
                 
                    HLSLPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    #pragma fragmentoption ARB_precision_hint_fastest
                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                 
                    struct Attributes {
                        float4 vertex : POSITION;
                        float2 texcoord: TEXCOORD0;
                    };
                 
                    struct Varyings {
                        float4 vertex : SV_POSITION;
                        float4 uvgrab : TEXCOORD0;
                        float2 uvbump : TEXCOORD1;
                        float2 uvmain : TEXCOORD2;
                    };
                 
                    Varyings vert (Attributes v) {
                        Varyings o;
                        o.vertex = TransformObjectToHClip(v.vertex);
                        #if UNITY_UV_STARTS_AT_TOP
                        float scale = -1.0;
                        #else
                        float scale = 1.0;
                        #endif
                        o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
                        o.uvgrab.zw = o.vertex.zw;
                        //o.uvbump = TRANSFORM_TEX( v.texcoord, _BumpMap );
                        //o.uvmain = TRANSFORM_TEX( v.texcoord, _BaseMap );
                        return o;
                    }

                    SAMPLER(sampler_linear_clamp);
                    TEXTURE2D(_GrabTexture);
                    TEXTURE2D(_MainTex);
                    CBUFFER_START(UnityPerMaterial)

                    sampler2D _GrabTexture_ST;
                    float4 _GrabTexture_TexelSize;
                    float _BumpAmt;
                    sampler2D _MainTex_ST;
                    float _Saturation;
                    float _Distortion;
                    float _Bloom;
                    half4 _Color;
                    
                    CBUFFER_END
                 
                    half4 frag( Varyings i ) : COLOR {
                        // calculate perturbed coordinates
                        //half2 bump = UnpackNormal(tex2D( _BumpMap, i.uvbump )).rg; // we could optimize this by just reading the x  y without reconstructing the Z
                        float2 offset = _BumpAmt * _GrabTexture_TexelSize.xy;
                        i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
                     
                        half4 col = SAMPLE_TEXTURE2D(_GrabTexture, sampler_linear_clamp, i.uvgrab.xy / i.uvgrab.w);
                        half4 tint = SAMPLE_TEXTURE2D( _MainTex, sampler_linear_clamp, i.uvgrab.xy ) * _Color;

                        float3 intensity = dot(col.rgb, float3 (0.212, 0.715, 0.072));
                        col.rgb = lerp(col.rgb,intensity , _Saturation);
                     
                        return col * tint;
                    }
                    ENDHLSL
                }
            }
        }
    }
