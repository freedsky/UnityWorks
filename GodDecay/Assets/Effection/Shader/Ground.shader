Shader "GodDecay/Environment/Ground"
{
    //forward��add��pass��Ⱦ������������
    Properties
    {
        _DiffuseTexture("Diffuse Texture",2D) = "white" {}
        _Diffuse("Diffuse", Color) = (1, 1, 1, 1)
        _Specular("Specular", Color) = (1, 1, 1, 1)
        _BumpMap("Bump Map", 2D) = "bump" {}
        _RampTex("Ramp Tex",2D) = "white" {}
        _BumpScale("Bump Scale", Float) = 1.0
        _Gloss("Gloss", Range(8.0, 256)) = 20
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM
            
            #pragma multi_compile_fwdbase	

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            uniform sampler2D _DiffuseTexture;
            uniform float4 _DiffuseTexture_ST;

            uniform sampler2D _BumpMap;
            uniform float4 _BumpMap_ST;
            uniform float _BumpScale;

            uniform sampler2D _RampTex;
            uniform float4 _RampTex_ST;

            fixed4 _Diffuse;
            fixed4 _Specular;
            float _Gloss;

            struct a2v 
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f 
            {
                float4 pos : SV_POSITION;
                fixed4 uv : TEXCOORD0;
                float4 TtoW0 : TEXCOORD1;
                float4 TtoW1 : TEXCOORD2;
                float4 TtoW2 : TEXCOORD3;
                //����һ��������Ӱ�������������
                SHADOW_COORDS(4)
            };

            v2f vert(a2v v) 
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                o.uv.xy = v.texcoord.xy * _DiffuseTexture_ST.xy + _DiffuseTexture_ST.zw;
                o.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;

                //�����߿ռ��µı任�����Ϊ��������
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
                fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

                o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
                o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
                o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
                //������Ӱ����������
                TRANSFER_SHADOW(o);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 worldPos = float3(i.TtoW0.w,i.TtoW1.w,i.TtoW2.w);
                fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);

                fixed3 tangentNomal = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
                tangentNomal.xy *= _BumpScale;
                tangentNomal.z = sqrt(1.0 - saturate(dot(tangentNomal.xy, tangentNomal.xy)));
                //�Ѳ�������������ת��������ռ���
                tangentNomal = normalize(half3(dot(i.TtoW0.xyz, tangentNomal), dot(i.TtoW1.xyz, tangentNomal), dot(i.TtoW2.xyz, tangentNomal)));

                fixed3 albedo = tex2D(_DiffuseTexture,i.uv).rgb * _Diffuse.rgb;

                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

                float diff = dot(tangentNomal, lightDir) * 0.5f + 0.5f;
                //��Ϊ�������ֶ��г��Զ���⣬���ж�����Թأ��ǲ��־�Ϊ0��������Ҳ����0��
                fixed3 diffuse = diff * _LightColor0.rgb * tex2D(_RampTex, fixed2(diff, diff)).rgb * albedo;

                fixed3 halfDir = normalize(viewDir + lightDir);
                float spec = pow(max(dot(halfDir, tangentNomal), 0.0f), _Gloss);
                fixed3 specular = spec * _Specular.rgb * _LightColor0.rgb;

                fixed shadow = SHADOW_ATTENUATION(i);

                fixed atten = 1.0f;

                return fixed4(ambient + (diffuse + specular) * atten * shadow, 1.0f);
            }

            ENDCG
        }
        Pass
        {
            Tags { "LightMode" = "ForwardAdd" }

            Blend One One

            CGPROGRAM

            #pragma multi_compile_fwdadd

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            uniform sampler2D _DiffuseTexture;
            uniform float4 _DiffuseTexture_ST;

            uniform sampler2D _BumpMap;
            uniform float4 _BumpMap_ST;
            uniform float _BumpScale;

            uniform sampler2D _RampTex;
            uniform float4 _RampTex_ST;

            fixed4 _Diffuse;
            fixed4 _Specular;
            float _Gloss;

            struct a2v 
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f 
            {
                float4 pos : SV_POSITION;
                fixed4 uv : TEXCOORD0;
                float4 TtoW0 : TEXCOORD1;
                float4 TtoW1 : TEXCOORD2;
                float4 TtoW2 : TEXCOORD3;
                SHADOW_COORDS(4)
            };

            v2f vert(a2v v) 
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                o.uv.xy = v.texcoord.xy * _DiffuseTexture_ST.xy + _DiffuseTexture_ST.zw;
                o.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;

                //�����߿ռ��µı任�����Ϊ��������
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
                fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

                o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
                o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
                o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

                TRANSFER_SHADOW(o);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 worldPos = float3(i.TtoW0.w,i.TtoW1.w,i.TtoW2.w);
                //���ڲ�ͬ���͵Ĺ��ȡ�������ñ����ķ�ʽҲ��һ������Ҫ�ڻ�ȡ��Դ����λ�õȱ���ʱ�����жϴ���
                #ifdef USING_DIRECTIONAL_LIGHT
                    fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                #else
                    fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz - worldPos);
                #endif

                fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos.xyz);

                fixed3 tangentNomal = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
                tangentNomal.xy *= _BumpScale;
                tangentNomal.z = sqrt(1.0 - saturate(dot(tangentNomal.xy, tangentNomal.xy)));
                //�Ѳ�������������ת��������ռ���
                tangentNomal = normalize(half3(dot(i.TtoW0.xyz, tangentNomal), dot(i.TtoW1.xyz, tangentNomal), dot(i.TtoW2.xyz, tangentNomal)));

                fixed3 ambient = tex2D(_DiffuseTexture, i.uv).rgb * _Diffuse.rgb;

                float diff = dot(tangentNomal, lightDir) * 0.5f + 0.5f;
                fixed3 diffuse = diff * ambient * tex2D(_RampTex, fixed2(diff, diff)).rgb * _LightColor0.rgb;

                fixed3 halfDir = normalize(viewDir + lightDir);
                float spec = pow(max(dot(halfDir, tangentNomal), 0.0f), _Gloss);
                fixed3 specular = spec * _Specular.rgb * _LightColor0.rgb;


                //�������˥������Ӱ
                #ifdef USING_DIRECTIONAL_LIGHT
                    fixed atten = 1.0f;
                #else
                    float3 lightCoord = mul(unity_WorldToLight, float4(worldPos, 1)).xyz;
                    fixed atten = tex2D(_LightTexture0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
                #endif

                fixed shadow = SHADOW_ATTENUATION(i);

                return fixed4((diffuse + specular) * atten * shadow, 1.0f);
            }
            ENDCG
        }
    }
    FallBack "Specular"
}
