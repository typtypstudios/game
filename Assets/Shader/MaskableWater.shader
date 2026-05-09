Shader "Shader Graphs/MaskableWaterUnlit"
{
    Properties
    {
        [Toggle(_WORLD_SPACE_UV)]_WORLD_SPACE_UV("World Space UV", Float) = 0
        [Toggle(_PERSPECTIVE)]_PERSPECTIVE("Perspective?", Float) = 0
        _DepthFadeDistance("DepthFadeDistance", Range(0, 300)) = 10
        [HDR]_DeepColor("DeepColor", Color) = (0.03993591, 0.04130278, 0.2981131, 0)
        [HDR]_ShallowColor("ShallowColor", Color) = (0.1256888, 0.1310457, 0.4792453, 0)
        [Toggle(_A_SHORT_HIKE_MODE)]_A_SHORT_HIKE_MODE("A Short Hike Mode?", Float) = 0
        _Steps("Steps", Range(1, 200)) = 10
        _HorizonDistance("HorizonDistance", Range(1, 200)) = 5
        [HDR]_HorizonColor("HorizonColor", Color) = (1.833962, 0.9239017, 1.346016, 0)
        _RefractionSpeed("RefractionSpeed", Float) = 0
        _RefractionScale("RefractionScale", Float) = 10
        _GradientRefractionScale("GradientRefractionScale", Float) = 10
        _RefractionStrength("RefractionStrength", Float) = 0
        _SurfaceFoamDirection("SurfaceFoamDirection", Float) = 0
        _SurfaceFoamSpeed("SurfaceFoamSpeed", Float) = 0
        _SurfaceFoamTiling("SurfaceFoamTiling", Float) = 0
        _SurfaceFoamDistorsion("SurfaceFoamDistorsion", Float) = 0
        [NoScaleOffset]_SurfaceFoamTexture("SurfaceFoamTexture", 2D) = "white" {}
        [HDR]_SurfaceFoamColor("SurfaceFoamColor", Color) = (0.9320754, 0.9320754, 0.9320754, 0)
        [NoScaleOffset]_SecondaryFoamTex("SecondaryFoamTex", 2D) = "white" {}
        [HDR]_SecondaryFoamColor("SecondaryFoamColor", Color) = (0.9320754, 0.9320754, 0.9320754, 0)
        _FoamUVsOffset("FoamUVsOffset", Vector, 2) = (1, 1, 0, 0)
        [NoScaleOffset]_IntersectionFoamTexture("IntersectionFoamTexture", 2D) = "white" {}
        [HDR]_IntersectionFoamColor("IntersectionFoamColor", Color) = (0, 0.593867, 2.491031, 0.6862745)
        _IntersectionFoamDepth("IntersectionFoamDepth", Float) = 1
        _IntersectionFoamFade("IntersectionFoamFade", Float) = 0
        _IntersectionFoamTiling("IntersectionFoamTiling", Float) = 0
        _IntersectionFoamSpeed("IntersectionFoamSpeed", Float) = 0
        _IntersectionFoamCutoff("IntersectionFoamCutoff", Float) = 1
        _IntersectionFoamDirection("IntersectionFoamDirection", Float) = 0
        [NoScaleOffset]_NormalTexture("NormalTexture", 2D) = "white" {}
        _NormalStrength("NormalStrength", Float) = 1
        _NormalSpeed("NormalSpeed", Float) = 0
        _NormalScale("NormalScale", Float) = 0
        _LightingSmoothness("LightingSmoothness", Float) = 0
        _LightingHardness("LightingHardness", Float) = 0
        [HDR]_SpecularColor("SpecularColor", Color) = (1.879245, 1.879245, 1.879245, 0.5333334)
        _WaveSteepness("WaveSteepness", Float) = 0
        _WaveLength("WaveLength", Float) = 0
        _WaveSpeed("WaveSpeed", Float) = 0
        _WaveDirections("WaveDirections", Vector, 4) = (0, 0, 0, 0)
        [NoScaleOffset]_CaveTexture("CaveTexture", 2D) = "white" {}
        [HDR]_CaveColor("CaveColor", Color) = (0.0005352272, 0, 0.2830189, 0)
        [HDR]_CaveSecondaryColor("CaveSecondaryColor", Color) = (0.0005352272, 0, 0.2830189, 0)
        _CaveScale("CaveScale", Vector, 2) = (0, 0, 0, 0)
        _CaveDistortion("CaveDistortion", Float) = 0
        _CaveOffset("CaveOffset", Vector, 2) = (0, 0, 0, 0)
        [Toggle(_REFLECTIONS)]_REFLECTIONS("Reflections?", Float) = 0
        _ReflectionDistortion("ReflectionDistortion", Float) = 1
        _ReflectionBlend("ReflectionBlend", Range(0, 1)) = 1
        [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Geometry"
            "DisableBatching"="False"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalUnlitSubTarget"
        }

        Stencil
        {
            Ref 1
            Comp Equal
            Pass Keep
            ReadMask 255
            WriteMask 255
        }

        Pass
        {
            Name "Universal Forward"
            Tags
            {
                // LightMode: <None>
            }
        
        // Render State
        Cull Off
        Blend One Zero
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ USE_LEGACY_LIGHTMAPS
        #pragma multi_compile _ LIGHTMAP_BICUBIC_SAMPLING
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        #pragma shader_feature_local _ _WORLD_SPACE_UV
        #pragma shader_feature_local _ _A_SHORT_HIKE_MODE
        #pragma shader_feature_local _ _REFLECTIONS
        #pragma shader_feature_local _ _PERSPECTIVE
        
        #if defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_0
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_1
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_2
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE)
            #define KEYWORD_PERMUTATION_3
        #elif defined(_WORLD_SPACE_UV) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_4
        #elif defined(_WORLD_SPACE_UV) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_5
        #elif defined(_WORLD_SPACE_UV) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_6
        #elif defined(_WORLD_SPACE_UV)
            #define KEYWORD_PERMUTATION_7
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_8
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_9
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_10
        #elif defined(_A_SHORT_HIKE_MODE)
            #define KEYWORD_PERMUTATION_11
        #elif defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_12
        #elif defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_13
        #elif defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_14
        #else
            #define KEYWORD_PERMUTATION_15
        #endif
        
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_UNLIT
        #define _FOG_FRAGMENT 1
        #define UNLIT_DEFAULT_DECAL_BLENDING 1
        #define UNLIT_DEFAULT_SSAO 1
        #define REQUIRE_DEPTH_TEXTURE
        #define REQUIRE_OPAQUE_TEXTURE
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Fog.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 ViewSpaceNormal;
             float3 WorldSpaceNormal;
             float3 WorldSpaceTangent;
             float3 WorldSpaceBiTangent;
             float3 WorldSpaceViewDirection;
             float3 ObjectSpacePosition;
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float2 NDCPosition;
             float2 PixelPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 tangentWS : INTERP0;
             float4 texCoord0 : INTERP1;
             float3 positionWS : INTERP2;
             float3 normalWS : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.tangentWS.xyzw = input.tangentWS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.tangentWS = input.tangentWS.xyzw;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _WaveLength;
        float4 _CaveSecondaryColor;
        float _WaveSpeed;
        float _DepthFadeDistance;
        float _IntersectionFoamTiling;
        float4 _SecondaryFoamTex_TexelSize;
        float4 _SecondaryFoamColor;
        float _IntersectionFoamSpeed;
        float _IntersectionFoamDirection;
        float _IntersectionFoamCutoff;
        float4 _ShallowColor;
        float4 _DeepColor;
        float _Steps;
        float _HorizonDistance;
        float4 _HorizonColor;
        float _RefractionSpeed;
        float _RefractionScale;
        float _GradientRefractionScale;
        float _RefractionStrength;
        float _SurfaceFoamDirection;
        float _SurfaceFoamSpeed;
        float _SurfaceFoamTiling;
        float _SurfaceFoamDistorsion;
        float4 _SurfaceFoamTexture_TexelSize;
        float4 _SurfaceFoamColor;
        float _IntersectionFoamDepth;
        float _IntersectionFoamFade;
        float4 _IntersectionFoamTexture_TexelSize;
        float4 _IntersectionFoamColor;
        float _NormalScale;
        float _NormalSpeed;
        float4 _NormalTexture_TexelSize;
        float _NormalStrength;
        float _LightingSmoothness;
        float _LightingHardness;
        float4 _SpecularColor;
        float _WaveSteepness;
        float4 _WaveDirections;
        float4 _CaveTexture_TexelSize;
        float4 _CaveColor;
        float _CaveDistortion;
        float2 _CaveScale;
        float _ReflectionDistortion;
        float _ReflectionBlend;
        float2 _FoamUVsOffset;
        float2 _CaveOffset;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SecondaryFoamTex);
        SAMPLER(sampler_SecondaryFoamTex);
        TEXTURE2D(_SurfaceFoamTexture);
        SAMPLER(sampler_SurfaceFoamTexture);
        TEXTURE2D(_IntersectionFoamTexture);
        SAMPLER(sampler_IntersectionFoamTexture);
        TEXTURE2D(_NormalTexture);
        SAMPLER(sampler_NormalTexture);
        TEXTURE2D(_CaveTexture);
        SAMPLER(sampler_CaveTexture);
        float _TileSize;
        TEXTURE2D(_ReflectionMap);
        SAMPLER(sampler_ReflectionMap);
        float4 _ReflectionMap_TexelSize;
        
        // Graph Includes
        #include_with_pragmas "Assets/Art/UI/AureDevGames/Water Stylized Shader Orto & Perspective Camera/Shader/HLSL/GrestnerWaves.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"
        #include_with_pragmas "Assets/Art/UI/AureDevGames/Water Stylized Shader Orto & Perspective Camera/Shader/HLSL/DistortUV.hlsl"
        #include_with_pragmas "Assets/Art/UI/AureDevGames/Water Stylized Shader Orto & Perspective Camera/Shader/HLSL/MainLighting.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float
        {
        float3 ObjectSpacePosition;
        };
        
        void SG_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float(float _WaveSteepness, float _WaveLength, float _WaveSpeed, float4 _WaveDirections, Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float IN, out float3 PositionWithWaveOffset_1)
        {
        float3 _Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3;
        {
        // Converting Position from Object to AbsoluteWorld via world space
        float3 world;
        world = TransformObjectToWorld(IN.ObjectSpacePosition.xyz);
        _Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3 = GetAbsolutePositionWS(world);
        }
        float _Property_39d578b2ea3d4928a4a2aac434ebc5c8_Out_0_Float = _WaveSteepness;
        float _Property_4e4135449a284017bef47c0e872e7104_Out_0_Float = _WaveLength;
        float _Property_04f74ba2bc544fdcb5e19ff8a137defb_Out_0_Float = _WaveSpeed;
        float4 _Property_90991e6e61204fd4a1ae25860c15dac2_Out_0_Vector4 = _WaveDirections;
        float3 _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3;
        float3 _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Normal_5_Vector3;
        GerstnerWaves_float(_Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3, _Property_39d578b2ea3d4928a4a2aac434ebc5c8_Out_0_Float, _Property_4e4135449a284017bef47c0e872e7104_Out_0_Float, _Property_04f74ba2bc544fdcb5e19ff8a137defb_Out_0_Float, _Property_90991e6e61204fd4a1ae25860c15dac2_Out_0_Vector4, float(1), _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3, _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Normal_5_Vector3);
        float3 _Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3;
        Unity_Add_float3(_Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3, _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3, _Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3);
        float3 _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3;
        _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3 = TransformWorldToObject(_Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3.xyz);
        PositionWithWaveOffset_1 = _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3;
        }
        
        void Unity_Reciprocal_Fast_float(float In, out float Out)
        {
            Out = rcp(In);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
        Out = A * B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        float2 Unity_GradientNoise_LegacyMod_Dir_float(float2 p)
        {
        float x; Hash_LegacyMod_2_1_float(p, x);
        return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }
        
        void Unity_GradientNoise_LegacyMod_float (float2 UV, float3 Scale, out float Out)
        {
        float2 p = UV * Scale.xy;
        float2 ip = floor(p);
        float2 fp = frac(p);
        float d00 = dot(Unity_GradientNoise_LegacyMod_Dir_float(ip), fp);
        float d01 = dot(Unity_GradientNoise_LegacyMod_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
        float d10 = dot(Unity_GradientNoise_LegacyMod_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
        float d11 = dot(Unity_GradientNoise_LegacyMod_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
        fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
        Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_RefractedUV_dfdc6038fe7bdd5479fef69a0fcdb7ab_float
        {
        float2 NDCPosition;
        half4 uv0;
        float3 TimeParameters;
        };
        
        void SG_RefractedUV_dfdc6038fe7bdd5479fef69a0fcdb7ab_float(float _RefractionSpeed, float _RefractionScale, float _GradientRefractionScale, float _RefractionStrength, Bindings_RefractedUV_dfdc6038fe7bdd5479fef69a0fcdb7ab_float IN, out float4 Out_1)
        {
        float4 _ScreenPosition_df4df06bf0bd410e849370e7f915ce20_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
        float _Property_6f2e2eea5f1542be862fc193a4f11e4f_Out_0_Float = _RefractionScale;
        float _Reciprocal_c26c7813ffc1401d8343e505cf3d6b8e_Out_1_Float;
        Unity_Reciprocal_Fast_float(_Property_6f2e2eea5f1542be862fc193a4f11e4f_Out_0_Float, _Reciprocal_c26c7813ffc1401d8343e505cf3d6b8e_Out_1_Float);
        float _Property_8fef6762630f4a338c9146b52f2255f8_Out_0_Float = _RefractionSpeed;
        float _Multiply_44cb146f6607445f8fd249e1966a7b09_Out_2_Float;
        Unity_Multiply_float_float(_Property_8fef6762630f4a338c9146b52f2255f8_Out_0_Float, IN.TimeParameters.x, _Multiply_44cb146f6607445f8fd249e1966a7b09_Out_2_Float);
        float2 _TilingAndOffset_0d3bc9acf8f54350b7aeed81c3a55b94_Out_3_Vector2;
        Unity_TilingAndOffset_float(IN.uv0.xy, (_Reciprocal_c26c7813ffc1401d8343e505cf3d6b8e_Out_1_Float.xx), (_Multiply_44cb146f6607445f8fd249e1966a7b09_Out_2_Float.xx), _TilingAndOffset_0d3bc9acf8f54350b7aeed81c3a55b94_Out_3_Vector2);
        float _Property_2087245bf65447009de8a4234c28ef5d_Out_0_Float = _GradientRefractionScale;
        float _GradientNoise_23f5075437694dcfa3f424e64e24a034_Out_2_Float;
        Unity_GradientNoise_LegacyMod_float(_TilingAndOffset_0d3bc9acf8f54350b7aeed81c3a55b94_Out_3_Vector2, _Property_2087245bf65447009de8a4234c28ef5d_Out_0_Float, _GradientNoise_23f5075437694dcfa3f424e64e24a034_Out_2_Float);
        float _Remap_92f97d94f5a64376842fea592a767e02_Out_3_Float;
        Unity_Remap_float(_GradientNoise_23f5075437694dcfa3f424e64e24a034_Out_2_Float, float2 (0, 1), float2 (-1, 1), _Remap_92f97d94f5a64376842fea592a767e02_Out_3_Float);
        float _Property_eee1994acb674489901dde01c3c3316d_Out_0_Float = _RefractionStrength;
        float _Multiply_b39bd58102fe470c8e1e7f1ce82a040d_Out_2_Float;
        Unity_Multiply_float_float(_Remap_92f97d94f5a64376842fea592a767e02_Out_3_Float, _Property_eee1994acb674489901dde01c3c3316d_Out_0_Float, _Multiply_b39bd58102fe470c8e1e7f1ce82a040d_Out_2_Float);
        float4 _Add_7d79ed8fd86a46e7a268b2fd5dbe2bff_Out_2_Vector4;
        Unity_Add_float4(_ScreenPosition_df4df06bf0bd410e849370e7f915ce20_Out_0_Vector4, (_Multiply_b39bd58102fe470c8e1e7f1ce82a040d_Out_2_Float.xxxx), _Add_7d79ed8fd86a46e7a268b2fd5dbe2bff_Out_2_Vector4);
        Out_1 = _Add_7d79ed8fd86a46e7a268b2fd5dbe2bff_Out_2_Vector4;
        }
        
        void Unity_ViewVectorWorld_float(out float3 Out, float3 WorldSpacePosition)
        {
            Out = _WorldSpaceCameraPos.xyz - GetAbsolutePositionWS(WorldSpacePosition);
            if(!IsPerspectiveProjection())
            {
                Out = GetViewForwardDir() * dot(Out, GetViewForwardDir());
            }
        }
        
        void Unity_Negate_float3(float3 In, out float3 Out)
        {
            Out = -1 * In;
        }
        
        void Unity_Divide_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A / B;
        }
        
        void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
        {
            if (unity_OrthoParams.w == 1.0)
            {
                Out = LinearEyeDepth(ComputeWorldSpacePosition(UV.xy, SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), UNITY_MATRIX_I_VP), UNITY_MATRIX_V);
            }
            else
            {
                Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
            }
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
        Out = A * B;
        }
        
        void Unity_Subtract_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A - B;
        }
        
        void Unity_Negate_float(float In, out float Out)
        {
            Out = -1 * In;
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_Exponential_float(float In, out float Out)
        {
            Out = exp(In);
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        struct Bindings_DepthFadePerspective_3069152b34697f24bb49447c02013d98_float
        {
        float3 WorldSpacePosition;
        float4 ScreenPosition;
        };
        
        void SG_DepthFadePerspective_3069152b34697f24bb49447c02013d98_float(float _DepthFadeDistance, float2 _UV, Bindings_DepthFadePerspective_3069152b34697f24bb49447c02013d98_float IN, out float Linear_2)
        {
        float3 _ViewVector_f8eabe5a287144a6a6d4ef737338317b_Out_0_Vector3;
        Unity_ViewVectorWorld_float(_ViewVector_f8eabe5a287144a6a6d4ef737338317b_Out_0_Vector3, IN.WorldSpacePosition);
        float3 _Negate_d181b51abcbd4784938d7ef4debecdab_Out_1_Vector3;
        Unity_Negate_float3(_ViewVector_f8eabe5a287144a6a6d4ef737338317b_Out_0_Vector3, _Negate_d181b51abcbd4784938d7ef4debecdab_Out_1_Vector3);
        float4 _ScreenPosition_8e5642cf0f54480688de7e9414096c95_Out_0_Vector4 = IN.ScreenPosition;
        float _Split_47aeb1862e224482b604dba19d9665d1_R_1_Float = _ScreenPosition_8e5642cf0f54480688de7e9414096c95_Out_0_Vector4[0];
        float _Split_47aeb1862e224482b604dba19d9665d1_G_2_Float = _ScreenPosition_8e5642cf0f54480688de7e9414096c95_Out_0_Vector4[1];
        float _Split_47aeb1862e224482b604dba19d9665d1_B_3_Float = _ScreenPosition_8e5642cf0f54480688de7e9414096c95_Out_0_Vector4[2];
        float _Split_47aeb1862e224482b604dba19d9665d1_A_4_Float = _ScreenPosition_8e5642cf0f54480688de7e9414096c95_Out_0_Vector4[3];
        float3 _Divide_5a42a764edb04fdfb14c08f2df82448f_Out_2_Vector3;
        Unity_Divide_float3(_Negate_d181b51abcbd4784938d7ef4debecdab_Out_1_Vector3, (_Split_47aeb1862e224482b604dba19d9665d1_A_4_Float.xxx), _Divide_5a42a764edb04fdfb14c08f2df82448f_Out_2_Vector3);
        float2 _Property_d8a51218a3bb4194aa564336969f48bf_Out_0_Vector2 = _UV;
        float _SceneDepth_ae83678fa5f34fe095bf2035c218be22_Out_1_Float;
        Unity_SceneDepth_Eye_float((float4(_Property_d8a51218a3bb4194aa564336969f48bf_Out_0_Vector2, 0.0, 1.0)), _SceneDepth_ae83678fa5f34fe095bf2035c218be22_Out_1_Float);
        float3 _Multiply_36a67ab024a341298aa97e76a995919d_Out_2_Vector3;
        Unity_Multiply_float3_float3(_Divide_5a42a764edb04fdfb14c08f2df82448f_Out_2_Vector3, (_SceneDepth_ae83678fa5f34fe095bf2035c218be22_Out_1_Float.xxx), _Multiply_36a67ab024a341298aa97e76a995919d_Out_2_Vector3);
        float3 _Add_a73b06032a6b424ebc6dcb211cb1d7ec_Out_2_Vector3;
        Unity_Add_float3(_Multiply_36a67ab024a341298aa97e76a995919d_Out_2_Vector3, _WorldSpaceCameraPos, _Add_a73b06032a6b424ebc6dcb211cb1d7ec_Out_2_Vector3);
        float3 _Subtract_56e40e7df9514c6db684a06980c58e55_Out_2_Vector3;
        Unity_Subtract_float3(IN.WorldSpacePosition, _Add_a73b06032a6b424ebc6dcb211cb1d7ec_Out_2_Vector3, _Subtract_56e40e7df9514c6db684a06980c58e55_Out_2_Vector3);
        float _Split_8572215aaf714ae88e25b2db09553f0f_R_1_Float = _Subtract_56e40e7df9514c6db684a06980c58e55_Out_2_Vector3[0];
        float _Split_8572215aaf714ae88e25b2db09553f0f_G_2_Float = _Subtract_56e40e7df9514c6db684a06980c58e55_Out_2_Vector3[1];
        float _Split_8572215aaf714ae88e25b2db09553f0f_B_3_Float = _Subtract_56e40e7df9514c6db684a06980c58e55_Out_2_Vector3[2];
        float _Split_8572215aaf714ae88e25b2db09553f0f_A_4_Float = 0;
        float _Negate_38f930ab7f5148fbaca13a81a57e9720_Out_1_Float;
        Unity_Negate_float(_Split_8572215aaf714ae88e25b2db09553f0f_G_2_Float, _Negate_38f930ab7f5148fbaca13a81a57e9720_Out_1_Float);
        float _Property_58c167a0b3db4df1a74645ae7044c728_Out_0_Float = _DepthFadeDistance;
        float _Divide_1c7f36016a6143bfb91d1e0cefd0e84b_Out_2_Float;
        Unity_Divide_float(_Negate_38f930ab7f5148fbaca13a81a57e9720_Out_1_Float, _Property_58c167a0b3db4df1a74645ae7044c728_Out_0_Float, _Divide_1c7f36016a6143bfb91d1e0cefd0e84b_Out_2_Float);
        float _Exponential_5f2f255a73714085921dbe98b929d51a_Out_1_Float;
        Unity_Exponential_float(_Divide_1c7f36016a6143bfb91d1e0cefd0e84b_Out_2_Float, _Exponential_5f2f255a73714085921dbe98b929d51a_Out_1_Float);
        float _Saturate_4c21ba80863f474f9b1768b39a43d251_Out_1_Float;
        Unity_Saturate_float(_Exponential_5f2f255a73714085921dbe98b929d51a_Out_1_Float, _Saturate_4c21ba80863f474f9b1768b39a43d251_Out_1_Float);
        Linear_2 = _Saturate_4c21ba80863f474f9b1768b39a43d251_Out_1_Float;
        }
        
        void Unity_SceneDepth_Raw_float(float4 UV, out float Out)
        {
            Out = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy);
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        struct Bindings_DepthFade_1d0eb39db86b53245b582a69cdb87443_float
        {
        float4 ScreenPosition;
        };
        
        void SG_DepthFade_1d0eb39db86b53245b582a69cdb87443_float(float _DepthFadeDistance, float2 _UV, Bindings_DepthFade_1d0eb39db86b53245b582a69cdb87443_float IN, out float Linear_2)
        {
        float2 _Property_46bd4502b44f465ab1623fd92c5e4785_Out_0_Vector2 = _UV;
        float _SceneDepth_905c05d0cfa04274a8511269c1782b79_Out_1_Float;
        Unity_SceneDepth_Raw_float((float4(_Property_46bd4502b44f465ab1623fd92c5e4785_Out_0_Vector2, 0.0, 1.0)), _SceneDepth_905c05d0cfa04274a8511269c1782b79_Out_1_Float);
        float _Lerp_35568103c6e249c689ec4fd27f5e4ddd_Out_3_Float;
        Unity_Lerp_float(_ProjectionParams.z, _ProjectionParams.y, _SceneDepth_905c05d0cfa04274a8511269c1782b79_Out_1_Float, _Lerp_35568103c6e249c689ec4fd27f5e4ddd_Out_3_Float);
        float4 _ScreenPosition_e23a8dc3158f4a659ad17c8b8aff1495_Out_0_Vector4 = IN.ScreenPosition;
        float _Split_f8af98270ae4406cab897d102df99496_R_1_Float = _ScreenPosition_e23a8dc3158f4a659ad17c8b8aff1495_Out_0_Vector4[0];
        float _Split_f8af98270ae4406cab897d102df99496_G_2_Float = _ScreenPosition_e23a8dc3158f4a659ad17c8b8aff1495_Out_0_Vector4[1];
        float _Split_f8af98270ae4406cab897d102df99496_B_3_Float = _ScreenPosition_e23a8dc3158f4a659ad17c8b8aff1495_Out_0_Vector4[2];
        float _Split_f8af98270ae4406cab897d102df99496_A_4_Float = _ScreenPosition_e23a8dc3158f4a659ad17c8b8aff1495_Out_0_Vector4[3];
        float _Lerp_a9295eccb1424170a4e481b272012487_Out_3_Float;
        Unity_Lerp_float(_ProjectionParams.z, _ProjectionParams.y, _Split_f8af98270ae4406cab897d102df99496_B_3_Float, _Lerp_a9295eccb1424170a4e481b272012487_Out_3_Float);
        float _Subtract_e132f361175045cfb3fe9a7dca63a082_Out_2_Float;
        Unity_Subtract_float(_Lerp_35568103c6e249c689ec4fd27f5e4ddd_Out_3_Float, _Lerp_a9295eccb1424170a4e481b272012487_Out_3_Float, _Subtract_e132f361175045cfb3fe9a7dca63a082_Out_2_Float);
        float _Property_c3605d6a15074412bba55544d393b337_Out_0_Float = _DepthFadeDistance;
        float _Divide_bbbe2d9f0c3d47ddab512dc7748b6b89_Out_2_Float;
        Unity_Divide_float(_Subtract_e132f361175045cfb3fe9a7dca63a082_Out_2_Float, _Property_c3605d6a15074412bba55544d393b337_Out_0_Float, _Divide_bbbe2d9f0c3d47ddab512dc7748b6b89_Out_2_Float);
        float _Saturate_2a826d10488946e1b5f71424889b8f7a_Out_1_Float;
        Unity_Saturate_float(_Divide_bbbe2d9f0c3d47ddab512dc7748b6b89_Out_2_Float, _Saturate_2a826d10488946e1b5f71424889b8f7a_Out_1_Float);
        float _OneMinus_a6da5fafbee84ff782930c7bb3c4a8de_Out_1_Float;
        Unity_OneMinus_float(_Saturate_2a826d10488946e1b5f71424889b8f7a_Out_1_Float, _OneMinus_a6da5fafbee84ff782930c7bb3c4a8de_Out_1_Float);
        Linear_2 = _OneMinus_a6da5fafbee84ff782930c7bb3c4a8de_Out_1_Float;
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Posterize_float4(float4 In, float4 Steps, out float4 Out)
        {
            Out = floor(In * Steps) / Steps;
        }
        
        struct Bindings_GetDepth_370b72acb6baa764089512ed583870c4_float
        {
        float3 WorldSpacePosition;
        float4 ScreenPosition;
        float2 NDCPosition;
        half4 uv0;
        float3 TimeParameters;
        };
        
        void SG_GetDepth_370b72acb6baa764089512ed583870c4_float(float _DepthFadeDistance, float _Steps, float _RefractionSpeed, float _RefractionScale, float _GradientRefractionScale, float _RefractionStrength, float4 _ShallowColor, float4 _DeepColor, Bindings_GetDepth_370b72acb6baa764089512ed583870c4_float IN, out float4 RefractedUVs_1, out float4 Color_2)
        {
        float _Property_c733aafdfa2a4bb68b7ced01fffc396b_Out_0_Float = _RefractionSpeed;
        float _Property_89b378a1d2144a94b621a6c09e56b37c_Out_0_Float = _RefractionScale;
        float _Property_af7f8a7d74a74ab0a5f78596da12e191_Out_0_Float = _GradientRefractionScale;
        float _Property_61a396eba2ff468993be562ba48b818c_Out_0_Float = _RefractionStrength;
        Bindings_RefractedUV_dfdc6038fe7bdd5479fef69a0fcdb7ab_float _RefractedUV_fcd804a6c7444188a93a683c15c9f99a;
        _RefractedUV_fcd804a6c7444188a93a683c15c9f99a.NDCPosition = IN.NDCPosition;
        _RefractedUV_fcd804a6c7444188a93a683c15c9f99a.uv0 = IN.uv0;
        _RefractedUV_fcd804a6c7444188a93a683c15c9f99a.TimeParameters = IN.TimeParameters;
        half4 _RefractedUV_fcd804a6c7444188a93a683c15c9f99a_Out_1_Vector4;
        SG_RefractedUV_dfdc6038fe7bdd5479fef69a0fcdb7ab_float(_Property_c733aafdfa2a4bb68b7ced01fffc396b_Out_0_Float, _Property_89b378a1d2144a94b621a6c09e56b37c_Out_0_Float, _Property_af7f8a7d74a74ab0a5f78596da12e191_Out_0_Float, _Property_61a396eba2ff468993be562ba48b818c_Out_0_Float, _RefractedUV_fcd804a6c7444188a93a683c15c9f99a, _RefractedUV_fcd804a6c7444188a93a683c15c9f99a_Out_1_Vector4);
        float4 _Property_573520c11ff4493bb507d97f8a6f35c5_Out_0_Vector4 = _ShallowColor;
        float4 _Property_355869e5a92e4d5cbbacd7ea72fca417_Out_0_Vector4 = _DeepColor;
        float _Property_7a320c1c0e864afbb48839a06a61e877_Out_0_Float = _DepthFadeDistance;
        Bindings_DepthFadePerspective_3069152b34697f24bb49447c02013d98_float _DepthFadePerspective_d9affccc6fe74de2a2be6a12c8551576;
        _DepthFadePerspective_d9affccc6fe74de2a2be6a12c8551576.WorldSpacePosition = IN.WorldSpacePosition;
        _DepthFadePerspective_d9affccc6fe74de2a2be6a12c8551576.ScreenPosition = IN.ScreenPosition;
        float _DepthFadePerspective_d9affccc6fe74de2a2be6a12c8551576_Linear_2_Float;
        SG_DepthFadePerspective_3069152b34697f24bb49447c02013d98_float(_Property_7a320c1c0e864afbb48839a06a61e877_Out_0_Float, (_RefractedUV_fcd804a6c7444188a93a683c15c9f99a_Out_1_Vector4.xy), _DepthFadePerspective_d9affccc6fe74de2a2be6a12c8551576, _DepthFadePerspective_d9affccc6fe74de2a2be6a12c8551576_Linear_2_Float);
        Bindings_DepthFade_1d0eb39db86b53245b582a69cdb87443_float _DepthFade_84a135a9b3d547a7996fb768c62e334a;
        _DepthFade_84a135a9b3d547a7996fb768c62e334a.ScreenPosition = IN.ScreenPosition;
        half _DepthFade_84a135a9b3d547a7996fb768c62e334a_Linear_2_Float;
        SG_DepthFade_1d0eb39db86b53245b582a69cdb87443_float(_Property_7a320c1c0e864afbb48839a06a61e877_Out_0_Float, (_RefractedUV_fcd804a6c7444188a93a683c15c9f99a_Out_1_Vector4.xy), _DepthFade_84a135a9b3d547a7996fb768c62e334a, _DepthFade_84a135a9b3d547a7996fb768c62e334a_Linear_2_Float);
        #if defined(_PERSPECTIVE)
        float _Perspective_9b0c09a153cd4283a1dfa9ae48236cfd_Out_0_Float = _DepthFadePerspective_d9affccc6fe74de2a2be6a12c8551576_Linear_2_Float;
        #else
        float _Perspective_9b0c09a153cd4283a1dfa9ae48236cfd_Out_0_Float = _DepthFade_84a135a9b3d547a7996fb768c62e334a_Linear_2_Float;
        #endif
        float4 _Lerp_228b4c34a3434455a17ce3b570ccbf49_Out_3_Vector4;
        Unity_Lerp_float4(_Property_573520c11ff4493bb507d97f8a6f35c5_Out_0_Vector4, _Property_355869e5a92e4d5cbbacd7ea72fca417_Out_0_Vector4, (_Perspective_9b0c09a153cd4283a1dfa9ae48236cfd_Out_0_Float.xxxx), _Lerp_228b4c34a3434455a17ce3b570ccbf49_Out_3_Vector4);
        float _Property_442a941c64b64104be37b457e429de9f_Out_0_Float = _Steps;
        float4 _Posterize_cc9876a22c014323bf597f3e9f25d3a5_Out_2_Vector4;
        Unity_Posterize_float4(_Lerp_228b4c34a3434455a17ce3b570ccbf49_Out_3_Vector4, (_Property_442a941c64b64104be37b457e429de9f_Out_0_Float.xxxx), _Posterize_cc9876a22c014323bf597f3e9f25d3a5_Out_2_Vector4);
        #if defined(_A_SHORT_HIKE_MODE)
        float4 _AShortHikeMode_940dc24d9c7b4f5184729184326be4bc_Out_0_Vector4 = _Posterize_cc9876a22c014323bf597f3e9f25d3a5_Out_2_Vector4;
        #else
        float4 _AShortHikeMode_940dc24d9c7b4f5184729184326be4bc_Out_0_Vector4 = _Lerp_228b4c34a3434455a17ce3b570ccbf49_Out_3_Vector4;
        #endif
        RefractedUVs_1 = _RefractedUV_fcd804a6c7444188a93a683c15c9f99a_Out_1_Vector4;
        Color_2 = _AShortHikeMode_940dc24d9c7b4f5184729184326be4bc_Out_0_Vector4;
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), ViewDir))), Power);
        }
        
        struct Bindings_FresnelCalculation_f346593723353f4488f8a099aef520eb_float
        {
        float3 WorldSpaceNormal;
        float3 ViewSpaceNormal;
        float3 WorldSpaceViewDirection;
        };
        
        void SG_FresnelCalculation_f346593723353f4488f8a099aef520eb_float(float _HorizonDistance, Bindings_FresnelCalculation_f346593723353f4488f8a099aef520eb_float IN, out float FresnelResult_1)
        {
        float _Property_bb1b567c8cd54712be0aa1ab56158c06_Out_0_Float = _HorizonDistance;
        float _FresnelEffect_5cb1ab2db50c4d85aa1315d66acc019b_Out_3_Float;
        Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_bb1b567c8cd54712be0aa1ab56158c06_Out_0_Float, _FresnelEffect_5cb1ab2db50c4d85aa1315d66acc019b_Out_3_Float);
        float3 _Vector3_3df06ab61c94407b9b2bb68b2bd8c970_Out_0_Vector3 = float3(float(0), float(0), float(1));
        float _Property_3a9a52a5e7f84053bce12874f9ecb16e_Out_0_Float = _HorizonDistance;
        float _FresnelEffect_d5defa2cb078451f8600e86b230e0a30_Out_3_Float;
        Unity_FresnelEffect_float(IN.ViewSpaceNormal, _Vector3_3df06ab61c94407b9b2bb68b2bd8c970_Out_0_Vector3, _Property_3a9a52a5e7f84053bce12874f9ecb16e_Out_0_Float, _FresnelEffect_d5defa2cb078451f8600e86b230e0a30_Out_3_Float);
        #if defined(_PERSPECTIVE)
        float _Perspective_e2d93f83b3444bdc9b039aff2c3f109d_Out_0_Float = _FresnelEffect_5cb1ab2db50c4d85aa1315d66acc019b_Out_3_Float;
        #else
        float _Perspective_e2d93f83b3444bdc9b039aff2c3f109d_Out_0_Float = _FresnelEffect_d5defa2cb078451f8600e86b230e0a30_Out_3_Float;
        #endif
        FresnelResult_1 = _Perspective_e2d93f83b3444bdc9b039aff2c3f109d_Out_0_Float;
        }
        
        void Unity_SceneColor_float(float4 UV, out float3 Out)
        {
            Out = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV.xy);
        }
        
        struct Bindings_BlendObjectColor_a02636e967a650d4db669ee613538c46_float
        {
        };
        
        void SG_BlendObjectColor_a02636e967a650d4db669ee613538c46_float(float4 _RefractedUVs, float4 _BaseBlend, Bindings_BlendObjectColor_a02636e967a650d4db669ee613538c46_float IN, out float3 ObjectBlend_1)
        {
        float4 _Property_4f56835f385c4497ad58269d3e3ce6ed_Out_0_Vector4 = _RefractedUVs;
        float3 _SceneColor_b32346b7705c4334a0ac120dcde6901b_Out_1_Vector3;
        Unity_SceneColor_float(_Property_4f56835f385c4497ad58269d3e3ce6ed_Out_0_Vector4, _SceneColor_b32346b7705c4334a0ac120dcde6901b_Out_1_Vector3);
        float4 _Property_8790a77a1a4f4a63b11b3c4719708e2d_Out_0_Vector4 = _BaseBlend;
        float _Split_e4aad686056f4ef79a23e5ee7e845512_R_1_Float = _Property_8790a77a1a4f4a63b11b3c4719708e2d_Out_0_Vector4[0];
        float _Split_e4aad686056f4ef79a23e5ee7e845512_G_2_Float = _Property_8790a77a1a4f4a63b11b3c4719708e2d_Out_0_Vector4[1];
        float _Split_e4aad686056f4ef79a23e5ee7e845512_B_3_Float = _Property_8790a77a1a4f4a63b11b3c4719708e2d_Out_0_Vector4[2];
        float _Split_e4aad686056f4ef79a23e5ee7e845512_A_4_Float = _Property_8790a77a1a4f4a63b11b3c4719708e2d_Out_0_Vector4[3];
        float _OneMinus_dd70c5f5558d4199a84216d1b5da52c2_Out_1_Float;
        Unity_OneMinus_float(_Split_e4aad686056f4ef79a23e5ee7e845512_A_4_Float, _OneMinus_dd70c5f5558d4199a84216d1b5da52c2_Out_1_Float);
        float3 _Multiply_b7686a561f2542a8b83587eee957226c_Out_2_Vector3;
        Unity_Multiply_float3_float3(_SceneColor_b32346b7705c4334a0ac120dcde6901b_Out_1_Vector3, (_OneMinus_dd70c5f5558d4199a84216d1b5da52c2_Out_1_Float.xxx), _Multiply_b7686a561f2542a8b83587eee957226c_Out_2_Vector3);
        ObjectBlend_1 = _Multiply_b7686a561f2542a8b83587eee957226c_Out_2_Vector3;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Negate_float2(float2 In, out float2 Out)
        {
            Out = -1 * In;
        }
        
        struct Bindings_CalculateUVs_60d47073ac03df54f9a15991680de154_float
        {
        float3 WorldSpacePosition;
        half4 uv0;
        };
        
        void SG_CalculateUVs_60d47073ac03df54f9a15991680de154_float(Bindings_CalculateUVs_60d47073ac03df54f9a15991680de154_float IN, out float2 OutUVs_1)
        {
        float _Split_3c90832226cd494ea28785fdc0cd3dc9_R_1_Float = IN.WorldSpacePosition[0];
        float _Split_3c90832226cd494ea28785fdc0cd3dc9_G_2_Float = IN.WorldSpacePosition[1];
        float _Split_3c90832226cd494ea28785fdc0cd3dc9_B_3_Float = IN.WorldSpacePosition[2];
        float _Split_3c90832226cd494ea28785fdc0cd3dc9_A_4_Float = 0;
        float2 _Vector2_536cab5e22a44d6d811a24b0e0e70684_Out_0_Vector2 = float2(_Split_3c90832226cd494ea28785fdc0cd3dc9_R_1_Float, _Split_3c90832226cd494ea28785fdc0cd3dc9_B_3_Float);
        float _Float_d2a1470c5d4f42498583a13546adc89f_Out_0_Float = float(0.1);
        float2 _Multiply_72d6d69cb24149cf8949c1b441fd6553_Out_2_Vector2;
        Unity_Multiply_float2_float2(_Vector2_536cab5e22a44d6d811a24b0e0e70684_Out_0_Vector2, (_Float_d2a1470c5d4f42498583a13546adc89f_Out_0_Float.xx), _Multiply_72d6d69cb24149cf8949c1b441fd6553_Out_2_Vector2);
        float2 _Negate_ea982ed93e8b41c1acbc0c7beddc5cb0_Out_1_Vector2;
        Unity_Negate_float2(_Multiply_72d6d69cb24149cf8949c1b441fd6553_Out_2_Vector2, _Negate_ea982ed93e8b41c1acbc0c7beddc5cb0_Out_1_Vector2);
        float4 _UV_8ca0102c0c1e4dd6b63c964274692cf2_Out_0_Vector4 = IN.uv0;
        #if defined(_WORLD_SPACE_UV)
        float2 _WorldSpaceUV_1110b220d2374503b2e7abe001c6420a_Out_0_Vector2 = _Negate_ea982ed93e8b41c1acbc0c7beddc5cb0_Out_1_Vector2;
        #else
        float2 _WorldSpaceUV_1110b220d2374503b2e7abe001c6420a_Out_0_Vector2 = (_UV_8ca0102c0c1e4dd6b63c964274692cf2_Out_0_Vector4.xy);
        #endif
        OutUVs_1 = _WorldSpaceUV_1110b220d2374503b2e7abe001c6420a_Out_0_Vector2;
        }
        
        void Unity_InvertColors_float4(float4 In, float4 InvertColors, out float4 Out)
        {
        Out = abs(InvertColors - In);
        }
        
        struct Bindings_InvertColors_a5c43e269304bff4a91c002de46388cb_float
        {
        };
        
        void SG_InvertColors_a5c43e269304bff4a91c002de46388cb_float(float4 _InputColor, Bindings_InvertColors_a5c43e269304bff4a91c002de46388cb_float IN, out float4 Output_1)
        {
        float4 _Property_3ba350a2ed3c45a6b7b551a667768c4f_Out_0_Vector4 = _InputColor;
        float4 _InvertColors_04f15eda597141b2a2df5fd6f33e184d_Out_1_Vector4;
        float4 _InvertColors_04f15eda597141b2a2df5fd6f33e184d_InvertColors = float4 (1, 1, 1, 0);
        Unity_InvertColors_float4(_Property_3ba350a2ed3c45a6b7b551a667768c4f_Out_0_Vector4, _InvertColors_04f15eda597141b2a2df5fd6f33e184d_InvertColors, _InvertColors_04f15eda597141b2a2df5fd6f33e184d_Out_1_Vector4);
        Output_1 = _InvertColors_04f15eda597141b2a2df5fd6f33e184d_Out_1_Vector4;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
        Out = A * B;
        }
        
        struct Bindings_Caves_3e5c142028ba07d48bba33c5843ba17e_float
        {
        };
        
        void SG_Caves_3e5c142028ba07d48bba33c5843ba17e_float(float2 _UVs, float2 _CaveScale, float2 _CaveOffset, float _CaveDistortion, UnityTexture2D _CaveTexture, float4 _CaveColor, float _CaveSteps, Bindings_Caves_3e5c142028ba07d48bba33c5843ba17e_float IN, out float4 CaveResult_1)
        {
        float4 _Property_cd4c3ae601304329890ad7ec6b15bd87_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_CaveColor) : _CaveColor;
        float _Split_5dc03030aee348088967c93897290323_R_1_Float = _Property_cd4c3ae601304329890ad7ec6b15bd87_Out_0_Vector4[0];
        float _Split_5dc03030aee348088967c93897290323_G_2_Float = _Property_cd4c3ae601304329890ad7ec6b15bd87_Out_0_Vector4[1];
        float _Split_5dc03030aee348088967c93897290323_B_3_Float = _Property_cd4c3ae601304329890ad7ec6b15bd87_Out_0_Vector4[2];
        float _Split_5dc03030aee348088967c93897290323_A_4_Float = _Property_cd4c3ae601304329890ad7ec6b15bd87_Out_0_Vector4[3];
        UnityTexture2D _Property_3b2b9c24469d4f2ba6d01ced7669fe09_Out_0_Texture2D = _CaveTexture;
        float2 _Property_402263488ae84f86891ad666a842ae5b_Out_0_Vector2 = _UVs;
        float2 _Property_a282ff33e0ce4b5f82f90283e3462360_Out_0_Vector2 = _CaveScale;
        float2 _Property_3e7ad0d7638f4386bf8a554e3efafc78_Out_0_Vector2 = _CaveOffset;
        float2 _TilingAndOffset_7cf0fae9a7aa4e29a98c855468b7ac0d_Out_3_Vector2;
        Unity_TilingAndOffset_float(_Property_402263488ae84f86891ad666a842ae5b_Out_0_Vector2, _Property_a282ff33e0ce4b5f82f90283e3462360_Out_0_Vector2, _Property_3e7ad0d7638f4386bf8a554e3efafc78_Out_0_Vector2, _TilingAndOffset_7cf0fae9a7aa4e29a98c855468b7ac0d_Out_3_Vector2);
        float _Property_a056c657e9bc40398fd6f7d24aaa536a_Out_0_Float = _CaveDistortion;
        float2 _DistortUVCustomFunction_b45cce0f371c47409824f9da6f9978e4_Out_2_Vector2;
        DistortUV_float(_TilingAndOffset_7cf0fae9a7aa4e29a98c855468b7ac0d_Out_3_Vector2, _Property_a056c657e9bc40398fd6f7d24aaa536a_Out_0_Float, _DistortUVCustomFunction_b45cce0f371c47409824f9da6f9978e4_Out_2_Vector2);
        float4 _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_3b2b9c24469d4f2ba6d01ced7669fe09_Out_0_Texture2D.tex, _Property_3b2b9c24469d4f2ba6d01ced7669fe09_Out_0_Texture2D.samplerstate, _Property_3b2b9c24469d4f2ba6d01ced7669fe09_Out_0_Texture2D.GetTransformedUV(_DistortUVCustomFunction_b45cce0f371c47409824f9da6f9978e4_Out_2_Vector2) );
        float _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_R_4_Float = _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_RGBA_0_Vector4.r;
        float _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_G_5_Float = _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_RGBA_0_Vector4.g;
        float _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_B_6_Float = _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_RGBA_0_Vector4.b;
        float _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_A_7_Float = _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_RGBA_0_Vector4.a;
        Bindings_InvertColors_a5c43e269304bff4a91c002de46388cb_float _InvertColors_510d701d607d46aabb64028a0d8d87dc;
        float4 _InvertColors_510d701d607d46aabb64028a0d8d87dc_Output_1_Vector4;
        SG_InvertColors_a5c43e269304bff4a91c002de46388cb_float((_SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_R_4_Float.xxxx), _InvertColors_510d701d607d46aabb64028a0d8d87dc, _InvertColors_510d701d607d46aabb64028a0d8d87dc_Output_1_Vector4);
        float4 _Multiply_2465dbb958a04bb692c3d0cd819466f0_Out_2_Vector4;
        Unity_Multiply_float4_float4((_Split_5dc03030aee348088967c93897290323_A_4_Float.xxxx), _InvertColors_510d701d607d46aabb64028a0d8d87dc_Output_1_Vector4, _Multiply_2465dbb958a04bb692c3d0cd819466f0_Out_2_Vector4);
        CaveResult_1 = _Multiply_2465dbb958a04bb692c3d0cd819466f0_Out_2_Vector4;
        }
        
        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_Reflections_2a83053698b0a9d46a141dd7d36ac2ac_float
        {
        float2 NDCPosition;
        };
        
        void SG_Reflections_2a83053698b0a9d46a141dd7d36ac2ac_float(UnityTexture2D _ReflectionMap, float _ReflectionDistortion, float _ReflectionBlend, Bindings_Reflections_2a83053698b0a9d46a141dd7d36ac2ac_float IN, out float4 ReflexColor_1, out float ReflexResult_2)
        {
        UnityTexture2D _Property_01b9233f96594a05b2c16963f58eb91b_Out_0_Texture2D = _ReflectionMap;
        float4 _ScreenPosition_da0e6619fc3f4e22984f43026a0b5c6f_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
        float _Property_4aeb41f2c0de45b8959feed6af6998ee_Out_0_Float = _ReflectionDistortion;
        float2 _DistortUVCustomFunction_3f714cd044804cf598e93af0be6d96e0_Out_2_Vector2;
        DistortUV_float((_ScreenPosition_da0e6619fc3f4e22984f43026a0b5c6f_Out_0_Vector4.xy), _Property_4aeb41f2c0de45b8959feed6af6998ee_Out_0_Float, _DistortUVCustomFunction_3f714cd044804cf598e93af0be6d96e0_Out_2_Vector2);
        float4 _SampleTexture2D_143e4398349948279eee045e0f9170eb_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_01b9233f96594a05b2c16963f58eb91b_Out_0_Texture2D.tex, _Property_01b9233f96594a05b2c16963f58eb91b_Out_0_Texture2D.samplerstate, _Property_01b9233f96594a05b2c16963f58eb91b_Out_0_Texture2D.GetTransformedUV(_DistortUVCustomFunction_3f714cd044804cf598e93af0be6d96e0_Out_2_Vector2) );
        float _SampleTexture2D_143e4398349948279eee045e0f9170eb_R_4_Float = _SampleTexture2D_143e4398349948279eee045e0f9170eb_RGBA_0_Vector4.r;
        float _SampleTexture2D_143e4398349948279eee045e0f9170eb_G_5_Float = _SampleTexture2D_143e4398349948279eee045e0f9170eb_RGBA_0_Vector4.g;
        float _SampleTexture2D_143e4398349948279eee045e0f9170eb_B_6_Float = _SampleTexture2D_143e4398349948279eee045e0f9170eb_RGBA_0_Vector4.b;
        float _SampleTexture2D_143e4398349948279eee045e0f9170eb_A_7_Float = _SampleTexture2D_143e4398349948279eee045e0f9170eb_RGBA_0_Vector4.a;
        float _Property_7a32419bb4c146e68196c2d5904b5441_Out_0_Float = _ReflectionBlend;
        ReflexColor_1 = _SampleTexture2D_143e4398349948279eee045e0f9170eb_RGBA_0_Vector4;
        ReflexResult_2 = _Property_7a32419bb4c146e68196c2d5904b5441_Out_0_Float;
        }
        
        void Unity_Cosine_float(float In, out float Out)
        {
            Out = cos(In);
        }
        
        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }
        
        void Unity_Normalize_float2(float2 In, out float2 Out)
        {
            Out = normalize(In);
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_PanningUVs_ef286e626cee63841803a024235a644f_float
        {
        float3 TimeParameters;
        };
        
        void SG_PanningUVs_ef286e626cee63841803a024235a644f_float(float _Direction, float _Speed, float2 _UV, float _Tiling, float2 _Offset, Bindings_PanningUVs_ef286e626cee63841803a024235a644f_float IN, out float2 UVOut_1)
        {
        float2 _Property_fa3bd6908dc14afe9e9a8fa8f17ecdb7_Out_0_Vector2 = _UV;
        float _Property_942aaf71f8624c5b9c5b0d1dd04d30f7_Out_0_Float = _Tiling;
        float2 _Multiply_d04caa119e264eca905786c7590f6758_Out_2_Vector2;
        Unity_Multiply_float2_float2(_Property_fa3bd6908dc14afe9e9a8fa8f17ecdb7_Out_0_Vector2, (_Property_942aaf71f8624c5b9c5b0d1dd04d30f7_Out_0_Float.xx), _Multiply_d04caa119e264eca905786c7590f6758_Out_2_Vector2);
        float _Property_c88c4b7bebfd4fc9bf845a4e710e2e20_Out_0_Float = _Direction;
        float _Multiply_b7c33bfe44644595a67b0bcaad29d479_Out_2_Float;
        Unity_Multiply_float_float(_Property_c88c4b7bebfd4fc9bf845a4e710e2e20_Out_0_Float, 2, _Multiply_b7c33bfe44644595a67b0bcaad29d479_Out_2_Float);
        float _Subtract_bd70d912c9e44cb4b775def278ab8299_Out_2_Float;
        Unity_Subtract_float(_Multiply_b7c33bfe44644595a67b0bcaad29d479_Out_2_Float, float(1), _Subtract_bd70d912c9e44cb4b775def278ab8299_Out_2_Float);
        float Constant_469e2f41ab9a469fbded168e7b78fd45 = 3.141593;
        float _Multiply_a2e7e533449a4a72879d25e5c791d0bb_Out_2_Float;
        Unity_Multiply_float_float(_Subtract_bd70d912c9e44cb4b775def278ab8299_Out_2_Float, Constant_469e2f41ab9a469fbded168e7b78fd45, _Multiply_a2e7e533449a4a72879d25e5c791d0bb_Out_2_Float);
        float _Cosine_3e0d75ecc2d14605aad7642b8bcdc461_Out_1_Float;
        Unity_Cosine_float(_Multiply_a2e7e533449a4a72879d25e5c791d0bb_Out_2_Float, _Cosine_3e0d75ecc2d14605aad7642b8bcdc461_Out_1_Float);
        float _Sine_d7fde7b84a36487ca0571ab4b845689a_Out_1_Float;
        Unity_Sine_float(_Multiply_a2e7e533449a4a72879d25e5c791d0bb_Out_2_Float, _Sine_d7fde7b84a36487ca0571ab4b845689a_Out_1_Float);
        float2 _Vector2_5822526fa5384bc2993095be23b1ac5c_Out_0_Vector2 = float2(_Cosine_3e0d75ecc2d14605aad7642b8bcdc461_Out_1_Float, _Sine_d7fde7b84a36487ca0571ab4b845689a_Out_1_Float);
        float2 _Normalize_7ef1b5e1c36845dcb3532f1070eab07d_Out_1_Vector2;
        Unity_Normalize_float2(_Vector2_5822526fa5384bc2993095be23b1ac5c_Out_0_Vector2, _Normalize_7ef1b5e1c36845dcb3532f1070eab07d_Out_1_Vector2);
        float _Property_06fb219b3c4348db8a16781439392559_Out_0_Float = _Speed;
        float _Multiply_0b0996e501624102825e5d908b3a7ded_Out_2_Float;
        Unity_Multiply_float_float(IN.TimeParameters.x, _Property_06fb219b3c4348db8a16781439392559_Out_0_Float, _Multiply_0b0996e501624102825e5d908b3a7ded_Out_2_Float);
        float2 _Multiply_e6e19d2e699747abafa7335b844aa827_Out_2_Vector2;
        Unity_Multiply_float2_float2(_Normalize_7ef1b5e1c36845dcb3532f1070eab07d_Out_1_Vector2, (_Multiply_0b0996e501624102825e5d908b3a7ded_Out_2_Float.xx), _Multiply_e6e19d2e699747abafa7335b844aa827_Out_2_Vector2);
        float2 _Add_0cf695f5db0b477e9565fc781fea6cab_Out_2_Vector2;
        Unity_Add_float2(_Multiply_d04caa119e264eca905786c7590f6758_Out_2_Vector2, _Multiply_e6e19d2e699747abafa7335b844aa827_Out_2_Vector2, _Add_0cf695f5db0b477e9565fc781fea6cab_Out_2_Vector2);
        float2 _Property_0b770ef0e6db42bba96e87ccc9764d0a_Out_0_Vector2 = _Offset;
        float2 _Add_da503712d45c458e95b43c94682472af_Out_2_Vector2;
        Unity_Add_float2(_Add_0cf695f5db0b477e9565fc781fea6cab_Out_2_Vector2, _Property_0b770ef0e6db42bba96e87ccc9764d0a_Out_0_Vector2, _Add_da503712d45c458e95b43c94682472af_Out_2_Vector2);
        UVOut_1 = _Add_da503712d45c458e95b43c94682472af_Out_2_Vector2;
        }
        
        struct Bindings_SecondaryFoam_a707540033b78f44bbef5139f65097d0_float
        {
        float3 TimeParameters;
        };
        
        void SG_SecondaryFoam_a707540033b78f44bbef5139f65097d0_float(float _FoamDirection, float _FoamSpeed, float2 _UVs, float _FoamTiling, float2 _FoamOffset, float _FoamDistortion, UnityTexture2D _FoamTexture, float4 _FoamColor, Bindings_SecondaryFoam_a707540033b78f44bbef5139f65097d0_float IN, out float4 SecondaryFoamResult_1)
        {
        UnityTexture2D _Property_5cd791bd402243e9b2fe160b9ad987dd_Out_0_Texture2D = _FoamTexture;
        float _Property_639af70a388d4c20b6ba8f1a885ccce2_Out_0_Float = _FoamDirection;
        float _Property_bd29fc115c4546dc958cdc06b9143f4a_Out_0_Float = _FoamSpeed;
        float2 _Property_bce6ecfee5474ddab7a5d7f51f6cc08b_Out_0_Vector2 = _UVs;
        float _Property_598525370a5f40929f9000a2467ce4f9_Out_0_Float = _FoamTiling;
        float2 _Property_caa367a036da4cad8383f95ca75df903_Out_0_Vector2 = _FoamOffset;
        Bindings_PanningUVs_ef286e626cee63841803a024235a644f_float _PanningUVs_34a31afa5e2346d5b3aaa70850f3527c;
        _PanningUVs_34a31afa5e2346d5b3aaa70850f3527c.TimeParameters = IN.TimeParameters;
        half2 _PanningUVs_34a31afa5e2346d5b3aaa70850f3527c_UVOut_1_Vector2;
        SG_PanningUVs_ef286e626cee63841803a024235a644f_float(_Property_639af70a388d4c20b6ba8f1a885ccce2_Out_0_Float, _Property_bd29fc115c4546dc958cdc06b9143f4a_Out_0_Float, _Property_bce6ecfee5474ddab7a5d7f51f6cc08b_Out_0_Vector2, _Property_598525370a5f40929f9000a2467ce4f9_Out_0_Float, _Property_caa367a036da4cad8383f95ca75df903_Out_0_Vector2, _PanningUVs_34a31afa5e2346d5b3aaa70850f3527c, _PanningUVs_34a31afa5e2346d5b3aaa70850f3527c_UVOut_1_Vector2);
        float _Property_93624abbebe64bc6a23ac0fcc02ec55d_Out_0_Float = _FoamDistortion;
        float2 _DistortUVCustomFunction_40d46d29f5404679973fed10fa7b54ff_Out_2_Vector2;
        DistortUV_float(_PanningUVs_34a31afa5e2346d5b3aaa70850f3527c_UVOut_1_Vector2, _Property_93624abbebe64bc6a23ac0fcc02ec55d_Out_0_Float, _DistortUVCustomFunction_40d46d29f5404679973fed10fa7b54ff_Out_2_Vector2);
        float4 _SampleTexture2D_1a216311884a4e8683fd5938622d3402_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_5cd791bd402243e9b2fe160b9ad987dd_Out_0_Texture2D.tex, _Property_5cd791bd402243e9b2fe160b9ad987dd_Out_0_Texture2D.samplerstate, _Property_5cd791bd402243e9b2fe160b9ad987dd_Out_0_Texture2D.GetTransformedUV(_DistortUVCustomFunction_40d46d29f5404679973fed10fa7b54ff_Out_2_Vector2) );
        float _SampleTexture2D_1a216311884a4e8683fd5938622d3402_R_4_Float = _SampleTexture2D_1a216311884a4e8683fd5938622d3402_RGBA_0_Vector4.r;
        float _SampleTexture2D_1a216311884a4e8683fd5938622d3402_G_5_Float = _SampleTexture2D_1a216311884a4e8683fd5938622d3402_RGBA_0_Vector4.g;
        float _SampleTexture2D_1a216311884a4e8683fd5938622d3402_B_6_Float = _SampleTexture2D_1a216311884a4e8683fd5938622d3402_RGBA_0_Vector4.b;
        float _SampleTexture2D_1a216311884a4e8683fd5938622d3402_A_7_Float = _SampleTexture2D_1a216311884a4e8683fd5938622d3402_RGBA_0_Vector4.a;
        float4 _Property_236d6b687b6444c0b1db6e064076ddf4_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_FoamColor) : _FoamColor;
        float _Split_29237e4769584ccab6c7fad57c825ede_R_1_Float = _Property_236d6b687b6444c0b1db6e064076ddf4_Out_0_Vector4[0];
        float _Split_29237e4769584ccab6c7fad57c825ede_G_2_Float = _Property_236d6b687b6444c0b1db6e064076ddf4_Out_0_Vector4[1];
        float _Split_29237e4769584ccab6c7fad57c825ede_B_3_Float = _Property_236d6b687b6444c0b1db6e064076ddf4_Out_0_Vector4[2];
        float _Split_29237e4769584ccab6c7fad57c825ede_A_4_Float = _Property_236d6b687b6444c0b1db6e064076ddf4_Out_0_Vector4[3];
        float _Multiply_7e4e840580f44d6d816c3daffe008e63_Out_2_Float;
        Unity_Multiply_float_float(_SampleTexture2D_1a216311884a4e8683fd5938622d3402_R_4_Float, _Split_29237e4769584ccab6c7fad57c825ede_A_4_Float, _Multiply_7e4e840580f44d6d816c3daffe008e63_Out_2_Float);
        SecondaryFoamResult_1 = (_Multiply_7e4e840580f44d6d816c3daffe008e63_Out_2_Float.xxxx);
        }
        
        struct Bindings_PrimaryFoam_f41bb18b12540894cadcf85fcc438801_float
        {
        float3 TimeParameters;
        };
        
        void SG_PrimaryFoam_f41bb18b12540894cadcf85fcc438801_float(float _FoamDirection, float _FoamSpeed, float _FoamTiling, float _FoamDistortion, float2 _UVS, UnityTexture2D _FoamTexture, float4 _FoamColor, Bindings_PrimaryFoam_f41bb18b12540894cadcf85fcc438801_float IN, out float4 FoamResult_1)
        {
        UnityTexture2D _Property_f3f9f2f74aba4fbf9a3ce681a0f63fb9_Out_0_Texture2D = _FoamTexture;
        float _Property_da04d3c39d5743c58c63df85172de1d9_Out_0_Float = _FoamDirection;
        float _Property_d2db43137223470fb028d2186633912f_Out_0_Float = _FoamSpeed;
        float2 _Property_65972f26cc794487af7180bcb97a89bc_Out_0_Vector2 = _UVS;
        float _Property_f3b4abaec7fc41ff9898ad9f70756e98_Out_0_Float = _FoamTiling;
        Bindings_PanningUVs_ef286e626cee63841803a024235a644f_float _PanningUVs_aaf741e14dc140e7b99b861bdf19f098;
        _PanningUVs_aaf741e14dc140e7b99b861bdf19f098.TimeParameters = IN.TimeParameters;
        half2 _PanningUVs_aaf741e14dc140e7b99b861bdf19f098_UVOut_1_Vector2;
        SG_PanningUVs_ef286e626cee63841803a024235a644f_float(_Property_da04d3c39d5743c58c63df85172de1d9_Out_0_Float, _Property_d2db43137223470fb028d2186633912f_Out_0_Float, _Property_65972f26cc794487af7180bcb97a89bc_Out_0_Vector2, _Property_f3b4abaec7fc41ff9898ad9f70756e98_Out_0_Float, half2 (0, 0), _PanningUVs_aaf741e14dc140e7b99b861bdf19f098, _PanningUVs_aaf741e14dc140e7b99b861bdf19f098_UVOut_1_Vector2);
        float _Property_e4d9b3d9ed934e7dab9f09af0b6e6b6e_Out_0_Float = _FoamDistortion;
        float2 _DistortUVCustomFunction_0552020a4ba34f43a99711cb8a336320_Out_2_Vector2;
        DistortUV_float(_PanningUVs_aaf741e14dc140e7b99b861bdf19f098_UVOut_1_Vector2, _Property_e4d9b3d9ed934e7dab9f09af0b6e6b6e_Out_0_Float, _DistortUVCustomFunction_0552020a4ba34f43a99711cb8a336320_Out_2_Vector2);
        float4 _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_f3f9f2f74aba4fbf9a3ce681a0f63fb9_Out_0_Texture2D.tex, _Property_f3f9f2f74aba4fbf9a3ce681a0f63fb9_Out_0_Texture2D.samplerstate, _Property_f3f9f2f74aba4fbf9a3ce681a0f63fb9_Out_0_Texture2D.GetTransformedUV(_DistortUVCustomFunction_0552020a4ba34f43a99711cb8a336320_Out_2_Vector2) );
        float _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_R_4_Float = _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_RGBA_0_Vector4.r;
        float _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_G_5_Float = _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_RGBA_0_Vector4.g;
        float _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_B_6_Float = _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_RGBA_0_Vector4.b;
        float _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_A_7_Float = _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_RGBA_0_Vector4.a;
        float4 _Property_6db672f2d1e64c12bad7ab94896b619b_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_FoamColor) : _FoamColor;
        float _Split_74f92acb87644a86ad859aab45ff575e_R_1_Float = _Property_6db672f2d1e64c12bad7ab94896b619b_Out_0_Vector4[0];
        float _Split_74f92acb87644a86ad859aab45ff575e_G_2_Float = _Property_6db672f2d1e64c12bad7ab94896b619b_Out_0_Vector4[1];
        float _Split_74f92acb87644a86ad859aab45ff575e_B_3_Float = _Property_6db672f2d1e64c12bad7ab94896b619b_Out_0_Vector4[2];
        float _Split_74f92acb87644a86ad859aab45ff575e_A_4_Float = _Property_6db672f2d1e64c12bad7ab94896b619b_Out_0_Vector4[3];
        float _Multiply_016b5234eb4847c5961f4a6df79e55ce_Out_2_Float;
        Unity_Multiply_float_float(_SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_R_4_Float, _Split_74f92acb87644a86ad859aab45ff575e_A_4_Float, _Multiply_016b5234eb4847c5961f4a6df79e55ce_Out_2_Float);
        FoamResult_1 = (_Multiply_016b5234eb4847c5961f4a6df79e55ce_Out_2_Float.xxxx);
        }
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        struct Bindings_IntersectionFoam_e12de5bce35668a4e8839cbe5732d905_float
        {
        float3 WorldSpacePosition;
        float4 ScreenPosition;
        float2 NDCPosition;
        float3 TimeParameters;
        };
        
        void SG_IntersectionFoam_e12de5bce35668a4e8839cbe5732d905_float(float _IntersectionFoamDepth, float _IntersectionFoamDirection, float _IntersectionFoamSpeed, float2 _UVs, float _IntersectionFoamTiling, float _IntersectionFoamFade, float _IntersectionFoamCutoff, UnityTexture2D _IntersectionFoamTexture, float4 _IntersectionFoamColor, Bindings_IntersectionFoam_e12de5bce35668a4e8839cbe5732d905_float IN, out float4 IntersectionFoamResult_1)
        {
        float _Property_436db722ba574b9da5152ea0b0fc269a_Out_0_Float = _IntersectionFoamDepth;
        float4 _ScreenPosition_d0ee1edadfe44fe1a6aa5df080569063_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
        Bindings_DepthFadePerspective_3069152b34697f24bb49447c02013d98_float _DepthFadePerspective_170dce4f8e3a4f75b1113c08817d6d10;
        _DepthFadePerspective_170dce4f8e3a4f75b1113c08817d6d10.WorldSpacePosition = IN.WorldSpacePosition;
        _DepthFadePerspective_170dce4f8e3a4f75b1113c08817d6d10.ScreenPosition = IN.ScreenPosition;
        float _DepthFadePerspective_170dce4f8e3a4f75b1113c08817d6d10_Linear_2_Float;
        SG_DepthFadePerspective_3069152b34697f24bb49447c02013d98_float(_Property_436db722ba574b9da5152ea0b0fc269a_Out_0_Float, (_ScreenPosition_d0ee1edadfe44fe1a6aa5df080569063_Out_0_Vector4.xy), _DepthFadePerspective_170dce4f8e3a4f75b1113c08817d6d10, _DepthFadePerspective_170dce4f8e3a4f75b1113c08817d6d10_Linear_2_Float);
        Bindings_DepthFade_1d0eb39db86b53245b582a69cdb87443_float _DepthFade_011030f3d4d0420b919705a8a53dedb6;
        _DepthFade_011030f3d4d0420b919705a8a53dedb6.ScreenPosition = IN.ScreenPosition;
        half _DepthFade_011030f3d4d0420b919705a8a53dedb6_Linear_2_Float;
        SG_DepthFade_1d0eb39db86b53245b582a69cdb87443_float(_Property_436db722ba574b9da5152ea0b0fc269a_Out_0_Float, (_ScreenPosition_d0ee1edadfe44fe1a6aa5df080569063_Out_0_Vector4.xy), _DepthFade_011030f3d4d0420b919705a8a53dedb6, _DepthFade_011030f3d4d0420b919705a8a53dedb6_Linear_2_Float);
        #if defined(_PERSPECTIVE)
        float _Perspective_917cfd8f51134dddafbdf7fcdbc16980_Out_0_Float = _DepthFadePerspective_170dce4f8e3a4f75b1113c08817d6d10_Linear_2_Float;
        #else
        float _Perspective_917cfd8f51134dddafbdf7fcdbc16980_Out_0_Float = _DepthFade_011030f3d4d0420b919705a8a53dedb6_Linear_2_Float;
        #endif
        float _Property_2c38ceda851245a9af3cedf5fa670532_Out_0_Float = _IntersectionFoamCutoff;
        float _Multiply_66d0b19b85644261a46d60dd4513434e_Out_2_Float;
        Unity_Multiply_float_float(_Perspective_917cfd8f51134dddafbdf7fcdbc16980_Out_0_Float, _Property_2c38ceda851245a9af3cedf5fa670532_Out_0_Float, _Multiply_66d0b19b85644261a46d60dd4513434e_Out_2_Float);
        float _OneMinus_879cfebaf09a4041ba20d931184cf4f9_Out_1_Float;
        Unity_OneMinus_float(_Multiply_66d0b19b85644261a46d60dd4513434e_Out_2_Float, _OneMinus_879cfebaf09a4041ba20d931184cf4f9_Out_1_Float);
        UnityTexture2D _Property_bc5252e2c5b74edbaa8ba7f5a162d720_Out_0_Texture2D = _IntersectionFoamTexture;
        float _Property_1f3b9e9e32a54263b64e2dfaa1da2891_Out_0_Float = _IntersectionFoamDirection;
        float _Property_940a015d94754f2cb866cb17053cdcc9_Out_0_Float = _IntersectionFoamSpeed;
        float2 _Property_32a7ac96a5dc47bd88f9784f3e38a5ab_Out_0_Vector2 = _UVs;
        float _Property_6fc7fe69b6fc4920b11d0aee35f0c39a_Out_0_Float = _IntersectionFoamTiling;
        Bindings_PanningUVs_ef286e626cee63841803a024235a644f_float _PanningUVs_b5fb72b5c5f74791bff52b9acb830aa0;
        _PanningUVs_b5fb72b5c5f74791bff52b9acb830aa0.TimeParameters = IN.TimeParameters;
        half2 _PanningUVs_b5fb72b5c5f74791bff52b9acb830aa0_UVOut_1_Vector2;
        SG_PanningUVs_ef286e626cee63841803a024235a644f_float(_Property_1f3b9e9e32a54263b64e2dfaa1da2891_Out_0_Float, _Property_940a015d94754f2cb866cb17053cdcc9_Out_0_Float, _Property_32a7ac96a5dc47bd88f9784f3e38a5ab_Out_0_Vector2, _Property_6fc7fe69b6fc4920b11d0aee35f0c39a_Out_0_Float, half2 (0, 0), _PanningUVs_b5fb72b5c5f74791bff52b9acb830aa0, _PanningUVs_b5fb72b5c5f74791bff52b9acb830aa0_UVOut_1_Vector2);
        float4 _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_bc5252e2c5b74edbaa8ba7f5a162d720_Out_0_Texture2D.tex, _Property_bc5252e2c5b74edbaa8ba7f5a162d720_Out_0_Texture2D.samplerstate, _Property_bc5252e2c5b74edbaa8ba7f5a162d720_Out_0_Texture2D.GetTransformedUV(_PanningUVs_b5fb72b5c5f74791bff52b9acb830aa0_UVOut_1_Vector2) );
        _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_RGBA_0_Vector4.rgb = UnpackNormalRGB(_SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_RGBA_0_Vector4);
        float _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_R_4_Float = _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_RGBA_0_Vector4.r;
        float _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_G_5_Float = _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_RGBA_0_Vector4.g;
        float _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_B_6_Float = _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_RGBA_0_Vector4.b;
        float _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_A_7_Float = _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_RGBA_0_Vector4.a;
        float _Step_640da6258eee484699374502b11f5859_Out_2_Float;
        Unity_Step_float(_OneMinus_879cfebaf09a4041ba20d931184cf4f9_Out_1_Float, _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_R_4_Float, _Step_640da6258eee484699374502b11f5859_Out_2_Float);
        float4 _Property_1b2a727d2d9a4b35a4b84b80d8cc241b_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_IntersectionFoamColor) : _IntersectionFoamColor;
        float _Split_0fdcdfb033da473fa176a759fabdf261_R_1_Float = _Property_1b2a727d2d9a4b35a4b84b80d8cc241b_Out_0_Vector4[0];
        float _Split_0fdcdfb033da473fa176a759fabdf261_G_2_Float = _Property_1b2a727d2d9a4b35a4b84b80d8cc241b_Out_0_Vector4[1];
        float _Split_0fdcdfb033da473fa176a759fabdf261_B_3_Float = _Property_1b2a727d2d9a4b35a4b84b80d8cc241b_Out_0_Vector4[2];
        float _Split_0fdcdfb033da473fa176a759fabdf261_A_4_Float = _Property_1b2a727d2d9a4b35a4b84b80d8cc241b_Out_0_Vector4[3];
        float _Multiply_4b309a3865b54e3490078c2758f00293_Out_2_Float;
        Unity_Multiply_float_float(_Step_640da6258eee484699374502b11f5859_Out_2_Float, _Split_0fdcdfb033da473fa176a759fabdf261_A_4_Float, _Multiply_4b309a3865b54e3490078c2758f00293_Out_2_Float);
        IntersectionFoamResult_1 = (_Multiply_4b309a3865b54e3490078c2758f00293_Out_2_Float.xxxx);
        }
        
        void Unity_NormalBlend_float(float3 A, float3 B, out float3 Out)
        {
            Out = SafeNormalize(float3(A.rg + B.rg, A.b * B.b));
        }
        
        struct Bindings_BlendedNormals_01824997d3b28f944887693b0e1d6405_float
        {
        float3 TimeParameters;
        };
        
        void SG_BlendedNormals_01824997d3b28f944887693b0e1d6405_float(float _NormalScale, float _NormalSpeed, UnityTexture2D _NormalTexture, float2 _UVs, Bindings_BlendedNormals_01824997d3b28f944887693b0e1d6405_float IN, out float3 Out_0)
        {
        UnityTexture2D _Property_b7e00044a0874370a521db8cbe781666_Out_0_Texture2D = _NormalTexture;
        float _Property_82e7e1ba78984b8f86383489bc8d0b1d_Out_0_Float = _NormalSpeed;
        float _Multiply_5e387d16860b440bb01f80e49690829a_Out_2_Float;
        Unity_Multiply_float_float(-0.5, _Property_82e7e1ba78984b8f86383489bc8d0b1d_Out_0_Float, _Multiply_5e387d16860b440bb01f80e49690829a_Out_2_Float);
        float2 _Property_d868665dbff94b6590abe31c7c0c6327_Out_0_Vector2 = _UVs;
        float _Property_5fd79ab306504e55aa9fa9ce57ddbb1b_Out_0_Float = _NormalScale;
        float _Multiply_63cc307a7007489dbadda4faf1dfa495_Out_2_Float;
        Unity_Multiply_float_float(0.5, _Property_5fd79ab306504e55aa9fa9ce57ddbb1b_Out_0_Float, _Multiply_63cc307a7007489dbadda4faf1dfa495_Out_2_Float);
        float _Reciprocal_4d8c53063c7c4694a2bc9fd174344747_Out_1_Float;
        Unity_Reciprocal_Fast_float(_Multiply_63cc307a7007489dbadda4faf1dfa495_Out_2_Float, _Reciprocal_4d8c53063c7c4694a2bc9fd174344747_Out_1_Float);
        Bindings_PanningUVs_ef286e626cee63841803a024235a644f_float _PanningUVs_c3d15fa8145b400cbfe094f7a9014648;
        _PanningUVs_c3d15fa8145b400cbfe094f7a9014648.TimeParameters = IN.TimeParameters;
        half2 _PanningUVs_c3d15fa8145b400cbfe094f7a9014648_UVOut_1_Vector2;
        SG_PanningUVs_ef286e626cee63841803a024235a644f_float(half(1), _Multiply_5e387d16860b440bb01f80e49690829a_Out_2_Float, _Property_d868665dbff94b6590abe31c7c0c6327_Out_0_Vector2, _Reciprocal_4d8c53063c7c4694a2bc9fd174344747_Out_1_Float, half2 (0, 0), _PanningUVs_c3d15fa8145b400cbfe094f7a9014648, _PanningUVs_c3d15fa8145b400cbfe094f7a9014648_UVOut_1_Vector2);
        float4 _SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_b7e00044a0874370a521db8cbe781666_Out_0_Texture2D.tex, _Property_b7e00044a0874370a521db8cbe781666_Out_0_Texture2D.samplerstate, _Property_b7e00044a0874370a521db8cbe781666_Out_0_Texture2D.GetTransformedUV(_PanningUVs_c3d15fa8145b400cbfe094f7a9014648_UVOut_1_Vector2) );
        _SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4.rgb = UnpackNormal(_SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4);
        float _SampleTexture2D_64a0d66e81d94af8907615615e56c648_R_4_Float = _SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4.r;
        float _SampleTexture2D_64a0d66e81d94af8907615615e56c648_G_5_Float = _SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4.g;
        float _SampleTexture2D_64a0d66e81d94af8907615615e56c648_B_6_Float = _SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4.b;
        float _SampleTexture2D_64a0d66e81d94af8907615615e56c648_A_7_Float = _SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4.a;
        float _Property_b3d54c3bdd024196aa765640b786350b_Out_0_Float = _NormalSpeed;
        float2 _Property_1d4d5f262eee4960a52cb63f61c3acbc_Out_0_Vector2 = _UVs;
        float _Property_425ab0f9b8b940939b9ae0d2b672a08d_Out_0_Float = _NormalScale;
        float _Reciprocal_8276cd463ae64d68a1e9c4bdff77f869_Out_1_Float;
        Unity_Reciprocal_Fast_float(_Property_425ab0f9b8b940939b9ae0d2b672a08d_Out_0_Float, _Reciprocal_8276cd463ae64d68a1e9c4bdff77f869_Out_1_Float);
        Bindings_PanningUVs_ef286e626cee63841803a024235a644f_float _PanningUVs_fb9f884294f840c4aa4334e5528e58be;
        _PanningUVs_fb9f884294f840c4aa4334e5528e58be.TimeParameters = IN.TimeParameters;
        half2 _PanningUVs_fb9f884294f840c4aa4334e5528e58be_UVOut_1_Vector2;
        SG_PanningUVs_ef286e626cee63841803a024235a644f_float(half(1), _Property_b3d54c3bdd024196aa765640b786350b_Out_0_Float, _Property_1d4d5f262eee4960a52cb63f61c3acbc_Out_0_Vector2, _Reciprocal_8276cd463ae64d68a1e9c4bdff77f869_Out_1_Float, half2 (0, 0), _PanningUVs_fb9f884294f840c4aa4334e5528e58be, _PanningUVs_fb9f884294f840c4aa4334e5528e58be_UVOut_1_Vector2);
        float4 _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_b7e00044a0874370a521db8cbe781666_Out_0_Texture2D.tex, _Property_b7e00044a0874370a521db8cbe781666_Out_0_Texture2D.samplerstate, _Property_b7e00044a0874370a521db8cbe781666_Out_0_Texture2D.GetTransformedUV(_PanningUVs_fb9f884294f840c4aa4334e5528e58be_UVOut_1_Vector2) );
        _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4.rgb = UnpackNormal(_SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4);
        float _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_R_4_Float = _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4.r;
        float _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_G_5_Float = _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4.g;
        float _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_B_6_Float = _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4.b;
        float _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_A_7_Float = _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4.a;
        float3 _NormalBlend_355e560adba24e7a8abe7a7673ad28d2_Out_2_Vector3;
        Unity_NormalBlend_float((_SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4.xyz), (_SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4.xyz), _NormalBlend_355e560adba24e7a8abe7a7673ad28d2_Out_2_Vector3);
        Out_0 = _NormalBlend_355e560adba24e7a8abe7a7673ad28d2_Out_2_Vector3;
        }
        
        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }
        
        struct Bindings_Specular_ca077344d37a82d46ba9cd29d65fb670_float
        {
        float3 WorldSpaceNormal;
        float3 WorldSpaceTangent;
        float3 WorldSpaceBiTangent;
        float3 WorldSpacePosition;
        float3 TimeParameters;
        };
        
        void SG_Specular_ca077344d37a82d46ba9cd29d65fb670_float(float _NormalScale, float _NormalSpeed, UnityTexture2D _NormalTexture, float _NormalStrength, float _LightingSmoothness, float _LightingHardness, float4 _SpecularColor, float2 _UVs, Bindings_Specular_ca077344d37a82d46ba9cd29d65fb670_float IN, out float3 Specular_1)
        {
        float _Property_12642a3bcf964c959ae2bbac7e5400ac_Out_0_Float = _NormalScale;
        float _Property_9a81b54411dc4443a34f009659e2b4d9_Out_0_Float = _NormalSpeed;
        UnityTexture2D _Property_229609cb9075441ca1c4520de10d655f_Out_0_Texture2D = _NormalTexture;
        float2 _Property_5b27f589f80742bcbfe965375ab83205_Out_0_Vector2 = _UVs;
        Bindings_BlendedNormals_01824997d3b28f944887693b0e1d6405_float _BlendedNormals_9340ea003cbd468e862462200f580f96;
        _BlendedNormals_9340ea003cbd468e862462200f580f96.TimeParameters = IN.TimeParameters;
        half3 _BlendedNormals_9340ea003cbd468e862462200f580f96_Out_0_Vector3;
        SG_BlendedNormals_01824997d3b28f944887693b0e1d6405_float(_Property_12642a3bcf964c959ae2bbac7e5400ac_Out_0_Float, _Property_9a81b54411dc4443a34f009659e2b4d9_Out_0_Float, _Property_229609cb9075441ca1c4520de10d655f_Out_0_Texture2D, _Property_5b27f589f80742bcbfe965375ab83205_Out_0_Vector2, _BlendedNormals_9340ea003cbd468e862462200f580f96, _BlendedNormals_9340ea003cbd468e862462200f580f96_Out_0_Vector3);
        float _Property_61fd9c0ec35a4d238019a6212f701e6f_Out_0_Float = _NormalStrength;
        float3 _NormalStrength_5fc187d2cc214a3aa352e512a627ae29_Out_2_Vector3;
        Unity_NormalStrength_float(_BlendedNormals_9340ea003cbd468e862462200f580f96_Out_0_Vector3, _Property_61fd9c0ec35a4d238019a6212f701e6f_Out_0_Float, _NormalStrength_5fc187d2cc214a3aa352e512a627ae29_Out_2_Vector3);
        float3 _Transform_f0cb3b0524334f779bf2800dae8c021e_Out_1_Vector3;
        {
        float3x3 tangentTransform = float3x3(IN.WorldSpaceTangent, IN.WorldSpaceBiTangent, IN.WorldSpaceNormal);
        _Transform_f0cb3b0524334f779bf2800dae8c021e_Out_1_Vector3 = TransformTangentToWorld(_NormalStrength_5fc187d2cc214a3aa352e512a627ae29_Out_2_Vector3.xyz, tangentTransform, true);
        }
        float3 _ViewVector_1252c7c64b0e4e6f855eab336f1a48a8_Out_0_Vector3;
        Unity_ViewVectorWorld_float(_ViewVector_1252c7c64b0e4e6f855eab336f1a48a8_Out_0_Vector3, IN.WorldSpacePosition);
        float3 _Vector3_e77fc3854050436d8a46ff8b9f3b1ff2_Out_0_Vector3 = float3(float(0), float(0), float(1));
        #if defined(_PERSPECTIVE)
        float3 _Perspective_322896f72bb640e09107587d0117cc53_Out_0_Vector3 = _ViewVector_1252c7c64b0e4e6f855eab336f1a48a8_Out_0_Vector3;
        #else
        float3 _Perspective_322896f72bb640e09107587d0117cc53_Out_0_Vector3 = _Vector3_e77fc3854050436d8a46ff8b9f3b1ff2_Out_0_Vector3;
        #endif
        float _MainLightingCustomFunction_08a62467374f4ec580e996e505530fff_Specular_4_Float;
        MainLighting_float(_Transform_f0cb3b0524334f779bf2800dae8c021e_Out_1_Vector3, IN.WorldSpacePosition, _Perspective_322896f72bb640e09107587d0117cc53_Out_0_Vector3, float(0), _MainLightingCustomFunction_08a62467374f4ec580e996e505530fff_Specular_4_Float);
        float _Step_7c27e8f5e968400b962535f046fd4f84_Out_2_Float;
        Unity_Step_float(float(0.5), _MainLightingCustomFunction_08a62467374f4ec580e996e505530fff_Specular_4_Float, _Step_7c27e8f5e968400b962535f046fd4f84_Out_2_Float);
        float _Property_070589d4b1b64f00877b042f5fa4786c_Out_0_Float = _LightingHardness;
        float _Lerp_7693c4d68e1a4276bcace636300a7f30_Out_3_Float;
        Unity_Lerp_float(_MainLightingCustomFunction_08a62467374f4ec580e996e505530fff_Specular_4_Float, _Step_7c27e8f5e968400b962535f046fd4f84_Out_2_Float, _Property_070589d4b1b64f00877b042f5fa4786c_Out_0_Float, _Lerp_7693c4d68e1a4276bcace636300a7f30_Out_3_Float);
        float4 _Property_2540d1f934ed4d15b7d62700a4f1adf2_Out_0_Vector4 = _SpecularColor;
        float4 _Multiply_e0f28cba39d84814946e9e3683c24585_Out_2_Vector4;
        Unity_Multiply_float4_float4((_Lerp_7693c4d68e1a4276bcace636300a7f30_Out_3_Float.xxxx), _Property_2540d1f934ed4d15b7d62700a4f1adf2_Out_0_Vector4, _Multiply_e0f28cba39d84814946e9e3683c24585_Out_2_Vector4);
        float _Split_cca0da3ff1c94d7b95381021f7e5824e_R_1_Float = _Property_2540d1f934ed4d15b7d62700a4f1adf2_Out_0_Vector4[0];
        float _Split_cca0da3ff1c94d7b95381021f7e5824e_G_2_Float = _Property_2540d1f934ed4d15b7d62700a4f1adf2_Out_0_Vector4[1];
        float _Split_cca0da3ff1c94d7b95381021f7e5824e_B_3_Float = _Property_2540d1f934ed4d15b7d62700a4f1adf2_Out_0_Vector4[2];
        float _Split_cca0da3ff1c94d7b95381021f7e5824e_A_4_Float = _Property_2540d1f934ed4d15b7d62700a4f1adf2_Out_0_Vector4[3];
        float4 _Multiply_e3b0a1558f044c5f9884617dd07c3456_Out_2_Vector4;
        Unity_Multiply_float4_float4(_Multiply_e0f28cba39d84814946e9e3683c24585_Out_2_Vector4, (_Split_cca0da3ff1c94d7b95381021f7e5824e_A_4_Float.xxxx), _Multiply_e3b0a1558f044c5f9884617dd07c3456_Out_2_Vector4);
        float _Property_d4211a44c8d34f3fb25fe762b3d7b5a5_Out_0_Float = _LightingSmoothness;
        float _Property_47eea6903fbd41518eb78ca7ba2ea49f_Out_0_Float = _LightingHardness;
        float3 _AdditionalLightingCustomFunction_f8bf1ccdd9a34ff1ba47f18b6139f837_Specular_5_Vector3;
        AdditionalLighting_float(_Transform_f0cb3b0524334f779bf2800dae8c021e_Out_1_Vector3, IN.WorldSpacePosition, _Perspective_322896f72bb640e09107587d0117cc53_Out_0_Vector3, _Property_d4211a44c8d34f3fb25fe762b3d7b5a5_Out_0_Float, _Property_47eea6903fbd41518eb78ca7ba2ea49f_Out_0_Float, _AdditionalLightingCustomFunction_f8bf1ccdd9a34ff1ba47f18b6139f837_Specular_5_Vector3);
        float3 _Add_bbd9570472ea43018aeac31b2cec79ef_Out_2_Vector3;
        Unity_Add_float3((_Multiply_e3b0a1558f044c5f9884617dd07c3456_Out_2_Vector4.xyz), _AdditionalLightingCustomFunction_f8bf1ccdd9a34ff1ba47f18b6139f837_Specular_5_Vector3, _Add_bbd9570472ea43018aeac31b2cec79ef_Out_2_Vector3);
        Specular_1 = _Add_bbd9570472ea43018aeac31b2cec79ef_Out_2_Vector3;
        }
        
        void Unity_Fog_float(out float4 Color, out float Density, float3 Position)
        {
            SHADERGRAPH_FOG(Position, Color, Density);
        }
        
        struct Bindings_Fog_4d8d465b4ba9a34499f11570d2bfbcf0_float
        {
        float3 ObjectSpacePosition;
        };
        
        void SG_Fog_4d8d465b4ba9a34499f11570d2bfbcf0_float(Bindings_Fog_4d8d465b4ba9a34499f11570d2bfbcf0_float IN, out float FogAmount_1, out float4 FogColor_2)
        {
        float4 _Fog_58a31c30c807423b9d9065753a0cbe96_Color_0_Vector4;
        float _Fog_58a31c30c807423b9d9065753a0cbe96_Density_1_Float;
        Unity_Fog_float(_Fog_58a31c30c807423b9d9065753a0cbe96_Color_0_Vector4, _Fog_58a31c30c807423b9d9065753a0cbe96_Density_1_Float, IN.ObjectSpacePosition);
        float _Saturate_57b873032001499d95a8956b1c4c9ff9_Out_1_Float;
        Unity_Saturate_float(_Fog_58a31c30c807423b9d9065753a0cbe96_Density_1_Float, _Saturate_57b873032001499d95a8956b1c4c9ff9_Out_1_Float);
        float4 _Fog_d79620e98a204435bdf933c422a158e0_Color_0_Vector4;
        float _Fog_d79620e98a204435bdf933c422a158e0_Density_1_Float;
        Unity_Fog_float(_Fog_d79620e98a204435bdf933c422a158e0_Color_0_Vector4, _Fog_d79620e98a204435bdf933c422a158e0_Density_1_Float, IN.ObjectSpacePosition);
        FogAmount_1 = _Saturate_57b873032001499d95a8956b1c4c9ff9_Out_1_Float;
        FogColor_2 = _Fog_d79620e98a204435bdf933c422a158e0_Color_0_Vector4;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Property_f4b92572a6b849978ea4164861fc96e7_Out_0_Float = _WaveSteepness;
            float _Property_60706828d6004c70825f62373cea2de7_Out_0_Float = _WaveLength;
            float _Property_2666eaa75aae48049c953771bc099709_Out_0_Float = _WaveSpeed;
            float4 _Property_cc5d2426eabf42799308261053db7fd7_Out_0_Vector4 = _WaveDirections;
            Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11;
            _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11.ObjectSpacePosition = IN.ObjectSpacePosition;
            float3 _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3;
            SG_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float(_Property_f4b92572a6b849978ea4164861fc96e7_Out_0_Float, _Property_60706828d6004c70825f62373cea2de7_Out_0_Float, _Property_2666eaa75aae48049c953771bc099709_Out_0_Float, _Property_cc5d2426eabf42799308261053db7fd7_Out_0_Vector4, _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11, _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3);
            description.Position = _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_1d602cb04f2a487bbe2a10a82c216988_Out_0_Float = _DepthFadeDistance;
            float _Property_7d6fc5bac56243d4b5ae0dcde436653d_Out_0_Float = _Steps;
            float _Property_e037029a81ca4277aa6156bc59761352_Out_0_Float = _RefractionSpeed;
            float _Property_99fa99535b074d1f91770ec4b97b3857_Out_0_Float = _RefractionScale;
            float _Property_ad37e2c0d61640a7bb0c4d98de3805d4_Out_0_Float = _GradientRefractionScale;
            float _Property_4a60bded2f1f4dd6945f5cf3f4fc789d_Out_0_Float = _RefractionStrength;
            float4 _Property_a9206916a2e3432bb1afa6b1d3655996_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_ShallowColor) : _ShallowColor;
            float4 _Property_e9f7f16ac69646e5908d8454ec7ef56f_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_DeepColor) : _DeepColor;
            Bindings_GetDepth_370b72acb6baa764089512ed583870c4_float _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e;
            _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e.WorldSpacePosition = IN.WorldSpacePosition;
            _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e.ScreenPosition = IN.ScreenPosition;
            _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e.NDCPosition = IN.NDCPosition;
            _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e.uv0 = IN.uv0;
            _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e.TimeParameters = IN.TimeParameters;
            float4 _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e_RefractedUVs_1_Vector4;
            float4 _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e_Color_2_Vector4;
            SG_GetDepth_370b72acb6baa764089512ed583870c4_float(_Property_1d602cb04f2a487bbe2a10a82c216988_Out_0_Float, _Property_7d6fc5bac56243d4b5ae0dcde436653d_Out_0_Float, _Property_e037029a81ca4277aa6156bc59761352_Out_0_Float, _Property_99fa99535b074d1f91770ec4b97b3857_Out_0_Float, _Property_ad37e2c0d61640a7bb0c4d98de3805d4_Out_0_Float, _Property_4a60bded2f1f4dd6945f5cf3f4fc789d_Out_0_Float, _Property_a9206916a2e3432bb1afa6b1d3655996_Out_0_Vector4, _Property_e9f7f16ac69646e5908d8454ec7ef56f_Out_0_Vector4, _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e, _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e_RefractedUVs_1_Vector4, _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e_Color_2_Vector4);
            float4 _Property_893a4c25d7bd4390a59efcfddbd60e36_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_HorizonColor) : _HorizonColor;
            float _Property_356b877dd8e2453c8037fed5c4738b25_Out_0_Float = _HorizonDistance;
            Bindings_FresnelCalculation_f346593723353f4488f8a099aef520eb_float _FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5;
            _FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5.WorldSpaceNormal = IN.WorldSpaceNormal;
            _FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5.ViewSpaceNormal = IN.ViewSpaceNormal;
            _FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5.WorldSpaceViewDirection = IN.WorldSpaceViewDirection;
            float _FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5_FresnelResult_1_Float;
            SG_FresnelCalculation_f346593723353f4488f8a099aef520eb_float(_Property_356b877dd8e2453c8037fed5c4738b25_Out_0_Float, _FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5, _FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5_FresnelResult_1_Float);
            float4 _Lerp_3aa30c6357614a61b17161f8c0633600_Out_3_Vector4;
            Unity_Lerp_float4(_GetDepth_52ad2e61abb04277a0dfe91bb3664d1e_Color_2_Vector4, _Property_893a4c25d7bd4390a59efcfddbd60e36_Out_0_Vector4, (_FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5_FresnelResult_1_Float.xxxx), _Lerp_3aa30c6357614a61b17161f8c0633600_Out_3_Vector4);
            Bindings_BlendObjectColor_a02636e967a650d4db669ee613538c46_float _BlendObjectColor_3ffec75a375b49dfa55b88d64e560cb9;
            float3 _BlendObjectColor_3ffec75a375b49dfa55b88d64e560cb9_ObjectBlend_1_Vector3;
            SG_BlendObjectColor_a02636e967a650d4db669ee613538c46_float(_GetDepth_52ad2e61abb04277a0dfe91bb3664d1e_RefractedUVs_1_Vector4, _Lerp_3aa30c6357614a61b17161f8c0633600_Out_3_Vector4, _BlendObjectColor_3ffec75a375b49dfa55b88d64e560cb9, _BlendObjectColor_3ffec75a375b49dfa55b88d64e560cb9_ObjectBlend_1_Vector3);
            float3 _Add_1dfe8dd4fc544cb1a6be38289da171bb_Out_2_Vector3;
            Unity_Add_float3(_BlendObjectColor_3ffec75a375b49dfa55b88d64e560cb9_ObjectBlend_1_Vector3, (_Lerp_3aa30c6357614a61b17161f8c0633600_Out_3_Vector4.xyz), _Add_1dfe8dd4fc544cb1a6be38289da171bb_Out_2_Vector3);
            float4 _Property_f31e3d68a9514af587c58953c6a37312_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_CaveColor) : _CaveColor;
            Bindings_CalculateUVs_60d47073ac03df54f9a15991680de154_float _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba;
            _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba.WorldSpacePosition = IN.WorldSpacePosition;
            _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba.uv0 = IN.uv0;
            float2 _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba_OutUVs_1_Vector2;
            SG_CalculateUVs_60d47073ac03df54f9a15991680de154_float(_CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba, _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba_OutUVs_1_Vector2);
            float2 _Property_b813bb0c2a834796bb918459b2e9f6f2_Out_0_Vector2 = _CaveScale;
            float2 _Property_f4eb27d4ee8b483c88526d64b0adcb71_Out_0_Vector2 = _CaveOffset;
            float _Property_d0f8505aba284e148b22af0ebde152ee_Out_0_Float = _CaveDistortion;
            UnityTexture2D _Property_e88c987f688347cfb013302786c36f42_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_CaveTexture);
            float4 _Property_a0ded2607a8749148ff7a887ebed0d9c_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_CaveColor) : _CaveColor;
            Bindings_Caves_3e5c142028ba07d48bba33c5843ba17e_float _Caves_bce6e1cd6e0a4b1c845eda4f734a4239;
            float4 _Caves_bce6e1cd6e0a4b1c845eda4f734a4239_CaveResult_1_Vector4;
            SG_Caves_3e5c142028ba07d48bba33c5843ba17e_float(_CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba_OutUVs_1_Vector2, _Property_b813bb0c2a834796bb918459b2e9f6f2_Out_0_Vector2, _Property_f4eb27d4ee8b483c88526d64b0adcb71_Out_0_Vector2, _Property_d0f8505aba284e148b22af0ebde152ee_Out_0_Float, _Property_e88c987f688347cfb013302786c36f42_Out_0_Texture2D, _Property_a0ded2607a8749148ff7a887ebed0d9c_Out_0_Vector4, float(50), _Caves_bce6e1cd6e0a4b1c845eda4f734a4239, _Caves_bce6e1cd6e0a4b1c845eda4f734a4239_CaveResult_1_Vector4);
            float3 _Lerp_9c2e40976f7e47008367af1bafb57231_Out_3_Vector3;
            Unity_Lerp_float3(_Add_1dfe8dd4fc544cb1a6be38289da171bb_Out_2_Vector3, (_Property_f31e3d68a9514af587c58953c6a37312_Out_0_Vector4.xyz), (_Caves_bce6e1cd6e0a4b1c845eda4f734a4239_CaveResult_1_Vector4.xyz), _Lerp_9c2e40976f7e47008367af1bafb57231_Out_3_Vector3);
            UnityTexture2D _Property_0eefa2a85d8a410bad14c729edc255c8_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_ReflectionMap);
            float _Property_b274ddde85464fe7990c99a3b9ff8986_Out_0_Float = _ReflectionDistortion;
            float _Property_379d70ad8d6b484e9cf65bd7ceb23f58_Out_0_Float = _ReflectionBlend;
            Bindings_Reflections_2a83053698b0a9d46a141dd7d36ac2ac_float _Reflections_c68a6bfeff564b248d4ce269caad599e;
            _Reflections_c68a6bfeff564b248d4ce269caad599e.NDCPosition = IN.NDCPosition;
            float4 _Reflections_c68a6bfeff564b248d4ce269caad599e_ReflexColor_1_Vector4;
            float _Reflections_c68a6bfeff564b248d4ce269caad599e_ReflexResult_2_Float;
            SG_Reflections_2a83053698b0a9d46a141dd7d36ac2ac_float(_Property_0eefa2a85d8a410bad14c729edc255c8_Out_0_Texture2D, _Property_b274ddde85464fe7990c99a3b9ff8986_Out_0_Float, _Property_379d70ad8d6b484e9cf65bd7ceb23f58_Out_0_Float, _Reflections_c68a6bfeff564b248d4ce269caad599e, _Reflections_c68a6bfeff564b248d4ce269caad599e_ReflexColor_1_Vector4, _Reflections_c68a6bfeff564b248d4ce269caad599e_ReflexResult_2_Float);
            float3 _Lerp_eae8f37e319e4e87a5ef928ede712a31_Out_3_Vector3;
            Unity_Lerp_float3(_Lerp_9c2e40976f7e47008367af1bafb57231_Out_3_Vector3, (_Reflections_c68a6bfeff564b248d4ce269caad599e_ReflexColor_1_Vector4.xyz), (_Reflections_c68a6bfeff564b248d4ce269caad599e_ReflexResult_2_Float.xxx), _Lerp_eae8f37e319e4e87a5ef928ede712a31_Out_3_Vector3);
            #if defined(_REFLECTIONS)
            float3 _Reflections_a210dad7ca474e3fb1719c7e933d3a27_Out_0_Vector3 = _Lerp_eae8f37e319e4e87a5ef928ede712a31_Out_3_Vector3;
            #else
            float3 _Reflections_a210dad7ca474e3fb1719c7e933d3a27_Out_0_Vector3 = _Lerp_9c2e40976f7e47008367af1bafb57231_Out_3_Vector3;
            #endif
            float4 _Property_81b51a63dd4c4bfb834b310d7e276279_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_SecondaryFoamColor) : _SecondaryFoamColor;
            float _Property_15d7e92bf5d541d9b7e1bbfadcc73a7f_Out_0_Float = _SurfaceFoamDirection;
            float _Property_711cbf38e2bb4a079d432afd2295ee81_Out_0_Float = _SurfaceFoamSpeed;
            float _Property_32a0f052e9444bdcb80f2f954608f8ef_Out_0_Float = _SurfaceFoamTiling;
            float2 _Property_17d01aaf1dda4850abc180a4ee955b3c_Out_0_Vector2 = _FoamUVsOffset;
            float _Property_ffebd665429e4cceaad03a9da3adafee_Out_0_Float = _SurfaceFoamDistorsion;
            UnityTexture2D _Property_d64c4dd59fff4c5b97b65f5f5c0c7f10_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_SecondaryFoamTex);
            float4 _Property_3451f3182b8241279c3bbb80d57d882c_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_SecondaryFoamColor) : _SecondaryFoamColor;
            Bindings_SecondaryFoam_a707540033b78f44bbef5139f65097d0_float _SecondaryFoam_5f90189370e9499fade77e767815c37a;
            _SecondaryFoam_5f90189370e9499fade77e767815c37a.TimeParameters = IN.TimeParameters;
            float4 _SecondaryFoam_5f90189370e9499fade77e767815c37a_SecondaryFoamResult_1_Vector4;
            SG_SecondaryFoam_a707540033b78f44bbef5139f65097d0_float(_Property_15d7e92bf5d541d9b7e1bbfadcc73a7f_Out_0_Float, _Property_711cbf38e2bb4a079d432afd2295ee81_Out_0_Float, _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba_OutUVs_1_Vector2, _Property_32a0f052e9444bdcb80f2f954608f8ef_Out_0_Float, _Property_17d01aaf1dda4850abc180a4ee955b3c_Out_0_Vector2, _Property_ffebd665429e4cceaad03a9da3adafee_Out_0_Float, _Property_d64c4dd59fff4c5b97b65f5f5c0c7f10_Out_0_Texture2D, _Property_3451f3182b8241279c3bbb80d57d882c_Out_0_Vector4, _SecondaryFoam_5f90189370e9499fade77e767815c37a, _SecondaryFoam_5f90189370e9499fade77e767815c37a_SecondaryFoamResult_1_Vector4);
            float3 _Lerp_f104552f399b462f82c9f7d9d4b08dc2_Out_3_Vector3;
            Unity_Lerp_float3(_Reflections_a210dad7ca474e3fb1719c7e933d3a27_Out_0_Vector3, (_Property_81b51a63dd4c4bfb834b310d7e276279_Out_0_Vector4.xyz), (_SecondaryFoam_5f90189370e9499fade77e767815c37a_SecondaryFoamResult_1_Vector4.xyz), _Lerp_f104552f399b462f82c9f7d9d4b08dc2_Out_3_Vector3);
            float4 _Property_da9173513832427bb5ab4ab2e5528b2b_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_SurfaceFoamColor) : _SurfaceFoamColor;
            float _Property_636aeb6fd5ae4972be2eb07aff88bb50_Out_0_Float = _SurfaceFoamDirection;
            float _Property_f4bf6eaa912341d1a75e44b6b46af453_Out_0_Float = _SurfaceFoamSpeed;
            float _Property_d25c273d7c7642d7a7dc57913ed42e97_Out_0_Float = _SurfaceFoamTiling;
            float _Property_1a3f66b4873543cda6a945906b57d7d8_Out_0_Float = _SurfaceFoamDistorsion;
            UnityTexture2D _Property_165b792376df4c00bf5ceebafff9099b_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_SurfaceFoamTexture);
            float4 _Property_689d2414eafc4fe4a7777244e0226de5_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_SurfaceFoamColor) : _SurfaceFoamColor;
            Bindings_PrimaryFoam_f41bb18b12540894cadcf85fcc438801_float _PrimaryFoam_4a7109ebe0ce44d88150306b8a89a15b;
            _PrimaryFoam_4a7109ebe0ce44d88150306b8a89a15b.TimeParameters = IN.TimeParameters;
            float4 _PrimaryFoam_4a7109ebe0ce44d88150306b8a89a15b_FoamResult_1_Vector4;
            SG_PrimaryFoam_f41bb18b12540894cadcf85fcc438801_float(_Property_636aeb6fd5ae4972be2eb07aff88bb50_Out_0_Float, _Property_f4bf6eaa912341d1a75e44b6b46af453_Out_0_Float, _Property_d25c273d7c7642d7a7dc57913ed42e97_Out_0_Float, _Property_1a3f66b4873543cda6a945906b57d7d8_Out_0_Float, _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba_OutUVs_1_Vector2, _Property_165b792376df4c00bf5ceebafff9099b_Out_0_Texture2D, _Property_689d2414eafc4fe4a7777244e0226de5_Out_0_Vector4, _PrimaryFoam_4a7109ebe0ce44d88150306b8a89a15b, _PrimaryFoam_4a7109ebe0ce44d88150306b8a89a15b_FoamResult_1_Vector4);
            float3 _Lerp_283283898a3f45b783820a658c09ee0e_Out_3_Vector3;
            Unity_Lerp_float3(_Lerp_f104552f399b462f82c9f7d9d4b08dc2_Out_3_Vector3, (_Property_da9173513832427bb5ab4ab2e5528b2b_Out_0_Vector4.xyz), (_PrimaryFoam_4a7109ebe0ce44d88150306b8a89a15b_FoamResult_1_Vector4.xyz), _Lerp_283283898a3f45b783820a658c09ee0e_Out_3_Vector3);
            float4 _Property_a28dab83abd146709ef4d7acf47620c9_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_IntersectionFoamColor) : _IntersectionFoamColor;
            float _Property_6d519a4006ce4a50b7180ef5a9b0d73f_Out_0_Float = _IntersectionFoamDepth;
            float _Property_1c21741f948044b38291462d9d9a80bc_Out_0_Float = _IntersectionFoamDirection;
            float _Property_c68c441254154b6598696f93cabb1974_Out_0_Float = _IntersectionFoamSpeed;
            float _Property_c27dd75d3883456b8cc7ecd9eea07821_Out_0_Float = _IntersectionFoamTiling;
            float _Property_998dfb4e05624d2a841294b3d6056443_Out_0_Float = _IntersectionFoamFade;
            float _Property_6c9ee0fa005147ccbc1e23124eddb903_Out_0_Float = _IntersectionFoamCutoff;
            UnityTexture2D _Property_262ae236fdea4725a5df623a31dc5a40_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_IntersectionFoamTexture);
            float4 _Property_83ad9b00f77a4d68a1a02e037f6da53f_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_IntersectionFoamColor) : _IntersectionFoamColor;
            Bindings_IntersectionFoam_e12de5bce35668a4e8839cbe5732d905_float _IntersectionFoam_111b1a7de3614aefbbfff3b435152564;
            _IntersectionFoam_111b1a7de3614aefbbfff3b435152564.WorldSpacePosition = IN.WorldSpacePosition;
            _IntersectionFoam_111b1a7de3614aefbbfff3b435152564.ScreenPosition = IN.ScreenPosition;
            _IntersectionFoam_111b1a7de3614aefbbfff3b435152564.NDCPosition = IN.NDCPosition;
            _IntersectionFoam_111b1a7de3614aefbbfff3b435152564.TimeParameters = IN.TimeParameters;
            float4 _IntersectionFoam_111b1a7de3614aefbbfff3b435152564_IntersectionFoamResult_1_Vector4;
            SG_IntersectionFoam_e12de5bce35668a4e8839cbe5732d905_float(_Property_6d519a4006ce4a50b7180ef5a9b0d73f_Out_0_Float, _Property_1c21741f948044b38291462d9d9a80bc_Out_0_Float, _Property_c68c441254154b6598696f93cabb1974_Out_0_Float, _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba_OutUVs_1_Vector2, _Property_c27dd75d3883456b8cc7ecd9eea07821_Out_0_Float, _Property_998dfb4e05624d2a841294b3d6056443_Out_0_Float, _Property_6c9ee0fa005147ccbc1e23124eddb903_Out_0_Float, _Property_262ae236fdea4725a5df623a31dc5a40_Out_0_Texture2D, _Property_83ad9b00f77a4d68a1a02e037f6da53f_Out_0_Vector4, _IntersectionFoam_111b1a7de3614aefbbfff3b435152564, _IntersectionFoam_111b1a7de3614aefbbfff3b435152564_IntersectionFoamResult_1_Vector4);
            float3 _Lerp_d89884addcfd45b3a3bb66e8d985f007_Out_3_Vector3;
            Unity_Lerp_float3(_Lerp_283283898a3f45b783820a658c09ee0e_Out_3_Vector3, (_Property_a28dab83abd146709ef4d7acf47620c9_Out_0_Vector4.xyz), (_IntersectionFoam_111b1a7de3614aefbbfff3b435152564_IntersectionFoamResult_1_Vector4.xyz), _Lerp_d89884addcfd45b3a3bb66e8d985f007_Out_3_Vector3);
            float _Property_7d5a7594d9d645ab99b46d3eee118d5c_Out_0_Float = _NormalScale;
            float _Property_4d1d135eaa684e87a7f73553b8717c03_Out_0_Float = _NormalSpeed;
            UnityTexture2D _Property_f51209adc1e24e0f997c51a1a1ab46b5_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_NormalTexture);
            float _Property_999b6f77135943d7adfa043c98a9ea04_Out_0_Float = _NormalStrength;
            float _Property_0299898b57e04518ad9f1b61a9bef7d6_Out_0_Float = _LightingSmoothness;
            float _Property_c5979b329857400396134e141ceffd60_Out_0_Float = _LightingHardness;
            float4 _Property_f09aaf76f58144498339d31ddb56a129_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_SpecularColor) : _SpecularColor;
            Bindings_Specular_ca077344d37a82d46ba9cd29d65fb670_float _Specular_467937ce566d4d7387886e0b29b4205e;
            _Specular_467937ce566d4d7387886e0b29b4205e.WorldSpaceNormal = IN.WorldSpaceNormal;
            _Specular_467937ce566d4d7387886e0b29b4205e.WorldSpaceTangent = IN.WorldSpaceTangent;
            _Specular_467937ce566d4d7387886e0b29b4205e.WorldSpaceBiTangent = IN.WorldSpaceBiTangent;
            _Specular_467937ce566d4d7387886e0b29b4205e.WorldSpacePosition = IN.WorldSpacePosition;
            _Specular_467937ce566d4d7387886e0b29b4205e.TimeParameters = IN.TimeParameters;
            float3 _Specular_467937ce566d4d7387886e0b29b4205e_Specular_1_Vector3;
            SG_Specular_ca077344d37a82d46ba9cd29d65fb670_float(_Property_7d5a7594d9d645ab99b46d3eee118d5c_Out_0_Float, _Property_4d1d135eaa684e87a7f73553b8717c03_Out_0_Float, _Property_f51209adc1e24e0f997c51a1a1ab46b5_Out_0_Texture2D, _Property_999b6f77135943d7adfa043c98a9ea04_Out_0_Float, _Property_0299898b57e04518ad9f1b61a9bef7d6_Out_0_Float, _Property_c5979b329857400396134e141ceffd60_Out_0_Float, _Property_f09aaf76f58144498339d31ddb56a129_Out_0_Vector4, _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba_OutUVs_1_Vector2, _Specular_467937ce566d4d7387886e0b29b4205e, _Specular_467937ce566d4d7387886e0b29b4205e_Specular_1_Vector3);
            float3 _Add_8502fe58d1c0413598d6f47c5cbc17f6_Out_2_Vector3;
            Unity_Add_float3(_Lerp_d89884addcfd45b3a3bb66e8d985f007_Out_3_Vector3, _Specular_467937ce566d4d7387886e0b29b4205e_Specular_1_Vector3, _Add_8502fe58d1c0413598d6f47c5cbc17f6_Out_2_Vector3);
            Bindings_Fog_4d8d465b4ba9a34499f11570d2bfbcf0_float _Fog_421529a4b940458890852d5853ae0e26;
            _Fog_421529a4b940458890852d5853ae0e26.ObjectSpacePosition = IN.ObjectSpacePosition;
            float _Fog_421529a4b940458890852d5853ae0e26_FogAmount_1_Float;
            float4 _Fog_421529a4b940458890852d5853ae0e26_FogColor_2_Vector4;
            SG_Fog_4d8d465b4ba9a34499f11570d2bfbcf0_float(_Fog_421529a4b940458890852d5853ae0e26, _Fog_421529a4b940458890852d5853ae0e26_FogAmount_1_Float, _Fog_421529a4b940458890852d5853ae0e26_FogColor_2_Vector4);
            float3 _Lerp_2d6e34c6906c4c55966370e2c9504dca_Out_3_Vector3;
            Unity_Lerp_float3(_Add_8502fe58d1c0413598d6f47c5cbc17f6_Out_2_Vector3, (_Fog_421529a4b940458890852d5853ae0e26_FogColor_2_Vector4.xyz), (_Fog_421529a4b940458890852d5853ae0e26_FogAmount_1_Float.xxx), _Lerp_2d6e34c6906c4c55966370e2c9504dca_Out_3_Vector3);
            surface.BaseColor = _Lerp_2d6e34c6906c4c55966370e2c9504dca_Out_3_Vector3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
            // use bitangent on the fly like in hdrp
            // IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
            float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0)* GetOddNegativeScale();
            float3 bitang = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
            output.ViewSpaceNormal = mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_I_V);         // transposed multiplication by inverse matrix to handle normal scale
        
            // to pr               eserve mikktspace compliance we use same scale renormFactor as was used on the normal.
            // This                is explained in section 2.2 in "surface gradient based bump mapping framework"
            output.WorldSpaceTangent = renormFactor * input.tangentWS.xyz;
            output.WorldSpaceBiTangent = renormFactor * bitang;
        
            output.WorldSpaceViewDirection = GetWorldSpaceNormalizeViewDir(input.positionWS);
            output.WorldSpacePosition = input.positionWS;
            output.ObjectSpacePosition = TransformWorldToObject(input.positionWS);
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
            output.uv0 = input.texCoord0;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }
        
        // Render State
        Cull Off
        ZTest LEqual
        ZWrite On
        ColorMask R
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        #pragma shader_feature_local _ _WORLD_SPACE_UV
        #pragma shader_feature_local _ _A_SHORT_HIKE_MODE
        #pragma shader_feature_local _ _REFLECTIONS
        #pragma shader_feature_local _ _PERSPECTIVE
        
        #if defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_0
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_1
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_2
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE)
            #define KEYWORD_PERMUTATION_3
        #elif defined(_WORLD_SPACE_UV) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_4
        #elif defined(_WORLD_SPACE_UV) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_5
        #elif defined(_WORLD_SPACE_UV) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_6
        #elif defined(_WORLD_SPACE_UV)
            #define KEYWORD_PERMUTATION_7
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_8
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_9
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_10
        #elif defined(_A_SHORT_HIKE_MODE)
            #define KEYWORD_PERMUTATION_11
        #elif defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_12
        #elif defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_13
        #elif defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_14
        #else
            #define KEYWORD_PERMUTATION_15
        #endif
        
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _WaveLength;
        float4 _CaveSecondaryColor;
        float _WaveSpeed;
        float _DepthFadeDistance;
        float _IntersectionFoamTiling;
        float4 _SecondaryFoamTex_TexelSize;
        float4 _SecondaryFoamColor;
        float _IntersectionFoamSpeed;
        float _IntersectionFoamDirection;
        float _IntersectionFoamCutoff;
        float4 _ShallowColor;
        float4 _DeepColor;
        float _Steps;
        float _HorizonDistance;
        float4 _HorizonColor;
        float _RefractionSpeed;
        float _RefractionScale;
        float _GradientRefractionScale;
        float _RefractionStrength;
        float _SurfaceFoamDirection;
        float _SurfaceFoamSpeed;
        float _SurfaceFoamTiling;
        float _SurfaceFoamDistorsion;
        float4 _SurfaceFoamTexture_TexelSize;
        float4 _SurfaceFoamColor;
        float _IntersectionFoamDepth;
        float _IntersectionFoamFade;
        float4 _IntersectionFoamTexture_TexelSize;
        float4 _IntersectionFoamColor;
        float _NormalScale;
        float _NormalSpeed;
        float4 _NormalTexture_TexelSize;
        float _NormalStrength;
        float _LightingSmoothness;
        float _LightingHardness;
        float4 _SpecularColor;
        float _WaveSteepness;
        float4 _WaveDirections;
        float4 _CaveTexture_TexelSize;
        float4 _CaveColor;
        float _CaveDistortion;
        float2 _CaveScale;
        float _ReflectionDistortion;
        float _ReflectionBlend;
        float2 _FoamUVsOffset;
        float2 _CaveOffset;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SecondaryFoamTex);
        SAMPLER(sampler_SecondaryFoamTex);
        TEXTURE2D(_SurfaceFoamTexture);
        SAMPLER(sampler_SurfaceFoamTexture);
        TEXTURE2D(_IntersectionFoamTexture);
        SAMPLER(sampler_IntersectionFoamTexture);
        TEXTURE2D(_NormalTexture);
        SAMPLER(sampler_NormalTexture);
        TEXTURE2D(_CaveTexture);
        SAMPLER(sampler_CaveTexture);
        float _TileSize;
        TEXTURE2D(_ReflectionMap);
        SAMPLER(sampler_ReflectionMap);
        float4 _ReflectionMap_TexelSize;
        
        // Graph Includes
        #include_with_pragmas "Assets/Art/UI/AureDevGames/Water Stylized Shader Orto & Perspective Camera/Shader/HLSL/GrestnerWaves.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float
        {
        float3 ObjectSpacePosition;
        };
        
        void SG_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float(float _WaveSteepness, float _WaveLength, float _WaveSpeed, float4 _WaveDirections, Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float IN, out float3 PositionWithWaveOffset_1)
        {
        float3 _Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3;
        {
        // Converting Position from Object to AbsoluteWorld via world space
        float3 world;
        world = TransformObjectToWorld(IN.ObjectSpacePosition.xyz);
        _Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3 = GetAbsolutePositionWS(world);
        }
        float _Property_39d578b2ea3d4928a4a2aac434ebc5c8_Out_0_Float = _WaveSteepness;
        float _Property_4e4135449a284017bef47c0e872e7104_Out_0_Float = _WaveLength;
        float _Property_04f74ba2bc544fdcb5e19ff8a137defb_Out_0_Float = _WaveSpeed;
        float4 _Property_90991e6e61204fd4a1ae25860c15dac2_Out_0_Vector4 = _WaveDirections;
        float3 _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3;
        float3 _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Normal_5_Vector3;
        GerstnerWaves_float(_Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3, _Property_39d578b2ea3d4928a4a2aac434ebc5c8_Out_0_Float, _Property_4e4135449a284017bef47c0e872e7104_Out_0_Float, _Property_04f74ba2bc544fdcb5e19ff8a137defb_Out_0_Float, _Property_90991e6e61204fd4a1ae25860c15dac2_Out_0_Vector4, float(1), _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3, _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Normal_5_Vector3);
        float3 _Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3;
        Unity_Add_float3(_Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3, _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3, _Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3);
        float3 _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3;
        _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3 = TransformWorldToObject(_Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3.xyz);
        PositionWithWaveOffset_1 = _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Property_f4b92572a6b849978ea4164861fc96e7_Out_0_Float = _WaveSteepness;
            float _Property_60706828d6004c70825f62373cea2de7_Out_0_Float = _WaveLength;
            float _Property_2666eaa75aae48049c953771bc099709_Out_0_Float = _WaveSpeed;
            float4 _Property_cc5d2426eabf42799308261053db7fd7_Out_0_Vector4 = _WaveDirections;
            Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11;
            _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11.ObjectSpacePosition = IN.ObjectSpacePosition;
            float3 _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3;
            SG_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float(_Property_f4b92572a6b849978ea4164861fc96e7_Out_0_Float, _Property_60706828d6004c70825f62373cea2de7_Out_0_Float, _Property_2666eaa75aae48049c953771bc099709_Out_0_Float, _Property_cc5d2426eabf42799308261053db7fd7_Out_0_Vector4, _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11, _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3);
            description.Position = _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "MotionVectors"
            Tags
            {
                "LightMode" = "MotionVectors"
            }
        
        // Render State
        Cull Off
        ZTest LEqual
        ZWrite On
        ColorMask RG
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 3.5
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        #pragma shader_feature_local _ _WORLD_SPACE_UV
        #pragma shader_feature_local _ _A_SHORT_HIKE_MODE
        #pragma shader_feature_local _ _REFLECTIONS
        #pragma shader_feature_local _ _PERSPECTIVE
        
        #if defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_0
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_1
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_2
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE)
            #define KEYWORD_PERMUTATION_3
        #elif defined(_WORLD_SPACE_UV) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_4
        #elif defined(_WORLD_SPACE_UV) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_5
        #elif defined(_WORLD_SPACE_UV) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_6
        #elif defined(_WORLD_SPACE_UV)
            #define KEYWORD_PERMUTATION_7
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_8
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_9
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_10
        #elif defined(_A_SHORT_HIKE_MODE)
            #define KEYWORD_PERMUTATION_11
        #elif defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_12
        #elif defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_13
        #elif defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_14
        #else
            #define KEYWORD_PERMUTATION_15
        #endif
        
        
        // Defines
        
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_MOTION_VECTORS
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _WaveLength;
        float4 _CaveSecondaryColor;
        float _WaveSpeed;
        float _DepthFadeDistance;
        float _IntersectionFoamTiling;
        float4 _SecondaryFoamTex_TexelSize;
        float4 _SecondaryFoamColor;
        float _IntersectionFoamSpeed;
        float _IntersectionFoamDirection;
        float _IntersectionFoamCutoff;
        float4 _ShallowColor;
        float4 _DeepColor;
        float _Steps;
        float _HorizonDistance;
        float4 _HorizonColor;
        float _RefractionSpeed;
        float _RefractionScale;
        float _GradientRefractionScale;
        float _RefractionStrength;
        float _SurfaceFoamDirection;
        float _SurfaceFoamSpeed;
        float _SurfaceFoamTiling;
        float _SurfaceFoamDistorsion;
        float4 _SurfaceFoamTexture_TexelSize;
        float4 _SurfaceFoamColor;
        float _IntersectionFoamDepth;
        float _IntersectionFoamFade;
        float4 _IntersectionFoamTexture_TexelSize;
        float4 _IntersectionFoamColor;
        float _NormalScale;
        float _NormalSpeed;
        float4 _NormalTexture_TexelSize;
        float _NormalStrength;
        float _LightingSmoothness;
        float _LightingHardness;
        float4 _SpecularColor;
        float _WaveSteepness;
        float4 _WaveDirections;
        float4 _CaveTexture_TexelSize;
        float4 _CaveColor;
        float _CaveDistortion;
        float2 _CaveScale;
        float _ReflectionDistortion;
        float _ReflectionBlend;
        float2 _FoamUVsOffset;
        float2 _CaveOffset;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SecondaryFoamTex);
        SAMPLER(sampler_SecondaryFoamTex);
        TEXTURE2D(_SurfaceFoamTexture);
        SAMPLER(sampler_SurfaceFoamTexture);
        TEXTURE2D(_IntersectionFoamTexture);
        SAMPLER(sampler_IntersectionFoamTexture);
        TEXTURE2D(_NormalTexture);
        SAMPLER(sampler_NormalTexture);
        TEXTURE2D(_CaveTexture);
        SAMPLER(sampler_CaveTexture);
        float _TileSize;
        TEXTURE2D(_ReflectionMap);
        SAMPLER(sampler_ReflectionMap);
        float4 _ReflectionMap_TexelSize;
        
        // Graph Includes
        #include_with_pragmas "Assets/Art/UI/AureDevGames/Water Stylized Shader Orto & Perspective Camera/Shader/HLSL/GrestnerWaves.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float
        {
        float3 ObjectSpacePosition;
        };
        
        void SG_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float(float _WaveSteepness, float _WaveLength, float _WaveSpeed, float4 _WaveDirections, Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float IN, out float3 PositionWithWaveOffset_1)
        {
        float3 _Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3;
        {
        // Converting Position from Object to AbsoluteWorld via world space
        float3 world;
        world = TransformObjectToWorld(IN.ObjectSpacePosition.xyz);
        _Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3 = GetAbsolutePositionWS(world);
        }
        float _Property_39d578b2ea3d4928a4a2aac434ebc5c8_Out_0_Float = _WaveSteepness;
        float _Property_4e4135449a284017bef47c0e872e7104_Out_0_Float = _WaveLength;
        float _Property_04f74ba2bc544fdcb5e19ff8a137defb_Out_0_Float = _WaveSpeed;
        float4 _Property_90991e6e61204fd4a1ae25860c15dac2_Out_0_Vector4 = _WaveDirections;
        float3 _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3;
        float3 _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Normal_5_Vector3;
        GerstnerWaves_float(_Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3, _Property_39d578b2ea3d4928a4a2aac434ebc5c8_Out_0_Float, _Property_4e4135449a284017bef47c0e872e7104_Out_0_Float, _Property_04f74ba2bc544fdcb5e19ff8a137defb_Out_0_Float, _Property_90991e6e61204fd4a1ae25860c15dac2_Out_0_Vector4, float(1), _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3, _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Normal_5_Vector3);
        float3 _Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3;
        Unity_Add_float3(_Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3, _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3, _Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3);
        float3 _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3;
        _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3 = TransformWorldToObject(_Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3.xyz);
        PositionWithWaveOffset_1 = _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Property_f4b92572a6b849978ea4164861fc96e7_Out_0_Float = _WaveSteepness;
            float _Property_60706828d6004c70825f62373cea2de7_Out_0_Float = _WaveLength;
            float _Property_2666eaa75aae48049c953771bc099709_Out_0_Float = _WaveSpeed;
            float4 _Property_cc5d2426eabf42799308261053db7fd7_Out_0_Vector4 = _WaveDirections;
            Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11;
            _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11.ObjectSpacePosition = IN.ObjectSpacePosition;
            float3 _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3;
            SG_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float(_Property_f4b92572a6b849978ea4164861fc96e7_Out_0_Float, _Property_60706828d6004c70825f62373cea2de7_Out_0_Float, _Property_2666eaa75aae48049c953771bc099709_Out_0_Float, _Property_cc5d2426eabf42799308261053db7fd7_Out_0_Vector4, _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11, _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3);
            description.Position = _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/MotionVectorPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthNormalsOnly"
            Tags
            {
                "LightMode" = "DepthNormalsOnly"
            }
        
        // Render State
        Cull Off
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
        #pragma shader_feature_local _ _WORLD_SPACE_UV
        #pragma shader_feature_local _ _A_SHORT_HIKE_MODE
        #pragma shader_feature_local _ _REFLECTIONS
        #pragma shader_feature_local _ _PERSPECTIVE
        
        #if defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_0
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_1
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_2
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE)
            #define KEYWORD_PERMUTATION_3
        #elif defined(_WORLD_SPACE_UV) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_4
        #elif defined(_WORLD_SPACE_UV) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_5
        #elif defined(_WORLD_SPACE_UV) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_6
        #elif defined(_WORLD_SPACE_UV)
            #define KEYWORD_PERMUTATION_7
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_8
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_9
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_10
        #elif defined(_A_SHORT_HIKE_MODE)
            #define KEYWORD_PERMUTATION_11
        #elif defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_12
        #elif defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_13
        #elif defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_14
        #else
            #define KEYWORD_PERMUTATION_15
        #endif
        
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_NORMAL_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _WaveLength;
        float4 _CaveSecondaryColor;
        float _WaveSpeed;
        float _DepthFadeDistance;
        float _IntersectionFoamTiling;
        float4 _SecondaryFoamTex_TexelSize;
        float4 _SecondaryFoamColor;
        float _IntersectionFoamSpeed;
        float _IntersectionFoamDirection;
        float _IntersectionFoamCutoff;
        float4 _ShallowColor;
        float4 _DeepColor;
        float _Steps;
        float _HorizonDistance;
        float4 _HorizonColor;
        float _RefractionSpeed;
        float _RefractionScale;
        float _GradientRefractionScale;
        float _RefractionStrength;
        float _SurfaceFoamDirection;
        float _SurfaceFoamSpeed;
        float _SurfaceFoamTiling;
        float _SurfaceFoamDistorsion;
        float4 _SurfaceFoamTexture_TexelSize;
        float4 _SurfaceFoamColor;
        float _IntersectionFoamDepth;
        float _IntersectionFoamFade;
        float4 _IntersectionFoamTexture_TexelSize;
        float4 _IntersectionFoamColor;
        float _NormalScale;
        float _NormalSpeed;
        float4 _NormalTexture_TexelSize;
        float _NormalStrength;
        float _LightingSmoothness;
        float _LightingHardness;
        float4 _SpecularColor;
        float _WaveSteepness;
        float4 _WaveDirections;
        float4 _CaveTexture_TexelSize;
        float4 _CaveColor;
        float _CaveDistortion;
        float2 _CaveScale;
        float _ReflectionDistortion;
        float _ReflectionBlend;
        float2 _FoamUVsOffset;
        float2 _CaveOffset;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SecondaryFoamTex);
        SAMPLER(sampler_SecondaryFoamTex);
        TEXTURE2D(_SurfaceFoamTexture);
        SAMPLER(sampler_SurfaceFoamTexture);
        TEXTURE2D(_IntersectionFoamTexture);
        SAMPLER(sampler_IntersectionFoamTexture);
        TEXTURE2D(_NormalTexture);
        SAMPLER(sampler_NormalTexture);
        TEXTURE2D(_CaveTexture);
        SAMPLER(sampler_CaveTexture);
        float _TileSize;
        TEXTURE2D(_ReflectionMap);
        SAMPLER(sampler_ReflectionMap);
        float4 _ReflectionMap_TexelSize;
        
        // Graph Includes
        #include_with_pragmas "Assets/Art/UI/AureDevGames/Water Stylized Shader Orto & Perspective Camera/Shader/HLSL/GrestnerWaves.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float
        {
        float3 ObjectSpacePosition;
        };
        
        void SG_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float(float _WaveSteepness, float _WaveLength, float _WaveSpeed, float4 _WaveDirections, Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float IN, out float3 PositionWithWaveOffset_1)
        {
        float3 _Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3;
        {
        // Converting Position from Object to AbsoluteWorld via world space
        float3 world;
        world = TransformObjectToWorld(IN.ObjectSpacePosition.xyz);
        _Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3 = GetAbsolutePositionWS(world);
        }
        float _Property_39d578b2ea3d4928a4a2aac434ebc5c8_Out_0_Float = _WaveSteepness;
        float _Property_4e4135449a284017bef47c0e872e7104_Out_0_Float = _WaveLength;
        float _Property_04f74ba2bc544fdcb5e19ff8a137defb_Out_0_Float = _WaveSpeed;
        float4 _Property_90991e6e61204fd4a1ae25860c15dac2_Out_0_Vector4 = _WaveDirections;
        float3 _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3;
        float3 _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Normal_5_Vector3;
        GerstnerWaves_float(_Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3, _Property_39d578b2ea3d4928a4a2aac434ebc5c8_Out_0_Float, _Property_4e4135449a284017bef47c0e872e7104_Out_0_Float, _Property_04f74ba2bc544fdcb5e19ff8a137defb_Out_0_Float, _Property_90991e6e61204fd4a1ae25860c15dac2_Out_0_Vector4, float(1), _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3, _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Normal_5_Vector3);
        float3 _Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3;
        Unity_Add_float3(_Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3, _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3, _Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3);
        float3 _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3;
        _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3 = TransformWorldToObject(_Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3.xyz);
        PositionWithWaveOffset_1 = _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Property_f4b92572a6b849978ea4164861fc96e7_Out_0_Float = _WaveSteepness;
            float _Property_60706828d6004c70825f62373cea2de7_Out_0_Float = _WaveLength;
            float _Property_2666eaa75aae48049c953771bc099709_Out_0_Float = _WaveSpeed;
            float4 _Property_cc5d2426eabf42799308261053db7fd7_Out_0_Vector4 = _WaveDirections;
            Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11;
            _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11.ObjectSpacePosition = IN.ObjectSpacePosition;
            float3 _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3;
            SG_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float(_Property_f4b92572a6b849978ea4164861fc96e7_Out_0_Float, _Property_60706828d6004c70825f62373cea2de7_Out_0_Float, _Property_2666eaa75aae48049c953771bc099709_Out_0_Float, _Property_cc5d2426eabf42799308261053db7fd7_Out_0_Vector4, _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11, _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3);
            description.Position = _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }
        
        // Render State
        Cull Off
        ZTest LEqual
        ZWrite On
        ColorMask 0
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
        #pragma shader_feature_local _ _WORLD_SPACE_UV
        #pragma shader_feature_local _ _A_SHORT_HIKE_MODE
        #pragma shader_feature_local _ _REFLECTIONS
        #pragma shader_feature_local _ _PERSPECTIVE
        
        #if defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_0
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_1
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_2
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE)
            #define KEYWORD_PERMUTATION_3
        #elif defined(_WORLD_SPACE_UV) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_4
        #elif defined(_WORLD_SPACE_UV) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_5
        #elif defined(_WORLD_SPACE_UV) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_6
        #elif defined(_WORLD_SPACE_UV)
            #define KEYWORD_PERMUTATION_7
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_8
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_9
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_10
        #elif defined(_A_SHORT_HIKE_MODE)
            #define KEYWORD_PERMUTATION_11
        #elif defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_12
        #elif defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_13
        #elif defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_14
        #else
            #define KEYWORD_PERMUTATION_15
        #endif
        
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_NORMAL_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SHADOWCASTER
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _WaveLength;
        float4 _CaveSecondaryColor;
        float _WaveSpeed;
        float _DepthFadeDistance;
        float _IntersectionFoamTiling;
        float4 _SecondaryFoamTex_TexelSize;
        float4 _SecondaryFoamColor;
        float _IntersectionFoamSpeed;
        float _IntersectionFoamDirection;
        float _IntersectionFoamCutoff;
        float4 _ShallowColor;
        float4 _DeepColor;
        float _Steps;
        float _HorizonDistance;
        float4 _HorizonColor;
        float _RefractionSpeed;
        float _RefractionScale;
        float _GradientRefractionScale;
        float _RefractionStrength;
        float _SurfaceFoamDirection;
        float _SurfaceFoamSpeed;
        float _SurfaceFoamTiling;
        float _SurfaceFoamDistorsion;
        float4 _SurfaceFoamTexture_TexelSize;
        float4 _SurfaceFoamColor;
        float _IntersectionFoamDepth;
        float _IntersectionFoamFade;
        float4 _IntersectionFoamTexture_TexelSize;
        float4 _IntersectionFoamColor;
        float _NormalScale;
        float _NormalSpeed;
        float4 _NormalTexture_TexelSize;
        float _NormalStrength;
        float _LightingSmoothness;
        float _LightingHardness;
        float4 _SpecularColor;
        float _WaveSteepness;
        float4 _WaveDirections;
        float4 _CaveTexture_TexelSize;
        float4 _CaveColor;
        float _CaveDistortion;
        float2 _CaveScale;
        float _ReflectionDistortion;
        float _ReflectionBlend;
        float2 _FoamUVsOffset;
        float2 _CaveOffset;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SecondaryFoamTex);
        SAMPLER(sampler_SecondaryFoamTex);
        TEXTURE2D(_SurfaceFoamTexture);
        SAMPLER(sampler_SurfaceFoamTexture);
        TEXTURE2D(_IntersectionFoamTexture);
        SAMPLER(sampler_IntersectionFoamTexture);
        TEXTURE2D(_NormalTexture);
        SAMPLER(sampler_NormalTexture);
        TEXTURE2D(_CaveTexture);
        SAMPLER(sampler_CaveTexture);
        float _TileSize;
        TEXTURE2D(_ReflectionMap);
        SAMPLER(sampler_ReflectionMap);
        float4 _ReflectionMap_TexelSize;
        
        // Graph Includes
        #include_with_pragmas "Assets/Art/UI/AureDevGames/Water Stylized Shader Orto & Perspective Camera/Shader/HLSL/GrestnerWaves.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float
        {
        float3 ObjectSpacePosition;
        };
        
        void SG_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float(float _WaveSteepness, float _WaveLength, float _WaveSpeed, float4 _WaveDirections, Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float IN, out float3 PositionWithWaveOffset_1)
        {
        float3 _Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3;
        {
        // Converting Position from Object to AbsoluteWorld via world space
        float3 world;
        world = TransformObjectToWorld(IN.ObjectSpacePosition.xyz);
        _Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3 = GetAbsolutePositionWS(world);
        }
        float _Property_39d578b2ea3d4928a4a2aac434ebc5c8_Out_0_Float = _WaveSteepness;
        float _Property_4e4135449a284017bef47c0e872e7104_Out_0_Float = _WaveLength;
        float _Property_04f74ba2bc544fdcb5e19ff8a137defb_Out_0_Float = _WaveSpeed;
        float4 _Property_90991e6e61204fd4a1ae25860c15dac2_Out_0_Vector4 = _WaveDirections;
        float3 _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3;
        float3 _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Normal_5_Vector3;
        GerstnerWaves_float(_Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3, _Property_39d578b2ea3d4928a4a2aac434ebc5c8_Out_0_Float, _Property_4e4135449a284017bef47c0e872e7104_Out_0_Float, _Property_04f74ba2bc544fdcb5e19ff8a137defb_Out_0_Float, _Property_90991e6e61204fd4a1ae25860c15dac2_Out_0_Vector4, float(1), _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3, _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Normal_5_Vector3);
        float3 _Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3;
        Unity_Add_float3(_Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3, _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3, _Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3);
        float3 _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3;
        _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3 = TransformWorldToObject(_Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3.xyz);
        PositionWithWaveOffset_1 = _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Property_f4b92572a6b849978ea4164861fc96e7_Out_0_Float = _WaveSteepness;
            float _Property_60706828d6004c70825f62373cea2de7_Out_0_Float = _WaveLength;
            float _Property_2666eaa75aae48049c953771bc099709_Out_0_Float = _WaveSpeed;
            float4 _Property_cc5d2426eabf42799308261053db7fd7_Out_0_Vector4 = _WaveDirections;
            Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11;
            _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11.ObjectSpacePosition = IN.ObjectSpacePosition;
            float3 _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3;
            SG_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float(_Property_f4b92572a6b849978ea4164861fc96e7_Out_0_Float, _Property_60706828d6004c70825f62373cea2de7_Out_0_Float, _Property_2666eaa75aae48049c953771bc099709_Out_0_Float, _Property_cc5d2426eabf42799308261053db7fd7_Out_0_Vector4, _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11, _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3);
            description.Position = _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "GBuffer"
            Tags
            {
                "LightMode" = "UniversalGBuffer"
            }
        
        // Render State
        Cull Off
        Blend One Zero
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles3 glcore
        #pragma multi_compile_instancing
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        #pragma multi_compile_fragment _ _RENDER_PASS_ENABLED
        #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
        #pragma multi_compile _ SHADOWS_SHADOWMASK
        #pragma shader_feature_local _ _WORLD_SPACE_UV
        #pragma shader_feature_local _ _A_SHORT_HIKE_MODE
        #pragma shader_feature_local _ _REFLECTIONS
        #pragma shader_feature_local _ _PERSPECTIVE
        
        #if defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_0
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_1
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_2
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE)
            #define KEYWORD_PERMUTATION_3
        #elif defined(_WORLD_SPACE_UV) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_4
        #elif defined(_WORLD_SPACE_UV) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_5
        #elif defined(_WORLD_SPACE_UV) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_6
        #elif defined(_WORLD_SPACE_UV)
            #define KEYWORD_PERMUTATION_7
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_8
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_9
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_10
        #elif defined(_A_SHORT_HIKE_MODE)
            #define KEYWORD_PERMUTATION_11
        #elif defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_12
        #elif defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_13
        #elif defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_14
        #else
            #define KEYWORD_PERMUTATION_15
        #endif
        
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_GBUFFER
        #define REQUIRE_DEPTH_TEXTURE
        #define REQUIRE_OPAQUE_TEXTURE
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
            #if !defined(LIGHTMAP_ON)
             float3 sh;
            #endif
            #if defined(USE_APV_PROBE_OCCLUSION)
             float4 probeOcclusion;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 ViewSpaceNormal;
             float3 WorldSpaceNormal;
             float3 WorldSpaceTangent;
             float3 WorldSpaceBiTangent;
             float3 WorldSpaceViewDirection;
             float3 ObjectSpacePosition;
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float2 NDCPosition;
             float2 PixelPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
            #if !defined(LIGHTMAP_ON)
             float3 sh : INTERP0;
            #endif
            #if defined(USE_APV_PROBE_OCCLUSION)
             float4 probeOcclusion : INTERP1;
            #endif
             float4 tangentWS : INTERP2;
             float4 texCoord0 : INTERP3;
             float3 positionWS : INTERP4;
             float3 normalWS : INTERP5;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            #if !defined(LIGHTMAP_ON)
            output.sh = input.sh;
            #endif
            #if defined(USE_APV_PROBE_OCCLUSION)
            output.probeOcclusion = input.probeOcclusion;
            #endif
            output.tangentWS.xyzw = input.tangentWS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if !defined(LIGHTMAP_ON)
            output.sh = input.sh;
            #endif
            #if defined(USE_APV_PROBE_OCCLUSION)
            output.probeOcclusion = input.probeOcclusion;
            #endif
            output.tangentWS = input.tangentWS.xyzw;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _WaveLength;
        float4 _CaveSecondaryColor;
        float _WaveSpeed;
        float _DepthFadeDistance;
        float _IntersectionFoamTiling;
        float4 _SecondaryFoamTex_TexelSize;
        float4 _SecondaryFoamColor;
        float _IntersectionFoamSpeed;
        float _IntersectionFoamDirection;
        float _IntersectionFoamCutoff;
        float4 _ShallowColor;
        float4 _DeepColor;
        float _Steps;
        float _HorizonDistance;
        float4 _HorizonColor;
        float _RefractionSpeed;
        float _RefractionScale;
        float _GradientRefractionScale;
        float _RefractionStrength;
        float _SurfaceFoamDirection;
        float _SurfaceFoamSpeed;
        float _SurfaceFoamTiling;
        float _SurfaceFoamDistorsion;
        float4 _SurfaceFoamTexture_TexelSize;
        float4 _SurfaceFoamColor;
        float _IntersectionFoamDepth;
        float _IntersectionFoamFade;
        float4 _IntersectionFoamTexture_TexelSize;
        float4 _IntersectionFoamColor;
        float _NormalScale;
        float _NormalSpeed;
        float4 _NormalTexture_TexelSize;
        float _NormalStrength;
        float _LightingSmoothness;
        float _LightingHardness;
        float4 _SpecularColor;
        float _WaveSteepness;
        float4 _WaveDirections;
        float4 _CaveTexture_TexelSize;
        float4 _CaveColor;
        float _CaveDistortion;
        float2 _CaveScale;
        float _ReflectionDistortion;
        float _ReflectionBlend;
        float2 _FoamUVsOffset;
        float2 _CaveOffset;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SecondaryFoamTex);
        SAMPLER(sampler_SecondaryFoamTex);
        TEXTURE2D(_SurfaceFoamTexture);
        SAMPLER(sampler_SurfaceFoamTexture);
        TEXTURE2D(_IntersectionFoamTexture);
        SAMPLER(sampler_IntersectionFoamTexture);
        TEXTURE2D(_NormalTexture);
        SAMPLER(sampler_NormalTexture);
        TEXTURE2D(_CaveTexture);
        SAMPLER(sampler_CaveTexture);
        float _TileSize;
        TEXTURE2D(_ReflectionMap);
        SAMPLER(sampler_ReflectionMap);
        float4 _ReflectionMap_TexelSize;
        
        // Graph Includes
        #include_with_pragmas "Assets/Art/UI/AureDevGames/Water Stylized Shader Orto & Perspective Camera/Shader/HLSL/GrestnerWaves.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"
        #include_with_pragmas "Assets/Art/UI/AureDevGames/Water Stylized Shader Orto & Perspective Camera/Shader/HLSL/DistortUV.hlsl"
        #include_with_pragmas "Assets/Art/UI/AureDevGames/Water Stylized Shader Orto & Perspective Camera/Shader/HLSL/MainLighting.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float
        {
        float3 ObjectSpacePosition;
        };
        
        void SG_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float(float _WaveSteepness, float _WaveLength, float _WaveSpeed, float4 _WaveDirections, Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float IN, out float3 PositionWithWaveOffset_1)
        {
        float3 _Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3;
        {
        // Converting Position from Object to AbsoluteWorld via world space
        float3 world;
        world = TransformObjectToWorld(IN.ObjectSpacePosition.xyz);
        _Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3 = GetAbsolutePositionWS(world);
        }
        float _Property_39d578b2ea3d4928a4a2aac434ebc5c8_Out_0_Float = _WaveSteepness;
        float _Property_4e4135449a284017bef47c0e872e7104_Out_0_Float = _WaveLength;
        float _Property_04f74ba2bc544fdcb5e19ff8a137defb_Out_0_Float = _WaveSpeed;
        float4 _Property_90991e6e61204fd4a1ae25860c15dac2_Out_0_Vector4 = _WaveDirections;
        float3 _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3;
        float3 _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Normal_5_Vector3;
        GerstnerWaves_float(_Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3, _Property_39d578b2ea3d4928a4a2aac434ebc5c8_Out_0_Float, _Property_4e4135449a284017bef47c0e872e7104_Out_0_Float, _Property_04f74ba2bc544fdcb5e19ff8a137defb_Out_0_Float, _Property_90991e6e61204fd4a1ae25860c15dac2_Out_0_Vector4, float(1), _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3, _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Normal_5_Vector3);
        float3 _Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3;
        Unity_Add_float3(_Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3, _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3, _Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3);
        float3 _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3;
        _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3 = TransformWorldToObject(_Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3.xyz);
        PositionWithWaveOffset_1 = _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3;
        }
        
        void Unity_Reciprocal_Fast_float(float In, out float Out)
        {
            Out = rcp(In);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
        Out = A * B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        float2 Unity_GradientNoise_LegacyMod_Dir_float(float2 p)
        {
        float x; Hash_LegacyMod_2_1_float(p, x);
        return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }
        
        void Unity_GradientNoise_LegacyMod_float (float2 UV, float3 Scale, out float Out)
        {
        float2 p = UV * Scale.xy;
        float2 ip = floor(p);
        float2 fp = frac(p);
        float d00 = dot(Unity_GradientNoise_LegacyMod_Dir_float(ip), fp);
        float d01 = dot(Unity_GradientNoise_LegacyMod_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
        float d10 = dot(Unity_GradientNoise_LegacyMod_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
        float d11 = dot(Unity_GradientNoise_LegacyMod_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
        fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
        Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_RefractedUV_dfdc6038fe7bdd5479fef69a0fcdb7ab_float
        {
        float2 NDCPosition;
        half4 uv0;
        float3 TimeParameters;
        };
        
        void SG_RefractedUV_dfdc6038fe7bdd5479fef69a0fcdb7ab_float(float _RefractionSpeed, float _RefractionScale, float _GradientRefractionScale, float _RefractionStrength, Bindings_RefractedUV_dfdc6038fe7bdd5479fef69a0fcdb7ab_float IN, out float4 Out_1)
        {
        float4 _ScreenPosition_df4df06bf0bd410e849370e7f915ce20_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
        float _Property_6f2e2eea5f1542be862fc193a4f11e4f_Out_0_Float = _RefractionScale;
        float _Reciprocal_c26c7813ffc1401d8343e505cf3d6b8e_Out_1_Float;
        Unity_Reciprocal_Fast_float(_Property_6f2e2eea5f1542be862fc193a4f11e4f_Out_0_Float, _Reciprocal_c26c7813ffc1401d8343e505cf3d6b8e_Out_1_Float);
        float _Property_8fef6762630f4a338c9146b52f2255f8_Out_0_Float = _RefractionSpeed;
        float _Multiply_44cb146f6607445f8fd249e1966a7b09_Out_2_Float;
        Unity_Multiply_float_float(_Property_8fef6762630f4a338c9146b52f2255f8_Out_0_Float, IN.TimeParameters.x, _Multiply_44cb146f6607445f8fd249e1966a7b09_Out_2_Float);
        float2 _TilingAndOffset_0d3bc9acf8f54350b7aeed81c3a55b94_Out_3_Vector2;
        Unity_TilingAndOffset_float(IN.uv0.xy, (_Reciprocal_c26c7813ffc1401d8343e505cf3d6b8e_Out_1_Float.xx), (_Multiply_44cb146f6607445f8fd249e1966a7b09_Out_2_Float.xx), _TilingAndOffset_0d3bc9acf8f54350b7aeed81c3a55b94_Out_3_Vector2);
        float _Property_2087245bf65447009de8a4234c28ef5d_Out_0_Float = _GradientRefractionScale;
        float _GradientNoise_23f5075437694dcfa3f424e64e24a034_Out_2_Float;
        Unity_GradientNoise_LegacyMod_float(_TilingAndOffset_0d3bc9acf8f54350b7aeed81c3a55b94_Out_3_Vector2, _Property_2087245bf65447009de8a4234c28ef5d_Out_0_Float, _GradientNoise_23f5075437694dcfa3f424e64e24a034_Out_2_Float);
        float _Remap_92f97d94f5a64376842fea592a767e02_Out_3_Float;
        Unity_Remap_float(_GradientNoise_23f5075437694dcfa3f424e64e24a034_Out_2_Float, float2 (0, 1), float2 (-1, 1), _Remap_92f97d94f5a64376842fea592a767e02_Out_3_Float);
        float _Property_eee1994acb674489901dde01c3c3316d_Out_0_Float = _RefractionStrength;
        float _Multiply_b39bd58102fe470c8e1e7f1ce82a040d_Out_2_Float;
        Unity_Multiply_float_float(_Remap_92f97d94f5a64376842fea592a767e02_Out_3_Float, _Property_eee1994acb674489901dde01c3c3316d_Out_0_Float, _Multiply_b39bd58102fe470c8e1e7f1ce82a040d_Out_2_Float);
        float4 _Add_7d79ed8fd86a46e7a268b2fd5dbe2bff_Out_2_Vector4;
        Unity_Add_float4(_ScreenPosition_df4df06bf0bd410e849370e7f915ce20_Out_0_Vector4, (_Multiply_b39bd58102fe470c8e1e7f1ce82a040d_Out_2_Float.xxxx), _Add_7d79ed8fd86a46e7a268b2fd5dbe2bff_Out_2_Vector4);
        Out_1 = _Add_7d79ed8fd86a46e7a268b2fd5dbe2bff_Out_2_Vector4;
        }
        
        void Unity_ViewVectorWorld_float(out float3 Out, float3 WorldSpacePosition)
        {
            Out = _WorldSpaceCameraPos.xyz - GetAbsolutePositionWS(WorldSpacePosition);
            if(!IsPerspectiveProjection())
            {
                Out = GetViewForwardDir() * dot(Out, GetViewForwardDir());
            }
        }
        
        void Unity_Negate_float3(float3 In, out float3 Out)
        {
            Out = -1 * In;
        }
        
        void Unity_Divide_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A / B;
        }
        
        void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
        {
            if (unity_OrthoParams.w == 1.0)
            {
                Out = LinearEyeDepth(ComputeWorldSpacePosition(UV.xy, SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), UNITY_MATRIX_I_VP), UNITY_MATRIX_V);
            }
            else
            {
                Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
            }
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
        Out = A * B;
        }
        
        void Unity_Subtract_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A - B;
        }
        
        void Unity_Negate_float(float In, out float Out)
        {
            Out = -1 * In;
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_Exponential_float(float In, out float Out)
        {
            Out = exp(In);
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        struct Bindings_DepthFadePerspective_3069152b34697f24bb49447c02013d98_float
        {
        float3 WorldSpacePosition;
        float4 ScreenPosition;
        };
        
        void SG_DepthFadePerspective_3069152b34697f24bb49447c02013d98_float(float _DepthFadeDistance, float2 _UV, Bindings_DepthFadePerspective_3069152b34697f24bb49447c02013d98_float IN, out float Linear_2)
        {
        float3 _ViewVector_f8eabe5a287144a6a6d4ef737338317b_Out_0_Vector3;
        Unity_ViewVectorWorld_float(_ViewVector_f8eabe5a287144a6a6d4ef737338317b_Out_0_Vector3, IN.WorldSpacePosition);
        float3 _Negate_d181b51abcbd4784938d7ef4debecdab_Out_1_Vector3;
        Unity_Negate_float3(_ViewVector_f8eabe5a287144a6a6d4ef737338317b_Out_0_Vector3, _Negate_d181b51abcbd4784938d7ef4debecdab_Out_1_Vector3);
        float4 _ScreenPosition_8e5642cf0f54480688de7e9414096c95_Out_0_Vector4 = IN.ScreenPosition;
        float _Split_47aeb1862e224482b604dba19d9665d1_R_1_Float = _ScreenPosition_8e5642cf0f54480688de7e9414096c95_Out_0_Vector4[0];
        float _Split_47aeb1862e224482b604dba19d9665d1_G_2_Float = _ScreenPosition_8e5642cf0f54480688de7e9414096c95_Out_0_Vector4[1];
        float _Split_47aeb1862e224482b604dba19d9665d1_B_3_Float = _ScreenPosition_8e5642cf0f54480688de7e9414096c95_Out_0_Vector4[2];
        float _Split_47aeb1862e224482b604dba19d9665d1_A_4_Float = _ScreenPosition_8e5642cf0f54480688de7e9414096c95_Out_0_Vector4[3];
        float3 _Divide_5a42a764edb04fdfb14c08f2df82448f_Out_2_Vector3;
        Unity_Divide_float3(_Negate_d181b51abcbd4784938d7ef4debecdab_Out_1_Vector3, (_Split_47aeb1862e224482b604dba19d9665d1_A_4_Float.xxx), _Divide_5a42a764edb04fdfb14c08f2df82448f_Out_2_Vector3);
        float2 _Property_d8a51218a3bb4194aa564336969f48bf_Out_0_Vector2 = _UV;
        float _SceneDepth_ae83678fa5f34fe095bf2035c218be22_Out_1_Float;
        Unity_SceneDepth_Eye_float((float4(_Property_d8a51218a3bb4194aa564336969f48bf_Out_0_Vector2, 0.0, 1.0)), _SceneDepth_ae83678fa5f34fe095bf2035c218be22_Out_1_Float);
        float3 _Multiply_36a67ab024a341298aa97e76a995919d_Out_2_Vector3;
        Unity_Multiply_float3_float3(_Divide_5a42a764edb04fdfb14c08f2df82448f_Out_2_Vector3, (_SceneDepth_ae83678fa5f34fe095bf2035c218be22_Out_1_Float.xxx), _Multiply_36a67ab024a341298aa97e76a995919d_Out_2_Vector3);
        float3 _Add_a73b06032a6b424ebc6dcb211cb1d7ec_Out_2_Vector3;
        Unity_Add_float3(_Multiply_36a67ab024a341298aa97e76a995919d_Out_2_Vector3, _WorldSpaceCameraPos, _Add_a73b06032a6b424ebc6dcb211cb1d7ec_Out_2_Vector3);
        float3 _Subtract_56e40e7df9514c6db684a06980c58e55_Out_2_Vector3;
        Unity_Subtract_float3(IN.WorldSpacePosition, _Add_a73b06032a6b424ebc6dcb211cb1d7ec_Out_2_Vector3, _Subtract_56e40e7df9514c6db684a06980c58e55_Out_2_Vector3);
        float _Split_8572215aaf714ae88e25b2db09553f0f_R_1_Float = _Subtract_56e40e7df9514c6db684a06980c58e55_Out_2_Vector3[0];
        float _Split_8572215aaf714ae88e25b2db09553f0f_G_2_Float = _Subtract_56e40e7df9514c6db684a06980c58e55_Out_2_Vector3[1];
        float _Split_8572215aaf714ae88e25b2db09553f0f_B_3_Float = _Subtract_56e40e7df9514c6db684a06980c58e55_Out_2_Vector3[2];
        float _Split_8572215aaf714ae88e25b2db09553f0f_A_4_Float = 0;
        float _Negate_38f930ab7f5148fbaca13a81a57e9720_Out_1_Float;
        Unity_Negate_float(_Split_8572215aaf714ae88e25b2db09553f0f_G_2_Float, _Negate_38f930ab7f5148fbaca13a81a57e9720_Out_1_Float);
        float _Property_58c167a0b3db4df1a74645ae7044c728_Out_0_Float = _DepthFadeDistance;
        float _Divide_1c7f36016a6143bfb91d1e0cefd0e84b_Out_2_Float;
        Unity_Divide_float(_Negate_38f930ab7f5148fbaca13a81a57e9720_Out_1_Float, _Property_58c167a0b3db4df1a74645ae7044c728_Out_0_Float, _Divide_1c7f36016a6143bfb91d1e0cefd0e84b_Out_2_Float);
        float _Exponential_5f2f255a73714085921dbe98b929d51a_Out_1_Float;
        Unity_Exponential_float(_Divide_1c7f36016a6143bfb91d1e0cefd0e84b_Out_2_Float, _Exponential_5f2f255a73714085921dbe98b929d51a_Out_1_Float);
        float _Saturate_4c21ba80863f474f9b1768b39a43d251_Out_1_Float;
        Unity_Saturate_float(_Exponential_5f2f255a73714085921dbe98b929d51a_Out_1_Float, _Saturate_4c21ba80863f474f9b1768b39a43d251_Out_1_Float);
        Linear_2 = _Saturate_4c21ba80863f474f9b1768b39a43d251_Out_1_Float;
        }
        
        void Unity_SceneDepth_Raw_float(float4 UV, out float Out)
        {
            Out = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy);
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        struct Bindings_DepthFade_1d0eb39db86b53245b582a69cdb87443_float
        {
        float4 ScreenPosition;
        };
        
        void SG_DepthFade_1d0eb39db86b53245b582a69cdb87443_float(float _DepthFadeDistance, float2 _UV, Bindings_DepthFade_1d0eb39db86b53245b582a69cdb87443_float IN, out float Linear_2)
        {
        float2 _Property_46bd4502b44f465ab1623fd92c5e4785_Out_0_Vector2 = _UV;
        float _SceneDepth_905c05d0cfa04274a8511269c1782b79_Out_1_Float;
        Unity_SceneDepth_Raw_float((float4(_Property_46bd4502b44f465ab1623fd92c5e4785_Out_0_Vector2, 0.0, 1.0)), _SceneDepth_905c05d0cfa04274a8511269c1782b79_Out_1_Float);
        float _Lerp_35568103c6e249c689ec4fd27f5e4ddd_Out_3_Float;
        Unity_Lerp_float(_ProjectionParams.z, _ProjectionParams.y, _SceneDepth_905c05d0cfa04274a8511269c1782b79_Out_1_Float, _Lerp_35568103c6e249c689ec4fd27f5e4ddd_Out_3_Float);
        float4 _ScreenPosition_e23a8dc3158f4a659ad17c8b8aff1495_Out_0_Vector4 = IN.ScreenPosition;
        float _Split_f8af98270ae4406cab897d102df99496_R_1_Float = _ScreenPosition_e23a8dc3158f4a659ad17c8b8aff1495_Out_0_Vector4[0];
        float _Split_f8af98270ae4406cab897d102df99496_G_2_Float = _ScreenPosition_e23a8dc3158f4a659ad17c8b8aff1495_Out_0_Vector4[1];
        float _Split_f8af98270ae4406cab897d102df99496_B_3_Float = _ScreenPosition_e23a8dc3158f4a659ad17c8b8aff1495_Out_0_Vector4[2];
        float _Split_f8af98270ae4406cab897d102df99496_A_4_Float = _ScreenPosition_e23a8dc3158f4a659ad17c8b8aff1495_Out_0_Vector4[3];
        float _Lerp_a9295eccb1424170a4e481b272012487_Out_3_Float;
        Unity_Lerp_float(_ProjectionParams.z, _ProjectionParams.y, _Split_f8af98270ae4406cab897d102df99496_B_3_Float, _Lerp_a9295eccb1424170a4e481b272012487_Out_3_Float);
        float _Subtract_e132f361175045cfb3fe9a7dca63a082_Out_2_Float;
        Unity_Subtract_float(_Lerp_35568103c6e249c689ec4fd27f5e4ddd_Out_3_Float, _Lerp_a9295eccb1424170a4e481b272012487_Out_3_Float, _Subtract_e132f361175045cfb3fe9a7dca63a082_Out_2_Float);
        float _Property_c3605d6a15074412bba55544d393b337_Out_0_Float = _DepthFadeDistance;
        float _Divide_bbbe2d9f0c3d47ddab512dc7748b6b89_Out_2_Float;
        Unity_Divide_float(_Subtract_e132f361175045cfb3fe9a7dca63a082_Out_2_Float, _Property_c3605d6a15074412bba55544d393b337_Out_0_Float, _Divide_bbbe2d9f0c3d47ddab512dc7748b6b89_Out_2_Float);
        float _Saturate_2a826d10488946e1b5f71424889b8f7a_Out_1_Float;
        Unity_Saturate_float(_Divide_bbbe2d9f0c3d47ddab512dc7748b6b89_Out_2_Float, _Saturate_2a826d10488946e1b5f71424889b8f7a_Out_1_Float);
        float _OneMinus_a6da5fafbee84ff782930c7bb3c4a8de_Out_1_Float;
        Unity_OneMinus_float(_Saturate_2a826d10488946e1b5f71424889b8f7a_Out_1_Float, _OneMinus_a6da5fafbee84ff782930c7bb3c4a8de_Out_1_Float);
        Linear_2 = _OneMinus_a6da5fafbee84ff782930c7bb3c4a8de_Out_1_Float;
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Posterize_float4(float4 In, float4 Steps, out float4 Out)
        {
            Out = floor(In * Steps) / Steps;
        }
        
        struct Bindings_GetDepth_370b72acb6baa764089512ed583870c4_float
        {
        float3 WorldSpacePosition;
        float4 ScreenPosition;
        float2 NDCPosition;
        half4 uv0;
        float3 TimeParameters;
        };
        
        void SG_GetDepth_370b72acb6baa764089512ed583870c4_float(float _DepthFadeDistance, float _Steps, float _RefractionSpeed, float _RefractionScale, float _GradientRefractionScale, float _RefractionStrength, float4 _ShallowColor, float4 _DeepColor, Bindings_GetDepth_370b72acb6baa764089512ed583870c4_float IN, out float4 RefractedUVs_1, out float4 Color_2)
        {
        float _Property_c733aafdfa2a4bb68b7ced01fffc396b_Out_0_Float = _RefractionSpeed;
        float _Property_89b378a1d2144a94b621a6c09e56b37c_Out_0_Float = _RefractionScale;
        float _Property_af7f8a7d74a74ab0a5f78596da12e191_Out_0_Float = _GradientRefractionScale;
        float _Property_61a396eba2ff468993be562ba48b818c_Out_0_Float = _RefractionStrength;
        Bindings_RefractedUV_dfdc6038fe7bdd5479fef69a0fcdb7ab_float _RefractedUV_fcd804a6c7444188a93a683c15c9f99a;
        _RefractedUV_fcd804a6c7444188a93a683c15c9f99a.NDCPosition = IN.NDCPosition;
        _RefractedUV_fcd804a6c7444188a93a683c15c9f99a.uv0 = IN.uv0;
        _RefractedUV_fcd804a6c7444188a93a683c15c9f99a.TimeParameters = IN.TimeParameters;
        half4 _RefractedUV_fcd804a6c7444188a93a683c15c9f99a_Out_1_Vector4;
        SG_RefractedUV_dfdc6038fe7bdd5479fef69a0fcdb7ab_float(_Property_c733aafdfa2a4bb68b7ced01fffc396b_Out_0_Float, _Property_89b378a1d2144a94b621a6c09e56b37c_Out_0_Float, _Property_af7f8a7d74a74ab0a5f78596da12e191_Out_0_Float, _Property_61a396eba2ff468993be562ba48b818c_Out_0_Float, _RefractedUV_fcd804a6c7444188a93a683c15c9f99a, _RefractedUV_fcd804a6c7444188a93a683c15c9f99a_Out_1_Vector4);
        float4 _Property_573520c11ff4493bb507d97f8a6f35c5_Out_0_Vector4 = _ShallowColor;
        float4 _Property_355869e5a92e4d5cbbacd7ea72fca417_Out_0_Vector4 = _DeepColor;
        float _Property_7a320c1c0e864afbb48839a06a61e877_Out_0_Float = _DepthFadeDistance;
        Bindings_DepthFadePerspective_3069152b34697f24bb49447c02013d98_float _DepthFadePerspective_d9affccc6fe74de2a2be6a12c8551576;
        _DepthFadePerspective_d9affccc6fe74de2a2be6a12c8551576.WorldSpacePosition = IN.WorldSpacePosition;
        _DepthFadePerspective_d9affccc6fe74de2a2be6a12c8551576.ScreenPosition = IN.ScreenPosition;
        float _DepthFadePerspective_d9affccc6fe74de2a2be6a12c8551576_Linear_2_Float;
        SG_DepthFadePerspective_3069152b34697f24bb49447c02013d98_float(_Property_7a320c1c0e864afbb48839a06a61e877_Out_0_Float, (_RefractedUV_fcd804a6c7444188a93a683c15c9f99a_Out_1_Vector4.xy), _DepthFadePerspective_d9affccc6fe74de2a2be6a12c8551576, _DepthFadePerspective_d9affccc6fe74de2a2be6a12c8551576_Linear_2_Float);
        Bindings_DepthFade_1d0eb39db86b53245b582a69cdb87443_float _DepthFade_84a135a9b3d547a7996fb768c62e334a;
        _DepthFade_84a135a9b3d547a7996fb768c62e334a.ScreenPosition = IN.ScreenPosition;
        half _DepthFade_84a135a9b3d547a7996fb768c62e334a_Linear_2_Float;
        SG_DepthFade_1d0eb39db86b53245b582a69cdb87443_float(_Property_7a320c1c0e864afbb48839a06a61e877_Out_0_Float, (_RefractedUV_fcd804a6c7444188a93a683c15c9f99a_Out_1_Vector4.xy), _DepthFade_84a135a9b3d547a7996fb768c62e334a, _DepthFade_84a135a9b3d547a7996fb768c62e334a_Linear_2_Float);
        #if defined(_PERSPECTIVE)
        float _Perspective_9b0c09a153cd4283a1dfa9ae48236cfd_Out_0_Float = _DepthFadePerspective_d9affccc6fe74de2a2be6a12c8551576_Linear_2_Float;
        #else
        float _Perspective_9b0c09a153cd4283a1dfa9ae48236cfd_Out_0_Float = _DepthFade_84a135a9b3d547a7996fb768c62e334a_Linear_2_Float;
        #endif
        float4 _Lerp_228b4c34a3434455a17ce3b570ccbf49_Out_3_Vector4;
        Unity_Lerp_float4(_Property_573520c11ff4493bb507d97f8a6f35c5_Out_0_Vector4, _Property_355869e5a92e4d5cbbacd7ea72fca417_Out_0_Vector4, (_Perspective_9b0c09a153cd4283a1dfa9ae48236cfd_Out_0_Float.xxxx), _Lerp_228b4c34a3434455a17ce3b570ccbf49_Out_3_Vector4);
        float _Property_442a941c64b64104be37b457e429de9f_Out_0_Float = _Steps;
        float4 _Posterize_cc9876a22c014323bf597f3e9f25d3a5_Out_2_Vector4;
        Unity_Posterize_float4(_Lerp_228b4c34a3434455a17ce3b570ccbf49_Out_3_Vector4, (_Property_442a941c64b64104be37b457e429de9f_Out_0_Float.xxxx), _Posterize_cc9876a22c014323bf597f3e9f25d3a5_Out_2_Vector4);
        #if defined(_A_SHORT_HIKE_MODE)
        float4 _AShortHikeMode_940dc24d9c7b4f5184729184326be4bc_Out_0_Vector4 = _Posterize_cc9876a22c014323bf597f3e9f25d3a5_Out_2_Vector4;
        #else
        float4 _AShortHikeMode_940dc24d9c7b4f5184729184326be4bc_Out_0_Vector4 = _Lerp_228b4c34a3434455a17ce3b570ccbf49_Out_3_Vector4;
        #endif
        RefractedUVs_1 = _RefractedUV_fcd804a6c7444188a93a683c15c9f99a_Out_1_Vector4;
        Color_2 = _AShortHikeMode_940dc24d9c7b4f5184729184326be4bc_Out_0_Vector4;
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), ViewDir))), Power);
        }
        
        struct Bindings_FresnelCalculation_f346593723353f4488f8a099aef520eb_float
        {
        float3 WorldSpaceNormal;
        float3 ViewSpaceNormal;
        float3 WorldSpaceViewDirection;
        };
        
        void SG_FresnelCalculation_f346593723353f4488f8a099aef520eb_float(float _HorizonDistance, Bindings_FresnelCalculation_f346593723353f4488f8a099aef520eb_float IN, out float FresnelResult_1)
        {
        float _Property_bb1b567c8cd54712be0aa1ab56158c06_Out_0_Float = _HorizonDistance;
        float _FresnelEffect_5cb1ab2db50c4d85aa1315d66acc019b_Out_3_Float;
        Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_bb1b567c8cd54712be0aa1ab56158c06_Out_0_Float, _FresnelEffect_5cb1ab2db50c4d85aa1315d66acc019b_Out_3_Float);
        float3 _Vector3_3df06ab61c94407b9b2bb68b2bd8c970_Out_0_Vector3 = float3(float(0), float(0), float(1));
        float _Property_3a9a52a5e7f84053bce12874f9ecb16e_Out_0_Float = _HorizonDistance;
        float _FresnelEffect_d5defa2cb078451f8600e86b230e0a30_Out_3_Float;
        Unity_FresnelEffect_float(IN.ViewSpaceNormal, _Vector3_3df06ab61c94407b9b2bb68b2bd8c970_Out_0_Vector3, _Property_3a9a52a5e7f84053bce12874f9ecb16e_Out_0_Float, _FresnelEffect_d5defa2cb078451f8600e86b230e0a30_Out_3_Float);
        #if defined(_PERSPECTIVE)
        float _Perspective_e2d93f83b3444bdc9b039aff2c3f109d_Out_0_Float = _FresnelEffect_5cb1ab2db50c4d85aa1315d66acc019b_Out_3_Float;
        #else
        float _Perspective_e2d93f83b3444bdc9b039aff2c3f109d_Out_0_Float = _FresnelEffect_d5defa2cb078451f8600e86b230e0a30_Out_3_Float;
        #endif
        FresnelResult_1 = _Perspective_e2d93f83b3444bdc9b039aff2c3f109d_Out_0_Float;
        }
        
        void Unity_SceneColor_float(float4 UV, out float3 Out)
        {
            Out = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV.xy);
        }
        
        struct Bindings_BlendObjectColor_a02636e967a650d4db669ee613538c46_float
        {
        };
        
        void SG_BlendObjectColor_a02636e967a650d4db669ee613538c46_float(float4 _RefractedUVs, float4 _BaseBlend, Bindings_BlendObjectColor_a02636e967a650d4db669ee613538c46_float IN, out float3 ObjectBlend_1)
        {
        float4 _Property_4f56835f385c4497ad58269d3e3ce6ed_Out_0_Vector4 = _RefractedUVs;
        float3 _SceneColor_b32346b7705c4334a0ac120dcde6901b_Out_1_Vector3;
        Unity_SceneColor_float(_Property_4f56835f385c4497ad58269d3e3ce6ed_Out_0_Vector4, _SceneColor_b32346b7705c4334a0ac120dcde6901b_Out_1_Vector3);
        float4 _Property_8790a77a1a4f4a63b11b3c4719708e2d_Out_0_Vector4 = _BaseBlend;
        float _Split_e4aad686056f4ef79a23e5ee7e845512_R_1_Float = _Property_8790a77a1a4f4a63b11b3c4719708e2d_Out_0_Vector4[0];
        float _Split_e4aad686056f4ef79a23e5ee7e845512_G_2_Float = _Property_8790a77a1a4f4a63b11b3c4719708e2d_Out_0_Vector4[1];
        float _Split_e4aad686056f4ef79a23e5ee7e845512_B_3_Float = _Property_8790a77a1a4f4a63b11b3c4719708e2d_Out_0_Vector4[2];
        float _Split_e4aad686056f4ef79a23e5ee7e845512_A_4_Float = _Property_8790a77a1a4f4a63b11b3c4719708e2d_Out_0_Vector4[3];
        float _OneMinus_dd70c5f5558d4199a84216d1b5da52c2_Out_1_Float;
        Unity_OneMinus_float(_Split_e4aad686056f4ef79a23e5ee7e845512_A_4_Float, _OneMinus_dd70c5f5558d4199a84216d1b5da52c2_Out_1_Float);
        float3 _Multiply_b7686a561f2542a8b83587eee957226c_Out_2_Vector3;
        Unity_Multiply_float3_float3(_SceneColor_b32346b7705c4334a0ac120dcde6901b_Out_1_Vector3, (_OneMinus_dd70c5f5558d4199a84216d1b5da52c2_Out_1_Float.xxx), _Multiply_b7686a561f2542a8b83587eee957226c_Out_2_Vector3);
        ObjectBlend_1 = _Multiply_b7686a561f2542a8b83587eee957226c_Out_2_Vector3;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Negate_float2(float2 In, out float2 Out)
        {
            Out = -1 * In;
        }
        
        struct Bindings_CalculateUVs_60d47073ac03df54f9a15991680de154_float
        {
        float3 WorldSpacePosition;
        half4 uv0;
        };
        
        void SG_CalculateUVs_60d47073ac03df54f9a15991680de154_float(Bindings_CalculateUVs_60d47073ac03df54f9a15991680de154_float IN, out float2 OutUVs_1)
        {
        float _Split_3c90832226cd494ea28785fdc0cd3dc9_R_1_Float = IN.WorldSpacePosition[0];
        float _Split_3c90832226cd494ea28785fdc0cd3dc9_G_2_Float = IN.WorldSpacePosition[1];
        float _Split_3c90832226cd494ea28785fdc0cd3dc9_B_3_Float = IN.WorldSpacePosition[2];
        float _Split_3c90832226cd494ea28785fdc0cd3dc9_A_4_Float = 0;
        float2 _Vector2_536cab5e22a44d6d811a24b0e0e70684_Out_0_Vector2 = float2(_Split_3c90832226cd494ea28785fdc0cd3dc9_R_1_Float, _Split_3c90832226cd494ea28785fdc0cd3dc9_B_3_Float);
        float _Float_d2a1470c5d4f42498583a13546adc89f_Out_0_Float = float(0.1);
        float2 _Multiply_72d6d69cb24149cf8949c1b441fd6553_Out_2_Vector2;
        Unity_Multiply_float2_float2(_Vector2_536cab5e22a44d6d811a24b0e0e70684_Out_0_Vector2, (_Float_d2a1470c5d4f42498583a13546adc89f_Out_0_Float.xx), _Multiply_72d6d69cb24149cf8949c1b441fd6553_Out_2_Vector2);
        float2 _Negate_ea982ed93e8b41c1acbc0c7beddc5cb0_Out_1_Vector2;
        Unity_Negate_float2(_Multiply_72d6d69cb24149cf8949c1b441fd6553_Out_2_Vector2, _Negate_ea982ed93e8b41c1acbc0c7beddc5cb0_Out_1_Vector2);
        float4 _UV_8ca0102c0c1e4dd6b63c964274692cf2_Out_0_Vector4 = IN.uv0;
        #if defined(_WORLD_SPACE_UV)
        float2 _WorldSpaceUV_1110b220d2374503b2e7abe001c6420a_Out_0_Vector2 = _Negate_ea982ed93e8b41c1acbc0c7beddc5cb0_Out_1_Vector2;
        #else
        float2 _WorldSpaceUV_1110b220d2374503b2e7abe001c6420a_Out_0_Vector2 = (_UV_8ca0102c0c1e4dd6b63c964274692cf2_Out_0_Vector4.xy);
        #endif
        OutUVs_1 = _WorldSpaceUV_1110b220d2374503b2e7abe001c6420a_Out_0_Vector2;
        }
        
        void Unity_InvertColors_float4(float4 In, float4 InvertColors, out float4 Out)
        {
        Out = abs(InvertColors - In);
        }
        
        struct Bindings_InvertColors_a5c43e269304bff4a91c002de46388cb_float
        {
        };
        
        void SG_InvertColors_a5c43e269304bff4a91c002de46388cb_float(float4 _InputColor, Bindings_InvertColors_a5c43e269304bff4a91c002de46388cb_float IN, out float4 Output_1)
        {
        float4 _Property_3ba350a2ed3c45a6b7b551a667768c4f_Out_0_Vector4 = _InputColor;
        float4 _InvertColors_04f15eda597141b2a2df5fd6f33e184d_Out_1_Vector4;
        float4 _InvertColors_04f15eda597141b2a2df5fd6f33e184d_InvertColors = float4 (1, 1, 1, 0);
        Unity_InvertColors_float4(_Property_3ba350a2ed3c45a6b7b551a667768c4f_Out_0_Vector4, _InvertColors_04f15eda597141b2a2df5fd6f33e184d_InvertColors, _InvertColors_04f15eda597141b2a2df5fd6f33e184d_Out_1_Vector4);
        Output_1 = _InvertColors_04f15eda597141b2a2df5fd6f33e184d_Out_1_Vector4;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
        Out = A * B;
        }
        
        struct Bindings_Caves_3e5c142028ba07d48bba33c5843ba17e_float
        {
        };
        
        void SG_Caves_3e5c142028ba07d48bba33c5843ba17e_float(float2 _UVs, float2 _CaveScale, float2 _CaveOffset, float _CaveDistortion, UnityTexture2D _CaveTexture, float4 _CaveColor, float _CaveSteps, Bindings_Caves_3e5c142028ba07d48bba33c5843ba17e_float IN, out float4 CaveResult_1)
        {
        float4 _Property_cd4c3ae601304329890ad7ec6b15bd87_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_CaveColor) : _CaveColor;
        float _Split_5dc03030aee348088967c93897290323_R_1_Float = _Property_cd4c3ae601304329890ad7ec6b15bd87_Out_0_Vector4[0];
        float _Split_5dc03030aee348088967c93897290323_G_2_Float = _Property_cd4c3ae601304329890ad7ec6b15bd87_Out_0_Vector4[1];
        float _Split_5dc03030aee348088967c93897290323_B_3_Float = _Property_cd4c3ae601304329890ad7ec6b15bd87_Out_0_Vector4[2];
        float _Split_5dc03030aee348088967c93897290323_A_4_Float = _Property_cd4c3ae601304329890ad7ec6b15bd87_Out_0_Vector4[3];
        UnityTexture2D _Property_3b2b9c24469d4f2ba6d01ced7669fe09_Out_0_Texture2D = _CaveTexture;
        float2 _Property_402263488ae84f86891ad666a842ae5b_Out_0_Vector2 = _UVs;
        float2 _Property_a282ff33e0ce4b5f82f90283e3462360_Out_0_Vector2 = _CaveScale;
        float2 _Property_3e7ad0d7638f4386bf8a554e3efafc78_Out_0_Vector2 = _CaveOffset;
        float2 _TilingAndOffset_7cf0fae9a7aa4e29a98c855468b7ac0d_Out_3_Vector2;
        Unity_TilingAndOffset_float(_Property_402263488ae84f86891ad666a842ae5b_Out_0_Vector2, _Property_a282ff33e0ce4b5f82f90283e3462360_Out_0_Vector2, _Property_3e7ad0d7638f4386bf8a554e3efafc78_Out_0_Vector2, _TilingAndOffset_7cf0fae9a7aa4e29a98c855468b7ac0d_Out_3_Vector2);
        float _Property_a056c657e9bc40398fd6f7d24aaa536a_Out_0_Float = _CaveDistortion;
        float2 _DistortUVCustomFunction_b45cce0f371c47409824f9da6f9978e4_Out_2_Vector2;
        DistortUV_float(_TilingAndOffset_7cf0fae9a7aa4e29a98c855468b7ac0d_Out_3_Vector2, _Property_a056c657e9bc40398fd6f7d24aaa536a_Out_0_Float, _DistortUVCustomFunction_b45cce0f371c47409824f9da6f9978e4_Out_2_Vector2);
        float4 _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_3b2b9c24469d4f2ba6d01ced7669fe09_Out_0_Texture2D.tex, _Property_3b2b9c24469d4f2ba6d01ced7669fe09_Out_0_Texture2D.samplerstate, _Property_3b2b9c24469d4f2ba6d01ced7669fe09_Out_0_Texture2D.GetTransformedUV(_DistortUVCustomFunction_b45cce0f371c47409824f9da6f9978e4_Out_2_Vector2) );
        float _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_R_4_Float = _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_RGBA_0_Vector4.r;
        float _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_G_5_Float = _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_RGBA_0_Vector4.g;
        float _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_B_6_Float = _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_RGBA_0_Vector4.b;
        float _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_A_7_Float = _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_RGBA_0_Vector4.a;
        Bindings_InvertColors_a5c43e269304bff4a91c002de46388cb_float _InvertColors_510d701d607d46aabb64028a0d8d87dc;
        float4 _InvertColors_510d701d607d46aabb64028a0d8d87dc_Output_1_Vector4;
        SG_InvertColors_a5c43e269304bff4a91c002de46388cb_float((_SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_R_4_Float.xxxx), _InvertColors_510d701d607d46aabb64028a0d8d87dc, _InvertColors_510d701d607d46aabb64028a0d8d87dc_Output_1_Vector4);
        float4 _Multiply_2465dbb958a04bb692c3d0cd819466f0_Out_2_Vector4;
        Unity_Multiply_float4_float4((_Split_5dc03030aee348088967c93897290323_A_4_Float.xxxx), _InvertColors_510d701d607d46aabb64028a0d8d87dc_Output_1_Vector4, _Multiply_2465dbb958a04bb692c3d0cd819466f0_Out_2_Vector4);
        CaveResult_1 = _Multiply_2465dbb958a04bb692c3d0cd819466f0_Out_2_Vector4;
        }
        
        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_Reflections_2a83053698b0a9d46a141dd7d36ac2ac_float
        {
        float2 NDCPosition;
        };
        
        void SG_Reflections_2a83053698b0a9d46a141dd7d36ac2ac_float(UnityTexture2D _ReflectionMap, float _ReflectionDistortion, float _ReflectionBlend, Bindings_Reflections_2a83053698b0a9d46a141dd7d36ac2ac_float IN, out float4 ReflexColor_1, out float ReflexResult_2)
        {
        UnityTexture2D _Property_01b9233f96594a05b2c16963f58eb91b_Out_0_Texture2D = _ReflectionMap;
        float4 _ScreenPosition_da0e6619fc3f4e22984f43026a0b5c6f_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
        float _Property_4aeb41f2c0de45b8959feed6af6998ee_Out_0_Float = _ReflectionDistortion;
        float2 _DistortUVCustomFunction_3f714cd044804cf598e93af0be6d96e0_Out_2_Vector2;
        DistortUV_float((_ScreenPosition_da0e6619fc3f4e22984f43026a0b5c6f_Out_0_Vector4.xy), _Property_4aeb41f2c0de45b8959feed6af6998ee_Out_0_Float, _DistortUVCustomFunction_3f714cd044804cf598e93af0be6d96e0_Out_2_Vector2);
        float4 _SampleTexture2D_143e4398349948279eee045e0f9170eb_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_01b9233f96594a05b2c16963f58eb91b_Out_0_Texture2D.tex, _Property_01b9233f96594a05b2c16963f58eb91b_Out_0_Texture2D.samplerstate, _Property_01b9233f96594a05b2c16963f58eb91b_Out_0_Texture2D.GetTransformedUV(_DistortUVCustomFunction_3f714cd044804cf598e93af0be6d96e0_Out_2_Vector2) );
        float _SampleTexture2D_143e4398349948279eee045e0f9170eb_R_4_Float = _SampleTexture2D_143e4398349948279eee045e0f9170eb_RGBA_0_Vector4.r;
        float _SampleTexture2D_143e4398349948279eee045e0f9170eb_G_5_Float = _SampleTexture2D_143e4398349948279eee045e0f9170eb_RGBA_0_Vector4.g;
        float _SampleTexture2D_143e4398349948279eee045e0f9170eb_B_6_Float = _SampleTexture2D_143e4398349948279eee045e0f9170eb_RGBA_0_Vector4.b;
        float _SampleTexture2D_143e4398349948279eee045e0f9170eb_A_7_Float = _SampleTexture2D_143e4398349948279eee045e0f9170eb_RGBA_0_Vector4.a;
        float _Property_7a32419bb4c146e68196c2d5904b5441_Out_0_Float = _ReflectionBlend;
        ReflexColor_1 = _SampleTexture2D_143e4398349948279eee045e0f9170eb_RGBA_0_Vector4;
        ReflexResult_2 = _Property_7a32419bb4c146e68196c2d5904b5441_Out_0_Float;
        }
        
        void Unity_Cosine_float(float In, out float Out)
        {
            Out = cos(In);
        }
        
        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }
        
        void Unity_Normalize_float2(float2 In, out float2 Out)
        {
            Out = normalize(In);
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_PanningUVs_ef286e626cee63841803a024235a644f_float
        {
        float3 TimeParameters;
        };
        
        void SG_PanningUVs_ef286e626cee63841803a024235a644f_float(float _Direction, float _Speed, float2 _UV, float _Tiling, float2 _Offset, Bindings_PanningUVs_ef286e626cee63841803a024235a644f_float IN, out float2 UVOut_1)
        {
        float2 _Property_fa3bd6908dc14afe9e9a8fa8f17ecdb7_Out_0_Vector2 = _UV;
        float _Property_942aaf71f8624c5b9c5b0d1dd04d30f7_Out_0_Float = _Tiling;
        float2 _Multiply_d04caa119e264eca905786c7590f6758_Out_2_Vector2;
        Unity_Multiply_float2_float2(_Property_fa3bd6908dc14afe9e9a8fa8f17ecdb7_Out_0_Vector2, (_Property_942aaf71f8624c5b9c5b0d1dd04d30f7_Out_0_Float.xx), _Multiply_d04caa119e264eca905786c7590f6758_Out_2_Vector2);
        float _Property_c88c4b7bebfd4fc9bf845a4e710e2e20_Out_0_Float = _Direction;
        float _Multiply_b7c33bfe44644595a67b0bcaad29d479_Out_2_Float;
        Unity_Multiply_float_float(_Property_c88c4b7bebfd4fc9bf845a4e710e2e20_Out_0_Float, 2, _Multiply_b7c33bfe44644595a67b0bcaad29d479_Out_2_Float);
        float _Subtract_bd70d912c9e44cb4b775def278ab8299_Out_2_Float;
        Unity_Subtract_float(_Multiply_b7c33bfe44644595a67b0bcaad29d479_Out_2_Float, float(1), _Subtract_bd70d912c9e44cb4b775def278ab8299_Out_2_Float);
        float Constant_469e2f41ab9a469fbded168e7b78fd45 = 3.141593;
        float _Multiply_a2e7e533449a4a72879d25e5c791d0bb_Out_2_Float;
        Unity_Multiply_float_float(_Subtract_bd70d912c9e44cb4b775def278ab8299_Out_2_Float, Constant_469e2f41ab9a469fbded168e7b78fd45, _Multiply_a2e7e533449a4a72879d25e5c791d0bb_Out_2_Float);
        float _Cosine_3e0d75ecc2d14605aad7642b8bcdc461_Out_1_Float;
        Unity_Cosine_float(_Multiply_a2e7e533449a4a72879d25e5c791d0bb_Out_2_Float, _Cosine_3e0d75ecc2d14605aad7642b8bcdc461_Out_1_Float);
        float _Sine_d7fde7b84a36487ca0571ab4b845689a_Out_1_Float;
        Unity_Sine_float(_Multiply_a2e7e533449a4a72879d25e5c791d0bb_Out_2_Float, _Sine_d7fde7b84a36487ca0571ab4b845689a_Out_1_Float);
        float2 _Vector2_5822526fa5384bc2993095be23b1ac5c_Out_0_Vector2 = float2(_Cosine_3e0d75ecc2d14605aad7642b8bcdc461_Out_1_Float, _Sine_d7fde7b84a36487ca0571ab4b845689a_Out_1_Float);
        float2 _Normalize_7ef1b5e1c36845dcb3532f1070eab07d_Out_1_Vector2;
        Unity_Normalize_float2(_Vector2_5822526fa5384bc2993095be23b1ac5c_Out_0_Vector2, _Normalize_7ef1b5e1c36845dcb3532f1070eab07d_Out_1_Vector2);
        float _Property_06fb219b3c4348db8a16781439392559_Out_0_Float = _Speed;
        float _Multiply_0b0996e501624102825e5d908b3a7ded_Out_2_Float;
        Unity_Multiply_float_float(IN.TimeParameters.x, _Property_06fb219b3c4348db8a16781439392559_Out_0_Float, _Multiply_0b0996e501624102825e5d908b3a7ded_Out_2_Float);
        float2 _Multiply_e6e19d2e699747abafa7335b844aa827_Out_2_Vector2;
        Unity_Multiply_float2_float2(_Normalize_7ef1b5e1c36845dcb3532f1070eab07d_Out_1_Vector2, (_Multiply_0b0996e501624102825e5d908b3a7ded_Out_2_Float.xx), _Multiply_e6e19d2e699747abafa7335b844aa827_Out_2_Vector2);
        float2 _Add_0cf695f5db0b477e9565fc781fea6cab_Out_2_Vector2;
        Unity_Add_float2(_Multiply_d04caa119e264eca905786c7590f6758_Out_2_Vector2, _Multiply_e6e19d2e699747abafa7335b844aa827_Out_2_Vector2, _Add_0cf695f5db0b477e9565fc781fea6cab_Out_2_Vector2);
        float2 _Property_0b770ef0e6db42bba96e87ccc9764d0a_Out_0_Vector2 = _Offset;
        float2 _Add_da503712d45c458e95b43c94682472af_Out_2_Vector2;
        Unity_Add_float2(_Add_0cf695f5db0b477e9565fc781fea6cab_Out_2_Vector2, _Property_0b770ef0e6db42bba96e87ccc9764d0a_Out_0_Vector2, _Add_da503712d45c458e95b43c94682472af_Out_2_Vector2);
        UVOut_1 = _Add_da503712d45c458e95b43c94682472af_Out_2_Vector2;
        }
        
        struct Bindings_SecondaryFoam_a707540033b78f44bbef5139f65097d0_float
        {
        float3 TimeParameters;
        };
        
        void SG_SecondaryFoam_a707540033b78f44bbef5139f65097d0_float(float _FoamDirection, float _FoamSpeed, float2 _UVs, float _FoamTiling, float2 _FoamOffset, float _FoamDistortion, UnityTexture2D _FoamTexture, float4 _FoamColor, Bindings_SecondaryFoam_a707540033b78f44bbef5139f65097d0_float IN, out float4 SecondaryFoamResult_1)
        {
        UnityTexture2D _Property_5cd791bd402243e9b2fe160b9ad987dd_Out_0_Texture2D = _FoamTexture;
        float _Property_639af70a388d4c20b6ba8f1a885ccce2_Out_0_Float = _FoamDirection;
        float _Property_bd29fc115c4546dc958cdc06b9143f4a_Out_0_Float = _FoamSpeed;
        float2 _Property_bce6ecfee5474ddab7a5d7f51f6cc08b_Out_0_Vector2 = _UVs;
        float _Property_598525370a5f40929f9000a2467ce4f9_Out_0_Float = _FoamTiling;
        float2 _Property_caa367a036da4cad8383f95ca75df903_Out_0_Vector2 = _FoamOffset;
        Bindings_PanningUVs_ef286e626cee63841803a024235a644f_float _PanningUVs_34a31afa5e2346d5b3aaa70850f3527c;
        _PanningUVs_34a31afa5e2346d5b3aaa70850f3527c.TimeParameters = IN.TimeParameters;
        half2 _PanningUVs_34a31afa5e2346d5b3aaa70850f3527c_UVOut_1_Vector2;
        SG_PanningUVs_ef286e626cee63841803a024235a644f_float(_Property_639af70a388d4c20b6ba8f1a885ccce2_Out_0_Float, _Property_bd29fc115c4546dc958cdc06b9143f4a_Out_0_Float, _Property_bce6ecfee5474ddab7a5d7f51f6cc08b_Out_0_Vector2, _Property_598525370a5f40929f9000a2467ce4f9_Out_0_Float, _Property_caa367a036da4cad8383f95ca75df903_Out_0_Vector2, _PanningUVs_34a31afa5e2346d5b3aaa70850f3527c, _PanningUVs_34a31afa5e2346d5b3aaa70850f3527c_UVOut_1_Vector2);
        float _Property_93624abbebe64bc6a23ac0fcc02ec55d_Out_0_Float = _FoamDistortion;
        float2 _DistortUVCustomFunction_40d46d29f5404679973fed10fa7b54ff_Out_2_Vector2;
        DistortUV_float(_PanningUVs_34a31afa5e2346d5b3aaa70850f3527c_UVOut_1_Vector2, _Property_93624abbebe64bc6a23ac0fcc02ec55d_Out_0_Float, _DistortUVCustomFunction_40d46d29f5404679973fed10fa7b54ff_Out_2_Vector2);
        float4 _SampleTexture2D_1a216311884a4e8683fd5938622d3402_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_5cd791bd402243e9b2fe160b9ad987dd_Out_0_Texture2D.tex, _Property_5cd791bd402243e9b2fe160b9ad987dd_Out_0_Texture2D.samplerstate, _Property_5cd791bd402243e9b2fe160b9ad987dd_Out_0_Texture2D.GetTransformedUV(_DistortUVCustomFunction_40d46d29f5404679973fed10fa7b54ff_Out_2_Vector2) );
        float _SampleTexture2D_1a216311884a4e8683fd5938622d3402_R_4_Float = _SampleTexture2D_1a216311884a4e8683fd5938622d3402_RGBA_0_Vector4.r;
        float _SampleTexture2D_1a216311884a4e8683fd5938622d3402_G_5_Float = _SampleTexture2D_1a216311884a4e8683fd5938622d3402_RGBA_0_Vector4.g;
        float _SampleTexture2D_1a216311884a4e8683fd5938622d3402_B_6_Float = _SampleTexture2D_1a216311884a4e8683fd5938622d3402_RGBA_0_Vector4.b;
        float _SampleTexture2D_1a216311884a4e8683fd5938622d3402_A_7_Float = _SampleTexture2D_1a216311884a4e8683fd5938622d3402_RGBA_0_Vector4.a;
        float4 _Property_236d6b687b6444c0b1db6e064076ddf4_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_FoamColor) : _FoamColor;
        float _Split_29237e4769584ccab6c7fad57c825ede_R_1_Float = _Property_236d6b687b6444c0b1db6e064076ddf4_Out_0_Vector4[0];
        float _Split_29237e4769584ccab6c7fad57c825ede_G_2_Float = _Property_236d6b687b6444c0b1db6e064076ddf4_Out_0_Vector4[1];
        float _Split_29237e4769584ccab6c7fad57c825ede_B_3_Float = _Property_236d6b687b6444c0b1db6e064076ddf4_Out_0_Vector4[2];
        float _Split_29237e4769584ccab6c7fad57c825ede_A_4_Float = _Property_236d6b687b6444c0b1db6e064076ddf4_Out_0_Vector4[3];
        float _Multiply_7e4e840580f44d6d816c3daffe008e63_Out_2_Float;
        Unity_Multiply_float_float(_SampleTexture2D_1a216311884a4e8683fd5938622d3402_R_4_Float, _Split_29237e4769584ccab6c7fad57c825ede_A_4_Float, _Multiply_7e4e840580f44d6d816c3daffe008e63_Out_2_Float);
        SecondaryFoamResult_1 = (_Multiply_7e4e840580f44d6d816c3daffe008e63_Out_2_Float.xxxx);
        }
        
        struct Bindings_PrimaryFoam_f41bb18b12540894cadcf85fcc438801_float
        {
        float3 TimeParameters;
        };
        
        void SG_PrimaryFoam_f41bb18b12540894cadcf85fcc438801_float(float _FoamDirection, float _FoamSpeed, float _FoamTiling, float _FoamDistortion, float2 _UVS, UnityTexture2D _FoamTexture, float4 _FoamColor, Bindings_PrimaryFoam_f41bb18b12540894cadcf85fcc438801_float IN, out float4 FoamResult_1)
        {
        UnityTexture2D _Property_f3f9f2f74aba4fbf9a3ce681a0f63fb9_Out_0_Texture2D = _FoamTexture;
        float _Property_da04d3c39d5743c58c63df85172de1d9_Out_0_Float = _FoamDirection;
        float _Property_d2db43137223470fb028d2186633912f_Out_0_Float = _FoamSpeed;
        float2 _Property_65972f26cc794487af7180bcb97a89bc_Out_0_Vector2 = _UVS;
        float _Property_f3b4abaec7fc41ff9898ad9f70756e98_Out_0_Float = _FoamTiling;
        Bindings_PanningUVs_ef286e626cee63841803a024235a644f_float _PanningUVs_aaf741e14dc140e7b99b861bdf19f098;
        _PanningUVs_aaf741e14dc140e7b99b861bdf19f098.TimeParameters = IN.TimeParameters;
        half2 _PanningUVs_aaf741e14dc140e7b99b861bdf19f098_UVOut_1_Vector2;
        SG_PanningUVs_ef286e626cee63841803a024235a644f_float(_Property_da04d3c39d5743c58c63df85172de1d9_Out_0_Float, _Property_d2db43137223470fb028d2186633912f_Out_0_Float, _Property_65972f26cc794487af7180bcb97a89bc_Out_0_Vector2, _Property_f3b4abaec7fc41ff9898ad9f70756e98_Out_0_Float, half2 (0, 0), _PanningUVs_aaf741e14dc140e7b99b861bdf19f098, _PanningUVs_aaf741e14dc140e7b99b861bdf19f098_UVOut_1_Vector2);
        float _Property_e4d9b3d9ed934e7dab9f09af0b6e6b6e_Out_0_Float = _FoamDistortion;
        float2 _DistortUVCustomFunction_0552020a4ba34f43a99711cb8a336320_Out_2_Vector2;
        DistortUV_float(_PanningUVs_aaf741e14dc140e7b99b861bdf19f098_UVOut_1_Vector2, _Property_e4d9b3d9ed934e7dab9f09af0b6e6b6e_Out_0_Float, _DistortUVCustomFunction_0552020a4ba34f43a99711cb8a336320_Out_2_Vector2);
        float4 _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_f3f9f2f74aba4fbf9a3ce681a0f63fb9_Out_0_Texture2D.tex, _Property_f3f9f2f74aba4fbf9a3ce681a0f63fb9_Out_0_Texture2D.samplerstate, _Property_f3f9f2f74aba4fbf9a3ce681a0f63fb9_Out_0_Texture2D.GetTransformedUV(_DistortUVCustomFunction_0552020a4ba34f43a99711cb8a336320_Out_2_Vector2) );
        float _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_R_4_Float = _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_RGBA_0_Vector4.r;
        float _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_G_5_Float = _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_RGBA_0_Vector4.g;
        float _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_B_6_Float = _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_RGBA_0_Vector4.b;
        float _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_A_7_Float = _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_RGBA_0_Vector4.a;
        float4 _Property_6db672f2d1e64c12bad7ab94896b619b_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_FoamColor) : _FoamColor;
        float _Split_74f92acb87644a86ad859aab45ff575e_R_1_Float = _Property_6db672f2d1e64c12bad7ab94896b619b_Out_0_Vector4[0];
        float _Split_74f92acb87644a86ad859aab45ff575e_G_2_Float = _Property_6db672f2d1e64c12bad7ab94896b619b_Out_0_Vector4[1];
        float _Split_74f92acb87644a86ad859aab45ff575e_B_3_Float = _Property_6db672f2d1e64c12bad7ab94896b619b_Out_0_Vector4[2];
        float _Split_74f92acb87644a86ad859aab45ff575e_A_4_Float = _Property_6db672f2d1e64c12bad7ab94896b619b_Out_0_Vector4[3];
        float _Multiply_016b5234eb4847c5961f4a6df79e55ce_Out_2_Float;
        Unity_Multiply_float_float(_SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_R_4_Float, _Split_74f92acb87644a86ad859aab45ff575e_A_4_Float, _Multiply_016b5234eb4847c5961f4a6df79e55ce_Out_2_Float);
        FoamResult_1 = (_Multiply_016b5234eb4847c5961f4a6df79e55ce_Out_2_Float.xxxx);
        }
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        struct Bindings_IntersectionFoam_e12de5bce35668a4e8839cbe5732d905_float
        {
        float3 WorldSpacePosition;
        float4 ScreenPosition;
        float2 NDCPosition;
        float3 TimeParameters;
        };
        
        void SG_IntersectionFoam_e12de5bce35668a4e8839cbe5732d905_float(float _IntersectionFoamDepth, float _IntersectionFoamDirection, float _IntersectionFoamSpeed, float2 _UVs, float _IntersectionFoamTiling, float _IntersectionFoamFade, float _IntersectionFoamCutoff, UnityTexture2D _IntersectionFoamTexture, float4 _IntersectionFoamColor, Bindings_IntersectionFoam_e12de5bce35668a4e8839cbe5732d905_float IN, out float4 IntersectionFoamResult_1)
        {
        float _Property_436db722ba574b9da5152ea0b0fc269a_Out_0_Float = _IntersectionFoamDepth;
        float4 _ScreenPosition_d0ee1edadfe44fe1a6aa5df080569063_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
        Bindings_DepthFadePerspective_3069152b34697f24bb49447c02013d98_float _DepthFadePerspective_170dce4f8e3a4f75b1113c08817d6d10;
        _DepthFadePerspective_170dce4f8e3a4f75b1113c08817d6d10.WorldSpacePosition = IN.WorldSpacePosition;
        _DepthFadePerspective_170dce4f8e3a4f75b1113c08817d6d10.ScreenPosition = IN.ScreenPosition;
        float _DepthFadePerspective_170dce4f8e3a4f75b1113c08817d6d10_Linear_2_Float;
        SG_DepthFadePerspective_3069152b34697f24bb49447c02013d98_float(_Property_436db722ba574b9da5152ea0b0fc269a_Out_0_Float, (_ScreenPosition_d0ee1edadfe44fe1a6aa5df080569063_Out_0_Vector4.xy), _DepthFadePerspective_170dce4f8e3a4f75b1113c08817d6d10, _DepthFadePerspective_170dce4f8e3a4f75b1113c08817d6d10_Linear_2_Float);
        Bindings_DepthFade_1d0eb39db86b53245b582a69cdb87443_float _DepthFade_011030f3d4d0420b919705a8a53dedb6;
        _DepthFade_011030f3d4d0420b919705a8a53dedb6.ScreenPosition = IN.ScreenPosition;
        half _DepthFade_011030f3d4d0420b919705a8a53dedb6_Linear_2_Float;
        SG_DepthFade_1d0eb39db86b53245b582a69cdb87443_float(_Property_436db722ba574b9da5152ea0b0fc269a_Out_0_Float, (_ScreenPosition_d0ee1edadfe44fe1a6aa5df080569063_Out_0_Vector4.xy), _DepthFade_011030f3d4d0420b919705a8a53dedb6, _DepthFade_011030f3d4d0420b919705a8a53dedb6_Linear_2_Float);
        #if defined(_PERSPECTIVE)
        float _Perspective_917cfd8f51134dddafbdf7fcdbc16980_Out_0_Float = _DepthFadePerspective_170dce4f8e3a4f75b1113c08817d6d10_Linear_2_Float;
        #else
        float _Perspective_917cfd8f51134dddafbdf7fcdbc16980_Out_0_Float = _DepthFade_011030f3d4d0420b919705a8a53dedb6_Linear_2_Float;
        #endif
        float _Property_2c38ceda851245a9af3cedf5fa670532_Out_0_Float = _IntersectionFoamCutoff;
        float _Multiply_66d0b19b85644261a46d60dd4513434e_Out_2_Float;
        Unity_Multiply_float_float(_Perspective_917cfd8f51134dddafbdf7fcdbc16980_Out_0_Float, _Property_2c38ceda851245a9af3cedf5fa670532_Out_0_Float, _Multiply_66d0b19b85644261a46d60dd4513434e_Out_2_Float);
        float _OneMinus_879cfebaf09a4041ba20d931184cf4f9_Out_1_Float;
        Unity_OneMinus_float(_Multiply_66d0b19b85644261a46d60dd4513434e_Out_2_Float, _OneMinus_879cfebaf09a4041ba20d931184cf4f9_Out_1_Float);
        UnityTexture2D _Property_bc5252e2c5b74edbaa8ba7f5a162d720_Out_0_Texture2D = _IntersectionFoamTexture;
        float _Property_1f3b9e9e32a54263b64e2dfaa1da2891_Out_0_Float = _IntersectionFoamDirection;
        float _Property_940a015d94754f2cb866cb17053cdcc9_Out_0_Float = _IntersectionFoamSpeed;
        float2 _Property_32a7ac96a5dc47bd88f9784f3e38a5ab_Out_0_Vector2 = _UVs;
        float _Property_6fc7fe69b6fc4920b11d0aee35f0c39a_Out_0_Float = _IntersectionFoamTiling;
        Bindings_PanningUVs_ef286e626cee63841803a024235a644f_float _PanningUVs_b5fb72b5c5f74791bff52b9acb830aa0;
        _PanningUVs_b5fb72b5c5f74791bff52b9acb830aa0.TimeParameters = IN.TimeParameters;
        half2 _PanningUVs_b5fb72b5c5f74791bff52b9acb830aa0_UVOut_1_Vector2;
        SG_PanningUVs_ef286e626cee63841803a024235a644f_float(_Property_1f3b9e9e32a54263b64e2dfaa1da2891_Out_0_Float, _Property_940a015d94754f2cb866cb17053cdcc9_Out_0_Float, _Property_32a7ac96a5dc47bd88f9784f3e38a5ab_Out_0_Vector2, _Property_6fc7fe69b6fc4920b11d0aee35f0c39a_Out_0_Float, half2 (0, 0), _PanningUVs_b5fb72b5c5f74791bff52b9acb830aa0, _PanningUVs_b5fb72b5c5f74791bff52b9acb830aa0_UVOut_1_Vector2);
        float4 _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_bc5252e2c5b74edbaa8ba7f5a162d720_Out_0_Texture2D.tex, _Property_bc5252e2c5b74edbaa8ba7f5a162d720_Out_0_Texture2D.samplerstate, _Property_bc5252e2c5b74edbaa8ba7f5a162d720_Out_0_Texture2D.GetTransformedUV(_PanningUVs_b5fb72b5c5f74791bff52b9acb830aa0_UVOut_1_Vector2) );
        _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_RGBA_0_Vector4.rgb = UnpackNormalRGB(_SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_RGBA_0_Vector4);
        float _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_R_4_Float = _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_RGBA_0_Vector4.r;
        float _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_G_5_Float = _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_RGBA_0_Vector4.g;
        float _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_B_6_Float = _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_RGBA_0_Vector4.b;
        float _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_A_7_Float = _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_RGBA_0_Vector4.a;
        float _Step_640da6258eee484699374502b11f5859_Out_2_Float;
        Unity_Step_float(_OneMinus_879cfebaf09a4041ba20d931184cf4f9_Out_1_Float, _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_R_4_Float, _Step_640da6258eee484699374502b11f5859_Out_2_Float);
        float4 _Property_1b2a727d2d9a4b35a4b84b80d8cc241b_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_IntersectionFoamColor) : _IntersectionFoamColor;
        float _Split_0fdcdfb033da473fa176a759fabdf261_R_1_Float = _Property_1b2a727d2d9a4b35a4b84b80d8cc241b_Out_0_Vector4[0];
        float _Split_0fdcdfb033da473fa176a759fabdf261_G_2_Float = _Property_1b2a727d2d9a4b35a4b84b80d8cc241b_Out_0_Vector4[1];
        float _Split_0fdcdfb033da473fa176a759fabdf261_B_3_Float = _Property_1b2a727d2d9a4b35a4b84b80d8cc241b_Out_0_Vector4[2];
        float _Split_0fdcdfb033da473fa176a759fabdf261_A_4_Float = _Property_1b2a727d2d9a4b35a4b84b80d8cc241b_Out_0_Vector4[3];
        float _Multiply_4b309a3865b54e3490078c2758f00293_Out_2_Float;
        Unity_Multiply_float_float(_Step_640da6258eee484699374502b11f5859_Out_2_Float, _Split_0fdcdfb033da473fa176a759fabdf261_A_4_Float, _Multiply_4b309a3865b54e3490078c2758f00293_Out_2_Float);
        IntersectionFoamResult_1 = (_Multiply_4b309a3865b54e3490078c2758f00293_Out_2_Float.xxxx);
        }
        
        void Unity_NormalBlend_float(float3 A, float3 B, out float3 Out)
        {
            Out = SafeNormalize(float3(A.rg + B.rg, A.b * B.b));
        }
        
        struct Bindings_BlendedNormals_01824997d3b28f944887693b0e1d6405_float
        {
        float3 TimeParameters;
        };
        
        void SG_BlendedNormals_01824997d3b28f944887693b0e1d6405_float(float _NormalScale, float _NormalSpeed, UnityTexture2D _NormalTexture, float2 _UVs, Bindings_BlendedNormals_01824997d3b28f944887693b0e1d6405_float IN, out float3 Out_0)
        {
        UnityTexture2D _Property_b7e00044a0874370a521db8cbe781666_Out_0_Texture2D = _NormalTexture;
        float _Property_82e7e1ba78984b8f86383489bc8d0b1d_Out_0_Float = _NormalSpeed;
        float _Multiply_5e387d16860b440bb01f80e49690829a_Out_2_Float;
        Unity_Multiply_float_float(-0.5, _Property_82e7e1ba78984b8f86383489bc8d0b1d_Out_0_Float, _Multiply_5e387d16860b440bb01f80e49690829a_Out_2_Float);
        float2 _Property_d868665dbff94b6590abe31c7c0c6327_Out_0_Vector2 = _UVs;
        float _Property_5fd79ab306504e55aa9fa9ce57ddbb1b_Out_0_Float = _NormalScale;
        float _Multiply_63cc307a7007489dbadda4faf1dfa495_Out_2_Float;
        Unity_Multiply_float_float(0.5, _Property_5fd79ab306504e55aa9fa9ce57ddbb1b_Out_0_Float, _Multiply_63cc307a7007489dbadda4faf1dfa495_Out_2_Float);
        float _Reciprocal_4d8c53063c7c4694a2bc9fd174344747_Out_1_Float;
        Unity_Reciprocal_Fast_float(_Multiply_63cc307a7007489dbadda4faf1dfa495_Out_2_Float, _Reciprocal_4d8c53063c7c4694a2bc9fd174344747_Out_1_Float);
        Bindings_PanningUVs_ef286e626cee63841803a024235a644f_float _PanningUVs_c3d15fa8145b400cbfe094f7a9014648;
        _PanningUVs_c3d15fa8145b400cbfe094f7a9014648.TimeParameters = IN.TimeParameters;
        half2 _PanningUVs_c3d15fa8145b400cbfe094f7a9014648_UVOut_1_Vector2;
        SG_PanningUVs_ef286e626cee63841803a024235a644f_float(half(1), _Multiply_5e387d16860b440bb01f80e49690829a_Out_2_Float, _Property_d868665dbff94b6590abe31c7c0c6327_Out_0_Vector2, _Reciprocal_4d8c53063c7c4694a2bc9fd174344747_Out_1_Float, half2 (0, 0), _PanningUVs_c3d15fa8145b400cbfe094f7a9014648, _PanningUVs_c3d15fa8145b400cbfe094f7a9014648_UVOut_1_Vector2);
        float4 _SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_b7e00044a0874370a521db8cbe781666_Out_0_Texture2D.tex, _Property_b7e00044a0874370a521db8cbe781666_Out_0_Texture2D.samplerstate, _Property_b7e00044a0874370a521db8cbe781666_Out_0_Texture2D.GetTransformedUV(_PanningUVs_c3d15fa8145b400cbfe094f7a9014648_UVOut_1_Vector2) );
        _SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4.rgb = UnpackNormal(_SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4);
        float _SampleTexture2D_64a0d66e81d94af8907615615e56c648_R_4_Float = _SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4.r;
        float _SampleTexture2D_64a0d66e81d94af8907615615e56c648_G_5_Float = _SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4.g;
        float _SampleTexture2D_64a0d66e81d94af8907615615e56c648_B_6_Float = _SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4.b;
        float _SampleTexture2D_64a0d66e81d94af8907615615e56c648_A_7_Float = _SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4.a;
        float _Property_b3d54c3bdd024196aa765640b786350b_Out_0_Float = _NormalSpeed;
        float2 _Property_1d4d5f262eee4960a52cb63f61c3acbc_Out_0_Vector2 = _UVs;
        float _Property_425ab0f9b8b940939b9ae0d2b672a08d_Out_0_Float = _NormalScale;
        float _Reciprocal_8276cd463ae64d68a1e9c4bdff77f869_Out_1_Float;
        Unity_Reciprocal_Fast_float(_Property_425ab0f9b8b940939b9ae0d2b672a08d_Out_0_Float, _Reciprocal_8276cd463ae64d68a1e9c4bdff77f869_Out_1_Float);
        Bindings_PanningUVs_ef286e626cee63841803a024235a644f_float _PanningUVs_fb9f884294f840c4aa4334e5528e58be;
        _PanningUVs_fb9f884294f840c4aa4334e5528e58be.TimeParameters = IN.TimeParameters;
        half2 _PanningUVs_fb9f884294f840c4aa4334e5528e58be_UVOut_1_Vector2;
        SG_PanningUVs_ef286e626cee63841803a024235a644f_float(half(1), _Property_b3d54c3bdd024196aa765640b786350b_Out_0_Float, _Property_1d4d5f262eee4960a52cb63f61c3acbc_Out_0_Vector2, _Reciprocal_8276cd463ae64d68a1e9c4bdff77f869_Out_1_Float, half2 (0, 0), _PanningUVs_fb9f884294f840c4aa4334e5528e58be, _PanningUVs_fb9f884294f840c4aa4334e5528e58be_UVOut_1_Vector2);
        float4 _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_b7e00044a0874370a521db8cbe781666_Out_0_Texture2D.tex, _Property_b7e00044a0874370a521db8cbe781666_Out_0_Texture2D.samplerstate, _Property_b7e00044a0874370a521db8cbe781666_Out_0_Texture2D.GetTransformedUV(_PanningUVs_fb9f884294f840c4aa4334e5528e58be_UVOut_1_Vector2) );
        _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4.rgb = UnpackNormal(_SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4);
        float _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_R_4_Float = _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4.r;
        float _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_G_5_Float = _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4.g;
        float _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_B_6_Float = _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4.b;
        float _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_A_7_Float = _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4.a;
        float3 _NormalBlend_355e560adba24e7a8abe7a7673ad28d2_Out_2_Vector3;
        Unity_NormalBlend_float((_SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4.xyz), (_SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4.xyz), _NormalBlend_355e560adba24e7a8abe7a7673ad28d2_Out_2_Vector3);
        Out_0 = _NormalBlend_355e560adba24e7a8abe7a7673ad28d2_Out_2_Vector3;
        }
        
        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }
        
        struct Bindings_Specular_ca077344d37a82d46ba9cd29d65fb670_float
        {
        float3 WorldSpaceNormal;
        float3 WorldSpaceTangent;
        float3 WorldSpaceBiTangent;
        float3 WorldSpacePosition;
        float3 TimeParameters;
        };
        
        void SG_Specular_ca077344d37a82d46ba9cd29d65fb670_float(float _NormalScale, float _NormalSpeed, UnityTexture2D _NormalTexture, float _NormalStrength, float _LightingSmoothness, float _LightingHardness, float4 _SpecularColor, float2 _UVs, Bindings_Specular_ca077344d37a82d46ba9cd29d65fb670_float IN, out float3 Specular_1)
        {
        float _Property_12642a3bcf964c959ae2bbac7e5400ac_Out_0_Float = _NormalScale;
        float _Property_9a81b54411dc4443a34f009659e2b4d9_Out_0_Float = _NormalSpeed;
        UnityTexture2D _Property_229609cb9075441ca1c4520de10d655f_Out_0_Texture2D = _NormalTexture;
        float2 _Property_5b27f589f80742bcbfe965375ab83205_Out_0_Vector2 = _UVs;
        Bindings_BlendedNormals_01824997d3b28f944887693b0e1d6405_float _BlendedNormals_9340ea003cbd468e862462200f580f96;
        _BlendedNormals_9340ea003cbd468e862462200f580f96.TimeParameters = IN.TimeParameters;
        half3 _BlendedNormals_9340ea003cbd468e862462200f580f96_Out_0_Vector3;
        SG_BlendedNormals_01824997d3b28f944887693b0e1d6405_float(_Property_12642a3bcf964c959ae2bbac7e5400ac_Out_0_Float, _Property_9a81b54411dc4443a34f009659e2b4d9_Out_0_Float, _Property_229609cb9075441ca1c4520de10d655f_Out_0_Texture2D, _Property_5b27f589f80742bcbfe965375ab83205_Out_0_Vector2, _BlendedNormals_9340ea003cbd468e862462200f580f96, _BlendedNormals_9340ea003cbd468e862462200f580f96_Out_0_Vector3);
        float _Property_61fd9c0ec35a4d238019a6212f701e6f_Out_0_Float = _NormalStrength;
        float3 _NormalStrength_5fc187d2cc214a3aa352e512a627ae29_Out_2_Vector3;
        Unity_NormalStrength_float(_BlendedNormals_9340ea003cbd468e862462200f580f96_Out_0_Vector3, _Property_61fd9c0ec35a4d238019a6212f701e6f_Out_0_Float, _NormalStrength_5fc187d2cc214a3aa352e512a627ae29_Out_2_Vector3);
        float3 _Transform_f0cb3b0524334f779bf2800dae8c021e_Out_1_Vector3;
        {
        float3x3 tangentTransform = float3x3(IN.WorldSpaceTangent, IN.WorldSpaceBiTangent, IN.WorldSpaceNormal);
        _Transform_f0cb3b0524334f779bf2800dae8c021e_Out_1_Vector3 = TransformTangentToWorld(_NormalStrength_5fc187d2cc214a3aa352e512a627ae29_Out_2_Vector3.xyz, tangentTransform, true);
        }
        float3 _ViewVector_1252c7c64b0e4e6f855eab336f1a48a8_Out_0_Vector3;
        Unity_ViewVectorWorld_float(_ViewVector_1252c7c64b0e4e6f855eab336f1a48a8_Out_0_Vector3, IN.WorldSpacePosition);
        float3 _Vector3_e77fc3854050436d8a46ff8b9f3b1ff2_Out_0_Vector3 = float3(float(0), float(0), float(1));
        #if defined(_PERSPECTIVE)
        float3 _Perspective_322896f72bb640e09107587d0117cc53_Out_0_Vector3 = _ViewVector_1252c7c64b0e4e6f855eab336f1a48a8_Out_0_Vector3;
        #else
        float3 _Perspective_322896f72bb640e09107587d0117cc53_Out_0_Vector3 = _Vector3_e77fc3854050436d8a46ff8b9f3b1ff2_Out_0_Vector3;
        #endif
        float _MainLightingCustomFunction_08a62467374f4ec580e996e505530fff_Specular_4_Float;
        MainLighting_float(_Transform_f0cb3b0524334f779bf2800dae8c021e_Out_1_Vector3, IN.WorldSpacePosition, _Perspective_322896f72bb640e09107587d0117cc53_Out_0_Vector3, float(0), _MainLightingCustomFunction_08a62467374f4ec580e996e505530fff_Specular_4_Float);
        float _Step_7c27e8f5e968400b962535f046fd4f84_Out_2_Float;
        Unity_Step_float(float(0.5), _MainLightingCustomFunction_08a62467374f4ec580e996e505530fff_Specular_4_Float, _Step_7c27e8f5e968400b962535f046fd4f84_Out_2_Float);
        float _Property_070589d4b1b64f00877b042f5fa4786c_Out_0_Float = _LightingHardness;
        float _Lerp_7693c4d68e1a4276bcace636300a7f30_Out_3_Float;
        Unity_Lerp_float(_MainLightingCustomFunction_08a62467374f4ec580e996e505530fff_Specular_4_Float, _Step_7c27e8f5e968400b962535f046fd4f84_Out_2_Float, _Property_070589d4b1b64f00877b042f5fa4786c_Out_0_Float, _Lerp_7693c4d68e1a4276bcace636300a7f30_Out_3_Float);
        float4 _Property_2540d1f934ed4d15b7d62700a4f1adf2_Out_0_Vector4 = _SpecularColor;
        float4 _Multiply_e0f28cba39d84814946e9e3683c24585_Out_2_Vector4;
        Unity_Multiply_float4_float4((_Lerp_7693c4d68e1a4276bcace636300a7f30_Out_3_Float.xxxx), _Property_2540d1f934ed4d15b7d62700a4f1adf2_Out_0_Vector4, _Multiply_e0f28cba39d84814946e9e3683c24585_Out_2_Vector4);
        float _Split_cca0da3ff1c94d7b95381021f7e5824e_R_1_Float = _Property_2540d1f934ed4d15b7d62700a4f1adf2_Out_0_Vector4[0];
        float _Split_cca0da3ff1c94d7b95381021f7e5824e_G_2_Float = _Property_2540d1f934ed4d15b7d62700a4f1adf2_Out_0_Vector4[1];
        float _Split_cca0da3ff1c94d7b95381021f7e5824e_B_3_Float = _Property_2540d1f934ed4d15b7d62700a4f1adf2_Out_0_Vector4[2];
        float _Split_cca0da3ff1c94d7b95381021f7e5824e_A_4_Float = _Property_2540d1f934ed4d15b7d62700a4f1adf2_Out_0_Vector4[3];
        float4 _Multiply_e3b0a1558f044c5f9884617dd07c3456_Out_2_Vector4;
        Unity_Multiply_float4_float4(_Multiply_e0f28cba39d84814946e9e3683c24585_Out_2_Vector4, (_Split_cca0da3ff1c94d7b95381021f7e5824e_A_4_Float.xxxx), _Multiply_e3b0a1558f044c5f9884617dd07c3456_Out_2_Vector4);
        float _Property_d4211a44c8d34f3fb25fe762b3d7b5a5_Out_0_Float = _LightingSmoothness;
        float _Property_47eea6903fbd41518eb78ca7ba2ea49f_Out_0_Float = _LightingHardness;
        float3 _AdditionalLightingCustomFunction_f8bf1ccdd9a34ff1ba47f18b6139f837_Specular_5_Vector3;
        AdditionalLighting_float(_Transform_f0cb3b0524334f779bf2800dae8c021e_Out_1_Vector3, IN.WorldSpacePosition, _Perspective_322896f72bb640e09107587d0117cc53_Out_0_Vector3, _Property_d4211a44c8d34f3fb25fe762b3d7b5a5_Out_0_Float, _Property_47eea6903fbd41518eb78ca7ba2ea49f_Out_0_Float, _AdditionalLightingCustomFunction_f8bf1ccdd9a34ff1ba47f18b6139f837_Specular_5_Vector3);
        float3 _Add_bbd9570472ea43018aeac31b2cec79ef_Out_2_Vector3;
        Unity_Add_float3((_Multiply_e3b0a1558f044c5f9884617dd07c3456_Out_2_Vector4.xyz), _AdditionalLightingCustomFunction_f8bf1ccdd9a34ff1ba47f18b6139f837_Specular_5_Vector3, _Add_bbd9570472ea43018aeac31b2cec79ef_Out_2_Vector3);
        Specular_1 = _Add_bbd9570472ea43018aeac31b2cec79ef_Out_2_Vector3;
        }
        
        void Unity_Fog_float(out float4 Color, out float Density, float3 Position)
        {
            SHADERGRAPH_FOG(Position, Color, Density);
        }
        
        struct Bindings_Fog_4d8d465b4ba9a34499f11570d2bfbcf0_float
        {
        float3 ObjectSpacePosition;
        };
        
        void SG_Fog_4d8d465b4ba9a34499f11570d2bfbcf0_float(Bindings_Fog_4d8d465b4ba9a34499f11570d2bfbcf0_float IN, out float FogAmount_1, out float4 FogColor_2)
        {
        float4 _Fog_58a31c30c807423b9d9065753a0cbe96_Color_0_Vector4;
        float _Fog_58a31c30c807423b9d9065753a0cbe96_Density_1_Float;
        Unity_Fog_float(_Fog_58a31c30c807423b9d9065753a0cbe96_Color_0_Vector4, _Fog_58a31c30c807423b9d9065753a0cbe96_Density_1_Float, IN.ObjectSpacePosition);
        float _Saturate_57b873032001499d95a8956b1c4c9ff9_Out_1_Float;
        Unity_Saturate_float(_Fog_58a31c30c807423b9d9065753a0cbe96_Density_1_Float, _Saturate_57b873032001499d95a8956b1c4c9ff9_Out_1_Float);
        float4 _Fog_d79620e98a204435bdf933c422a158e0_Color_0_Vector4;
        float _Fog_d79620e98a204435bdf933c422a158e0_Density_1_Float;
        Unity_Fog_float(_Fog_d79620e98a204435bdf933c422a158e0_Color_0_Vector4, _Fog_d79620e98a204435bdf933c422a158e0_Density_1_Float, IN.ObjectSpacePosition);
        FogAmount_1 = _Saturate_57b873032001499d95a8956b1c4c9ff9_Out_1_Float;
        FogColor_2 = _Fog_d79620e98a204435bdf933c422a158e0_Color_0_Vector4;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Property_f4b92572a6b849978ea4164861fc96e7_Out_0_Float = _WaveSteepness;
            float _Property_60706828d6004c70825f62373cea2de7_Out_0_Float = _WaveLength;
            float _Property_2666eaa75aae48049c953771bc099709_Out_0_Float = _WaveSpeed;
            float4 _Property_cc5d2426eabf42799308261053db7fd7_Out_0_Vector4 = _WaveDirections;
            Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11;
            _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11.ObjectSpacePosition = IN.ObjectSpacePosition;
            float3 _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3;
            SG_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float(_Property_f4b92572a6b849978ea4164861fc96e7_Out_0_Float, _Property_60706828d6004c70825f62373cea2de7_Out_0_Float, _Property_2666eaa75aae48049c953771bc099709_Out_0_Float, _Property_cc5d2426eabf42799308261053db7fd7_Out_0_Vector4, _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11, _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3);
            description.Position = _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_1d602cb04f2a487bbe2a10a82c216988_Out_0_Float = _DepthFadeDistance;
            float _Property_7d6fc5bac56243d4b5ae0dcde436653d_Out_0_Float = _Steps;
            float _Property_e037029a81ca4277aa6156bc59761352_Out_0_Float = _RefractionSpeed;
            float _Property_99fa99535b074d1f91770ec4b97b3857_Out_0_Float = _RefractionScale;
            float _Property_ad37e2c0d61640a7bb0c4d98de3805d4_Out_0_Float = _GradientRefractionScale;
            float _Property_4a60bded2f1f4dd6945f5cf3f4fc789d_Out_0_Float = _RefractionStrength;
            float4 _Property_a9206916a2e3432bb1afa6b1d3655996_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_ShallowColor) : _ShallowColor;
            float4 _Property_e9f7f16ac69646e5908d8454ec7ef56f_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_DeepColor) : _DeepColor;
            Bindings_GetDepth_370b72acb6baa764089512ed583870c4_float _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e;
            _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e.WorldSpacePosition = IN.WorldSpacePosition;
            _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e.ScreenPosition = IN.ScreenPosition;
            _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e.NDCPosition = IN.NDCPosition;
            _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e.uv0 = IN.uv0;
            _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e.TimeParameters = IN.TimeParameters;
            float4 _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e_RefractedUVs_1_Vector4;
            float4 _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e_Color_2_Vector4;
            SG_GetDepth_370b72acb6baa764089512ed583870c4_float(_Property_1d602cb04f2a487bbe2a10a82c216988_Out_0_Float, _Property_7d6fc5bac56243d4b5ae0dcde436653d_Out_0_Float, _Property_e037029a81ca4277aa6156bc59761352_Out_0_Float, _Property_99fa99535b074d1f91770ec4b97b3857_Out_0_Float, _Property_ad37e2c0d61640a7bb0c4d98de3805d4_Out_0_Float, _Property_4a60bded2f1f4dd6945f5cf3f4fc789d_Out_0_Float, _Property_a9206916a2e3432bb1afa6b1d3655996_Out_0_Vector4, _Property_e9f7f16ac69646e5908d8454ec7ef56f_Out_0_Vector4, _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e, _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e_RefractedUVs_1_Vector4, _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e_Color_2_Vector4);
            float4 _Property_893a4c25d7bd4390a59efcfddbd60e36_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_HorizonColor) : _HorizonColor;
            float _Property_356b877dd8e2453c8037fed5c4738b25_Out_0_Float = _HorizonDistance;
            Bindings_FresnelCalculation_f346593723353f4488f8a099aef520eb_float _FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5;
            _FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5.WorldSpaceNormal = IN.WorldSpaceNormal;
            _FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5.ViewSpaceNormal = IN.ViewSpaceNormal;
            _FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5.WorldSpaceViewDirection = IN.WorldSpaceViewDirection;
            float _FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5_FresnelResult_1_Float;
            SG_FresnelCalculation_f346593723353f4488f8a099aef520eb_float(_Property_356b877dd8e2453c8037fed5c4738b25_Out_0_Float, _FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5, _FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5_FresnelResult_1_Float);
            float4 _Lerp_3aa30c6357614a61b17161f8c0633600_Out_3_Vector4;
            Unity_Lerp_float4(_GetDepth_52ad2e61abb04277a0dfe91bb3664d1e_Color_2_Vector4, _Property_893a4c25d7bd4390a59efcfddbd60e36_Out_0_Vector4, (_FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5_FresnelResult_1_Float.xxxx), _Lerp_3aa30c6357614a61b17161f8c0633600_Out_3_Vector4);
            Bindings_BlendObjectColor_a02636e967a650d4db669ee613538c46_float _BlendObjectColor_3ffec75a375b49dfa55b88d64e560cb9;
            float3 _BlendObjectColor_3ffec75a375b49dfa55b88d64e560cb9_ObjectBlend_1_Vector3;
            SG_BlendObjectColor_a02636e967a650d4db669ee613538c46_float(_GetDepth_52ad2e61abb04277a0dfe91bb3664d1e_RefractedUVs_1_Vector4, _Lerp_3aa30c6357614a61b17161f8c0633600_Out_3_Vector4, _BlendObjectColor_3ffec75a375b49dfa55b88d64e560cb9, _BlendObjectColor_3ffec75a375b49dfa55b88d64e560cb9_ObjectBlend_1_Vector3);
            float3 _Add_1dfe8dd4fc544cb1a6be38289da171bb_Out_2_Vector3;
            Unity_Add_float3(_BlendObjectColor_3ffec75a375b49dfa55b88d64e560cb9_ObjectBlend_1_Vector3, (_Lerp_3aa30c6357614a61b17161f8c0633600_Out_3_Vector4.xyz), _Add_1dfe8dd4fc544cb1a6be38289da171bb_Out_2_Vector3);
            float4 _Property_f31e3d68a9514af587c58953c6a37312_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_CaveColor) : _CaveColor;
            Bindings_CalculateUVs_60d47073ac03df54f9a15991680de154_float _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba;
            _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba.WorldSpacePosition = IN.WorldSpacePosition;
            _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba.uv0 = IN.uv0;
            float2 _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba_OutUVs_1_Vector2;
            SG_CalculateUVs_60d47073ac03df54f9a15991680de154_float(_CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba, _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba_OutUVs_1_Vector2);
            float2 _Property_b813bb0c2a834796bb918459b2e9f6f2_Out_0_Vector2 = _CaveScale;
            float2 _Property_f4eb27d4ee8b483c88526d64b0adcb71_Out_0_Vector2 = _CaveOffset;
            float _Property_d0f8505aba284e148b22af0ebde152ee_Out_0_Float = _CaveDistortion;
            UnityTexture2D _Property_e88c987f688347cfb013302786c36f42_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_CaveTexture);
            float4 _Property_a0ded2607a8749148ff7a887ebed0d9c_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_CaveColor) : _CaveColor;
            Bindings_Caves_3e5c142028ba07d48bba33c5843ba17e_float _Caves_bce6e1cd6e0a4b1c845eda4f734a4239;
            float4 _Caves_bce6e1cd6e0a4b1c845eda4f734a4239_CaveResult_1_Vector4;
            SG_Caves_3e5c142028ba07d48bba33c5843ba17e_float(_CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba_OutUVs_1_Vector2, _Property_b813bb0c2a834796bb918459b2e9f6f2_Out_0_Vector2, _Property_f4eb27d4ee8b483c88526d64b0adcb71_Out_0_Vector2, _Property_d0f8505aba284e148b22af0ebde152ee_Out_0_Float, _Property_e88c987f688347cfb013302786c36f42_Out_0_Texture2D, _Property_a0ded2607a8749148ff7a887ebed0d9c_Out_0_Vector4, float(50), _Caves_bce6e1cd6e0a4b1c845eda4f734a4239, _Caves_bce6e1cd6e0a4b1c845eda4f734a4239_CaveResult_1_Vector4);
            float3 _Lerp_9c2e40976f7e47008367af1bafb57231_Out_3_Vector3;
            Unity_Lerp_float3(_Add_1dfe8dd4fc544cb1a6be38289da171bb_Out_2_Vector3, (_Property_f31e3d68a9514af587c58953c6a37312_Out_0_Vector4.xyz), (_Caves_bce6e1cd6e0a4b1c845eda4f734a4239_CaveResult_1_Vector4.xyz), _Lerp_9c2e40976f7e47008367af1bafb57231_Out_3_Vector3);
            UnityTexture2D _Property_0eefa2a85d8a410bad14c729edc255c8_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_ReflectionMap);
            float _Property_b274ddde85464fe7990c99a3b9ff8986_Out_0_Float = _ReflectionDistortion;
            float _Property_379d70ad8d6b484e9cf65bd7ceb23f58_Out_0_Float = _ReflectionBlend;
            Bindings_Reflections_2a83053698b0a9d46a141dd7d36ac2ac_float _Reflections_c68a6bfeff564b248d4ce269caad599e;
            _Reflections_c68a6bfeff564b248d4ce269caad599e.NDCPosition = IN.NDCPosition;
            float4 _Reflections_c68a6bfeff564b248d4ce269caad599e_ReflexColor_1_Vector4;
            float _Reflections_c68a6bfeff564b248d4ce269caad599e_ReflexResult_2_Float;
            SG_Reflections_2a83053698b0a9d46a141dd7d36ac2ac_float(_Property_0eefa2a85d8a410bad14c729edc255c8_Out_0_Texture2D, _Property_b274ddde85464fe7990c99a3b9ff8986_Out_0_Float, _Property_379d70ad8d6b484e9cf65bd7ceb23f58_Out_0_Float, _Reflections_c68a6bfeff564b248d4ce269caad599e, _Reflections_c68a6bfeff564b248d4ce269caad599e_ReflexColor_1_Vector4, _Reflections_c68a6bfeff564b248d4ce269caad599e_ReflexResult_2_Float);
            float3 _Lerp_eae8f37e319e4e87a5ef928ede712a31_Out_3_Vector3;
            Unity_Lerp_float3(_Lerp_9c2e40976f7e47008367af1bafb57231_Out_3_Vector3, (_Reflections_c68a6bfeff564b248d4ce269caad599e_ReflexColor_1_Vector4.xyz), (_Reflections_c68a6bfeff564b248d4ce269caad599e_ReflexResult_2_Float.xxx), _Lerp_eae8f37e319e4e87a5ef928ede712a31_Out_3_Vector3);
            #if defined(_REFLECTIONS)
            float3 _Reflections_a210dad7ca474e3fb1719c7e933d3a27_Out_0_Vector3 = _Lerp_eae8f37e319e4e87a5ef928ede712a31_Out_3_Vector3;
            #else
            float3 _Reflections_a210dad7ca474e3fb1719c7e933d3a27_Out_0_Vector3 = _Lerp_9c2e40976f7e47008367af1bafb57231_Out_3_Vector3;
            #endif
            float4 _Property_81b51a63dd4c4bfb834b310d7e276279_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_SecondaryFoamColor) : _SecondaryFoamColor;
            float _Property_15d7e92bf5d541d9b7e1bbfadcc73a7f_Out_0_Float = _SurfaceFoamDirection;
            float _Property_711cbf38e2bb4a079d432afd2295ee81_Out_0_Float = _SurfaceFoamSpeed;
            float _Property_32a0f052e9444bdcb80f2f954608f8ef_Out_0_Float = _SurfaceFoamTiling;
            float2 _Property_17d01aaf1dda4850abc180a4ee955b3c_Out_0_Vector2 = _FoamUVsOffset;
            float _Property_ffebd665429e4cceaad03a9da3adafee_Out_0_Float = _SurfaceFoamDistorsion;
            UnityTexture2D _Property_d64c4dd59fff4c5b97b65f5f5c0c7f10_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_SecondaryFoamTex);
            float4 _Property_3451f3182b8241279c3bbb80d57d882c_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_SecondaryFoamColor) : _SecondaryFoamColor;
            Bindings_SecondaryFoam_a707540033b78f44bbef5139f65097d0_float _SecondaryFoam_5f90189370e9499fade77e767815c37a;
            _SecondaryFoam_5f90189370e9499fade77e767815c37a.TimeParameters = IN.TimeParameters;
            float4 _SecondaryFoam_5f90189370e9499fade77e767815c37a_SecondaryFoamResult_1_Vector4;
            SG_SecondaryFoam_a707540033b78f44bbef5139f65097d0_float(_Property_15d7e92bf5d541d9b7e1bbfadcc73a7f_Out_0_Float, _Property_711cbf38e2bb4a079d432afd2295ee81_Out_0_Float, _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba_OutUVs_1_Vector2, _Property_32a0f052e9444bdcb80f2f954608f8ef_Out_0_Float, _Property_17d01aaf1dda4850abc180a4ee955b3c_Out_0_Vector2, _Property_ffebd665429e4cceaad03a9da3adafee_Out_0_Float, _Property_d64c4dd59fff4c5b97b65f5f5c0c7f10_Out_0_Texture2D, _Property_3451f3182b8241279c3bbb80d57d882c_Out_0_Vector4, _SecondaryFoam_5f90189370e9499fade77e767815c37a, _SecondaryFoam_5f90189370e9499fade77e767815c37a_SecondaryFoamResult_1_Vector4);
            float3 _Lerp_f104552f399b462f82c9f7d9d4b08dc2_Out_3_Vector3;
            Unity_Lerp_float3(_Reflections_a210dad7ca474e3fb1719c7e933d3a27_Out_0_Vector3, (_Property_81b51a63dd4c4bfb834b310d7e276279_Out_0_Vector4.xyz), (_SecondaryFoam_5f90189370e9499fade77e767815c37a_SecondaryFoamResult_1_Vector4.xyz), _Lerp_f104552f399b462f82c9f7d9d4b08dc2_Out_3_Vector3);
            float4 _Property_da9173513832427bb5ab4ab2e5528b2b_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_SurfaceFoamColor) : _SurfaceFoamColor;
            float _Property_636aeb6fd5ae4972be2eb07aff88bb50_Out_0_Float = _SurfaceFoamDirection;
            float _Property_f4bf6eaa912341d1a75e44b6b46af453_Out_0_Float = _SurfaceFoamSpeed;
            float _Property_d25c273d7c7642d7a7dc57913ed42e97_Out_0_Float = _SurfaceFoamTiling;
            float _Property_1a3f66b4873543cda6a945906b57d7d8_Out_0_Float = _SurfaceFoamDistorsion;
            UnityTexture2D _Property_165b792376df4c00bf5ceebafff9099b_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_SurfaceFoamTexture);
            float4 _Property_689d2414eafc4fe4a7777244e0226de5_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_SurfaceFoamColor) : _SurfaceFoamColor;
            Bindings_PrimaryFoam_f41bb18b12540894cadcf85fcc438801_float _PrimaryFoam_4a7109ebe0ce44d88150306b8a89a15b;
            _PrimaryFoam_4a7109ebe0ce44d88150306b8a89a15b.TimeParameters = IN.TimeParameters;
            float4 _PrimaryFoam_4a7109ebe0ce44d88150306b8a89a15b_FoamResult_1_Vector4;
            SG_PrimaryFoam_f41bb18b12540894cadcf85fcc438801_float(_Property_636aeb6fd5ae4972be2eb07aff88bb50_Out_0_Float, _Property_f4bf6eaa912341d1a75e44b6b46af453_Out_0_Float, _Property_d25c273d7c7642d7a7dc57913ed42e97_Out_0_Float, _Property_1a3f66b4873543cda6a945906b57d7d8_Out_0_Float, _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba_OutUVs_1_Vector2, _Property_165b792376df4c00bf5ceebafff9099b_Out_0_Texture2D, _Property_689d2414eafc4fe4a7777244e0226de5_Out_0_Vector4, _PrimaryFoam_4a7109ebe0ce44d88150306b8a89a15b, _PrimaryFoam_4a7109ebe0ce44d88150306b8a89a15b_FoamResult_1_Vector4);
            float3 _Lerp_283283898a3f45b783820a658c09ee0e_Out_3_Vector3;
            Unity_Lerp_float3(_Lerp_f104552f399b462f82c9f7d9d4b08dc2_Out_3_Vector3, (_Property_da9173513832427bb5ab4ab2e5528b2b_Out_0_Vector4.xyz), (_PrimaryFoam_4a7109ebe0ce44d88150306b8a89a15b_FoamResult_1_Vector4.xyz), _Lerp_283283898a3f45b783820a658c09ee0e_Out_3_Vector3);
            float4 _Property_a28dab83abd146709ef4d7acf47620c9_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_IntersectionFoamColor) : _IntersectionFoamColor;
            float _Property_6d519a4006ce4a50b7180ef5a9b0d73f_Out_0_Float = _IntersectionFoamDepth;
            float _Property_1c21741f948044b38291462d9d9a80bc_Out_0_Float = _IntersectionFoamDirection;
            float _Property_c68c441254154b6598696f93cabb1974_Out_0_Float = _IntersectionFoamSpeed;
            float _Property_c27dd75d3883456b8cc7ecd9eea07821_Out_0_Float = _IntersectionFoamTiling;
            float _Property_998dfb4e05624d2a841294b3d6056443_Out_0_Float = _IntersectionFoamFade;
            float _Property_6c9ee0fa005147ccbc1e23124eddb903_Out_0_Float = _IntersectionFoamCutoff;
            UnityTexture2D _Property_262ae236fdea4725a5df623a31dc5a40_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_IntersectionFoamTexture);
            float4 _Property_83ad9b00f77a4d68a1a02e037f6da53f_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_IntersectionFoamColor) : _IntersectionFoamColor;
            Bindings_IntersectionFoam_e12de5bce35668a4e8839cbe5732d905_float _IntersectionFoam_111b1a7de3614aefbbfff3b435152564;
            _IntersectionFoam_111b1a7de3614aefbbfff3b435152564.WorldSpacePosition = IN.WorldSpacePosition;
            _IntersectionFoam_111b1a7de3614aefbbfff3b435152564.ScreenPosition = IN.ScreenPosition;
            _IntersectionFoam_111b1a7de3614aefbbfff3b435152564.NDCPosition = IN.NDCPosition;
            _IntersectionFoam_111b1a7de3614aefbbfff3b435152564.TimeParameters = IN.TimeParameters;
            float4 _IntersectionFoam_111b1a7de3614aefbbfff3b435152564_IntersectionFoamResult_1_Vector4;
            SG_IntersectionFoam_e12de5bce35668a4e8839cbe5732d905_float(_Property_6d519a4006ce4a50b7180ef5a9b0d73f_Out_0_Float, _Property_1c21741f948044b38291462d9d9a80bc_Out_0_Float, _Property_c68c441254154b6598696f93cabb1974_Out_0_Float, _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba_OutUVs_1_Vector2, _Property_c27dd75d3883456b8cc7ecd9eea07821_Out_0_Float, _Property_998dfb4e05624d2a841294b3d6056443_Out_0_Float, _Property_6c9ee0fa005147ccbc1e23124eddb903_Out_0_Float, _Property_262ae236fdea4725a5df623a31dc5a40_Out_0_Texture2D, _Property_83ad9b00f77a4d68a1a02e037f6da53f_Out_0_Vector4, _IntersectionFoam_111b1a7de3614aefbbfff3b435152564, _IntersectionFoam_111b1a7de3614aefbbfff3b435152564_IntersectionFoamResult_1_Vector4);
            float3 _Lerp_d89884addcfd45b3a3bb66e8d985f007_Out_3_Vector3;
            Unity_Lerp_float3(_Lerp_283283898a3f45b783820a658c09ee0e_Out_3_Vector3, (_Property_a28dab83abd146709ef4d7acf47620c9_Out_0_Vector4.xyz), (_IntersectionFoam_111b1a7de3614aefbbfff3b435152564_IntersectionFoamResult_1_Vector4.xyz), _Lerp_d89884addcfd45b3a3bb66e8d985f007_Out_3_Vector3);
            float _Property_7d5a7594d9d645ab99b46d3eee118d5c_Out_0_Float = _NormalScale;
            float _Property_4d1d135eaa684e87a7f73553b8717c03_Out_0_Float = _NormalSpeed;
            UnityTexture2D _Property_f51209adc1e24e0f997c51a1a1ab46b5_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_NormalTexture);
            float _Property_999b6f77135943d7adfa043c98a9ea04_Out_0_Float = _NormalStrength;
            float _Property_0299898b57e04518ad9f1b61a9bef7d6_Out_0_Float = _LightingSmoothness;
            float _Property_c5979b329857400396134e141ceffd60_Out_0_Float = _LightingHardness;
            float4 _Property_f09aaf76f58144498339d31ddb56a129_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_SpecularColor) : _SpecularColor;
            Bindings_Specular_ca077344d37a82d46ba9cd29d65fb670_float _Specular_467937ce566d4d7387886e0b29b4205e;
            _Specular_467937ce566d4d7387886e0b29b4205e.WorldSpaceNormal = IN.WorldSpaceNormal;
            _Specular_467937ce566d4d7387886e0b29b4205e.WorldSpaceTangent = IN.WorldSpaceTangent;
            _Specular_467937ce566d4d7387886e0b29b4205e.WorldSpaceBiTangent = IN.WorldSpaceBiTangent;
            _Specular_467937ce566d4d7387886e0b29b4205e.WorldSpacePosition = IN.WorldSpacePosition;
            _Specular_467937ce566d4d7387886e0b29b4205e.TimeParameters = IN.TimeParameters;
            float3 _Specular_467937ce566d4d7387886e0b29b4205e_Specular_1_Vector3;
            SG_Specular_ca077344d37a82d46ba9cd29d65fb670_float(_Property_7d5a7594d9d645ab99b46d3eee118d5c_Out_0_Float, _Property_4d1d135eaa684e87a7f73553b8717c03_Out_0_Float, _Property_f51209adc1e24e0f997c51a1a1ab46b5_Out_0_Texture2D, _Property_999b6f77135943d7adfa043c98a9ea04_Out_0_Float, _Property_0299898b57e04518ad9f1b61a9bef7d6_Out_0_Float, _Property_c5979b329857400396134e141ceffd60_Out_0_Float, _Property_f09aaf76f58144498339d31ddb56a129_Out_0_Vector4, _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba_OutUVs_1_Vector2, _Specular_467937ce566d4d7387886e0b29b4205e, _Specular_467937ce566d4d7387886e0b29b4205e_Specular_1_Vector3);
            float3 _Add_8502fe58d1c0413598d6f47c5cbc17f6_Out_2_Vector3;
            Unity_Add_float3(_Lerp_d89884addcfd45b3a3bb66e8d985f007_Out_3_Vector3, _Specular_467937ce566d4d7387886e0b29b4205e_Specular_1_Vector3, _Add_8502fe58d1c0413598d6f47c5cbc17f6_Out_2_Vector3);
            Bindings_Fog_4d8d465b4ba9a34499f11570d2bfbcf0_float _Fog_421529a4b940458890852d5853ae0e26;
            _Fog_421529a4b940458890852d5853ae0e26.ObjectSpacePosition = IN.ObjectSpacePosition;
            float _Fog_421529a4b940458890852d5853ae0e26_FogAmount_1_Float;
            float4 _Fog_421529a4b940458890852d5853ae0e26_FogColor_2_Vector4;
            SG_Fog_4d8d465b4ba9a34499f11570d2bfbcf0_float(_Fog_421529a4b940458890852d5853ae0e26, _Fog_421529a4b940458890852d5853ae0e26_FogAmount_1_Float, _Fog_421529a4b940458890852d5853ae0e26_FogColor_2_Vector4);
            float3 _Lerp_2d6e34c6906c4c55966370e2c9504dca_Out_3_Vector3;
            Unity_Lerp_float3(_Add_8502fe58d1c0413598d6f47c5cbc17f6_Out_2_Vector3, (_Fog_421529a4b940458890852d5853ae0e26_FogColor_2_Vector4.xyz), (_Fog_421529a4b940458890852d5853ae0e26_FogAmount_1_Float.xxx), _Lerp_2d6e34c6906c4c55966370e2c9504dca_Out_3_Vector3);
            surface.BaseColor = _Lerp_2d6e34c6906c4c55966370e2c9504dca_Out_3_Vector3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
            // use bitangent on the fly like in hdrp
            // IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
            float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0)* GetOddNegativeScale();
            float3 bitang = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
            output.ViewSpaceNormal = mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_I_V);         // transposed multiplication by inverse matrix to handle normal scale
        
            // to pr               eserve mikktspace compliance we use same scale renormFactor as was used on the normal.
            // This                is explained in section 2.2 in "surface gradient based bump mapping framework"
            output.WorldSpaceTangent = renormFactor * input.tangentWS.xyz;
            output.WorldSpaceBiTangent = renormFactor * bitang;
        
            output.WorldSpaceViewDirection = GetWorldSpaceNormalizeViewDir(input.positionWS);
            output.WorldSpacePosition = input.positionWS;
            output.ObjectSpacePosition = TransformWorldToObject(input.positionWS);
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
            output.uv0 = input.texCoord0;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitGBufferPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        #pragma shader_feature_local _ _WORLD_SPACE_UV
        #pragma shader_feature_local _ _A_SHORT_HIKE_MODE
        #pragma shader_feature_local _ _REFLECTIONS
        #pragma shader_feature_local _ _PERSPECTIVE
        
        #if defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_0
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_1
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_2
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE)
            #define KEYWORD_PERMUTATION_3
        #elif defined(_WORLD_SPACE_UV) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_4
        #elif defined(_WORLD_SPACE_UV) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_5
        #elif defined(_WORLD_SPACE_UV) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_6
        #elif defined(_WORLD_SPACE_UV)
            #define KEYWORD_PERMUTATION_7
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_8
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_9
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_10
        #elif defined(_A_SHORT_HIKE_MODE)
            #define KEYWORD_PERMUTATION_11
        #elif defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_12
        #elif defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_13
        #elif defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_14
        #else
            #define KEYWORD_PERMUTATION_15
        #endif
        
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _WaveLength;
        float4 _CaveSecondaryColor;
        float _WaveSpeed;
        float _DepthFadeDistance;
        float _IntersectionFoamTiling;
        float4 _SecondaryFoamTex_TexelSize;
        float4 _SecondaryFoamColor;
        float _IntersectionFoamSpeed;
        float _IntersectionFoamDirection;
        float _IntersectionFoamCutoff;
        float4 _ShallowColor;
        float4 _DeepColor;
        float _Steps;
        float _HorizonDistance;
        float4 _HorizonColor;
        float _RefractionSpeed;
        float _RefractionScale;
        float _GradientRefractionScale;
        float _RefractionStrength;
        float _SurfaceFoamDirection;
        float _SurfaceFoamSpeed;
        float _SurfaceFoamTiling;
        float _SurfaceFoamDistorsion;
        float4 _SurfaceFoamTexture_TexelSize;
        float4 _SurfaceFoamColor;
        float _IntersectionFoamDepth;
        float _IntersectionFoamFade;
        float4 _IntersectionFoamTexture_TexelSize;
        float4 _IntersectionFoamColor;
        float _NormalScale;
        float _NormalSpeed;
        float4 _NormalTexture_TexelSize;
        float _NormalStrength;
        float _LightingSmoothness;
        float _LightingHardness;
        float4 _SpecularColor;
        float _WaveSteepness;
        float4 _WaveDirections;
        float4 _CaveTexture_TexelSize;
        float4 _CaveColor;
        float _CaveDistortion;
        float2 _CaveScale;
        float _ReflectionDistortion;
        float _ReflectionBlend;
        float2 _FoamUVsOffset;
        float2 _CaveOffset;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SecondaryFoamTex);
        SAMPLER(sampler_SecondaryFoamTex);
        TEXTURE2D(_SurfaceFoamTexture);
        SAMPLER(sampler_SurfaceFoamTexture);
        TEXTURE2D(_IntersectionFoamTexture);
        SAMPLER(sampler_IntersectionFoamTexture);
        TEXTURE2D(_NormalTexture);
        SAMPLER(sampler_NormalTexture);
        TEXTURE2D(_CaveTexture);
        SAMPLER(sampler_CaveTexture);
        float _TileSize;
        TEXTURE2D(_ReflectionMap);
        SAMPLER(sampler_ReflectionMap);
        float4 _ReflectionMap_TexelSize;
        
        // Graph Includes
        #include_with_pragmas "Assets/Art/UI/AureDevGames/Water Stylized Shader Orto & Perspective Camera/Shader/HLSL/GrestnerWaves.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float
        {
        float3 ObjectSpacePosition;
        };
        
        void SG_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float(float _WaveSteepness, float _WaveLength, float _WaveSpeed, float4 _WaveDirections, Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float IN, out float3 PositionWithWaveOffset_1)
        {
        float3 _Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3;
        {
        // Converting Position from Object to AbsoluteWorld via world space
        float3 world;
        world = TransformObjectToWorld(IN.ObjectSpacePosition.xyz);
        _Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3 = GetAbsolutePositionWS(world);
        }
        float _Property_39d578b2ea3d4928a4a2aac434ebc5c8_Out_0_Float = _WaveSteepness;
        float _Property_4e4135449a284017bef47c0e872e7104_Out_0_Float = _WaveLength;
        float _Property_04f74ba2bc544fdcb5e19ff8a137defb_Out_0_Float = _WaveSpeed;
        float4 _Property_90991e6e61204fd4a1ae25860c15dac2_Out_0_Vector4 = _WaveDirections;
        float3 _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3;
        float3 _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Normal_5_Vector3;
        GerstnerWaves_float(_Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3, _Property_39d578b2ea3d4928a4a2aac434ebc5c8_Out_0_Float, _Property_4e4135449a284017bef47c0e872e7104_Out_0_Float, _Property_04f74ba2bc544fdcb5e19ff8a137defb_Out_0_Float, _Property_90991e6e61204fd4a1ae25860c15dac2_Out_0_Vector4, float(1), _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3, _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Normal_5_Vector3);
        float3 _Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3;
        Unity_Add_float3(_Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3, _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3, _Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3);
        float3 _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3;
        _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3 = TransformWorldToObject(_Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3.xyz);
        PositionWithWaveOffset_1 = _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Property_f4b92572a6b849978ea4164861fc96e7_Out_0_Float = _WaveSteepness;
            float _Property_60706828d6004c70825f62373cea2de7_Out_0_Float = _WaveLength;
            float _Property_2666eaa75aae48049c953771bc099709_Out_0_Float = _WaveSpeed;
            float4 _Property_cc5d2426eabf42799308261053db7fd7_Out_0_Vector4 = _WaveDirections;
            Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11;
            _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11.ObjectSpacePosition = IN.ObjectSpacePosition;
            float3 _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3;
            SG_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float(_Property_f4b92572a6b849978ea4164861fc96e7_Out_0_Float, _Property_60706828d6004c70825f62373cea2de7_Out_0_Float, _Property_2666eaa75aae48049c953771bc099709_Out_0_Float, _Property_cc5d2426eabf42799308261053db7fd7_Out_0_Vector4, _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11, _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3);
            description.Position = _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        #pragma shader_feature_local _ _WORLD_SPACE_UV
        #pragma shader_feature_local _ _A_SHORT_HIKE_MODE
        #pragma shader_feature_local _ _REFLECTIONS
        #pragma shader_feature_local _ _PERSPECTIVE
        
        #if defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_0
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_1
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_2
        #elif defined(_WORLD_SPACE_UV) && defined(_A_SHORT_HIKE_MODE)
            #define KEYWORD_PERMUTATION_3
        #elif defined(_WORLD_SPACE_UV) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_4
        #elif defined(_WORLD_SPACE_UV) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_5
        #elif defined(_WORLD_SPACE_UV) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_6
        #elif defined(_WORLD_SPACE_UV)
            #define KEYWORD_PERMUTATION_7
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_8
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_9
        #elif defined(_A_SHORT_HIKE_MODE) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_10
        #elif defined(_A_SHORT_HIKE_MODE)
            #define KEYWORD_PERMUTATION_11
        #elif defined(_REFLECTIONS) && defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_12
        #elif defined(_REFLECTIONS)
            #define KEYWORD_PERMUTATION_13
        #elif defined(_PERSPECTIVE)
            #define KEYWORD_PERMUTATION_14
        #else
            #define KEYWORD_PERMUTATION_15
        #endif
        
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        #define REQUIRE_DEPTH_TEXTURE
        #define REQUIRE_OPAQUE_TEXTURE
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 ViewSpaceNormal;
             float3 WorldSpaceNormal;
             float3 WorldSpaceTangent;
             float3 WorldSpaceBiTangent;
             float3 WorldSpaceViewDirection;
             float3 ObjectSpacePosition;
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float2 NDCPosition;
             float2 PixelPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 tangentWS : INTERP0;
             float4 texCoord0 : INTERP1;
             float3 positionWS : INTERP2;
             float3 normalWS : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.tangentWS.xyzw = input.tangentWS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.tangentWS = input.tangentWS.xyzw;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _WaveLength;
        float4 _CaveSecondaryColor;
        float _WaveSpeed;
        float _DepthFadeDistance;
        float _IntersectionFoamTiling;
        float4 _SecondaryFoamTex_TexelSize;
        float4 _SecondaryFoamColor;
        float _IntersectionFoamSpeed;
        float _IntersectionFoamDirection;
        float _IntersectionFoamCutoff;
        float4 _ShallowColor;
        float4 _DeepColor;
        float _Steps;
        float _HorizonDistance;
        float4 _HorizonColor;
        float _RefractionSpeed;
        float _RefractionScale;
        float _GradientRefractionScale;
        float _RefractionStrength;
        float _SurfaceFoamDirection;
        float _SurfaceFoamSpeed;
        float _SurfaceFoamTiling;
        float _SurfaceFoamDistorsion;
        float4 _SurfaceFoamTexture_TexelSize;
        float4 _SurfaceFoamColor;
        float _IntersectionFoamDepth;
        float _IntersectionFoamFade;
        float4 _IntersectionFoamTexture_TexelSize;
        float4 _IntersectionFoamColor;
        float _NormalScale;
        float _NormalSpeed;
        float4 _NormalTexture_TexelSize;
        float _NormalStrength;
        float _LightingSmoothness;
        float _LightingHardness;
        float4 _SpecularColor;
        float _WaveSteepness;
        float4 _WaveDirections;
        float4 _CaveTexture_TexelSize;
        float4 _CaveColor;
        float _CaveDistortion;
        float2 _CaveScale;
        float _ReflectionDistortion;
        float _ReflectionBlend;
        float2 _FoamUVsOffset;
        float2 _CaveOffset;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SecondaryFoamTex);
        SAMPLER(sampler_SecondaryFoamTex);
        TEXTURE2D(_SurfaceFoamTexture);
        SAMPLER(sampler_SurfaceFoamTexture);
        TEXTURE2D(_IntersectionFoamTexture);
        SAMPLER(sampler_IntersectionFoamTexture);
        TEXTURE2D(_NormalTexture);
        SAMPLER(sampler_NormalTexture);
        TEXTURE2D(_CaveTexture);
        SAMPLER(sampler_CaveTexture);
        float _TileSize;
        TEXTURE2D(_ReflectionMap);
        SAMPLER(sampler_ReflectionMap);
        float4 _ReflectionMap_TexelSize;
        
        // Graph Includes
        #include_with_pragmas "Assets/Art/UI/AureDevGames/Water Stylized Shader Orto & Perspective Camera/Shader/HLSL/GrestnerWaves.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"
        #include_with_pragmas "Assets/Art/UI/AureDevGames/Water Stylized Shader Orto & Perspective Camera/Shader/HLSL/DistortUV.hlsl"
        #include_with_pragmas "Assets/Art/UI/AureDevGames/Water Stylized Shader Orto & Perspective Camera/Shader/HLSL/MainLighting.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float
        {
        float3 ObjectSpacePosition;
        };
        
        void SG_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float(float _WaveSteepness, float _WaveLength, float _WaveSpeed, float4 _WaveDirections, Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float IN, out float3 PositionWithWaveOffset_1)
        {
        float3 _Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3;
        {
        // Converting Position from Object to AbsoluteWorld via world space
        float3 world;
        world = TransformObjectToWorld(IN.ObjectSpacePosition.xyz);
        _Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3 = GetAbsolutePositionWS(world);
        }
        float _Property_39d578b2ea3d4928a4a2aac434ebc5c8_Out_0_Float = _WaveSteepness;
        float _Property_4e4135449a284017bef47c0e872e7104_Out_0_Float = _WaveLength;
        float _Property_04f74ba2bc544fdcb5e19ff8a137defb_Out_0_Float = _WaveSpeed;
        float4 _Property_90991e6e61204fd4a1ae25860c15dac2_Out_0_Vector4 = _WaveDirections;
        float3 _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3;
        float3 _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Normal_5_Vector3;
        GerstnerWaves_float(_Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3, _Property_39d578b2ea3d4928a4a2aac434ebc5c8_Out_0_Float, _Property_4e4135449a284017bef47c0e872e7104_Out_0_Float, _Property_04f74ba2bc544fdcb5e19ff8a137defb_Out_0_Float, _Property_90991e6e61204fd4a1ae25860c15dac2_Out_0_Vector4, float(1), _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3, _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Normal_5_Vector3);
        float3 _Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3;
        Unity_Add_float3(_Transform_4b134d62316f4a5394b7f4fc3aba3fb2_Out_1_Vector3, _GerstnerWavesCustomFunction_c736eeb0eb5d49dd86ee00e96cba70a5_Offset_4_Vector3, _Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3);
        float3 _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3;
        _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3 = TransformWorldToObject(_Add_693c791ef37e49f387f1f940a739250d_Out_2_Vector3.xyz);
        PositionWithWaveOffset_1 = _Transform_6c7349f1b3984a45b4d05ab857e77835_Out_1_Vector3;
        }
        
        void Unity_Reciprocal_Fast_float(float In, out float Out)
        {
            Out = rcp(In);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
        Out = A * B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        float2 Unity_GradientNoise_LegacyMod_Dir_float(float2 p)
        {
        float x; Hash_LegacyMod_2_1_float(p, x);
        return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }
        
        void Unity_GradientNoise_LegacyMod_float (float2 UV, float3 Scale, out float Out)
        {
        float2 p = UV * Scale.xy;
        float2 ip = floor(p);
        float2 fp = frac(p);
        float d00 = dot(Unity_GradientNoise_LegacyMod_Dir_float(ip), fp);
        float d01 = dot(Unity_GradientNoise_LegacyMod_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
        float d10 = dot(Unity_GradientNoise_LegacyMod_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
        float d11 = dot(Unity_GradientNoise_LegacyMod_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
        fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
        Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_RefractedUV_dfdc6038fe7bdd5479fef69a0fcdb7ab_float
        {
        float2 NDCPosition;
        half4 uv0;
        float3 TimeParameters;
        };
        
        void SG_RefractedUV_dfdc6038fe7bdd5479fef69a0fcdb7ab_float(float _RefractionSpeed, float _RefractionScale, float _GradientRefractionScale, float _RefractionStrength, Bindings_RefractedUV_dfdc6038fe7bdd5479fef69a0fcdb7ab_float IN, out float4 Out_1)
        {
        float4 _ScreenPosition_df4df06bf0bd410e849370e7f915ce20_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
        float _Property_6f2e2eea5f1542be862fc193a4f11e4f_Out_0_Float = _RefractionScale;
        float _Reciprocal_c26c7813ffc1401d8343e505cf3d6b8e_Out_1_Float;
        Unity_Reciprocal_Fast_float(_Property_6f2e2eea5f1542be862fc193a4f11e4f_Out_0_Float, _Reciprocal_c26c7813ffc1401d8343e505cf3d6b8e_Out_1_Float);
        float _Property_8fef6762630f4a338c9146b52f2255f8_Out_0_Float = _RefractionSpeed;
        float _Multiply_44cb146f6607445f8fd249e1966a7b09_Out_2_Float;
        Unity_Multiply_float_float(_Property_8fef6762630f4a338c9146b52f2255f8_Out_0_Float, IN.TimeParameters.x, _Multiply_44cb146f6607445f8fd249e1966a7b09_Out_2_Float);
        float2 _TilingAndOffset_0d3bc9acf8f54350b7aeed81c3a55b94_Out_3_Vector2;
        Unity_TilingAndOffset_float(IN.uv0.xy, (_Reciprocal_c26c7813ffc1401d8343e505cf3d6b8e_Out_1_Float.xx), (_Multiply_44cb146f6607445f8fd249e1966a7b09_Out_2_Float.xx), _TilingAndOffset_0d3bc9acf8f54350b7aeed81c3a55b94_Out_3_Vector2);
        float _Property_2087245bf65447009de8a4234c28ef5d_Out_0_Float = _GradientRefractionScale;
        float _GradientNoise_23f5075437694dcfa3f424e64e24a034_Out_2_Float;
        Unity_GradientNoise_LegacyMod_float(_TilingAndOffset_0d3bc9acf8f54350b7aeed81c3a55b94_Out_3_Vector2, _Property_2087245bf65447009de8a4234c28ef5d_Out_0_Float, _GradientNoise_23f5075437694dcfa3f424e64e24a034_Out_2_Float);
        float _Remap_92f97d94f5a64376842fea592a767e02_Out_3_Float;
        Unity_Remap_float(_GradientNoise_23f5075437694dcfa3f424e64e24a034_Out_2_Float, float2 (0, 1), float2 (-1, 1), _Remap_92f97d94f5a64376842fea592a767e02_Out_3_Float);
        float _Property_eee1994acb674489901dde01c3c3316d_Out_0_Float = _RefractionStrength;
        float _Multiply_b39bd58102fe470c8e1e7f1ce82a040d_Out_2_Float;
        Unity_Multiply_float_float(_Remap_92f97d94f5a64376842fea592a767e02_Out_3_Float, _Property_eee1994acb674489901dde01c3c3316d_Out_0_Float, _Multiply_b39bd58102fe470c8e1e7f1ce82a040d_Out_2_Float);
        float4 _Add_7d79ed8fd86a46e7a268b2fd5dbe2bff_Out_2_Vector4;
        Unity_Add_float4(_ScreenPosition_df4df06bf0bd410e849370e7f915ce20_Out_0_Vector4, (_Multiply_b39bd58102fe470c8e1e7f1ce82a040d_Out_2_Float.xxxx), _Add_7d79ed8fd86a46e7a268b2fd5dbe2bff_Out_2_Vector4);
        Out_1 = _Add_7d79ed8fd86a46e7a268b2fd5dbe2bff_Out_2_Vector4;
        }
        
        void Unity_ViewVectorWorld_float(out float3 Out, float3 WorldSpacePosition)
        {
            Out = _WorldSpaceCameraPos.xyz - GetAbsolutePositionWS(WorldSpacePosition);
            if(!IsPerspectiveProjection())
            {
                Out = GetViewForwardDir() * dot(Out, GetViewForwardDir());
            }
        }
        
        void Unity_Negate_float3(float3 In, out float3 Out)
        {
            Out = -1 * In;
        }
        
        void Unity_Divide_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A / B;
        }
        
        void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
        {
            if (unity_OrthoParams.w == 1.0)
            {
                Out = LinearEyeDepth(ComputeWorldSpacePosition(UV.xy, SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), UNITY_MATRIX_I_VP), UNITY_MATRIX_V);
            }
            else
            {
                Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
            }
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
        Out = A * B;
        }
        
        void Unity_Subtract_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A - B;
        }
        
        void Unity_Negate_float(float In, out float Out)
        {
            Out = -1 * In;
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_Exponential_float(float In, out float Out)
        {
            Out = exp(In);
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        struct Bindings_DepthFadePerspective_3069152b34697f24bb49447c02013d98_float
        {
        float3 WorldSpacePosition;
        float4 ScreenPosition;
        };
        
        void SG_DepthFadePerspective_3069152b34697f24bb49447c02013d98_float(float _DepthFadeDistance, float2 _UV, Bindings_DepthFadePerspective_3069152b34697f24bb49447c02013d98_float IN, out float Linear_2)
        {
        float3 _ViewVector_f8eabe5a287144a6a6d4ef737338317b_Out_0_Vector3;
        Unity_ViewVectorWorld_float(_ViewVector_f8eabe5a287144a6a6d4ef737338317b_Out_0_Vector3, IN.WorldSpacePosition);
        float3 _Negate_d181b51abcbd4784938d7ef4debecdab_Out_1_Vector3;
        Unity_Negate_float3(_ViewVector_f8eabe5a287144a6a6d4ef737338317b_Out_0_Vector3, _Negate_d181b51abcbd4784938d7ef4debecdab_Out_1_Vector3);
        float4 _ScreenPosition_8e5642cf0f54480688de7e9414096c95_Out_0_Vector4 = IN.ScreenPosition;
        float _Split_47aeb1862e224482b604dba19d9665d1_R_1_Float = _ScreenPosition_8e5642cf0f54480688de7e9414096c95_Out_0_Vector4[0];
        float _Split_47aeb1862e224482b604dba19d9665d1_G_2_Float = _ScreenPosition_8e5642cf0f54480688de7e9414096c95_Out_0_Vector4[1];
        float _Split_47aeb1862e224482b604dba19d9665d1_B_3_Float = _ScreenPosition_8e5642cf0f54480688de7e9414096c95_Out_0_Vector4[2];
        float _Split_47aeb1862e224482b604dba19d9665d1_A_4_Float = _ScreenPosition_8e5642cf0f54480688de7e9414096c95_Out_0_Vector4[3];
        float3 _Divide_5a42a764edb04fdfb14c08f2df82448f_Out_2_Vector3;
        Unity_Divide_float3(_Negate_d181b51abcbd4784938d7ef4debecdab_Out_1_Vector3, (_Split_47aeb1862e224482b604dba19d9665d1_A_4_Float.xxx), _Divide_5a42a764edb04fdfb14c08f2df82448f_Out_2_Vector3);
        float2 _Property_d8a51218a3bb4194aa564336969f48bf_Out_0_Vector2 = _UV;
        float _SceneDepth_ae83678fa5f34fe095bf2035c218be22_Out_1_Float;
        Unity_SceneDepth_Eye_float((float4(_Property_d8a51218a3bb4194aa564336969f48bf_Out_0_Vector2, 0.0, 1.0)), _SceneDepth_ae83678fa5f34fe095bf2035c218be22_Out_1_Float);
        float3 _Multiply_36a67ab024a341298aa97e76a995919d_Out_2_Vector3;
        Unity_Multiply_float3_float3(_Divide_5a42a764edb04fdfb14c08f2df82448f_Out_2_Vector3, (_SceneDepth_ae83678fa5f34fe095bf2035c218be22_Out_1_Float.xxx), _Multiply_36a67ab024a341298aa97e76a995919d_Out_2_Vector3);
        float3 _Add_a73b06032a6b424ebc6dcb211cb1d7ec_Out_2_Vector3;
        Unity_Add_float3(_Multiply_36a67ab024a341298aa97e76a995919d_Out_2_Vector3, _WorldSpaceCameraPos, _Add_a73b06032a6b424ebc6dcb211cb1d7ec_Out_2_Vector3);
        float3 _Subtract_56e40e7df9514c6db684a06980c58e55_Out_2_Vector3;
        Unity_Subtract_float3(IN.WorldSpacePosition, _Add_a73b06032a6b424ebc6dcb211cb1d7ec_Out_2_Vector3, _Subtract_56e40e7df9514c6db684a06980c58e55_Out_2_Vector3);
        float _Split_8572215aaf714ae88e25b2db09553f0f_R_1_Float = _Subtract_56e40e7df9514c6db684a06980c58e55_Out_2_Vector3[0];
        float _Split_8572215aaf714ae88e25b2db09553f0f_G_2_Float = _Subtract_56e40e7df9514c6db684a06980c58e55_Out_2_Vector3[1];
        float _Split_8572215aaf714ae88e25b2db09553f0f_B_3_Float = _Subtract_56e40e7df9514c6db684a06980c58e55_Out_2_Vector3[2];
        float _Split_8572215aaf714ae88e25b2db09553f0f_A_4_Float = 0;
        float _Negate_38f930ab7f5148fbaca13a81a57e9720_Out_1_Float;
        Unity_Negate_float(_Split_8572215aaf714ae88e25b2db09553f0f_G_2_Float, _Negate_38f930ab7f5148fbaca13a81a57e9720_Out_1_Float);
        float _Property_58c167a0b3db4df1a74645ae7044c728_Out_0_Float = _DepthFadeDistance;
        float _Divide_1c7f36016a6143bfb91d1e0cefd0e84b_Out_2_Float;
        Unity_Divide_float(_Negate_38f930ab7f5148fbaca13a81a57e9720_Out_1_Float, _Property_58c167a0b3db4df1a74645ae7044c728_Out_0_Float, _Divide_1c7f36016a6143bfb91d1e0cefd0e84b_Out_2_Float);
        float _Exponential_5f2f255a73714085921dbe98b929d51a_Out_1_Float;
        Unity_Exponential_float(_Divide_1c7f36016a6143bfb91d1e0cefd0e84b_Out_2_Float, _Exponential_5f2f255a73714085921dbe98b929d51a_Out_1_Float);
        float _Saturate_4c21ba80863f474f9b1768b39a43d251_Out_1_Float;
        Unity_Saturate_float(_Exponential_5f2f255a73714085921dbe98b929d51a_Out_1_Float, _Saturate_4c21ba80863f474f9b1768b39a43d251_Out_1_Float);
        Linear_2 = _Saturate_4c21ba80863f474f9b1768b39a43d251_Out_1_Float;
        }
        
        void Unity_SceneDepth_Raw_float(float4 UV, out float Out)
        {
            Out = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy);
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        struct Bindings_DepthFade_1d0eb39db86b53245b582a69cdb87443_float
        {
        float4 ScreenPosition;
        };
        
        void SG_DepthFade_1d0eb39db86b53245b582a69cdb87443_float(float _DepthFadeDistance, float2 _UV, Bindings_DepthFade_1d0eb39db86b53245b582a69cdb87443_float IN, out float Linear_2)
        {
        float2 _Property_46bd4502b44f465ab1623fd92c5e4785_Out_0_Vector2 = _UV;
        float _SceneDepth_905c05d0cfa04274a8511269c1782b79_Out_1_Float;
        Unity_SceneDepth_Raw_float((float4(_Property_46bd4502b44f465ab1623fd92c5e4785_Out_0_Vector2, 0.0, 1.0)), _SceneDepth_905c05d0cfa04274a8511269c1782b79_Out_1_Float);
        float _Lerp_35568103c6e249c689ec4fd27f5e4ddd_Out_3_Float;
        Unity_Lerp_float(_ProjectionParams.z, _ProjectionParams.y, _SceneDepth_905c05d0cfa04274a8511269c1782b79_Out_1_Float, _Lerp_35568103c6e249c689ec4fd27f5e4ddd_Out_3_Float);
        float4 _ScreenPosition_e23a8dc3158f4a659ad17c8b8aff1495_Out_0_Vector4 = IN.ScreenPosition;
        float _Split_f8af98270ae4406cab897d102df99496_R_1_Float = _ScreenPosition_e23a8dc3158f4a659ad17c8b8aff1495_Out_0_Vector4[0];
        float _Split_f8af98270ae4406cab897d102df99496_G_2_Float = _ScreenPosition_e23a8dc3158f4a659ad17c8b8aff1495_Out_0_Vector4[1];
        float _Split_f8af98270ae4406cab897d102df99496_B_3_Float = _ScreenPosition_e23a8dc3158f4a659ad17c8b8aff1495_Out_0_Vector4[2];
        float _Split_f8af98270ae4406cab897d102df99496_A_4_Float = _ScreenPosition_e23a8dc3158f4a659ad17c8b8aff1495_Out_0_Vector4[3];
        float _Lerp_a9295eccb1424170a4e481b272012487_Out_3_Float;
        Unity_Lerp_float(_ProjectionParams.z, _ProjectionParams.y, _Split_f8af98270ae4406cab897d102df99496_B_3_Float, _Lerp_a9295eccb1424170a4e481b272012487_Out_3_Float);
        float _Subtract_e132f361175045cfb3fe9a7dca63a082_Out_2_Float;
        Unity_Subtract_float(_Lerp_35568103c6e249c689ec4fd27f5e4ddd_Out_3_Float, _Lerp_a9295eccb1424170a4e481b272012487_Out_3_Float, _Subtract_e132f361175045cfb3fe9a7dca63a082_Out_2_Float);
        float _Property_c3605d6a15074412bba55544d393b337_Out_0_Float = _DepthFadeDistance;
        float _Divide_bbbe2d9f0c3d47ddab512dc7748b6b89_Out_2_Float;
        Unity_Divide_float(_Subtract_e132f361175045cfb3fe9a7dca63a082_Out_2_Float, _Property_c3605d6a15074412bba55544d393b337_Out_0_Float, _Divide_bbbe2d9f0c3d47ddab512dc7748b6b89_Out_2_Float);
        float _Saturate_2a826d10488946e1b5f71424889b8f7a_Out_1_Float;
        Unity_Saturate_float(_Divide_bbbe2d9f0c3d47ddab512dc7748b6b89_Out_2_Float, _Saturate_2a826d10488946e1b5f71424889b8f7a_Out_1_Float);
        float _OneMinus_a6da5fafbee84ff782930c7bb3c4a8de_Out_1_Float;
        Unity_OneMinus_float(_Saturate_2a826d10488946e1b5f71424889b8f7a_Out_1_Float, _OneMinus_a6da5fafbee84ff782930c7bb3c4a8de_Out_1_Float);
        Linear_2 = _OneMinus_a6da5fafbee84ff782930c7bb3c4a8de_Out_1_Float;
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Posterize_float4(float4 In, float4 Steps, out float4 Out)
        {
            Out = floor(In * Steps) / Steps;
        }
        
        struct Bindings_GetDepth_370b72acb6baa764089512ed583870c4_float
        {
        float3 WorldSpacePosition;
        float4 ScreenPosition;
        float2 NDCPosition;
        half4 uv0;
        float3 TimeParameters;
        };
        
        void SG_GetDepth_370b72acb6baa764089512ed583870c4_float(float _DepthFadeDistance, float _Steps, float _RefractionSpeed, float _RefractionScale, float _GradientRefractionScale, float _RefractionStrength, float4 _ShallowColor, float4 _DeepColor, Bindings_GetDepth_370b72acb6baa764089512ed583870c4_float IN, out float4 RefractedUVs_1, out float4 Color_2)
        {
        float _Property_c733aafdfa2a4bb68b7ced01fffc396b_Out_0_Float = _RefractionSpeed;
        float _Property_89b378a1d2144a94b621a6c09e56b37c_Out_0_Float = _RefractionScale;
        float _Property_af7f8a7d74a74ab0a5f78596da12e191_Out_0_Float = _GradientRefractionScale;
        float _Property_61a396eba2ff468993be562ba48b818c_Out_0_Float = _RefractionStrength;
        Bindings_RefractedUV_dfdc6038fe7bdd5479fef69a0fcdb7ab_float _RefractedUV_fcd804a6c7444188a93a683c15c9f99a;
        _RefractedUV_fcd804a6c7444188a93a683c15c9f99a.NDCPosition = IN.NDCPosition;
        _RefractedUV_fcd804a6c7444188a93a683c15c9f99a.uv0 = IN.uv0;
        _RefractedUV_fcd804a6c7444188a93a683c15c9f99a.TimeParameters = IN.TimeParameters;
        half4 _RefractedUV_fcd804a6c7444188a93a683c15c9f99a_Out_1_Vector4;
        SG_RefractedUV_dfdc6038fe7bdd5479fef69a0fcdb7ab_float(_Property_c733aafdfa2a4bb68b7ced01fffc396b_Out_0_Float, _Property_89b378a1d2144a94b621a6c09e56b37c_Out_0_Float, _Property_af7f8a7d74a74ab0a5f78596da12e191_Out_0_Float, _Property_61a396eba2ff468993be562ba48b818c_Out_0_Float, _RefractedUV_fcd804a6c7444188a93a683c15c9f99a, _RefractedUV_fcd804a6c7444188a93a683c15c9f99a_Out_1_Vector4);
        float4 _Property_573520c11ff4493bb507d97f8a6f35c5_Out_0_Vector4 = _ShallowColor;
        float4 _Property_355869e5a92e4d5cbbacd7ea72fca417_Out_0_Vector4 = _DeepColor;
        float _Property_7a320c1c0e864afbb48839a06a61e877_Out_0_Float = _DepthFadeDistance;
        Bindings_DepthFadePerspective_3069152b34697f24bb49447c02013d98_float _DepthFadePerspective_d9affccc6fe74de2a2be6a12c8551576;
        _DepthFadePerspective_d9affccc6fe74de2a2be6a12c8551576.WorldSpacePosition = IN.WorldSpacePosition;
        _DepthFadePerspective_d9affccc6fe74de2a2be6a12c8551576.ScreenPosition = IN.ScreenPosition;
        float _DepthFadePerspective_d9affccc6fe74de2a2be6a12c8551576_Linear_2_Float;
        SG_DepthFadePerspective_3069152b34697f24bb49447c02013d98_float(_Property_7a320c1c0e864afbb48839a06a61e877_Out_0_Float, (_RefractedUV_fcd804a6c7444188a93a683c15c9f99a_Out_1_Vector4.xy), _DepthFadePerspective_d9affccc6fe74de2a2be6a12c8551576, _DepthFadePerspective_d9affccc6fe74de2a2be6a12c8551576_Linear_2_Float);
        Bindings_DepthFade_1d0eb39db86b53245b582a69cdb87443_float _DepthFade_84a135a9b3d547a7996fb768c62e334a;
        _DepthFade_84a135a9b3d547a7996fb768c62e334a.ScreenPosition = IN.ScreenPosition;
        half _DepthFade_84a135a9b3d547a7996fb768c62e334a_Linear_2_Float;
        SG_DepthFade_1d0eb39db86b53245b582a69cdb87443_float(_Property_7a320c1c0e864afbb48839a06a61e877_Out_0_Float, (_RefractedUV_fcd804a6c7444188a93a683c15c9f99a_Out_1_Vector4.xy), _DepthFade_84a135a9b3d547a7996fb768c62e334a, _DepthFade_84a135a9b3d547a7996fb768c62e334a_Linear_2_Float);
        #if defined(_PERSPECTIVE)
        float _Perspective_9b0c09a153cd4283a1dfa9ae48236cfd_Out_0_Float = _DepthFadePerspective_d9affccc6fe74de2a2be6a12c8551576_Linear_2_Float;
        #else
        float _Perspective_9b0c09a153cd4283a1dfa9ae48236cfd_Out_0_Float = _DepthFade_84a135a9b3d547a7996fb768c62e334a_Linear_2_Float;
        #endif
        float4 _Lerp_228b4c34a3434455a17ce3b570ccbf49_Out_3_Vector4;
        Unity_Lerp_float4(_Property_573520c11ff4493bb507d97f8a6f35c5_Out_0_Vector4, _Property_355869e5a92e4d5cbbacd7ea72fca417_Out_0_Vector4, (_Perspective_9b0c09a153cd4283a1dfa9ae48236cfd_Out_0_Float.xxxx), _Lerp_228b4c34a3434455a17ce3b570ccbf49_Out_3_Vector4);
        float _Property_442a941c64b64104be37b457e429de9f_Out_0_Float = _Steps;
        float4 _Posterize_cc9876a22c014323bf597f3e9f25d3a5_Out_2_Vector4;
        Unity_Posterize_float4(_Lerp_228b4c34a3434455a17ce3b570ccbf49_Out_3_Vector4, (_Property_442a941c64b64104be37b457e429de9f_Out_0_Float.xxxx), _Posterize_cc9876a22c014323bf597f3e9f25d3a5_Out_2_Vector4);
        #if defined(_A_SHORT_HIKE_MODE)
        float4 _AShortHikeMode_940dc24d9c7b4f5184729184326be4bc_Out_0_Vector4 = _Posterize_cc9876a22c014323bf597f3e9f25d3a5_Out_2_Vector4;
        #else
        float4 _AShortHikeMode_940dc24d9c7b4f5184729184326be4bc_Out_0_Vector4 = _Lerp_228b4c34a3434455a17ce3b570ccbf49_Out_3_Vector4;
        #endif
        RefractedUVs_1 = _RefractedUV_fcd804a6c7444188a93a683c15c9f99a_Out_1_Vector4;
        Color_2 = _AShortHikeMode_940dc24d9c7b4f5184729184326be4bc_Out_0_Vector4;
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), ViewDir))), Power);
        }
        
        struct Bindings_FresnelCalculation_f346593723353f4488f8a099aef520eb_float
        {
        float3 WorldSpaceNormal;
        float3 ViewSpaceNormal;
        float3 WorldSpaceViewDirection;
        };
        
        void SG_FresnelCalculation_f346593723353f4488f8a099aef520eb_float(float _HorizonDistance, Bindings_FresnelCalculation_f346593723353f4488f8a099aef520eb_float IN, out float FresnelResult_1)
        {
        float _Property_bb1b567c8cd54712be0aa1ab56158c06_Out_0_Float = _HorizonDistance;
        float _FresnelEffect_5cb1ab2db50c4d85aa1315d66acc019b_Out_3_Float;
        Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_bb1b567c8cd54712be0aa1ab56158c06_Out_0_Float, _FresnelEffect_5cb1ab2db50c4d85aa1315d66acc019b_Out_3_Float);
        float3 _Vector3_3df06ab61c94407b9b2bb68b2bd8c970_Out_0_Vector3 = float3(float(0), float(0), float(1));
        float _Property_3a9a52a5e7f84053bce12874f9ecb16e_Out_0_Float = _HorizonDistance;
        float _FresnelEffect_d5defa2cb078451f8600e86b230e0a30_Out_3_Float;
        Unity_FresnelEffect_float(IN.ViewSpaceNormal, _Vector3_3df06ab61c94407b9b2bb68b2bd8c970_Out_0_Vector3, _Property_3a9a52a5e7f84053bce12874f9ecb16e_Out_0_Float, _FresnelEffect_d5defa2cb078451f8600e86b230e0a30_Out_3_Float);
        #if defined(_PERSPECTIVE)
        float _Perspective_e2d93f83b3444bdc9b039aff2c3f109d_Out_0_Float = _FresnelEffect_5cb1ab2db50c4d85aa1315d66acc019b_Out_3_Float;
        #else
        float _Perspective_e2d93f83b3444bdc9b039aff2c3f109d_Out_0_Float = _FresnelEffect_d5defa2cb078451f8600e86b230e0a30_Out_3_Float;
        #endif
        FresnelResult_1 = _Perspective_e2d93f83b3444bdc9b039aff2c3f109d_Out_0_Float;
        }
        
        void Unity_SceneColor_float(float4 UV, out float3 Out)
        {
            Out = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV.xy);
        }
        
        struct Bindings_BlendObjectColor_a02636e967a650d4db669ee613538c46_float
        {
        };
        
        void SG_BlendObjectColor_a02636e967a650d4db669ee613538c46_float(float4 _RefractedUVs, float4 _BaseBlend, Bindings_BlendObjectColor_a02636e967a650d4db669ee613538c46_float IN, out float3 ObjectBlend_1)
        {
        float4 _Property_4f56835f385c4497ad58269d3e3ce6ed_Out_0_Vector4 = _RefractedUVs;
        float3 _SceneColor_b32346b7705c4334a0ac120dcde6901b_Out_1_Vector3;
        Unity_SceneColor_float(_Property_4f56835f385c4497ad58269d3e3ce6ed_Out_0_Vector4, _SceneColor_b32346b7705c4334a0ac120dcde6901b_Out_1_Vector3);
        float4 _Property_8790a77a1a4f4a63b11b3c4719708e2d_Out_0_Vector4 = _BaseBlend;
        float _Split_e4aad686056f4ef79a23e5ee7e845512_R_1_Float = _Property_8790a77a1a4f4a63b11b3c4719708e2d_Out_0_Vector4[0];
        float _Split_e4aad686056f4ef79a23e5ee7e845512_G_2_Float = _Property_8790a77a1a4f4a63b11b3c4719708e2d_Out_0_Vector4[1];
        float _Split_e4aad686056f4ef79a23e5ee7e845512_B_3_Float = _Property_8790a77a1a4f4a63b11b3c4719708e2d_Out_0_Vector4[2];
        float _Split_e4aad686056f4ef79a23e5ee7e845512_A_4_Float = _Property_8790a77a1a4f4a63b11b3c4719708e2d_Out_0_Vector4[3];
        float _OneMinus_dd70c5f5558d4199a84216d1b5da52c2_Out_1_Float;
        Unity_OneMinus_float(_Split_e4aad686056f4ef79a23e5ee7e845512_A_4_Float, _OneMinus_dd70c5f5558d4199a84216d1b5da52c2_Out_1_Float);
        float3 _Multiply_b7686a561f2542a8b83587eee957226c_Out_2_Vector3;
        Unity_Multiply_float3_float3(_SceneColor_b32346b7705c4334a0ac120dcde6901b_Out_1_Vector3, (_OneMinus_dd70c5f5558d4199a84216d1b5da52c2_Out_1_Float.xxx), _Multiply_b7686a561f2542a8b83587eee957226c_Out_2_Vector3);
        ObjectBlend_1 = _Multiply_b7686a561f2542a8b83587eee957226c_Out_2_Vector3;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Negate_float2(float2 In, out float2 Out)
        {
            Out = -1 * In;
        }
        
        struct Bindings_CalculateUVs_60d47073ac03df54f9a15991680de154_float
        {
        float3 WorldSpacePosition;
        half4 uv0;
        };
        
        void SG_CalculateUVs_60d47073ac03df54f9a15991680de154_float(Bindings_CalculateUVs_60d47073ac03df54f9a15991680de154_float IN, out float2 OutUVs_1)
        {
        float _Split_3c90832226cd494ea28785fdc0cd3dc9_R_1_Float = IN.WorldSpacePosition[0];
        float _Split_3c90832226cd494ea28785fdc0cd3dc9_G_2_Float = IN.WorldSpacePosition[1];
        float _Split_3c90832226cd494ea28785fdc0cd3dc9_B_3_Float = IN.WorldSpacePosition[2];
        float _Split_3c90832226cd494ea28785fdc0cd3dc9_A_4_Float = 0;
        float2 _Vector2_536cab5e22a44d6d811a24b0e0e70684_Out_0_Vector2 = float2(_Split_3c90832226cd494ea28785fdc0cd3dc9_R_1_Float, _Split_3c90832226cd494ea28785fdc0cd3dc9_B_3_Float);
        float _Float_d2a1470c5d4f42498583a13546adc89f_Out_0_Float = float(0.1);
        float2 _Multiply_72d6d69cb24149cf8949c1b441fd6553_Out_2_Vector2;
        Unity_Multiply_float2_float2(_Vector2_536cab5e22a44d6d811a24b0e0e70684_Out_0_Vector2, (_Float_d2a1470c5d4f42498583a13546adc89f_Out_0_Float.xx), _Multiply_72d6d69cb24149cf8949c1b441fd6553_Out_2_Vector2);
        float2 _Negate_ea982ed93e8b41c1acbc0c7beddc5cb0_Out_1_Vector2;
        Unity_Negate_float2(_Multiply_72d6d69cb24149cf8949c1b441fd6553_Out_2_Vector2, _Negate_ea982ed93e8b41c1acbc0c7beddc5cb0_Out_1_Vector2);
        float4 _UV_8ca0102c0c1e4dd6b63c964274692cf2_Out_0_Vector4 = IN.uv0;
        #if defined(_WORLD_SPACE_UV)
        float2 _WorldSpaceUV_1110b220d2374503b2e7abe001c6420a_Out_0_Vector2 = _Negate_ea982ed93e8b41c1acbc0c7beddc5cb0_Out_1_Vector2;
        #else
        float2 _WorldSpaceUV_1110b220d2374503b2e7abe001c6420a_Out_0_Vector2 = (_UV_8ca0102c0c1e4dd6b63c964274692cf2_Out_0_Vector4.xy);
        #endif
        OutUVs_1 = _WorldSpaceUV_1110b220d2374503b2e7abe001c6420a_Out_0_Vector2;
        }
        
        void Unity_InvertColors_float4(float4 In, float4 InvertColors, out float4 Out)
        {
        Out = abs(InvertColors - In);
        }
        
        struct Bindings_InvertColors_a5c43e269304bff4a91c002de46388cb_float
        {
        };
        
        void SG_InvertColors_a5c43e269304bff4a91c002de46388cb_float(float4 _InputColor, Bindings_InvertColors_a5c43e269304bff4a91c002de46388cb_float IN, out float4 Output_1)
        {
        float4 _Property_3ba350a2ed3c45a6b7b551a667768c4f_Out_0_Vector4 = _InputColor;
        float4 _InvertColors_04f15eda597141b2a2df5fd6f33e184d_Out_1_Vector4;
        float4 _InvertColors_04f15eda597141b2a2df5fd6f33e184d_InvertColors = float4 (1, 1, 1, 0);
        Unity_InvertColors_float4(_Property_3ba350a2ed3c45a6b7b551a667768c4f_Out_0_Vector4, _InvertColors_04f15eda597141b2a2df5fd6f33e184d_InvertColors, _InvertColors_04f15eda597141b2a2df5fd6f33e184d_Out_1_Vector4);
        Output_1 = _InvertColors_04f15eda597141b2a2df5fd6f33e184d_Out_1_Vector4;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
        Out = A * B;
        }
        
        struct Bindings_Caves_3e5c142028ba07d48bba33c5843ba17e_float
        {
        };
        
        void SG_Caves_3e5c142028ba07d48bba33c5843ba17e_float(float2 _UVs, float2 _CaveScale, float2 _CaveOffset, float _CaveDistortion, UnityTexture2D _CaveTexture, float4 _CaveColor, float _CaveSteps, Bindings_Caves_3e5c142028ba07d48bba33c5843ba17e_float IN, out float4 CaveResult_1)
        {
        float4 _Property_cd4c3ae601304329890ad7ec6b15bd87_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_CaveColor) : _CaveColor;
        float _Split_5dc03030aee348088967c93897290323_R_1_Float = _Property_cd4c3ae601304329890ad7ec6b15bd87_Out_0_Vector4[0];
        float _Split_5dc03030aee348088967c93897290323_G_2_Float = _Property_cd4c3ae601304329890ad7ec6b15bd87_Out_0_Vector4[1];
        float _Split_5dc03030aee348088967c93897290323_B_3_Float = _Property_cd4c3ae601304329890ad7ec6b15bd87_Out_0_Vector4[2];
        float _Split_5dc03030aee348088967c93897290323_A_4_Float = _Property_cd4c3ae601304329890ad7ec6b15bd87_Out_0_Vector4[3];
        UnityTexture2D _Property_3b2b9c24469d4f2ba6d01ced7669fe09_Out_0_Texture2D = _CaveTexture;
        float2 _Property_402263488ae84f86891ad666a842ae5b_Out_0_Vector2 = _UVs;
        float2 _Property_a282ff33e0ce4b5f82f90283e3462360_Out_0_Vector2 = _CaveScale;
        float2 _Property_3e7ad0d7638f4386bf8a554e3efafc78_Out_0_Vector2 = _CaveOffset;
        float2 _TilingAndOffset_7cf0fae9a7aa4e29a98c855468b7ac0d_Out_3_Vector2;
        Unity_TilingAndOffset_float(_Property_402263488ae84f86891ad666a842ae5b_Out_0_Vector2, _Property_a282ff33e0ce4b5f82f90283e3462360_Out_0_Vector2, _Property_3e7ad0d7638f4386bf8a554e3efafc78_Out_0_Vector2, _TilingAndOffset_7cf0fae9a7aa4e29a98c855468b7ac0d_Out_3_Vector2);
        float _Property_a056c657e9bc40398fd6f7d24aaa536a_Out_0_Float = _CaveDistortion;
        float2 _DistortUVCustomFunction_b45cce0f371c47409824f9da6f9978e4_Out_2_Vector2;
        DistortUV_float(_TilingAndOffset_7cf0fae9a7aa4e29a98c855468b7ac0d_Out_3_Vector2, _Property_a056c657e9bc40398fd6f7d24aaa536a_Out_0_Float, _DistortUVCustomFunction_b45cce0f371c47409824f9da6f9978e4_Out_2_Vector2);
        float4 _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_3b2b9c24469d4f2ba6d01ced7669fe09_Out_0_Texture2D.tex, _Property_3b2b9c24469d4f2ba6d01ced7669fe09_Out_0_Texture2D.samplerstate, _Property_3b2b9c24469d4f2ba6d01ced7669fe09_Out_0_Texture2D.GetTransformedUV(_DistortUVCustomFunction_b45cce0f371c47409824f9da6f9978e4_Out_2_Vector2) );
        float _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_R_4_Float = _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_RGBA_0_Vector4.r;
        float _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_G_5_Float = _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_RGBA_0_Vector4.g;
        float _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_B_6_Float = _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_RGBA_0_Vector4.b;
        float _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_A_7_Float = _SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_RGBA_0_Vector4.a;
        Bindings_InvertColors_a5c43e269304bff4a91c002de46388cb_float _InvertColors_510d701d607d46aabb64028a0d8d87dc;
        float4 _InvertColors_510d701d607d46aabb64028a0d8d87dc_Output_1_Vector4;
        SG_InvertColors_a5c43e269304bff4a91c002de46388cb_float((_SampleTexture2D_b892a2f983a945dc899621eaa5b701e3_R_4_Float.xxxx), _InvertColors_510d701d607d46aabb64028a0d8d87dc, _InvertColors_510d701d607d46aabb64028a0d8d87dc_Output_1_Vector4);
        float4 _Multiply_2465dbb958a04bb692c3d0cd819466f0_Out_2_Vector4;
        Unity_Multiply_float4_float4((_Split_5dc03030aee348088967c93897290323_A_4_Float.xxxx), _InvertColors_510d701d607d46aabb64028a0d8d87dc_Output_1_Vector4, _Multiply_2465dbb958a04bb692c3d0cd819466f0_Out_2_Vector4);
        CaveResult_1 = _Multiply_2465dbb958a04bb692c3d0cd819466f0_Out_2_Vector4;
        }
        
        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_Reflections_2a83053698b0a9d46a141dd7d36ac2ac_float
        {
        float2 NDCPosition;
        };
        
        void SG_Reflections_2a83053698b0a9d46a141dd7d36ac2ac_float(UnityTexture2D _ReflectionMap, float _ReflectionDistortion, float _ReflectionBlend, Bindings_Reflections_2a83053698b0a9d46a141dd7d36ac2ac_float IN, out float4 ReflexColor_1, out float ReflexResult_2)
        {
        UnityTexture2D _Property_01b9233f96594a05b2c16963f58eb91b_Out_0_Texture2D = _ReflectionMap;
        float4 _ScreenPosition_da0e6619fc3f4e22984f43026a0b5c6f_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
        float _Property_4aeb41f2c0de45b8959feed6af6998ee_Out_0_Float = _ReflectionDistortion;
        float2 _DistortUVCustomFunction_3f714cd044804cf598e93af0be6d96e0_Out_2_Vector2;
        DistortUV_float((_ScreenPosition_da0e6619fc3f4e22984f43026a0b5c6f_Out_0_Vector4.xy), _Property_4aeb41f2c0de45b8959feed6af6998ee_Out_0_Float, _DistortUVCustomFunction_3f714cd044804cf598e93af0be6d96e0_Out_2_Vector2);
        float4 _SampleTexture2D_143e4398349948279eee045e0f9170eb_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_01b9233f96594a05b2c16963f58eb91b_Out_0_Texture2D.tex, _Property_01b9233f96594a05b2c16963f58eb91b_Out_0_Texture2D.samplerstate, _Property_01b9233f96594a05b2c16963f58eb91b_Out_0_Texture2D.GetTransformedUV(_DistortUVCustomFunction_3f714cd044804cf598e93af0be6d96e0_Out_2_Vector2) );
        float _SampleTexture2D_143e4398349948279eee045e0f9170eb_R_4_Float = _SampleTexture2D_143e4398349948279eee045e0f9170eb_RGBA_0_Vector4.r;
        float _SampleTexture2D_143e4398349948279eee045e0f9170eb_G_5_Float = _SampleTexture2D_143e4398349948279eee045e0f9170eb_RGBA_0_Vector4.g;
        float _SampleTexture2D_143e4398349948279eee045e0f9170eb_B_6_Float = _SampleTexture2D_143e4398349948279eee045e0f9170eb_RGBA_0_Vector4.b;
        float _SampleTexture2D_143e4398349948279eee045e0f9170eb_A_7_Float = _SampleTexture2D_143e4398349948279eee045e0f9170eb_RGBA_0_Vector4.a;
        float _Property_7a32419bb4c146e68196c2d5904b5441_Out_0_Float = _ReflectionBlend;
        ReflexColor_1 = _SampleTexture2D_143e4398349948279eee045e0f9170eb_RGBA_0_Vector4;
        ReflexResult_2 = _Property_7a32419bb4c146e68196c2d5904b5441_Out_0_Float;
        }
        
        void Unity_Cosine_float(float In, out float Out)
        {
            Out = cos(In);
        }
        
        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }
        
        void Unity_Normalize_float2(float2 In, out float2 Out)
        {
            Out = normalize(In);
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_PanningUVs_ef286e626cee63841803a024235a644f_float
        {
        float3 TimeParameters;
        };
        
        void SG_PanningUVs_ef286e626cee63841803a024235a644f_float(float _Direction, float _Speed, float2 _UV, float _Tiling, float2 _Offset, Bindings_PanningUVs_ef286e626cee63841803a024235a644f_float IN, out float2 UVOut_1)
        {
        float2 _Property_fa3bd6908dc14afe9e9a8fa8f17ecdb7_Out_0_Vector2 = _UV;
        float _Property_942aaf71f8624c5b9c5b0d1dd04d30f7_Out_0_Float = _Tiling;
        float2 _Multiply_d04caa119e264eca905786c7590f6758_Out_2_Vector2;
        Unity_Multiply_float2_float2(_Property_fa3bd6908dc14afe9e9a8fa8f17ecdb7_Out_0_Vector2, (_Property_942aaf71f8624c5b9c5b0d1dd04d30f7_Out_0_Float.xx), _Multiply_d04caa119e264eca905786c7590f6758_Out_2_Vector2);
        float _Property_c88c4b7bebfd4fc9bf845a4e710e2e20_Out_0_Float = _Direction;
        float _Multiply_b7c33bfe44644595a67b0bcaad29d479_Out_2_Float;
        Unity_Multiply_float_float(_Property_c88c4b7bebfd4fc9bf845a4e710e2e20_Out_0_Float, 2, _Multiply_b7c33bfe44644595a67b0bcaad29d479_Out_2_Float);
        float _Subtract_bd70d912c9e44cb4b775def278ab8299_Out_2_Float;
        Unity_Subtract_float(_Multiply_b7c33bfe44644595a67b0bcaad29d479_Out_2_Float, float(1), _Subtract_bd70d912c9e44cb4b775def278ab8299_Out_2_Float);
        float Constant_469e2f41ab9a469fbded168e7b78fd45 = 3.141593;
        float _Multiply_a2e7e533449a4a72879d25e5c791d0bb_Out_2_Float;
        Unity_Multiply_float_float(_Subtract_bd70d912c9e44cb4b775def278ab8299_Out_2_Float, Constant_469e2f41ab9a469fbded168e7b78fd45, _Multiply_a2e7e533449a4a72879d25e5c791d0bb_Out_2_Float);
        float _Cosine_3e0d75ecc2d14605aad7642b8bcdc461_Out_1_Float;
        Unity_Cosine_float(_Multiply_a2e7e533449a4a72879d25e5c791d0bb_Out_2_Float, _Cosine_3e0d75ecc2d14605aad7642b8bcdc461_Out_1_Float);
        float _Sine_d7fde7b84a36487ca0571ab4b845689a_Out_1_Float;
        Unity_Sine_float(_Multiply_a2e7e533449a4a72879d25e5c791d0bb_Out_2_Float, _Sine_d7fde7b84a36487ca0571ab4b845689a_Out_1_Float);
        float2 _Vector2_5822526fa5384bc2993095be23b1ac5c_Out_0_Vector2 = float2(_Cosine_3e0d75ecc2d14605aad7642b8bcdc461_Out_1_Float, _Sine_d7fde7b84a36487ca0571ab4b845689a_Out_1_Float);
        float2 _Normalize_7ef1b5e1c36845dcb3532f1070eab07d_Out_1_Vector2;
        Unity_Normalize_float2(_Vector2_5822526fa5384bc2993095be23b1ac5c_Out_0_Vector2, _Normalize_7ef1b5e1c36845dcb3532f1070eab07d_Out_1_Vector2);
        float _Property_06fb219b3c4348db8a16781439392559_Out_0_Float = _Speed;
        float _Multiply_0b0996e501624102825e5d908b3a7ded_Out_2_Float;
        Unity_Multiply_float_float(IN.TimeParameters.x, _Property_06fb219b3c4348db8a16781439392559_Out_0_Float, _Multiply_0b0996e501624102825e5d908b3a7ded_Out_2_Float);
        float2 _Multiply_e6e19d2e699747abafa7335b844aa827_Out_2_Vector2;
        Unity_Multiply_float2_float2(_Normalize_7ef1b5e1c36845dcb3532f1070eab07d_Out_1_Vector2, (_Multiply_0b0996e501624102825e5d908b3a7ded_Out_2_Float.xx), _Multiply_e6e19d2e699747abafa7335b844aa827_Out_2_Vector2);
        float2 _Add_0cf695f5db0b477e9565fc781fea6cab_Out_2_Vector2;
        Unity_Add_float2(_Multiply_d04caa119e264eca905786c7590f6758_Out_2_Vector2, _Multiply_e6e19d2e699747abafa7335b844aa827_Out_2_Vector2, _Add_0cf695f5db0b477e9565fc781fea6cab_Out_2_Vector2);
        float2 _Property_0b770ef0e6db42bba96e87ccc9764d0a_Out_0_Vector2 = _Offset;
        float2 _Add_da503712d45c458e95b43c94682472af_Out_2_Vector2;
        Unity_Add_float2(_Add_0cf695f5db0b477e9565fc781fea6cab_Out_2_Vector2, _Property_0b770ef0e6db42bba96e87ccc9764d0a_Out_0_Vector2, _Add_da503712d45c458e95b43c94682472af_Out_2_Vector2);
        UVOut_1 = _Add_da503712d45c458e95b43c94682472af_Out_2_Vector2;
        }
        
        struct Bindings_SecondaryFoam_a707540033b78f44bbef5139f65097d0_float
        {
        float3 TimeParameters;
        };
        
        void SG_SecondaryFoam_a707540033b78f44bbef5139f65097d0_float(float _FoamDirection, float _FoamSpeed, float2 _UVs, float _FoamTiling, float2 _FoamOffset, float _FoamDistortion, UnityTexture2D _FoamTexture, float4 _FoamColor, Bindings_SecondaryFoam_a707540033b78f44bbef5139f65097d0_float IN, out float4 SecondaryFoamResult_1)
        {
        UnityTexture2D _Property_5cd791bd402243e9b2fe160b9ad987dd_Out_0_Texture2D = _FoamTexture;
        float _Property_639af70a388d4c20b6ba8f1a885ccce2_Out_0_Float = _FoamDirection;
        float _Property_bd29fc115c4546dc958cdc06b9143f4a_Out_0_Float = _FoamSpeed;
        float2 _Property_bce6ecfee5474ddab7a5d7f51f6cc08b_Out_0_Vector2 = _UVs;
        float _Property_598525370a5f40929f9000a2467ce4f9_Out_0_Float = _FoamTiling;
        float2 _Property_caa367a036da4cad8383f95ca75df903_Out_0_Vector2 = _FoamOffset;
        Bindings_PanningUVs_ef286e626cee63841803a024235a644f_float _PanningUVs_34a31afa5e2346d5b3aaa70850f3527c;
        _PanningUVs_34a31afa5e2346d5b3aaa70850f3527c.TimeParameters = IN.TimeParameters;
        half2 _PanningUVs_34a31afa5e2346d5b3aaa70850f3527c_UVOut_1_Vector2;
        SG_PanningUVs_ef286e626cee63841803a024235a644f_float(_Property_639af70a388d4c20b6ba8f1a885ccce2_Out_0_Float, _Property_bd29fc115c4546dc958cdc06b9143f4a_Out_0_Float, _Property_bce6ecfee5474ddab7a5d7f51f6cc08b_Out_0_Vector2, _Property_598525370a5f40929f9000a2467ce4f9_Out_0_Float, _Property_caa367a036da4cad8383f95ca75df903_Out_0_Vector2, _PanningUVs_34a31afa5e2346d5b3aaa70850f3527c, _PanningUVs_34a31afa5e2346d5b3aaa70850f3527c_UVOut_1_Vector2);
        float _Property_93624abbebe64bc6a23ac0fcc02ec55d_Out_0_Float = _FoamDistortion;
        float2 _DistortUVCustomFunction_40d46d29f5404679973fed10fa7b54ff_Out_2_Vector2;
        DistortUV_float(_PanningUVs_34a31afa5e2346d5b3aaa70850f3527c_UVOut_1_Vector2, _Property_93624abbebe64bc6a23ac0fcc02ec55d_Out_0_Float, _DistortUVCustomFunction_40d46d29f5404679973fed10fa7b54ff_Out_2_Vector2);
        float4 _SampleTexture2D_1a216311884a4e8683fd5938622d3402_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_5cd791bd402243e9b2fe160b9ad987dd_Out_0_Texture2D.tex, _Property_5cd791bd402243e9b2fe160b9ad987dd_Out_0_Texture2D.samplerstate, _Property_5cd791bd402243e9b2fe160b9ad987dd_Out_0_Texture2D.GetTransformedUV(_DistortUVCustomFunction_40d46d29f5404679973fed10fa7b54ff_Out_2_Vector2) );
        float _SampleTexture2D_1a216311884a4e8683fd5938622d3402_R_4_Float = _SampleTexture2D_1a216311884a4e8683fd5938622d3402_RGBA_0_Vector4.r;
        float _SampleTexture2D_1a216311884a4e8683fd5938622d3402_G_5_Float = _SampleTexture2D_1a216311884a4e8683fd5938622d3402_RGBA_0_Vector4.g;
        float _SampleTexture2D_1a216311884a4e8683fd5938622d3402_B_6_Float = _SampleTexture2D_1a216311884a4e8683fd5938622d3402_RGBA_0_Vector4.b;
        float _SampleTexture2D_1a216311884a4e8683fd5938622d3402_A_7_Float = _SampleTexture2D_1a216311884a4e8683fd5938622d3402_RGBA_0_Vector4.a;
        float4 _Property_236d6b687b6444c0b1db6e064076ddf4_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_FoamColor) : _FoamColor;
        float _Split_29237e4769584ccab6c7fad57c825ede_R_1_Float = _Property_236d6b687b6444c0b1db6e064076ddf4_Out_0_Vector4[0];
        float _Split_29237e4769584ccab6c7fad57c825ede_G_2_Float = _Property_236d6b687b6444c0b1db6e064076ddf4_Out_0_Vector4[1];
        float _Split_29237e4769584ccab6c7fad57c825ede_B_3_Float = _Property_236d6b687b6444c0b1db6e064076ddf4_Out_0_Vector4[2];
        float _Split_29237e4769584ccab6c7fad57c825ede_A_4_Float = _Property_236d6b687b6444c0b1db6e064076ddf4_Out_0_Vector4[3];
        float _Multiply_7e4e840580f44d6d816c3daffe008e63_Out_2_Float;
        Unity_Multiply_float_float(_SampleTexture2D_1a216311884a4e8683fd5938622d3402_R_4_Float, _Split_29237e4769584ccab6c7fad57c825ede_A_4_Float, _Multiply_7e4e840580f44d6d816c3daffe008e63_Out_2_Float);
        SecondaryFoamResult_1 = (_Multiply_7e4e840580f44d6d816c3daffe008e63_Out_2_Float.xxxx);
        }
        
        struct Bindings_PrimaryFoam_f41bb18b12540894cadcf85fcc438801_float
        {
        float3 TimeParameters;
        };
        
        void SG_PrimaryFoam_f41bb18b12540894cadcf85fcc438801_float(float _FoamDirection, float _FoamSpeed, float _FoamTiling, float _FoamDistortion, float2 _UVS, UnityTexture2D _FoamTexture, float4 _FoamColor, Bindings_PrimaryFoam_f41bb18b12540894cadcf85fcc438801_float IN, out float4 FoamResult_1)
        {
        UnityTexture2D _Property_f3f9f2f74aba4fbf9a3ce681a0f63fb9_Out_0_Texture2D = _FoamTexture;
        float _Property_da04d3c39d5743c58c63df85172de1d9_Out_0_Float = _FoamDirection;
        float _Property_d2db43137223470fb028d2186633912f_Out_0_Float = _FoamSpeed;
        float2 _Property_65972f26cc794487af7180bcb97a89bc_Out_0_Vector2 = _UVS;
        float _Property_f3b4abaec7fc41ff9898ad9f70756e98_Out_0_Float = _FoamTiling;
        Bindings_PanningUVs_ef286e626cee63841803a024235a644f_float _PanningUVs_aaf741e14dc140e7b99b861bdf19f098;
        _PanningUVs_aaf741e14dc140e7b99b861bdf19f098.TimeParameters = IN.TimeParameters;
        half2 _PanningUVs_aaf741e14dc140e7b99b861bdf19f098_UVOut_1_Vector2;
        SG_PanningUVs_ef286e626cee63841803a024235a644f_float(_Property_da04d3c39d5743c58c63df85172de1d9_Out_0_Float, _Property_d2db43137223470fb028d2186633912f_Out_0_Float, _Property_65972f26cc794487af7180bcb97a89bc_Out_0_Vector2, _Property_f3b4abaec7fc41ff9898ad9f70756e98_Out_0_Float, half2 (0, 0), _PanningUVs_aaf741e14dc140e7b99b861bdf19f098, _PanningUVs_aaf741e14dc140e7b99b861bdf19f098_UVOut_1_Vector2);
        float _Property_e4d9b3d9ed934e7dab9f09af0b6e6b6e_Out_0_Float = _FoamDistortion;
        float2 _DistortUVCustomFunction_0552020a4ba34f43a99711cb8a336320_Out_2_Vector2;
        DistortUV_float(_PanningUVs_aaf741e14dc140e7b99b861bdf19f098_UVOut_1_Vector2, _Property_e4d9b3d9ed934e7dab9f09af0b6e6b6e_Out_0_Float, _DistortUVCustomFunction_0552020a4ba34f43a99711cb8a336320_Out_2_Vector2);
        float4 _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_f3f9f2f74aba4fbf9a3ce681a0f63fb9_Out_0_Texture2D.tex, _Property_f3f9f2f74aba4fbf9a3ce681a0f63fb9_Out_0_Texture2D.samplerstate, _Property_f3f9f2f74aba4fbf9a3ce681a0f63fb9_Out_0_Texture2D.GetTransformedUV(_DistortUVCustomFunction_0552020a4ba34f43a99711cb8a336320_Out_2_Vector2) );
        float _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_R_4_Float = _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_RGBA_0_Vector4.r;
        float _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_G_5_Float = _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_RGBA_0_Vector4.g;
        float _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_B_6_Float = _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_RGBA_0_Vector4.b;
        float _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_A_7_Float = _SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_RGBA_0_Vector4.a;
        float4 _Property_6db672f2d1e64c12bad7ab94896b619b_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_FoamColor) : _FoamColor;
        float _Split_74f92acb87644a86ad859aab45ff575e_R_1_Float = _Property_6db672f2d1e64c12bad7ab94896b619b_Out_0_Vector4[0];
        float _Split_74f92acb87644a86ad859aab45ff575e_G_2_Float = _Property_6db672f2d1e64c12bad7ab94896b619b_Out_0_Vector4[1];
        float _Split_74f92acb87644a86ad859aab45ff575e_B_3_Float = _Property_6db672f2d1e64c12bad7ab94896b619b_Out_0_Vector4[2];
        float _Split_74f92acb87644a86ad859aab45ff575e_A_4_Float = _Property_6db672f2d1e64c12bad7ab94896b619b_Out_0_Vector4[3];
        float _Multiply_016b5234eb4847c5961f4a6df79e55ce_Out_2_Float;
        Unity_Multiply_float_float(_SampleTexture2D_1c51bd1b5e684bcb9f3c4cc1b7190188_R_4_Float, _Split_74f92acb87644a86ad859aab45ff575e_A_4_Float, _Multiply_016b5234eb4847c5961f4a6df79e55ce_Out_2_Float);
        FoamResult_1 = (_Multiply_016b5234eb4847c5961f4a6df79e55ce_Out_2_Float.xxxx);
        }
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        struct Bindings_IntersectionFoam_e12de5bce35668a4e8839cbe5732d905_float
        {
        float3 WorldSpacePosition;
        float4 ScreenPosition;
        float2 NDCPosition;
        float3 TimeParameters;
        };
        
        void SG_IntersectionFoam_e12de5bce35668a4e8839cbe5732d905_float(float _IntersectionFoamDepth, float _IntersectionFoamDirection, float _IntersectionFoamSpeed, float2 _UVs, float _IntersectionFoamTiling, float _IntersectionFoamFade, float _IntersectionFoamCutoff, UnityTexture2D _IntersectionFoamTexture, float4 _IntersectionFoamColor, Bindings_IntersectionFoam_e12de5bce35668a4e8839cbe5732d905_float IN, out float4 IntersectionFoamResult_1)
        {
        float _Property_436db722ba574b9da5152ea0b0fc269a_Out_0_Float = _IntersectionFoamDepth;
        float4 _ScreenPosition_d0ee1edadfe44fe1a6aa5df080569063_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
        Bindings_DepthFadePerspective_3069152b34697f24bb49447c02013d98_float _DepthFadePerspective_170dce4f8e3a4f75b1113c08817d6d10;
        _DepthFadePerspective_170dce4f8e3a4f75b1113c08817d6d10.WorldSpacePosition = IN.WorldSpacePosition;
        _DepthFadePerspective_170dce4f8e3a4f75b1113c08817d6d10.ScreenPosition = IN.ScreenPosition;
        float _DepthFadePerspective_170dce4f8e3a4f75b1113c08817d6d10_Linear_2_Float;
        SG_DepthFadePerspective_3069152b34697f24bb49447c02013d98_float(_Property_436db722ba574b9da5152ea0b0fc269a_Out_0_Float, (_ScreenPosition_d0ee1edadfe44fe1a6aa5df080569063_Out_0_Vector4.xy), _DepthFadePerspective_170dce4f8e3a4f75b1113c08817d6d10, _DepthFadePerspective_170dce4f8e3a4f75b1113c08817d6d10_Linear_2_Float);
        Bindings_DepthFade_1d0eb39db86b53245b582a69cdb87443_float _DepthFade_011030f3d4d0420b919705a8a53dedb6;
        _DepthFade_011030f3d4d0420b919705a8a53dedb6.ScreenPosition = IN.ScreenPosition;
        half _DepthFade_011030f3d4d0420b919705a8a53dedb6_Linear_2_Float;
        SG_DepthFade_1d0eb39db86b53245b582a69cdb87443_float(_Property_436db722ba574b9da5152ea0b0fc269a_Out_0_Float, (_ScreenPosition_d0ee1edadfe44fe1a6aa5df080569063_Out_0_Vector4.xy), _DepthFade_011030f3d4d0420b919705a8a53dedb6, _DepthFade_011030f3d4d0420b919705a8a53dedb6_Linear_2_Float);
        #if defined(_PERSPECTIVE)
        float _Perspective_917cfd8f51134dddafbdf7fcdbc16980_Out_0_Float = _DepthFadePerspective_170dce4f8e3a4f75b1113c08817d6d10_Linear_2_Float;
        #else
        float _Perspective_917cfd8f51134dddafbdf7fcdbc16980_Out_0_Float = _DepthFade_011030f3d4d0420b919705a8a53dedb6_Linear_2_Float;
        #endif
        float _Property_2c38ceda851245a9af3cedf5fa670532_Out_0_Float = _IntersectionFoamCutoff;
        float _Multiply_66d0b19b85644261a46d60dd4513434e_Out_2_Float;
        Unity_Multiply_float_float(_Perspective_917cfd8f51134dddafbdf7fcdbc16980_Out_0_Float, _Property_2c38ceda851245a9af3cedf5fa670532_Out_0_Float, _Multiply_66d0b19b85644261a46d60dd4513434e_Out_2_Float);
        float _OneMinus_879cfebaf09a4041ba20d931184cf4f9_Out_1_Float;
        Unity_OneMinus_float(_Multiply_66d0b19b85644261a46d60dd4513434e_Out_2_Float, _OneMinus_879cfebaf09a4041ba20d931184cf4f9_Out_1_Float);
        UnityTexture2D _Property_bc5252e2c5b74edbaa8ba7f5a162d720_Out_0_Texture2D = _IntersectionFoamTexture;
        float _Property_1f3b9e9e32a54263b64e2dfaa1da2891_Out_0_Float = _IntersectionFoamDirection;
        float _Property_940a015d94754f2cb866cb17053cdcc9_Out_0_Float = _IntersectionFoamSpeed;
        float2 _Property_32a7ac96a5dc47bd88f9784f3e38a5ab_Out_0_Vector2 = _UVs;
        float _Property_6fc7fe69b6fc4920b11d0aee35f0c39a_Out_0_Float = _IntersectionFoamTiling;
        Bindings_PanningUVs_ef286e626cee63841803a024235a644f_float _PanningUVs_b5fb72b5c5f74791bff52b9acb830aa0;
        _PanningUVs_b5fb72b5c5f74791bff52b9acb830aa0.TimeParameters = IN.TimeParameters;
        half2 _PanningUVs_b5fb72b5c5f74791bff52b9acb830aa0_UVOut_1_Vector2;
        SG_PanningUVs_ef286e626cee63841803a024235a644f_float(_Property_1f3b9e9e32a54263b64e2dfaa1da2891_Out_0_Float, _Property_940a015d94754f2cb866cb17053cdcc9_Out_0_Float, _Property_32a7ac96a5dc47bd88f9784f3e38a5ab_Out_0_Vector2, _Property_6fc7fe69b6fc4920b11d0aee35f0c39a_Out_0_Float, half2 (0, 0), _PanningUVs_b5fb72b5c5f74791bff52b9acb830aa0, _PanningUVs_b5fb72b5c5f74791bff52b9acb830aa0_UVOut_1_Vector2);
        float4 _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_bc5252e2c5b74edbaa8ba7f5a162d720_Out_0_Texture2D.tex, _Property_bc5252e2c5b74edbaa8ba7f5a162d720_Out_0_Texture2D.samplerstate, _Property_bc5252e2c5b74edbaa8ba7f5a162d720_Out_0_Texture2D.GetTransformedUV(_PanningUVs_b5fb72b5c5f74791bff52b9acb830aa0_UVOut_1_Vector2) );
        _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_RGBA_0_Vector4.rgb = UnpackNormalRGB(_SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_RGBA_0_Vector4);
        float _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_R_4_Float = _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_RGBA_0_Vector4.r;
        float _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_G_5_Float = _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_RGBA_0_Vector4.g;
        float _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_B_6_Float = _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_RGBA_0_Vector4.b;
        float _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_A_7_Float = _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_RGBA_0_Vector4.a;
        float _Step_640da6258eee484699374502b11f5859_Out_2_Float;
        Unity_Step_float(_OneMinus_879cfebaf09a4041ba20d931184cf4f9_Out_1_Float, _SampleTexture2D_f843b9b4b77d4162b11ff99a3daf8f8f_R_4_Float, _Step_640da6258eee484699374502b11f5859_Out_2_Float);
        float4 _Property_1b2a727d2d9a4b35a4b84b80d8cc241b_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_IntersectionFoamColor) : _IntersectionFoamColor;
        float _Split_0fdcdfb033da473fa176a759fabdf261_R_1_Float = _Property_1b2a727d2d9a4b35a4b84b80d8cc241b_Out_0_Vector4[0];
        float _Split_0fdcdfb033da473fa176a759fabdf261_G_2_Float = _Property_1b2a727d2d9a4b35a4b84b80d8cc241b_Out_0_Vector4[1];
        float _Split_0fdcdfb033da473fa176a759fabdf261_B_3_Float = _Property_1b2a727d2d9a4b35a4b84b80d8cc241b_Out_0_Vector4[2];
        float _Split_0fdcdfb033da473fa176a759fabdf261_A_4_Float = _Property_1b2a727d2d9a4b35a4b84b80d8cc241b_Out_0_Vector4[3];
        float _Multiply_4b309a3865b54e3490078c2758f00293_Out_2_Float;
        Unity_Multiply_float_float(_Step_640da6258eee484699374502b11f5859_Out_2_Float, _Split_0fdcdfb033da473fa176a759fabdf261_A_4_Float, _Multiply_4b309a3865b54e3490078c2758f00293_Out_2_Float);
        IntersectionFoamResult_1 = (_Multiply_4b309a3865b54e3490078c2758f00293_Out_2_Float.xxxx);
        }
        
        void Unity_NormalBlend_float(float3 A, float3 B, out float3 Out)
        {
            Out = SafeNormalize(float3(A.rg + B.rg, A.b * B.b));
        }
        
        struct Bindings_BlendedNormals_01824997d3b28f944887693b0e1d6405_float
        {
        float3 TimeParameters;
        };
        
        void SG_BlendedNormals_01824997d3b28f944887693b0e1d6405_float(float _NormalScale, float _NormalSpeed, UnityTexture2D _NormalTexture, float2 _UVs, Bindings_BlendedNormals_01824997d3b28f944887693b0e1d6405_float IN, out float3 Out_0)
        {
        UnityTexture2D _Property_b7e00044a0874370a521db8cbe781666_Out_0_Texture2D = _NormalTexture;
        float _Property_82e7e1ba78984b8f86383489bc8d0b1d_Out_0_Float = _NormalSpeed;
        float _Multiply_5e387d16860b440bb01f80e49690829a_Out_2_Float;
        Unity_Multiply_float_float(-0.5, _Property_82e7e1ba78984b8f86383489bc8d0b1d_Out_0_Float, _Multiply_5e387d16860b440bb01f80e49690829a_Out_2_Float);
        float2 _Property_d868665dbff94b6590abe31c7c0c6327_Out_0_Vector2 = _UVs;
        float _Property_5fd79ab306504e55aa9fa9ce57ddbb1b_Out_0_Float = _NormalScale;
        float _Multiply_63cc307a7007489dbadda4faf1dfa495_Out_2_Float;
        Unity_Multiply_float_float(0.5, _Property_5fd79ab306504e55aa9fa9ce57ddbb1b_Out_0_Float, _Multiply_63cc307a7007489dbadda4faf1dfa495_Out_2_Float);
        float _Reciprocal_4d8c53063c7c4694a2bc9fd174344747_Out_1_Float;
        Unity_Reciprocal_Fast_float(_Multiply_63cc307a7007489dbadda4faf1dfa495_Out_2_Float, _Reciprocal_4d8c53063c7c4694a2bc9fd174344747_Out_1_Float);
        Bindings_PanningUVs_ef286e626cee63841803a024235a644f_float _PanningUVs_c3d15fa8145b400cbfe094f7a9014648;
        _PanningUVs_c3d15fa8145b400cbfe094f7a9014648.TimeParameters = IN.TimeParameters;
        half2 _PanningUVs_c3d15fa8145b400cbfe094f7a9014648_UVOut_1_Vector2;
        SG_PanningUVs_ef286e626cee63841803a024235a644f_float(half(1), _Multiply_5e387d16860b440bb01f80e49690829a_Out_2_Float, _Property_d868665dbff94b6590abe31c7c0c6327_Out_0_Vector2, _Reciprocal_4d8c53063c7c4694a2bc9fd174344747_Out_1_Float, half2 (0, 0), _PanningUVs_c3d15fa8145b400cbfe094f7a9014648, _PanningUVs_c3d15fa8145b400cbfe094f7a9014648_UVOut_1_Vector2);
        float4 _SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_b7e00044a0874370a521db8cbe781666_Out_0_Texture2D.tex, _Property_b7e00044a0874370a521db8cbe781666_Out_0_Texture2D.samplerstate, _Property_b7e00044a0874370a521db8cbe781666_Out_0_Texture2D.GetTransformedUV(_PanningUVs_c3d15fa8145b400cbfe094f7a9014648_UVOut_1_Vector2) );
        _SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4.rgb = UnpackNormal(_SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4);
        float _SampleTexture2D_64a0d66e81d94af8907615615e56c648_R_4_Float = _SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4.r;
        float _SampleTexture2D_64a0d66e81d94af8907615615e56c648_G_5_Float = _SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4.g;
        float _SampleTexture2D_64a0d66e81d94af8907615615e56c648_B_6_Float = _SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4.b;
        float _SampleTexture2D_64a0d66e81d94af8907615615e56c648_A_7_Float = _SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4.a;
        float _Property_b3d54c3bdd024196aa765640b786350b_Out_0_Float = _NormalSpeed;
        float2 _Property_1d4d5f262eee4960a52cb63f61c3acbc_Out_0_Vector2 = _UVs;
        float _Property_425ab0f9b8b940939b9ae0d2b672a08d_Out_0_Float = _NormalScale;
        float _Reciprocal_8276cd463ae64d68a1e9c4bdff77f869_Out_1_Float;
        Unity_Reciprocal_Fast_float(_Property_425ab0f9b8b940939b9ae0d2b672a08d_Out_0_Float, _Reciprocal_8276cd463ae64d68a1e9c4bdff77f869_Out_1_Float);
        Bindings_PanningUVs_ef286e626cee63841803a024235a644f_float _PanningUVs_fb9f884294f840c4aa4334e5528e58be;
        _PanningUVs_fb9f884294f840c4aa4334e5528e58be.TimeParameters = IN.TimeParameters;
        half2 _PanningUVs_fb9f884294f840c4aa4334e5528e58be_UVOut_1_Vector2;
        SG_PanningUVs_ef286e626cee63841803a024235a644f_float(half(1), _Property_b3d54c3bdd024196aa765640b786350b_Out_0_Float, _Property_1d4d5f262eee4960a52cb63f61c3acbc_Out_0_Vector2, _Reciprocal_8276cd463ae64d68a1e9c4bdff77f869_Out_1_Float, half2 (0, 0), _PanningUVs_fb9f884294f840c4aa4334e5528e58be, _PanningUVs_fb9f884294f840c4aa4334e5528e58be_UVOut_1_Vector2);
        float4 _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_b7e00044a0874370a521db8cbe781666_Out_0_Texture2D.tex, _Property_b7e00044a0874370a521db8cbe781666_Out_0_Texture2D.samplerstate, _Property_b7e00044a0874370a521db8cbe781666_Out_0_Texture2D.GetTransformedUV(_PanningUVs_fb9f884294f840c4aa4334e5528e58be_UVOut_1_Vector2) );
        _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4.rgb = UnpackNormal(_SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4);
        float _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_R_4_Float = _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4.r;
        float _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_G_5_Float = _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4.g;
        float _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_B_6_Float = _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4.b;
        float _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_A_7_Float = _SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4.a;
        float3 _NormalBlend_355e560adba24e7a8abe7a7673ad28d2_Out_2_Vector3;
        Unity_NormalBlend_float((_SampleTexture2D_64a0d66e81d94af8907615615e56c648_RGBA_0_Vector4.xyz), (_SampleTexture2D_44bd9d57260746ac89b6d59ac7a5317d_RGBA_0_Vector4.xyz), _NormalBlend_355e560adba24e7a8abe7a7673ad28d2_Out_2_Vector3);
        Out_0 = _NormalBlend_355e560adba24e7a8abe7a7673ad28d2_Out_2_Vector3;
        }
        
        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }
        
        struct Bindings_Specular_ca077344d37a82d46ba9cd29d65fb670_float
        {
        float3 WorldSpaceNormal;
        float3 WorldSpaceTangent;
        float3 WorldSpaceBiTangent;
        float3 WorldSpacePosition;
        float3 TimeParameters;
        };
        
        void SG_Specular_ca077344d37a82d46ba9cd29d65fb670_float(float _NormalScale, float _NormalSpeed, UnityTexture2D _NormalTexture, float _NormalStrength, float _LightingSmoothness, float _LightingHardness, float4 _SpecularColor, float2 _UVs, Bindings_Specular_ca077344d37a82d46ba9cd29d65fb670_float IN, out float3 Specular_1)
        {
        float _Property_12642a3bcf964c959ae2bbac7e5400ac_Out_0_Float = _NormalScale;
        float _Property_9a81b54411dc4443a34f009659e2b4d9_Out_0_Float = _NormalSpeed;
        UnityTexture2D _Property_229609cb9075441ca1c4520de10d655f_Out_0_Texture2D = _NormalTexture;
        float2 _Property_5b27f589f80742bcbfe965375ab83205_Out_0_Vector2 = _UVs;
        Bindings_BlendedNormals_01824997d3b28f944887693b0e1d6405_float _BlendedNormals_9340ea003cbd468e862462200f580f96;
        _BlendedNormals_9340ea003cbd468e862462200f580f96.TimeParameters = IN.TimeParameters;
        half3 _BlendedNormals_9340ea003cbd468e862462200f580f96_Out_0_Vector3;
        SG_BlendedNormals_01824997d3b28f944887693b0e1d6405_float(_Property_12642a3bcf964c959ae2bbac7e5400ac_Out_0_Float, _Property_9a81b54411dc4443a34f009659e2b4d9_Out_0_Float, _Property_229609cb9075441ca1c4520de10d655f_Out_0_Texture2D, _Property_5b27f589f80742bcbfe965375ab83205_Out_0_Vector2, _BlendedNormals_9340ea003cbd468e862462200f580f96, _BlendedNormals_9340ea003cbd468e862462200f580f96_Out_0_Vector3);
        float _Property_61fd9c0ec35a4d238019a6212f701e6f_Out_0_Float = _NormalStrength;
        float3 _NormalStrength_5fc187d2cc214a3aa352e512a627ae29_Out_2_Vector3;
        Unity_NormalStrength_float(_BlendedNormals_9340ea003cbd468e862462200f580f96_Out_0_Vector3, _Property_61fd9c0ec35a4d238019a6212f701e6f_Out_0_Float, _NormalStrength_5fc187d2cc214a3aa352e512a627ae29_Out_2_Vector3);
        float3 _Transform_f0cb3b0524334f779bf2800dae8c021e_Out_1_Vector3;
        {
        float3x3 tangentTransform = float3x3(IN.WorldSpaceTangent, IN.WorldSpaceBiTangent, IN.WorldSpaceNormal);
        _Transform_f0cb3b0524334f779bf2800dae8c021e_Out_1_Vector3 = TransformTangentToWorld(_NormalStrength_5fc187d2cc214a3aa352e512a627ae29_Out_2_Vector3.xyz, tangentTransform, true);
        }
        float3 _ViewVector_1252c7c64b0e4e6f855eab336f1a48a8_Out_0_Vector3;
        Unity_ViewVectorWorld_float(_ViewVector_1252c7c64b0e4e6f855eab336f1a48a8_Out_0_Vector3, IN.WorldSpacePosition);
        float3 _Vector3_e77fc3854050436d8a46ff8b9f3b1ff2_Out_0_Vector3 = float3(float(0), float(0), float(1));
        #if defined(_PERSPECTIVE)
        float3 _Perspective_322896f72bb640e09107587d0117cc53_Out_0_Vector3 = _ViewVector_1252c7c64b0e4e6f855eab336f1a48a8_Out_0_Vector3;
        #else
        float3 _Perspective_322896f72bb640e09107587d0117cc53_Out_0_Vector3 = _Vector3_e77fc3854050436d8a46ff8b9f3b1ff2_Out_0_Vector3;
        #endif
        float _MainLightingCustomFunction_08a62467374f4ec580e996e505530fff_Specular_4_Float;
        MainLighting_float(_Transform_f0cb3b0524334f779bf2800dae8c021e_Out_1_Vector3, IN.WorldSpacePosition, _Perspective_322896f72bb640e09107587d0117cc53_Out_0_Vector3, float(0), _MainLightingCustomFunction_08a62467374f4ec580e996e505530fff_Specular_4_Float);
        float _Step_7c27e8f5e968400b962535f046fd4f84_Out_2_Float;
        Unity_Step_float(float(0.5), _MainLightingCustomFunction_08a62467374f4ec580e996e505530fff_Specular_4_Float, _Step_7c27e8f5e968400b962535f046fd4f84_Out_2_Float);
        float _Property_070589d4b1b64f00877b042f5fa4786c_Out_0_Float = _LightingHardness;
        float _Lerp_7693c4d68e1a4276bcace636300a7f30_Out_3_Float;
        Unity_Lerp_float(_MainLightingCustomFunction_08a62467374f4ec580e996e505530fff_Specular_4_Float, _Step_7c27e8f5e968400b962535f046fd4f84_Out_2_Float, _Property_070589d4b1b64f00877b042f5fa4786c_Out_0_Float, _Lerp_7693c4d68e1a4276bcace636300a7f30_Out_3_Float);
        float4 _Property_2540d1f934ed4d15b7d62700a4f1adf2_Out_0_Vector4 = _SpecularColor;
        float4 _Multiply_e0f28cba39d84814946e9e3683c24585_Out_2_Vector4;
        Unity_Multiply_float4_float4((_Lerp_7693c4d68e1a4276bcace636300a7f30_Out_3_Float.xxxx), _Property_2540d1f934ed4d15b7d62700a4f1adf2_Out_0_Vector4, _Multiply_e0f28cba39d84814946e9e3683c24585_Out_2_Vector4);
        float _Split_cca0da3ff1c94d7b95381021f7e5824e_R_1_Float = _Property_2540d1f934ed4d15b7d62700a4f1adf2_Out_0_Vector4[0];
        float _Split_cca0da3ff1c94d7b95381021f7e5824e_G_2_Float = _Property_2540d1f934ed4d15b7d62700a4f1adf2_Out_0_Vector4[1];
        float _Split_cca0da3ff1c94d7b95381021f7e5824e_B_3_Float = _Property_2540d1f934ed4d15b7d62700a4f1adf2_Out_0_Vector4[2];
        float _Split_cca0da3ff1c94d7b95381021f7e5824e_A_4_Float = _Property_2540d1f934ed4d15b7d62700a4f1adf2_Out_0_Vector4[3];
        float4 _Multiply_e3b0a1558f044c5f9884617dd07c3456_Out_2_Vector4;
        Unity_Multiply_float4_float4(_Multiply_e0f28cba39d84814946e9e3683c24585_Out_2_Vector4, (_Split_cca0da3ff1c94d7b95381021f7e5824e_A_4_Float.xxxx), _Multiply_e3b0a1558f044c5f9884617dd07c3456_Out_2_Vector4);
        float _Property_d4211a44c8d34f3fb25fe762b3d7b5a5_Out_0_Float = _LightingSmoothness;
        float _Property_47eea6903fbd41518eb78ca7ba2ea49f_Out_0_Float = _LightingHardness;
        float3 _AdditionalLightingCustomFunction_f8bf1ccdd9a34ff1ba47f18b6139f837_Specular_5_Vector3;
        AdditionalLighting_float(_Transform_f0cb3b0524334f779bf2800dae8c021e_Out_1_Vector3, IN.WorldSpacePosition, _Perspective_322896f72bb640e09107587d0117cc53_Out_0_Vector3, _Property_d4211a44c8d34f3fb25fe762b3d7b5a5_Out_0_Float, _Property_47eea6903fbd41518eb78ca7ba2ea49f_Out_0_Float, _AdditionalLightingCustomFunction_f8bf1ccdd9a34ff1ba47f18b6139f837_Specular_5_Vector3);
        float3 _Add_bbd9570472ea43018aeac31b2cec79ef_Out_2_Vector3;
        Unity_Add_float3((_Multiply_e3b0a1558f044c5f9884617dd07c3456_Out_2_Vector4.xyz), _AdditionalLightingCustomFunction_f8bf1ccdd9a34ff1ba47f18b6139f837_Specular_5_Vector3, _Add_bbd9570472ea43018aeac31b2cec79ef_Out_2_Vector3);
        Specular_1 = _Add_bbd9570472ea43018aeac31b2cec79ef_Out_2_Vector3;
        }
        
        void Unity_Fog_float(out float4 Color, out float Density, float3 Position)
        {
            SHADERGRAPH_FOG(Position, Color, Density);
        }
        
        struct Bindings_Fog_4d8d465b4ba9a34499f11570d2bfbcf0_float
        {
        float3 ObjectSpacePosition;
        };
        
        void SG_Fog_4d8d465b4ba9a34499f11570d2bfbcf0_float(Bindings_Fog_4d8d465b4ba9a34499f11570d2bfbcf0_float IN, out float FogAmount_1, out float4 FogColor_2)
        {
        float4 _Fog_58a31c30c807423b9d9065753a0cbe96_Color_0_Vector4;
        float _Fog_58a31c30c807423b9d9065753a0cbe96_Density_1_Float;
        Unity_Fog_float(_Fog_58a31c30c807423b9d9065753a0cbe96_Color_0_Vector4, _Fog_58a31c30c807423b9d9065753a0cbe96_Density_1_Float, IN.ObjectSpacePosition);
        float _Saturate_57b873032001499d95a8956b1c4c9ff9_Out_1_Float;
        Unity_Saturate_float(_Fog_58a31c30c807423b9d9065753a0cbe96_Density_1_Float, _Saturate_57b873032001499d95a8956b1c4c9ff9_Out_1_Float);
        float4 _Fog_d79620e98a204435bdf933c422a158e0_Color_0_Vector4;
        float _Fog_d79620e98a204435bdf933c422a158e0_Density_1_Float;
        Unity_Fog_float(_Fog_d79620e98a204435bdf933c422a158e0_Color_0_Vector4, _Fog_d79620e98a204435bdf933c422a158e0_Density_1_Float, IN.ObjectSpacePosition);
        FogAmount_1 = _Saturate_57b873032001499d95a8956b1c4c9ff9_Out_1_Float;
        FogColor_2 = _Fog_d79620e98a204435bdf933c422a158e0_Color_0_Vector4;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Property_f4b92572a6b849978ea4164861fc96e7_Out_0_Float = _WaveSteepness;
            float _Property_60706828d6004c70825f62373cea2de7_Out_0_Float = _WaveLength;
            float _Property_2666eaa75aae48049c953771bc099709_Out_0_Float = _WaveSpeed;
            float4 _Property_cc5d2426eabf42799308261053db7fd7_Out_0_Vector4 = _WaveDirections;
            Bindings_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11;
            _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11.ObjectSpacePosition = IN.ObjectSpacePosition;
            float3 _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3;
            SG_WavesCalculation_f15de5aaad0a8bf41b7c4272a40435b7_float(_Property_f4b92572a6b849978ea4164861fc96e7_Out_0_Float, _Property_60706828d6004c70825f62373cea2de7_Out_0_Float, _Property_2666eaa75aae48049c953771bc099709_Out_0_Float, _Property_cc5d2426eabf42799308261053db7fd7_Out_0_Vector4, _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11, _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3);
            description.Position = _WavesCalculation_8cc4ddbd21a94c5ca67f67baec8a0d11_PositionWithWaveOffset_1_Vector3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_1d602cb04f2a487bbe2a10a82c216988_Out_0_Float = _DepthFadeDistance;
            float _Property_7d6fc5bac56243d4b5ae0dcde436653d_Out_0_Float = _Steps;
            float _Property_e037029a81ca4277aa6156bc59761352_Out_0_Float = _RefractionSpeed;
            float _Property_99fa99535b074d1f91770ec4b97b3857_Out_0_Float = _RefractionScale;
            float _Property_ad37e2c0d61640a7bb0c4d98de3805d4_Out_0_Float = _GradientRefractionScale;
            float _Property_4a60bded2f1f4dd6945f5cf3f4fc789d_Out_0_Float = _RefractionStrength;
            float4 _Property_a9206916a2e3432bb1afa6b1d3655996_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_ShallowColor) : _ShallowColor;
            float4 _Property_e9f7f16ac69646e5908d8454ec7ef56f_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_DeepColor) : _DeepColor;
            Bindings_GetDepth_370b72acb6baa764089512ed583870c4_float _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e;
            _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e.WorldSpacePosition = IN.WorldSpacePosition;
            _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e.ScreenPosition = IN.ScreenPosition;
            _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e.NDCPosition = IN.NDCPosition;
            _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e.uv0 = IN.uv0;
            _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e.TimeParameters = IN.TimeParameters;
            float4 _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e_RefractedUVs_1_Vector4;
            float4 _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e_Color_2_Vector4;
            SG_GetDepth_370b72acb6baa764089512ed583870c4_float(_Property_1d602cb04f2a487bbe2a10a82c216988_Out_0_Float, _Property_7d6fc5bac56243d4b5ae0dcde436653d_Out_0_Float, _Property_e037029a81ca4277aa6156bc59761352_Out_0_Float, _Property_99fa99535b074d1f91770ec4b97b3857_Out_0_Float, _Property_ad37e2c0d61640a7bb0c4d98de3805d4_Out_0_Float, _Property_4a60bded2f1f4dd6945f5cf3f4fc789d_Out_0_Float, _Property_a9206916a2e3432bb1afa6b1d3655996_Out_0_Vector4, _Property_e9f7f16ac69646e5908d8454ec7ef56f_Out_0_Vector4, _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e, _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e_RefractedUVs_1_Vector4, _GetDepth_52ad2e61abb04277a0dfe91bb3664d1e_Color_2_Vector4);
            float4 _Property_893a4c25d7bd4390a59efcfddbd60e36_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_HorizonColor) : _HorizonColor;
            float _Property_356b877dd8e2453c8037fed5c4738b25_Out_0_Float = _HorizonDistance;
            Bindings_FresnelCalculation_f346593723353f4488f8a099aef520eb_float _FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5;
            _FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5.WorldSpaceNormal = IN.WorldSpaceNormal;
            _FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5.ViewSpaceNormal = IN.ViewSpaceNormal;
            _FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5.WorldSpaceViewDirection = IN.WorldSpaceViewDirection;
            float _FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5_FresnelResult_1_Float;
            SG_FresnelCalculation_f346593723353f4488f8a099aef520eb_float(_Property_356b877dd8e2453c8037fed5c4738b25_Out_0_Float, _FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5, _FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5_FresnelResult_1_Float);
            float4 _Lerp_3aa30c6357614a61b17161f8c0633600_Out_3_Vector4;
            Unity_Lerp_float4(_GetDepth_52ad2e61abb04277a0dfe91bb3664d1e_Color_2_Vector4, _Property_893a4c25d7bd4390a59efcfddbd60e36_Out_0_Vector4, (_FresnelCalculation_fe88e50882f44f65977e2e731f7b1be5_FresnelResult_1_Float.xxxx), _Lerp_3aa30c6357614a61b17161f8c0633600_Out_3_Vector4);
            Bindings_BlendObjectColor_a02636e967a650d4db669ee613538c46_float _BlendObjectColor_3ffec75a375b49dfa55b88d64e560cb9;
            float3 _BlendObjectColor_3ffec75a375b49dfa55b88d64e560cb9_ObjectBlend_1_Vector3;
            SG_BlendObjectColor_a02636e967a650d4db669ee613538c46_float(_GetDepth_52ad2e61abb04277a0dfe91bb3664d1e_RefractedUVs_1_Vector4, _Lerp_3aa30c6357614a61b17161f8c0633600_Out_3_Vector4, _BlendObjectColor_3ffec75a375b49dfa55b88d64e560cb9, _BlendObjectColor_3ffec75a375b49dfa55b88d64e560cb9_ObjectBlend_1_Vector3);
            float3 _Add_1dfe8dd4fc544cb1a6be38289da171bb_Out_2_Vector3;
            Unity_Add_float3(_BlendObjectColor_3ffec75a375b49dfa55b88d64e560cb9_ObjectBlend_1_Vector3, (_Lerp_3aa30c6357614a61b17161f8c0633600_Out_3_Vector4.xyz), _Add_1dfe8dd4fc544cb1a6be38289da171bb_Out_2_Vector3);
            float4 _Property_f31e3d68a9514af587c58953c6a37312_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_CaveColor) : _CaveColor;
            Bindings_CalculateUVs_60d47073ac03df54f9a15991680de154_float _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba;
            _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba.WorldSpacePosition = IN.WorldSpacePosition;
            _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba.uv0 = IN.uv0;
            float2 _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba_OutUVs_1_Vector2;
            SG_CalculateUVs_60d47073ac03df54f9a15991680de154_float(_CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba, _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba_OutUVs_1_Vector2);
            float2 _Property_b813bb0c2a834796bb918459b2e9f6f2_Out_0_Vector2 = _CaveScale;
            float2 _Property_f4eb27d4ee8b483c88526d64b0adcb71_Out_0_Vector2 = _CaveOffset;
            float _Property_d0f8505aba284e148b22af0ebde152ee_Out_0_Float = _CaveDistortion;
            UnityTexture2D _Property_e88c987f688347cfb013302786c36f42_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_CaveTexture);
            float4 _Property_a0ded2607a8749148ff7a887ebed0d9c_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_CaveColor) : _CaveColor;
            Bindings_Caves_3e5c142028ba07d48bba33c5843ba17e_float _Caves_bce6e1cd6e0a4b1c845eda4f734a4239;
            float4 _Caves_bce6e1cd6e0a4b1c845eda4f734a4239_CaveResult_1_Vector4;
            SG_Caves_3e5c142028ba07d48bba33c5843ba17e_float(_CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba_OutUVs_1_Vector2, _Property_b813bb0c2a834796bb918459b2e9f6f2_Out_0_Vector2, _Property_f4eb27d4ee8b483c88526d64b0adcb71_Out_0_Vector2, _Property_d0f8505aba284e148b22af0ebde152ee_Out_0_Float, _Property_e88c987f688347cfb013302786c36f42_Out_0_Texture2D, _Property_a0ded2607a8749148ff7a887ebed0d9c_Out_0_Vector4, float(50), _Caves_bce6e1cd6e0a4b1c845eda4f734a4239, _Caves_bce6e1cd6e0a4b1c845eda4f734a4239_CaveResult_1_Vector4);
            float3 _Lerp_9c2e40976f7e47008367af1bafb57231_Out_3_Vector3;
            Unity_Lerp_float3(_Add_1dfe8dd4fc544cb1a6be38289da171bb_Out_2_Vector3, (_Property_f31e3d68a9514af587c58953c6a37312_Out_0_Vector4.xyz), (_Caves_bce6e1cd6e0a4b1c845eda4f734a4239_CaveResult_1_Vector4.xyz), _Lerp_9c2e40976f7e47008367af1bafb57231_Out_3_Vector3);
            UnityTexture2D _Property_0eefa2a85d8a410bad14c729edc255c8_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_ReflectionMap);
            float _Property_b274ddde85464fe7990c99a3b9ff8986_Out_0_Float = _ReflectionDistortion;
            float _Property_379d70ad8d6b484e9cf65bd7ceb23f58_Out_0_Float = _ReflectionBlend;
            Bindings_Reflections_2a83053698b0a9d46a141dd7d36ac2ac_float _Reflections_c68a6bfeff564b248d4ce269caad599e;
            _Reflections_c68a6bfeff564b248d4ce269caad599e.NDCPosition = IN.NDCPosition;
            float4 _Reflections_c68a6bfeff564b248d4ce269caad599e_ReflexColor_1_Vector4;
            float _Reflections_c68a6bfeff564b248d4ce269caad599e_ReflexResult_2_Float;
            SG_Reflections_2a83053698b0a9d46a141dd7d36ac2ac_float(_Property_0eefa2a85d8a410bad14c729edc255c8_Out_0_Texture2D, _Property_b274ddde85464fe7990c99a3b9ff8986_Out_0_Float, _Property_379d70ad8d6b484e9cf65bd7ceb23f58_Out_0_Float, _Reflections_c68a6bfeff564b248d4ce269caad599e, _Reflections_c68a6bfeff564b248d4ce269caad599e_ReflexColor_1_Vector4, _Reflections_c68a6bfeff564b248d4ce269caad599e_ReflexResult_2_Float);
            float3 _Lerp_eae8f37e319e4e87a5ef928ede712a31_Out_3_Vector3;
            Unity_Lerp_float3(_Lerp_9c2e40976f7e47008367af1bafb57231_Out_3_Vector3, (_Reflections_c68a6bfeff564b248d4ce269caad599e_ReflexColor_1_Vector4.xyz), (_Reflections_c68a6bfeff564b248d4ce269caad599e_ReflexResult_2_Float.xxx), _Lerp_eae8f37e319e4e87a5ef928ede712a31_Out_3_Vector3);
            #if defined(_REFLECTIONS)
            float3 _Reflections_a210dad7ca474e3fb1719c7e933d3a27_Out_0_Vector3 = _Lerp_eae8f37e319e4e87a5ef928ede712a31_Out_3_Vector3;
            #else
            float3 _Reflections_a210dad7ca474e3fb1719c7e933d3a27_Out_0_Vector3 = _Lerp_9c2e40976f7e47008367af1bafb57231_Out_3_Vector3;
            #endif
            float4 _Property_81b51a63dd4c4bfb834b310d7e276279_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_SecondaryFoamColor) : _SecondaryFoamColor;
            float _Property_15d7e92bf5d541d9b7e1bbfadcc73a7f_Out_0_Float = _SurfaceFoamDirection;
            float _Property_711cbf38e2bb4a079d432afd2295ee81_Out_0_Float = _SurfaceFoamSpeed;
            float _Property_32a0f052e9444bdcb80f2f954608f8ef_Out_0_Float = _SurfaceFoamTiling;
            float2 _Property_17d01aaf1dda4850abc180a4ee955b3c_Out_0_Vector2 = _FoamUVsOffset;
            float _Property_ffebd665429e4cceaad03a9da3adafee_Out_0_Float = _SurfaceFoamDistorsion;
            UnityTexture2D _Property_d64c4dd59fff4c5b97b65f5f5c0c7f10_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_SecondaryFoamTex);
            float4 _Property_3451f3182b8241279c3bbb80d57d882c_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_SecondaryFoamColor) : _SecondaryFoamColor;
            Bindings_SecondaryFoam_a707540033b78f44bbef5139f65097d0_float _SecondaryFoam_5f90189370e9499fade77e767815c37a;
            _SecondaryFoam_5f90189370e9499fade77e767815c37a.TimeParameters = IN.TimeParameters;
            float4 _SecondaryFoam_5f90189370e9499fade77e767815c37a_SecondaryFoamResult_1_Vector4;
            SG_SecondaryFoam_a707540033b78f44bbef5139f65097d0_float(_Property_15d7e92bf5d541d9b7e1bbfadcc73a7f_Out_0_Float, _Property_711cbf38e2bb4a079d432afd2295ee81_Out_0_Float, _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba_OutUVs_1_Vector2, _Property_32a0f052e9444bdcb80f2f954608f8ef_Out_0_Float, _Property_17d01aaf1dda4850abc180a4ee955b3c_Out_0_Vector2, _Property_ffebd665429e4cceaad03a9da3adafee_Out_0_Float, _Property_d64c4dd59fff4c5b97b65f5f5c0c7f10_Out_0_Texture2D, _Property_3451f3182b8241279c3bbb80d57d882c_Out_0_Vector4, _SecondaryFoam_5f90189370e9499fade77e767815c37a, _SecondaryFoam_5f90189370e9499fade77e767815c37a_SecondaryFoamResult_1_Vector4);
            float3 _Lerp_f104552f399b462f82c9f7d9d4b08dc2_Out_3_Vector3;
            Unity_Lerp_float3(_Reflections_a210dad7ca474e3fb1719c7e933d3a27_Out_0_Vector3, (_Property_81b51a63dd4c4bfb834b310d7e276279_Out_0_Vector4.xyz), (_SecondaryFoam_5f90189370e9499fade77e767815c37a_SecondaryFoamResult_1_Vector4.xyz), _Lerp_f104552f399b462f82c9f7d9d4b08dc2_Out_3_Vector3);
            float4 _Property_da9173513832427bb5ab4ab2e5528b2b_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_SurfaceFoamColor) : _SurfaceFoamColor;
            float _Property_636aeb6fd5ae4972be2eb07aff88bb50_Out_0_Float = _SurfaceFoamDirection;
            float _Property_f4bf6eaa912341d1a75e44b6b46af453_Out_0_Float = _SurfaceFoamSpeed;
            float _Property_d25c273d7c7642d7a7dc57913ed42e97_Out_0_Float = _SurfaceFoamTiling;
            float _Property_1a3f66b4873543cda6a945906b57d7d8_Out_0_Float = _SurfaceFoamDistorsion;
            UnityTexture2D _Property_165b792376df4c00bf5ceebafff9099b_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_SurfaceFoamTexture);
            float4 _Property_689d2414eafc4fe4a7777244e0226de5_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_SurfaceFoamColor) : _SurfaceFoamColor;
            Bindings_PrimaryFoam_f41bb18b12540894cadcf85fcc438801_float _PrimaryFoam_4a7109ebe0ce44d88150306b8a89a15b;
            _PrimaryFoam_4a7109ebe0ce44d88150306b8a89a15b.TimeParameters = IN.TimeParameters;
            float4 _PrimaryFoam_4a7109ebe0ce44d88150306b8a89a15b_FoamResult_1_Vector4;
            SG_PrimaryFoam_f41bb18b12540894cadcf85fcc438801_float(_Property_636aeb6fd5ae4972be2eb07aff88bb50_Out_0_Float, _Property_f4bf6eaa912341d1a75e44b6b46af453_Out_0_Float, _Property_d25c273d7c7642d7a7dc57913ed42e97_Out_0_Float, _Property_1a3f66b4873543cda6a945906b57d7d8_Out_0_Float, _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba_OutUVs_1_Vector2, _Property_165b792376df4c00bf5ceebafff9099b_Out_0_Texture2D, _Property_689d2414eafc4fe4a7777244e0226de5_Out_0_Vector4, _PrimaryFoam_4a7109ebe0ce44d88150306b8a89a15b, _PrimaryFoam_4a7109ebe0ce44d88150306b8a89a15b_FoamResult_1_Vector4);
            float3 _Lerp_283283898a3f45b783820a658c09ee0e_Out_3_Vector3;
            Unity_Lerp_float3(_Lerp_f104552f399b462f82c9f7d9d4b08dc2_Out_3_Vector3, (_Property_da9173513832427bb5ab4ab2e5528b2b_Out_0_Vector4.xyz), (_PrimaryFoam_4a7109ebe0ce44d88150306b8a89a15b_FoamResult_1_Vector4.xyz), _Lerp_283283898a3f45b783820a658c09ee0e_Out_3_Vector3);
            float4 _Property_a28dab83abd146709ef4d7acf47620c9_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_IntersectionFoamColor) : _IntersectionFoamColor;
            float _Property_6d519a4006ce4a50b7180ef5a9b0d73f_Out_0_Float = _IntersectionFoamDepth;
            float _Property_1c21741f948044b38291462d9d9a80bc_Out_0_Float = _IntersectionFoamDirection;
            float _Property_c68c441254154b6598696f93cabb1974_Out_0_Float = _IntersectionFoamSpeed;
            float _Property_c27dd75d3883456b8cc7ecd9eea07821_Out_0_Float = _IntersectionFoamTiling;
            float _Property_998dfb4e05624d2a841294b3d6056443_Out_0_Float = _IntersectionFoamFade;
            float _Property_6c9ee0fa005147ccbc1e23124eddb903_Out_0_Float = _IntersectionFoamCutoff;
            UnityTexture2D _Property_262ae236fdea4725a5df623a31dc5a40_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_IntersectionFoamTexture);
            float4 _Property_83ad9b00f77a4d68a1a02e037f6da53f_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_IntersectionFoamColor) : _IntersectionFoamColor;
            Bindings_IntersectionFoam_e12de5bce35668a4e8839cbe5732d905_float _IntersectionFoam_111b1a7de3614aefbbfff3b435152564;
            _IntersectionFoam_111b1a7de3614aefbbfff3b435152564.WorldSpacePosition = IN.WorldSpacePosition;
            _IntersectionFoam_111b1a7de3614aefbbfff3b435152564.ScreenPosition = IN.ScreenPosition;
            _IntersectionFoam_111b1a7de3614aefbbfff3b435152564.NDCPosition = IN.NDCPosition;
            _IntersectionFoam_111b1a7de3614aefbbfff3b435152564.TimeParameters = IN.TimeParameters;
            float4 _IntersectionFoam_111b1a7de3614aefbbfff3b435152564_IntersectionFoamResult_1_Vector4;
            SG_IntersectionFoam_e12de5bce35668a4e8839cbe5732d905_float(_Property_6d519a4006ce4a50b7180ef5a9b0d73f_Out_0_Float, _Property_1c21741f948044b38291462d9d9a80bc_Out_0_Float, _Property_c68c441254154b6598696f93cabb1974_Out_0_Float, _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba_OutUVs_1_Vector2, _Property_c27dd75d3883456b8cc7ecd9eea07821_Out_0_Float, _Property_998dfb4e05624d2a841294b3d6056443_Out_0_Float, _Property_6c9ee0fa005147ccbc1e23124eddb903_Out_0_Float, _Property_262ae236fdea4725a5df623a31dc5a40_Out_0_Texture2D, _Property_83ad9b00f77a4d68a1a02e037f6da53f_Out_0_Vector4, _IntersectionFoam_111b1a7de3614aefbbfff3b435152564, _IntersectionFoam_111b1a7de3614aefbbfff3b435152564_IntersectionFoamResult_1_Vector4);
            float3 _Lerp_d89884addcfd45b3a3bb66e8d985f007_Out_3_Vector3;
            Unity_Lerp_float3(_Lerp_283283898a3f45b783820a658c09ee0e_Out_3_Vector3, (_Property_a28dab83abd146709ef4d7acf47620c9_Out_0_Vector4.xyz), (_IntersectionFoam_111b1a7de3614aefbbfff3b435152564_IntersectionFoamResult_1_Vector4.xyz), _Lerp_d89884addcfd45b3a3bb66e8d985f007_Out_3_Vector3);
            float _Property_7d5a7594d9d645ab99b46d3eee118d5c_Out_0_Float = _NormalScale;
            float _Property_4d1d135eaa684e87a7f73553b8717c03_Out_0_Float = _NormalSpeed;
            UnityTexture2D _Property_f51209adc1e24e0f997c51a1a1ab46b5_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_NormalTexture);
            float _Property_999b6f77135943d7adfa043c98a9ea04_Out_0_Float = _NormalStrength;
            float _Property_0299898b57e04518ad9f1b61a9bef7d6_Out_0_Float = _LightingSmoothness;
            float _Property_c5979b329857400396134e141ceffd60_Out_0_Float = _LightingHardness;
            float4 _Property_f09aaf76f58144498339d31ddb56a129_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_SpecularColor) : _SpecularColor;
            Bindings_Specular_ca077344d37a82d46ba9cd29d65fb670_float _Specular_467937ce566d4d7387886e0b29b4205e;
            _Specular_467937ce566d4d7387886e0b29b4205e.WorldSpaceNormal = IN.WorldSpaceNormal;
            _Specular_467937ce566d4d7387886e0b29b4205e.WorldSpaceTangent = IN.WorldSpaceTangent;
            _Specular_467937ce566d4d7387886e0b29b4205e.WorldSpaceBiTangent = IN.WorldSpaceBiTangent;
            _Specular_467937ce566d4d7387886e0b29b4205e.WorldSpacePosition = IN.WorldSpacePosition;
            _Specular_467937ce566d4d7387886e0b29b4205e.TimeParameters = IN.TimeParameters;
            float3 _Specular_467937ce566d4d7387886e0b29b4205e_Specular_1_Vector3;
            SG_Specular_ca077344d37a82d46ba9cd29d65fb670_float(_Property_7d5a7594d9d645ab99b46d3eee118d5c_Out_0_Float, _Property_4d1d135eaa684e87a7f73553b8717c03_Out_0_Float, _Property_f51209adc1e24e0f997c51a1a1ab46b5_Out_0_Texture2D, _Property_999b6f77135943d7adfa043c98a9ea04_Out_0_Float, _Property_0299898b57e04518ad9f1b61a9bef7d6_Out_0_Float, _Property_c5979b329857400396134e141ceffd60_Out_0_Float, _Property_f09aaf76f58144498339d31ddb56a129_Out_0_Vector4, _CalculateUVs_cf58acae5f744c028f3e78bc59d7eaba_OutUVs_1_Vector2, _Specular_467937ce566d4d7387886e0b29b4205e, _Specular_467937ce566d4d7387886e0b29b4205e_Specular_1_Vector3);
            float3 _Add_8502fe58d1c0413598d6f47c5cbc17f6_Out_2_Vector3;
            Unity_Add_float3(_Lerp_d89884addcfd45b3a3bb66e8d985f007_Out_3_Vector3, _Specular_467937ce566d4d7387886e0b29b4205e_Specular_1_Vector3, _Add_8502fe58d1c0413598d6f47c5cbc17f6_Out_2_Vector3);
            Bindings_Fog_4d8d465b4ba9a34499f11570d2bfbcf0_float _Fog_421529a4b940458890852d5853ae0e26;
            _Fog_421529a4b940458890852d5853ae0e26.ObjectSpacePosition = IN.ObjectSpacePosition;
            float _Fog_421529a4b940458890852d5853ae0e26_FogAmount_1_Float;
            float4 _Fog_421529a4b940458890852d5853ae0e26_FogColor_2_Vector4;
            SG_Fog_4d8d465b4ba9a34499f11570d2bfbcf0_float(_Fog_421529a4b940458890852d5853ae0e26, _Fog_421529a4b940458890852d5853ae0e26_FogAmount_1_Float, _Fog_421529a4b940458890852d5853ae0e26_FogColor_2_Vector4);
            float3 _Lerp_2d6e34c6906c4c55966370e2c9504dca_Out_3_Vector3;
            Unity_Lerp_float3(_Add_8502fe58d1c0413598d6f47c5cbc17f6_Out_2_Vector3, (_Fog_421529a4b940458890852d5853ae0e26_FogColor_2_Vector4.xyz), (_Fog_421529a4b940458890852d5853ae0e26_FogAmount_1_Float.xxx), _Lerp_2d6e34c6906c4c55966370e2c9504dca_Out_3_Vector3);
            surface.BaseColor = _Lerp_2d6e34c6906c4c55966370e2c9504dca_Out_3_Vector3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
            // use bitangent on the fly like in hdrp
            // IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
            float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0)* GetOddNegativeScale();
            float3 bitang = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
            output.ViewSpaceNormal = mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_I_V);         // transposed multiplication by inverse matrix to handle normal scale
        
            // to pr               eserve mikktspace compliance we use same scale renormFactor as was used on the normal.
            // This                is explained in section 2.2 in "surface gradient based bump mapping framework"
            output.WorldSpaceTangent = renormFactor * input.tangentWS.xyz;
            output.WorldSpaceBiTangent = renormFactor * bitang;
        
            output.WorldSpaceViewDirection = GetWorldSpaceNormalizeViewDir(input.positionWS);
            output.WorldSpacePosition = input.positionWS;
            output.ObjectSpacePosition = TransformWorldToObject(input.positionWS);
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
            output.uv0 = input.texCoord0;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    CustomEditorForRenderPipeline "UnityEditor.ShaderGraphUnlitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
    FallBack "Hidden/Shader Graph/FallbackError"
}