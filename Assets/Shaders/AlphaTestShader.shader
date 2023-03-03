Shader "URP/AlphaTestWithBothSide"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Main Tex", 2D) = "white" { }
        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        //      指定渲染通道使用URP渲染管线                     渲染队列 = 透明度测试       忽略投影                    渲染类型 = 透明镂空
        Tags { "RenderPipeline" = "UniversalRenderPipeline" "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
        
        Pass
        {
            Tags { "LightMode" = "UniversalForward" }
            
            // 关闭渲染剔除
            Cull Off
            
            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            // 引用URP函数库
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
            
            // 声明纹理
            TEXTURE2D(_MainTex);
            // 声明采样器
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
            half4 _Color;
            float4 _MainTex_ST;
            half _Cutoff;
            CBUFFER_END
            
            struct a2v
            {
                float4 vertex: POSITION;
                float3 normal: NORMAL;
                float4 texcoord: TEXCOORD0;
            };
            
            struct v2f
            {
                float4 pos: SV_POSITION;
                float3 worldNormal: TEXCOORD0;
                float3 worldPos: TEXCOORD1;
                float2 uv: TEXCOORD2;         
            };
            
            v2f vert(a2v v)
            {
                v2f o;
                // 初始化变量
                ZERO_INITIALIZE(v2f, o);
                
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                
                o.worldNormal = TransformObjectToWorldNormal(v.normal);
                
                o.worldPos = TransformObjectToWorld(v.vertex.xyz);
                
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
          
                return o;
            }
            
            half4 frag(v2f i): SV_Target
            {
                half3 worldNormal = normalize(i.worldNormal);
                half3 worldLightDir = normalize(TransformObjectToWorldDir(_MainLightPosition.xyz));
                
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                // 使用clip函数剔除透明像素
                clip(texColor.a - _Cutoff);
                
                half3 albedo = texColor.rgb * _Color.rgb;
                
                half3 ambient = _GlossyEnvironmentColor * albedo;
                
                half3 diffuse = _MainLightColor.rgb * albedo * max(0, dot(worldNormal, worldLightDir));
                
                
                return half4(ambient + diffuse, 1.0);
            }
            
            ENDHLSL
            
        }
    }
    FallBack "Packages/com.unity.render-pipelines.universal/SimpleLit"
}