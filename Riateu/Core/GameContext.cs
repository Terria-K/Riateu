using System.IO;
using MoonWorks;
using MoonWorks.Graphics;
using Riateu.Graphics;
using Riateu.Misc;

namespace Riateu;

/// <summary>
/// A class that contains all of the default pipelines and features that are needed for
/// other renderers like texts or standard rendering stuffs.
/// </summary>
public static class GameContext
{
    /// <summary>
    /// The application graphics device.
    /// </summary>
    public static GraphicsDevice GraphicsDevice;
    /// <summary>
    /// The default material for basic rendering.
    /// </summary>
    public static Material DefaultMaterial;
    /// <summary>
    /// A rendering material that uses R8G8B8A8 format.
    /// </summary>
    public static Material RGBMaterial;
    /// <summary>
    /// A rendering material designed specifically for text rendered in msdf format.
    /// </summary>
    public static Material MSDFMaterial;
    /// <summary>
    /// An instanced material use as an fast alternative for default pipeline.
    /// </summary>
    public static Material InstancedMaterial;
    /// <summary>
    /// A compute pipeline used for <see cref="Riateu.Graphics.Batch"/> to work.
    /// </summary>
    public static ComputePipeline SpriteBatchPipeline;

    internal static void Init(GraphicsDevice device, Window mainWindow)
    {
        GraphicsDevice = device;
        var positionTextureColor = Resources.PositionTextureColor;
        using var ms1 = new MemoryStream(positionTextureColor);
        Shader vertexPSC = new Shader(device, ms1, "main", new ShaderCreateInfo {
            ShaderStage = ShaderStage.Vertex,
            ShaderFormat = BackendShaderFormat,
            UniformBufferCount = 1
        });

        var textureFragment = Resources.Texture;
        using var ms2 = new MemoryStream(textureFragment);
        Shader fragmentPSC = new Shader(device, ms2, "main", new ShaderCreateInfo {
            ShaderStage = ShaderStage.Fragment,
            ShaderFormat = BackendShaderFormat,
            SamplerCount = 1
        });

        GraphicsPipelineCreateInfo pipelineCreateInfo = new GraphicsPipelineCreateInfo()
        {
            AttachmentInfo = new GraphicsPipelineAttachmentInfo(
                new ColorAttachmentDescription(mainWindow.SwapchainFormat,
                ColorAttachmentBlendState.AlphaBlend)
            ),
            DepthStencilState = DepthStencilState.Disable,
            MultisampleState = MultisampleState.None,
            PrimitiveType = PrimitiveType.TriangleList,
            RasterizerState = RasterizerState.CCW_CullNone,
            VertexShader = vertexPSC,
            FragmentShader = fragmentPSC,
            VertexInputState = VertexInputState.CreateSingleBinding<PositionTextureColorVertex>()
        };

        DefaultMaterial = new Material(new GraphicsPipeline(device, pipelineCreateInfo));

        GraphicsPipelineCreateInfo rgbCreateInfo = new GraphicsPipelineCreateInfo()
        {
            AttachmentInfo = new GraphicsPipelineAttachmentInfo(
                new ColorAttachmentDescription(TextureFormat.R8G8B8A8,
                ColorAttachmentBlendState.AlphaBlend)
            ),
            DepthStencilState = DepthStencilState.Disable,
            MultisampleState = MultisampleState.None,
            PrimitiveType = PrimitiveType.TriangleList,
            RasterizerState = RasterizerState.CCW_CullNone,
            VertexShader = vertexPSC,
            FragmentShader = fragmentPSC,
            VertexInputState = VertexInputState.CreateSingleBinding<PositionTextureColorVertex>()
        };

        RGBMaterial = new Material(new GraphicsPipeline(device, rgbCreateInfo));

        GraphicsPipelineCreateInfo msdfPipelineCreateInfo = new GraphicsPipelineCreateInfo()
        {
            AttachmentInfo = new GraphicsPipelineAttachmentInfo(
                new ColorAttachmentDescription(mainWindow.SwapchainFormat,
                ColorAttachmentBlendState.AlphaBlend)
            ),
            DepthStencilState = DepthStencilState.Disable,
            MultisampleState = MultisampleState.None,
            PrimitiveType = PrimitiveType.TriangleList,
            RasterizerState = RasterizerState.CCW_CullNone,
            VertexShader = device.TextVertexShader,
            FragmentShader = device.TextFragmentShader,
            VertexInputState = device.TextVertexInputState
        };

        MSDFMaterial = new Material(new GraphicsPipeline(device, msdfPipelineCreateInfo));

        var vertexBufferDescription = VertexBindingAndAttributes.Create<PositionVertex>(0);
        var instancedBufferDescription = VertexBindingAndAttributes.Create<InstancedVertex>(1, 1, VertexInputRate.Instance);

        var tileMapBytes = Resources.InstancedShader;
        using var ms3 = new MemoryStream(tileMapBytes);
        Shader instancedPSC = new Shader(device, ms3, "main", new ShaderCreateInfo {
            ShaderStage = ShaderStage.Vertex,
            ShaderFormat = BackendShaderFormat,
            UniformBufferCount = 1
        });

        GraphicsPipelineCreateInfo instancedPipelineCreateInfo = new GraphicsPipelineCreateInfo()
        {
            AttachmentInfo = new GraphicsPipelineAttachmentInfo(
                new ColorAttachmentDescription(TextureFormat.R8G8B8A8,
                ColorAttachmentBlendState.AlphaBlend)
            ),
            DepthStencilState = DepthStencilState.Disable,
            MultisampleState = MultisampleState.None,
            PrimitiveType = PrimitiveType.TriangleList,
            RasterizerState = RasterizerState.CCW_CullNone,
            VertexShader = instancedPSC,
            FragmentShader = fragmentPSC,
            VertexInputState = new VertexInputState([
                vertexBufferDescription,
                instancedBufferDescription
            ])
        };

        InstancedMaterial = new Material(new GraphicsPipeline(device, instancedPipelineCreateInfo));

        var spriteBatchShader = Resources.SpriteBatchShader;
        using var comp1 = new MemoryStream(spriteBatchShader);
        SpriteBatchPipeline = new ComputePipeline(device, comp1, "main", new ComputePipelineCreateInfo 
        {
            ShaderFormat = BackendShaderFormat,
            ReadWriteStorageBufferCount = 1,
            ReadOnlyStorageBufferCount = 1,
            ThreadCountX = 64,
            ThreadCountY = 1,
            ThreadCountZ = 1
        });
    }

    /// <summary>
    /// A preprocessed shader format depending on the target backend it uses.
    /// </summary>
    public const ShaderFormat BackendShaderFormat =
#if Metal
    ShaderFormat.Metal;
#elif D3D11
    ShaderFormat.HLSL;
#elif Vulkan
    ShaderFormat.SPIRV;
#endif
}
