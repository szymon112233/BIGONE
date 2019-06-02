Shader "Custom/BuildablePreviewShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_bColor("Buildable Color", Color) = (0,0,0,0)
		_ubColor("Unbuildable Color", Color) = (0,0,0,0)
		_Alpha("Transparency", Range(0,1)) = 0.0
		_isBuildable("Is Buildable?", Int) = 0
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        
		fixed4 _bColor;
		fixed4 _ubColor;
		fixed _Alpha;
		fixed _isBuildable;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			if (_isBuildable == 1)
				c = c * _bColor;
			else
				c = c * _ubColor;
            o.Albedo = c.rgb;
            o.Alpha = _Alpha;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
