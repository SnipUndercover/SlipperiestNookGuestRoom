texture input_texture : register(t0);
sampler input_sampler : register(s0);

uniform float flash_strength;

float4 PS_Flash(float3 position : SV_POSITION, float4 color : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    float4 texColor = tex2D(input_sampler, uv);
    return texColor + (float4(1., 1., 1., 1.) * flash_strength * texColor.a);
}

technique flash
{
    pass apply
    {
        PixelShader = compile ps_3_0 PS_Flash();
    }
}