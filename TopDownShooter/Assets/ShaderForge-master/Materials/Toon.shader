// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:32697,y:32912,varname:node_9361,prsc:2|emission-8618-RGB,custl-7428-OUT;n:type:ShaderForge.SFN_NormalVector,id:969,x:31345,y:32661,prsc:2,pt:False;n:type:ShaderForge.SFN_LightVector,id:2660,x:31345,y:32847,varname:node_2660,prsc:2;n:type:ShaderForge.SFN_Dot,id:8106,x:31517,y:32731,varname:node_8106,prsc:2,dt:0|A-969-OUT,B-2660-OUT;n:type:ShaderForge.SFN_Multiply,id:8463,x:31933,y:32782,varname:node_8463,prsc:2|A-4475-RGB,B-6196-OUT;n:type:ShaderForge.SFN_LightColor,id:4475,x:31702,y:32643,varname:node_4475,prsc:2;n:type:ShaderForge.SFN_LightAttenuation,id:4971,x:31517,y:32899,varname:node_4971,prsc:2;n:type:ShaderForge.SFN_Multiply,id:6196,x:31702,y:32797,varname:node_6196,prsc:2|A-8106-OUT,B-4971-OUT;n:type:ShaderForge.SFN_Posterize,id:2150,x:32127,y:32804,varname:node_2150,prsc:2|IN-8463-OUT,STPS-6405-OUT;n:type:ShaderForge.SFN_Color,id:3390,x:32098,y:32625,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_3390,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.1050644,c2:1,c3:0,c4:1;n:type:ShaderForge.SFN_Blend,id:7428,x:32315,y:32725,varname:node_7428,prsc:2,blmd:10,clmp:True|SRC-1784-OUT,DST-2150-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6405,x:31920,y:32951,ptovrint:False,ptlb:Cell Steps,ptin:_CellSteps,varname:node_6405,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:10;n:type:ShaderForge.SFN_AmbientLight,id:8618,x:32443,y:33030,varname:node_8618,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:9858,x:32140,y:32392,ptovrint:False,ptlb:Texture,ptin:_Texture,varname:node_9858,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1784,x:32325,y:32496,varname:node_1784,prsc:2|A-9858-RGB,B-3390-RGB;proporder:3390-6405-9858;pass:END;sub:END;*/

Shader "Shader Forge/Toon" {
    Properties {
        _Color ("Color", Color) = (0.1050644,1,0,1)
        _CellSteps ("Cell Steps", Float ) = 10
        _Texture ("Texture", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform float _CellSteps;
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                UNITY_LIGHT_ATTENUATION(attenuation,i, i.posWorld.xyz);
////// Emissive:
                float3 emissive = UNITY_LIGHTMODEL_AMBIENT.rgb;
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(i.uv0, _Texture));
                float3 finalColor = emissive + saturate(( floor((_LightColor0.rgb*(dot(i.normalDir,lightDirection)*attenuation)) * _CellSteps) / (_CellSteps - 1) > 0.5 ? (1.0-(1.0-2.0*(floor((_LightColor0.rgb*(dot(i.normalDir,lightDirection)*attenuation)) * _CellSteps) / (_CellSteps - 1)-0.5))*(1.0-(_Texture_var.rgb*_Color.rgb))) : (2.0*floor((_LightColor0.rgb*(dot(i.normalDir,lightDirection)*attenuation)) * _CellSteps) / (_CellSteps - 1)*(_Texture_var.rgb*_Color.rgb)) ));
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform float _CellSteps;
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                UNITY_LIGHT_ATTENUATION(attenuation,i, i.posWorld.xyz);
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(i.uv0, _Texture));
                float3 finalColor = saturate(( floor((_LightColor0.rgb*(dot(i.normalDir,lightDirection)*attenuation)) * _CellSteps) / (_CellSteps - 1) > 0.5 ? (1.0-(1.0-2.0*(floor((_LightColor0.rgb*(dot(i.normalDir,lightDirection)*attenuation)) * _CellSteps) / (_CellSteps - 1)-0.5))*(1.0-(_Texture_var.rgb*_Color.rgb))) : (2.0*floor((_LightColor0.rgb*(dot(i.normalDir,lightDirection)*attenuation)) * _CellSteps) / (_CellSteps - 1)*(_Texture_var.rgb*_Color.rgb)) ));
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
