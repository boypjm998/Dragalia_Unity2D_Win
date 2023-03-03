Shader "URP/AlphaBlendingWithZWrite"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Main Tex", 2D) = "white" { }
        _AlphaScale ("Alpha Scale", Range(0, 1)) = 1
    }
    SubShader
    {
        //         指定渲染通道使用URP渲染管线                  渲染队列 = 透明度混合        忽略投影 = Ture             渲染类型 = 透明物体
        Tags { "RenderPipeline" = "UniversalRenderPipeline" "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        
        
        // 添加额外的Pass，仅用于渲染到深度缓冲区
        Pass
        {
            // 开启深度写入
            ZWrite On
            // 用于控制Pass不写入任何颜色通道
            ColorMask 0
        }
        

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }
            // 关闭渲染剔除
            Cull Off
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha
            
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
            half _AlphaScale;
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
                
                half3 albedo = texColor.rgb * _Color.rgb;
                
                half3 ambient = _GlossyEnvironmentColor.xyz * albedo;
                
                half3 diffuse = _MainLightColor.rgb * albedo * max(0, dot(worldNormal, worldLightDir));
                
                return half4(ambient + diffuse, texColor.a * _AlphaScale);
            }
            
            ENDHLSL
            
        }
    }
    FallBack "Packages/com.unity.render-pipelines.universal/SimpleLit"
}