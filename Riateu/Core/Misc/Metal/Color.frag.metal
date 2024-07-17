// language: metal1.0
#include <metal_stdlib>
#include <simd/simd.h>

using metal::uint;


struct main_Input {
    metal::float4 color [[user(loc0), center_perspective]];
};
struct main_Output {
    metal::float4 member [[color(0)]];
};
fragment main_Output main_(
  main_Input varyings [[stage_in]]
) {
    const auto color = varyings.color;
    return main_Output { color };
}
