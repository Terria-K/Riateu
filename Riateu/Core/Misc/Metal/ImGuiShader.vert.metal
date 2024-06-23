// language: metal1.0
#include <metal_stdlib>
#include <simd/simd.h>

using metal::uint;

struct UniformBlock {
    metal::float4x4 MatrixUniform;
};
struct type_3 {
    metal::float4 outColor;
    metal::float2 outTexCoord;
    metal::float4 gl_Position;
};

void main_1(
    thread metal::float2& position_1,
    thread metal::float4& color_1,
    thread metal::float4& outColor,
    thread metal::float2& outTexCoord,
    constant UniformBlock& global,
    thread metal::float4& gl_Position
) {
    metal::float4 _e7 = color_1;
    outColor = _e7;
    metal::float2 _e8 = outTexCoord;
    outTexCoord = _e8;
    metal::float4x4 _e10 = global.MatrixUniform;
    metal::float2 _e11 = position_1;
    gl_Position = _e10 * metal::float4(_e11.x, _e11.y, 0.0, 1.0);
    return;
}

struct main_Input {
    metal::float2 position [[attribute(0)]];
    metal::float2 tex_coord [[attribute(1)]];
    metal::float4 color [[attribute(2)]];
};
struct main_Output {
    metal::float4 outColor [[user(loc0), center_perspective]];
    metal::float2 outTexCoord [[user(loc1), center_perspective]];
    metal::float4 gl_Position [[position]];
};
vertex main_Output main_(
  main_Input varyings [[stage_in]]
, constant UniformBlock& global [[user(fake0)]]
) {
    metal::float2 position_1 = {};
    metal::float2 tex_coord_1 = {};
    metal::float4 color_1 = {};
    metal::float4 outColor = {};
    metal::float2 outTexCoord = {};
    metal::float4 gl_Position = {};
    const auto position = varyings.position;
    const auto tex_coord = varyings.tex_coord;
    const auto color = varyings.color;
    position_1 = position;
    tex_coord_1 = tex_coord;
    color_1 = color;
    main_1(position_1, color_1, outColor, outTexCoord, global, gl_Position);
    metal::float4 _e19 = outColor;
    metal::float2 _e21 = outTexCoord;
    metal::float4 _e23 = gl_Position;
    const auto _tmp = type_3 {_e19, _e21, _e23};
    return main_Output { _tmp.outColor, _tmp.outTexCoord, _tmp.gl_Position };
}