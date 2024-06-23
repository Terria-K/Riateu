#!/usr/bin/bash

for filename in Shaders/*.glsl; do
    name=$(basename -- $filename .glsl)
    vk_out="Vulkan/$name.spv"
    metal_out="Metal/$name.metal"
    d3d11_out="D3D11/$name.hlsl"

    echo "Compiling with Vulkan..."
    naga $filename $vk_out
    echo "Compiling with Metal..."
    naga $filename $metal_out
    echo "Compiling with D3D11..."
    naga $filename $d3d11_out
done