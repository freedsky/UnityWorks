Shader "GodDecay/Environment/Sea"
{
    Properties
    {
        _WaterColor("Water Color", Color) = (1.0,1.0,1.0,1.0)

        _WaterDiffuseTexture("Water Diffuse Texture", 2D) = "white" {}
        _AmbientDiffuseTexture("Ambient Diffuse Texture", 2D) = "white" {}

        _NormalNoiseTexture("Normal Noise Map", 2D) = "bump" {}
        _NoiseXSpeed("Wave Horizontal Speed", Range(-0.1, 0.1)) = 0.01
        _NoiseYSpeed("Wave Vertical Speed", Range(-0.1, 0.1)) = 0.01

        _SpecularScale("Specular Scale", Range(0, 0.1)) = 0.01
        _Gloss("Gloss", Range(32.0,256.0)) = 32
    }
    SubShader
    {
        Tags {"RenderType" = "Opaque" }

        Pass
        {
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM
            //ˮ�������һ������Ӱ��
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            #pragma multi_compile_fwdbase

            #pragma vertex vert
            #pragma fragment frag

            fixed4 _WaterColor;

            sampler2D _WaterDiffuseTexture;
            float4 _WaterDiffuseTexture_ST;

            sampler2D _AmbientDiffuseTexture;
            float4 _AmbientDiffuseTexture_ST;

            sampler2D _NormalNoiseTexture;
            float4 _NormalNoiseTexture_ST;

            fixed _NoiseXSpeed;
            fixed _NoiseYSpeed;

            uniform fixed _SpecularScale;
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
                float4 uv1 : TEXCOORD0;
                float4 uv2 : TEXCOORD1;
                float4 TtoW0 : TEXCOORD2;
                float4 TtoW1 : TEXCOORD3;
                float4 TtoW2 : TEXCOORD4;
            };

            v2f vert(a2v v) 
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                //����������Ž��д���
                o.uv1.xy = TRANSFORM_TEX(v.texcoord, _WaterDiffuseTexture);
                o.uv2.xy = TRANSFORM_TEX(v.texcoord, _AmbientDiffuseTexture);
                o.uv1.zw = TRANSFORM_TEX(v.texcoord, _NormalNoiseTexture);
                //ͳͳתΪ����������д���
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
                fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;
                //��������ռ��µ�����ת������
                o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
                o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
                o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
                //�õ�����ռ��¹۲��߷���
                fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
                fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                //�Լ������õ�ʱ�����_Time��ƫ�Ƽ���
                float2 speed = _Time.y * float2(_NoiseXSpeed, _NoiseYSpeed);
                //����ʱ��������ƫ�������������[�������������������]�������β���
                fixed3 bump1 = UnpackNormal(tex2D(_NormalNoiseTexture, i.uv1.zw + speed)).rgb;
                fixed3 bump2 = UnpackNormal(tex2D(_NormalNoiseTexture, i.uv1.zw - speed)).rgb;
                fixed3 bump = normalize(bump1 + bump2);
                //����ƫ�ƺ�ķ�������
                bump = normalize(half3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump), dot(i.TtoW2.xyz, bump)));
                //Ȼ�����������ȥ������ͼ�ͼ��㷴�䲢����������ͼ
                fixed4 albedo = tex2D(_AmbientDiffuseTexture, i.uv2.xy + speed);
                //����ambient
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                
                float diff = max(dot(bump, lightDir), 0.0f);
                fixed3 diffuse = _LightColor0.rgb * tex2D(_WaterDiffuseTexture, i.uv1.xy + speed);

                fixed3 halfDir = normalize(viewDir + lightDir);
                float spec = pow(max(dot(halfDir, bump), 0.0f), _Gloss);
                fixed w = fwidth(spec) * 2.0f;
                fixed3 specular = spec * _LightColor0.rgb * lerp(0, 1, smoothstep(-w, w, spec + _SpecularScale - 1)) * step(0.0001, _SpecularScale);;

                return fixed4((ambient + diffuse + specular) * _WaterColor.rgb, 1);
            }

            ENDCG
        }
    }
    FallBack Off
}
