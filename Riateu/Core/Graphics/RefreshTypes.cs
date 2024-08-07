using System;
using System.Runtime.InteropServices;
using RefreshCS;

namespace Riateu.Graphics;

#region Enums
public enum PrimitiveType
{
    PointList,
    LineList,
    LineStrip,
    TriangleList,
    TriangleStrip
}

public enum LoadOp
{
    Load,
    Clear,
    DontCare
}

public enum StoreOp
{
    Store,
    DontCare
}

public enum IndexElementSize
{
    Sixteen,
    ThirtyTwo
}

public enum TextureFormat
{
    Invalid = -1,

    /* Unsigned Normalized Float Color Formats */
    R8G8B8A8,
    B8G8R8A8,
    B5G6R5,
    B5G5R5A1,
    B4G4R4A4,
    R10G10B10A2,
    R16G16,
    R16G16B16A16,
    R8,
    A8,
    /* Compressed Unsigned Normalized Float Color Formats */
    BC1,
    BC2,
    BC3,
    BC7,
    /* Signed Normalized Float Color Formats  */
    R8G8_SNORM,
    R8G8B8A8_SNORM,
    /* Signed Float Color Formats */
    R16_SFLOAT,
    R16G16_SFLOAT,
    R16G16B16A16_SFLOAT,
    R32_SFLOAT,
    R32G32_SFLOAT,
    R32G32B32A32_SFLOAT,
    /* Unsigned Integer Color Formats */
    R8_UINT,
    R8G8_UINT,
    R8G8B8A8_UINT,
    R16_UINT,
    R16G16_UINT,
    R16G16B16A16_UINT,
    /* SRGB Color Formats */
    R8G8B8A8_SRGB,
    B8G8R8A8_SRGB,
    /* Compressed SRGB Color Formats */
    BC3_SRGB,
    BC7_SRGB,
    /* Depth Formats */
    D16_UNORM,
    D24_UNORM,
    D32_SFLOAT,
    D24_UNORM_S8_UINT,
    D32_SFLOAT_S8_UINT
}

[Flags]
public enum TextureUsageFlags
{
    Sampler = 0x1,
    ColorTarget = 0x2,
    DepthStencil = 0x4,
    GraphicsStorage = 0x8,
    ComputeStorageRead = 0x20,
    ComputeStorageWrite = 0x40
}

public enum TextureType
{
    TwoD,
    ThreeD,
    Cube
}

public enum SampleCount
{
    One,
    Two,
    Four,
    Eight
}

public enum CubeMapFace
{
    PositiveX,
    NegativeX,
    PositiveY,
    NegativeY,
    PositiveZ,
    NegativeZ
}

[Flags]
public enum BufferUsageFlags
{
    Vertex = 0x1,
    Index = 0x2,
    Indirect = 0x4,
    GraphicsStorage = 0x8,
    ComputeStorageRead = 0x20,
    ComputeStorageWrite = 0x40
}

public enum TransferBufferUsage
{
    Upload,
    Download
}

public enum ShaderStage
{
    Vertex,
    Fragment
}

public enum ShaderFormat
{
    Invalid,
    SPIRV,
    HLSL,
    DXBC,
    DXIL,
    MSL,
    METALLIB,
    SECRET
}

public enum VertexElementFormat
{
    Uint,
    Float,
    Vector2,
    Vector3,
    Vector4,
    Color,
    Byte4,
    Short2,
    Short4,
    NormalizedShort2,
    NormalizedShort4,
    HalfVector2,
    HalfVector4
}

public enum VertexInputRate
{
    Vertex,
    Instance
}

public enum FillMode
{
    Fill,
    Line
}

public enum CullMode
{
    None,
    Front,
    Back
}

public enum FrontFace
{
    CounterClockwise,
    Clockwise
}

public enum CompareOp
{
    Never,
    Less,
    Equal,
    LessOrEqual,
    Greater,
    NotEqual,
    GreaterOrEqual,
    Always
}

public enum StencilOp
{
    Keep,
    Zero,
    Replace,
    IncrementAndClamp,
    DecrementAndClamp,
    Invert,
    IncrementAndWrap,
    DecrementAndWrap
}

public enum BlendOp
{
    Add,
    Subtract,
    ReverseSubtract,
    Min,
    Max
}

public enum BlendFactor
{
    Zero,
    One,
    SourceColor,
    OneMinusSourceColor,
    DestinationColor,
    OneMinusDestinationColor,
    SourceAlpha,
    OneMinusSourceAlpha,
    DestinationAlpha,
    OneMinusDestinationAlpha,
    ConstantColor,
    OneMinusConstantColor,
    SourceAlphaSaturate
}

[Flags]
public enum ColorComponentFlags
{
    None = 0x0,
    R = 0x1,
    G = 0x2,
    B = 0x4,
    A = 0x8,
    RGB = R | G | B,
    RGBA = R | G | B | A
}

public enum Filter
{
    Nearest,
    Linear
}

public enum SamplerMipmapMode
{
    Nearest,
    Linear
}

public enum SamplerAddressMode
{
    Repeat,
    MirroredRepeat,
    ClampToEdge
}

public enum PresentMode
{
    VSync,
    Immediate,
    Mailbox
}

public enum SwapchainComposition
{
    SDR,
    SDRLinear,
    HDRExtendedLinear,
    HDR10_ST2084
}

[Flags]
public enum BackendFlags
{
    Invalid = 0x0,
    Vulkan = 0x1,
    D3D11 = 0x2,
    Metal = 0x4,
    All = Vulkan | D3D11 | Metal
}

#endregion


#region Structs
public struct Viewport(float Width, float Height)
{
    public float X;
    public float Y;
    public float Width = Width;
    public float Height = Height;
    public float MinDepth;
    public float MaxDepth;


    public Refresh.Viewport ToSDLGpu() 
    {
        return new Refresh.Viewport() 
        {
            X = X,
            Y = Y,
            W = Width,
            H = Height,
            MinDepth = MinDepth,
            MaxDepth = MaxDepth
        };
    }
}

public struct TextureCreateInfo
{
    public uint Width;
    public uint Height;
    public uint Depth;
    public bool IsCube; 
    public uint LayerCount;
    public uint LevelCount;
    public SampleCount SampleCount;
    public TextureFormat Format;
    public TextureUsageFlags UsageFlags;

    public RefreshCS.Refresh.TextureCreateInfo ToSDLGpu() 
    {
        return new RefreshCS.Refresh.TextureCreateInfo() 
        {
            Width = Width,
            Height = Height,
            Depth = Depth,
            IsCube = IsCube ? 1 : 0,
            LayerCount = LayerCount,
            LevelCount = LayerCount,
            SampleCount = (RefreshCS.Refresh.SampleCount)SampleCount,
            Format = (RefreshCS.Refresh.TextureFormat)Format,
            UsageFlags = (RefreshCS.Refresh.TextureUsageFlags)UsageFlags
        };
    }
}

public struct ColorAttachmentInfo(TextureSlice slice, Color clearColor, bool cycle, LoadOp loadOp = LoadOp.Clear, StoreOp storeOp = StoreOp.Store)
{
    public TextureSlice TextureSlice = slice;
    public Color ClearColor = clearColor;
    public LoadOp LoadOp = loadOp;
    public StoreOp StoreOp = storeOp;
    public bool Cycle = cycle; 

    public RefreshCS.Refresh.ColorAttachmentInfo ToSDLGpu() 
    {
        return new RefreshCS.Refresh.ColorAttachmentInfo() 
        {
            TextureSlice = TextureSlice.ToSDLGpu(),
            ClearColor = ClearColor.ToSDLGpu(),
            LoadOp = (RefreshCS.Refresh.LoadOp)LoadOp,
            StoreOp = (RefreshCS.Refresh.StoreOp)StoreOp,
            Cycle = Cycle ? 1 : 0
        };
    }
}

public struct DepthStencilAttachmentInfo(
    Texture texture, 
    DepthStencilValue depthStencilValue, 
    bool cycle,
    LoadOp loadOp = LoadOp.DontCare, 
    StoreOp storeOp = StoreOp.DontCare,
    LoadOp stencilLoadOp = LoadOp.DontCare,
    StoreOp stencilStoreOp = StoreOp.DontCare
)
{
    public TextureSlice TextureSlice = texture;
    public DepthStencilValue DepthStencilClearValue = depthStencilValue;
    public LoadOp LoadOp = loadOp;
    public StoreOp StoreOp = storeOp;
    public LoadOp StencilLoadOp = stencilLoadOp;
    public StoreOp StencilStoreOp = stencilStoreOp;
    public bool Cycle = cycle;

    public Refresh.DepthStencilAttachmentInfo ToSDLGpu() 
    {
        return new Refresh.DepthStencilAttachmentInfo() 
        {
            TextureSlice = TextureSlice.ToSDLGpu(),
            DepthStencilClearValue = DepthStencilClearValue.ToSDLGpu(),
            LoadOp = (Refresh.LoadOp)LoadOp,
            StoreOp = (Refresh.StoreOp)StoreOp,
            StencilLoadOp = (Refresh.LoadOp)StencilLoadOp,
            StencilStoreOp = (Refresh.StoreOp)StencilLoadOp,
            Cycle = Cycle ? 1 : 0
        };
    }
}

public struct StorageBufferReadWriteBinding(RawBuffer buffer, bool cycle)
{
    public RawBuffer Buffer = buffer;
    public bool Cycle = cycle; 

    public Refresh.StorageBufferReadWriteBinding ToSDLGpu() 
    {
        return new Refresh.StorageBufferReadWriteBinding() 
        {
            Buffer = Buffer.Handle,
            Cycle = Cycle ? 1 : 0
        };
    }
}

public struct StorageTextureReadWriteBinding
{
    public TextureSlice TextureSlice;
    public bool Cycle;

    public Refresh.StorageTextureReadWriteBinding ToSDLGpu() 
    {
        return new Refresh.StorageTextureReadWriteBinding() 
        {
            TextureSlice = TextureSlice.ToSDLGpu(),
            Cycle = Cycle ? 1 : 0
        };
    }
}

public unsafe struct ComputePipelineCreateInfo
{
    public ShaderFormat ShaderFormat;
    public uint ReadOnlyStorageTextureCount;
    public uint ReadOnlyStorageBufferCount;
    public uint ReadWriteStorageTextureCount;
    public uint ReadWriteStorageBufferCount;
    public uint UniformBufferCount;
    public uint ThreadCountX;
    public uint ThreadCountY;
    public uint ThreadCountZ;
}

public struct DepthStencilValue(float depth, uint stencil)
{
    public float Depth = depth;
    public uint Stencil = stencil;

    public Refresh.DepthStencilValue ToSDLGpu() 
    {
        return new Refresh.DepthStencilValue() 
        {
            Depth = Depth,
            Stencil = Stencil
        };
    }
}

public struct TextureSlice(Texture texture, uint mipLevel = 0, uint layer = 0)
{
    public Texture Texture = texture;
    public uint MipLevel = mipLevel;
    public uint Layer = layer;

    public RefreshCS.Refresh.TextureSlice ToSDLGpu() 
    {
        return new RefreshCS.Refresh.TextureSlice() 
        {
            Texture = Texture.Handle,
            MipLevel = MipLevel,
            Layer = Layer
        };
    }
}

public struct TextureLocation(TextureSlice slice, uint x, uint y, uint z)
{
    public TextureSlice TextureSlice = slice;
    public uint X = x;
    public uint Y = y;
    public uint Z = z;

    public RefreshCS.Refresh.TextureLocation ToSDLGpu() 
    {
        return new RefreshCS.Refresh.TextureLocation() 
        {
            TextureSlice = TextureSlice.ToSDLGpu(),
            X = X,
            Y = Y,
            Z = Z
        };
    }
}

public struct TextureRegion
{
    public TextureSlice TextureSlice;
    public uint X;
    public uint Y;
    public uint Z;
    public uint W;
    public uint H;
    public uint D;

    public TextureRegion(TextureSlice textureSlice, uint x, uint y, uint z, uint width, uint height, uint depth)  
    {
        TextureSlice = textureSlice;
        X = x;
        Y = y;
        Z = z;
        W = width;
        H = height;
        D = depth;
    }

    public TextureRegion(Texture texture) 
    {
        TextureSlice = new TextureSlice(texture, 0, 0);
        X = 0;
        Y = 0;
        Z = 0;
        W = texture.Width;
        H = texture.Height;
        D = texture.Depth;
    }

    public RefreshCS.Refresh.TextureRegion ToSDLGpu() 
    {
        return new RefreshCS.Refresh.TextureRegion() 
        {
            TextureSlice = TextureSlice.ToSDLGpu(),
            X = X,
            Y = Y,
            Z = Z,
            W = W,
            H = H,
            D = D
        };
    }
}

public struct TransferBufferLocation(TransferBuffer buffer, uint offset = 0)
{
    public TransferBuffer TransferBuffer = buffer;
    public uint Offset = offset;

    public Refresh.TransferBufferLocation ToSDLGpu() 
    {
        return new Refresh.TransferBufferLocation() 
        {
            TransferBuffer= TransferBuffer.Handle,
            Offset = Offset,
        };
    }
}

public struct TransferBufferRegion(TransferBuffer buffer, uint offset, uint size)
{
    public TransferBuffer TransferBuffer = buffer;
    public uint Offset = offset;
    public uint Size = size;

    public Refresh.TransferBufferRegion ToSDLGpu() 
    {
        return new Refresh.TransferBufferRegion() 
        {
            TransferBuffer= TransferBuffer.Handle,
            Offset = Offset,
            Size = Size
        };
    }
}

public struct BufferRegion(RawBuffer buffer, uint offset, uint size)
{
    public RawBuffer Buffer = buffer;
    public uint Offset = offset;
    public uint Size = size;

    public Refresh.BufferRegion ToSDLGpu() 
    {
        return new Refresh.BufferRegion() 
        {
            Buffer = Buffer.Handle,
            Offset = Offset,
            Size = Size
        };
    }
}

public struct BufferLocation(RawBuffer buffer, uint offset)
{
    public RawBuffer Buffer = buffer;
    public uint Offset = offset;

    public Refresh.BufferLocation ToSDLGpu() 
    {
        return new Refresh.BufferLocation() 
        {
            Buffer = Buffer.Handle,
            Offset = Offset,
        };
    }
}

public struct ShaderCreateInfo
{
	public ShaderStage ShaderStage;
	public ShaderFormat ShaderFormat;
	public uint SamplerCount;
	public uint StorageTextureCount;
	public uint StorageBufferCount;
	public uint UniformBufferCount;

	public static ShaderCreateInfo None = new ShaderCreateInfo();
}

public struct GraphicsPipelineCreateInfo
{
    public Shader VertexShader;
    public Shader FragmentShader;
    public DepthStencilState DepthStencilState;
    public MultisampleState MultisampleState;
    public RasterizerState RasterizerState;
    public PrimitiveType PrimitiveType;
    public VertexInputState VertexInputState;
    public GraphicsPipelineAttachmentInfo AttachmentInfo;
    public BlendConstants BlendConstants;
}

public struct VertexInputState 
{
    public VertexBinding[] VertexBindings;
    public VertexAttribute[] VertexAttributes;

    public static readonly VertexInputState Empty = new VertexInputState() 
    {
        VertexBindings = Array.Empty<VertexBinding>(),
        VertexAttributes = Array.Empty<VertexAttribute>()
    };

    public VertexInputState(VertexBinding vertexBinding, VertexAttribute[] vertexAttributes) 
    {
        VertexBindings = [vertexBinding];
        VertexAttributes = vertexAttributes;
    }

    public VertexInputState(VertexBinding[] vertexBindings, VertexAttribute[] vertexAttributes) 
    {
        VertexBindings = vertexBindings;
        VertexAttributes = vertexAttributes;
    }
}

public struct GraphicsPipelineAttachmentInfo 
{
    public ColorAttachmentDescription[] ColorAttachmentDescriptions;
    public bool HasDepthStencilAttachment;
    public TextureFormat DepthStencilFormat;

    public GraphicsPipelineAttachmentInfo(params ColorAttachmentDescription[] colorAttachmentDescriptions) 
    {
        ColorAttachmentDescriptions = colorAttachmentDescriptions;
        HasDepthStencilAttachment = false;
        DepthStencilFormat = TextureFormat.D16_UNORM;
    }


    public GraphicsPipelineAttachmentInfo(TextureFormat depthStencilFormat, params ColorAttachmentDescription[] colorAttachmentDescriptions) 
    {
        ColorAttachmentDescriptions = colorAttachmentDescriptions;
        HasDepthStencilAttachment = true;
        DepthStencilFormat = depthStencilFormat;
    }
}

public struct ColorAttachmentBlendState 
{
    public bool BlendEnable;
    public BlendOp AlphaBlendOp;
    public BlendOp ColorBlendOp;
    public ColorComponentFlags ColorWriteMask;
    public BlendFactor DestinationAlphaBlendFactor;
    public BlendFactor DestinationColorBlendFactor;
    public BlendFactor SourceAlphaBlendFactor;
    public BlendFactor SourceColorBlendFactor;

	public static readonly ColorAttachmentBlendState Additive = new ColorAttachmentBlendState
	{
		BlendEnable = true,
		AlphaBlendOp = BlendOp.Add,
		ColorBlendOp = BlendOp.Add,
		ColorWriteMask = ColorComponentFlags.RGBA,
		SourceColorBlendFactor = BlendFactor.SourceAlpha,
		SourceAlphaBlendFactor = BlendFactor.SourceAlpha,
		DestinationColorBlendFactor = BlendFactor.One,
		DestinationAlphaBlendFactor = BlendFactor.One
	};

	public static readonly ColorAttachmentBlendState AlphaBlend = new ColorAttachmentBlendState
	{
		BlendEnable = true,
		AlphaBlendOp = BlendOp.Add,
		ColorBlendOp = BlendOp.Add,
		ColorWriteMask = ColorComponentFlags.RGBA,
		SourceColorBlendFactor = BlendFactor.One,
		SourceAlphaBlendFactor = BlendFactor.One,
		DestinationColorBlendFactor = BlendFactor.OneMinusSourceAlpha,
		DestinationAlphaBlendFactor = BlendFactor.OneMinusSourceAlpha
	};

	public static readonly ColorAttachmentBlendState NonPremultiplied = new ColorAttachmentBlendState
	{
		BlendEnable = true,
		AlphaBlendOp = BlendOp.Add,
		ColorBlendOp = BlendOp.Add,
		ColorWriteMask = ColorComponentFlags.RGBA,
		SourceColorBlendFactor = BlendFactor.SourceAlpha,
		SourceAlphaBlendFactor = BlendFactor.SourceAlpha,
		DestinationColorBlendFactor = BlendFactor.OneMinusSourceAlpha,
		DestinationAlphaBlendFactor = BlendFactor.OneMinusSourceAlpha
	};

	public static readonly ColorAttachmentBlendState Opaque = new ColorAttachmentBlendState
	{
		BlendEnable = true,
		AlphaBlendOp = BlendOp.Add,
		ColorBlendOp = BlendOp.Add,
		ColorWriteMask = ColorComponentFlags.RGBA,
		SourceColorBlendFactor = BlendFactor.One,
		SourceAlphaBlendFactor = BlendFactor.One,
		DestinationColorBlendFactor = BlendFactor.Zero,
		DestinationAlphaBlendFactor = BlendFactor.Zero
	};

	public static readonly ColorAttachmentBlendState None = new ColorAttachmentBlendState
	{
		BlendEnable = false,
		ColorWriteMask = ColorComponentFlags.RGBA
	};

	public static readonly ColorAttachmentBlendState Disable = new ColorAttachmentBlendState
	{
		BlendEnable = false,
		ColorWriteMask = ColorComponentFlags.None
	};

	public Refresh.ColorAttachmentBlendState ToSDLGpu()
	{
		return new Refresh.ColorAttachmentBlendState
		{
			BlendEnable = BlendEnable ? 1 : 0,
			AlphaBlendOp = (Refresh.BlendOp) AlphaBlendOp,
			ColorBlendOp = (Refresh.BlendOp) ColorBlendOp,
			ColorWriteMask = (Refresh.ColorComponentFlags) ColorWriteMask,
			DestinationAlphaBlendFactor = (Refresh.BlendFactor) DestinationAlphaBlendFactor,
			DestinationColorBlendFactor = (Refresh.BlendFactor) DestinationColorBlendFactor,
			SourceAlphaBlendFactor = (Refresh.BlendFactor) SourceAlphaBlendFactor,
			SourceColorBlendFactor = (Refresh.BlendFactor) SourceColorBlendFactor
		};
	}
}

[StructLayout(LayoutKind.Sequential)]
public struct ColorAttachmentDescription(TextureFormat format, ColorAttachmentBlendState blendState) 
{
    public TextureFormat Format = format;
    public ColorAttachmentBlendState BlendState = blendState;
}

public struct RasterizerState 
{
    public CullMode CullMode;
    public float DepthBiasClamp;
    public float DepthBiasConstantFactor;
    public bool DepthBiasEnable;
    public float DepthBiasSlopeFactor;
    public FillMode FillMode;
    public FrontFace FrontFace;

	public static readonly RasterizerState CW_CullFront = new RasterizerState
	{
		CullMode = CullMode.Front,
		FrontFace = FrontFace.Clockwise,
		FillMode = FillMode.Fill,
		DepthBiasEnable = false
	};

	public static readonly RasterizerState CW_CullBack = new RasterizerState
	{
		CullMode = CullMode.Back,
		FrontFace = FrontFace.Clockwise,
		FillMode = FillMode.Fill,
		DepthBiasEnable = false
	};

	public static readonly RasterizerState CW_CullNone = new RasterizerState
	{
		CullMode = CullMode.None,
		FrontFace = FrontFace.Clockwise,
		FillMode = FillMode.Fill,
		DepthBiasEnable = false
	};

	public static readonly RasterizerState CW_Wireframe = new RasterizerState
	{
		CullMode = CullMode.None,
		FrontFace = FrontFace.Clockwise,
		FillMode = FillMode.Line,
		DepthBiasEnable = false
	};

	public static readonly RasterizerState CCW_CullFront = new RasterizerState
	{
		CullMode = CullMode.Front,
		FrontFace = FrontFace.CounterClockwise,
		FillMode = FillMode.Fill,
		DepthBiasEnable = false
	};

	public static readonly RasterizerState CCW_CullBack = new RasterizerState
	{
		CullMode = CullMode.Back,
		FrontFace = FrontFace.CounterClockwise,
		FillMode = FillMode.Fill,
		DepthBiasEnable = false
	};

	public static readonly RasterizerState CCW_CullNone = new RasterizerState
	{
		CullMode = CullMode.None,
		FrontFace = FrontFace.CounterClockwise,
		FillMode = FillMode.Fill,
		DepthBiasEnable = false
	};

	public static readonly RasterizerState CCW_Wireframe = new RasterizerState
	{
		CullMode = CullMode.None,
		FrontFace = FrontFace.CounterClockwise,
		FillMode = FillMode.Line,
		DepthBiasEnable = false
	};

	public Refresh.RasterizerState ToSDLGpu()
	{
		return new Refresh.RasterizerState
		{
			CullMode = (Refresh.CullMode) CullMode,
			DepthBiasClamp = DepthBiasClamp,
			DepthBiasConstantFactor = DepthBiasConstantFactor,
			DepthBiasEnable = DepthBiasEnable ? 1 : 0,
			DepthBiasSlopeFactor = DepthBiasSlopeFactor,
			FillMode = (Refresh.FillMode) FillMode,
			FrontFace = (Refresh.FrontFace) FrontFace
		};
	}
}

public struct MultisampleState(SampleCount sampleCount, uint sampleMask)
{
    public SampleCount MultisampleCount = sampleCount;
    public uint SampleMask = sampleMask;

    public static readonly MultisampleState None = new MultisampleState() 
    {
        MultisampleCount = SampleCount.One,
        SampleMask = uint.MaxValue
    };

    public Refresh.MultisampleState ToSDLGpu() 
    {
        return new Refresh.MultisampleState() 
        {
            MultisampleCount = (Refresh.SampleCount)MultisampleCount,
            SampleMask = SampleMask
        };
    }
}

public struct BlendConstants 
{
    public float R, G, B, A;
}

public struct VertexAttribute(uint binding, uint location, VertexElementFormat format, uint offset)
{
    public uint Location = location;
    public uint Binding = binding;
    public VertexElementFormat Format = format;
    public uint Offset = offset;

    public Refresh.VertexAttribute ToSDLGpu() 
    {
        return new Refresh.VertexAttribute 
        {
            Location = Location,
            Binding = Binding,
            Format = (Refresh.VertexElementFormat)Format,
            Offset = Offset
        };
    }
}

public struct VertexBinding
{
    public uint Binding;
    public uint Stride;
    public VertexInputRate InputRate;
    public uint StepRate;

    public unsafe static VertexBinding Create<T>(uint binding, VertexInputRate inputRate = VertexInputRate.Vertex, uint stepRate = 1) 
    where T : unmanaged
    {
        return new VertexBinding 
        {
            Binding = binding,
            InputRate = inputRate,
            StepRate = stepRate,
            Stride = (uint)sizeof(T)
        };
    }

    public Refresh.VertexBinding ToSDLGpu() 
    {
        return new Refresh.VertexBinding() 
        {
            Binding = Binding,
            StepRate = StepRate,
            InputRate = (Refresh.VertexInputRate)InputRate,
            Stride = Stride
        };
    }
}

public struct SamplerCreateInfo
{
    public Filter MinFilter;
    public Filter MagFilter;
    public SamplerMipmapMode MipmapMode;
    public SamplerAddressMode AddressModeU;
    public SamplerAddressMode AddressModeV;
    public SamplerAddressMode AddressModeW;
    public float MipLodBias;
    public bool AnisotropyEnable;
    public float MaxAnisotropy;
    public bool CompareEnable;
    public CompareOp CompareOp;
    public float MinLod;
    public float MaxLod;

	public static readonly SamplerCreateInfo AnisotropicClamp = new SamplerCreateInfo
	{
		MinFilter = Filter.Linear,
		MagFilter = Filter.Linear,
		MipmapMode = SamplerMipmapMode.Linear,
		AddressModeU = SamplerAddressMode.ClampToEdge,
		AddressModeV = SamplerAddressMode.ClampToEdge,
		AddressModeW = SamplerAddressMode.ClampToEdge,
		CompareEnable = false,
		AnisotropyEnable = true,
		MaxAnisotropy = 4,
		MipLodBias = 0f,
		MinLod = 0,
		MaxLod = 1000
	};

	public static readonly SamplerCreateInfo AnisotropicWrap = new SamplerCreateInfo
	{
		MinFilter = Filter.Linear,
		MagFilter = Filter.Linear,
		MipmapMode = SamplerMipmapMode.Linear,
		AddressModeU = SamplerAddressMode.Repeat,
		AddressModeV = SamplerAddressMode.Repeat,
		AddressModeW = SamplerAddressMode.Repeat,
		CompareEnable = false,
		AnisotropyEnable = true,
		MaxAnisotropy = 4,
		MipLodBias = 0f,
		MinLod = 0,
		MaxLod = 1000
	};

	public static readonly SamplerCreateInfo LinearClamp = new SamplerCreateInfo
	{
		MinFilter = Filter.Linear,
		MagFilter = Filter.Linear,
		MipmapMode = SamplerMipmapMode.Linear,
		AddressModeU = SamplerAddressMode.ClampToEdge,
		AddressModeV = SamplerAddressMode.ClampToEdge,
		AddressModeW = SamplerAddressMode.ClampToEdge,
		CompareEnable = false,
		AnisotropyEnable = false,
		MipLodBias = 0f,
		MinLod = 0,
		MaxLod = 1000
	};

	public static readonly SamplerCreateInfo LinearWrap = new SamplerCreateInfo
	{
		MinFilter = Filter.Linear,
		MagFilter = Filter.Linear,
		MipmapMode = SamplerMipmapMode.Linear,
		AddressModeU = SamplerAddressMode.Repeat,
		AddressModeV = SamplerAddressMode.Repeat,
		AddressModeW = SamplerAddressMode.Repeat,
		CompareEnable = false,
		AnisotropyEnable = false,
		MipLodBias = 0f,
		MinLod = 0,
		MaxLod = 1000
	};

	public static readonly SamplerCreateInfo PointClamp = new SamplerCreateInfo
	{
		MinFilter = Filter.Nearest,
		MagFilter = Filter.Nearest,
		MipmapMode = SamplerMipmapMode.Nearest,
		AddressModeU = SamplerAddressMode.ClampToEdge,
		AddressModeV = SamplerAddressMode.ClampToEdge,
		AddressModeW = SamplerAddressMode.ClampToEdge,
		CompareEnable = false,
		AnisotropyEnable = false,
		MipLodBias = 0f,
		MinLod = 0,
		MaxLod = 1000
	};

	public static readonly SamplerCreateInfo PointWrap = new SamplerCreateInfo
	{
		MinFilter = Filter.Nearest,
		MagFilter = Filter.Nearest,
		MipmapMode = SamplerMipmapMode.Nearest,
		AddressModeU = SamplerAddressMode.Repeat,
		AddressModeV = SamplerAddressMode.Repeat,
		AddressModeW = SamplerAddressMode.Repeat,
		CompareEnable = false,
		AnisotropyEnable = false,
		MipLodBias = 0f,
		MinLod = 0,
		MaxLod = 1000
	};

    public RefreshCS.Refresh.SamplerCreateInfo ToSDLGpu() 
    {
        return new RefreshCS.Refresh.SamplerCreateInfo() 
        {
            MinFilter = (RefreshCS.Refresh.Filter)MinFilter,
            MagFilter = (RefreshCS.Refresh.Filter)MagFilter,
            MipmapMode = (RefreshCS.Refresh.SamplerMipmapMode)MipmapMode,
            AddressModeU = (RefreshCS.Refresh.SamplerAddressMode)AddressModeU,
            AddressModeV = (RefreshCS.Refresh.SamplerAddressMode)AddressModeV,
            AddressModeW = (RefreshCS.Refresh.SamplerAddressMode)AddressModeW,
            MipLodBias = MipLodBias,
            AnisotropyEnable = AnisotropyEnable ? 1 : 0,
            MaxAnisotropy = MaxAnisotropy,
            CompareEnable = CompareEnable ? 1 : 0,
            CompareOp = (RefreshCS.Refresh.CompareOp)CompareOp,
            MinLod = MinLod,
            MaxLod = MaxLod
        };
    }
}

public struct StencilOpState 
{
    public StencilOp FailOp;
    public StencilOp PassOp;
    public StencilOp DepthFailOp;
    public CompareOp CompareOp;

    public Refresh.StencilOpState ToSDLGpu() 
    {
        return new Refresh.StencilOpState() 
        {
            FailOp = (Refresh.StencilOp)FailOp,
            PassOp = (Refresh.StencilOp)PassOp,
            DepthFailOp = (Refresh.StencilOp)DepthFailOp,
            CompareOp = (Refresh.CompareOp)CompareOp
        };
    }
}

public struct DepthStencilState 
{
    public bool DepthTestEnable;
    public StencilOpState BackStencilState;
    public StencilOpState FrontStencilState;
    public uint CompareMask;
    public uint WriteMask;
    public uint Reference;
    public CompareOp CompareOp;
    public bool DepthWriteEnable;
    public bool StencilTestEnable;

	public static readonly DepthStencilState DepthReadWrite = new DepthStencilState
	{
		DepthTestEnable = true,
		DepthWriteEnable = true,
		StencilTestEnable = false,
		CompareOp = CompareOp.LessOrEqual
	};

	public static readonly DepthStencilState DepthRead = new DepthStencilState
	{
		DepthTestEnable = true,
		DepthWriteEnable = false,
		StencilTestEnable = false,
		CompareOp = CompareOp.LessOrEqual
	};

	public static readonly DepthStencilState Disable = new DepthStencilState
	{
		DepthTestEnable = false,
		DepthWriteEnable = false,
		StencilTestEnable = false
	};

    public Refresh.DepthStencilState ToSDLGpu() 
    {
        return new Refresh.DepthStencilState() 
        {
            DepthTestEnable = DepthTestEnable ? 1 : 0,
            BackStencilState = BackStencilState.ToSDLGpu(),
            FrontStencilState = FrontStencilState.ToSDLGpu(),
            CompareMask = CompareMask,
            WriteMask = WriteMask,
            Reference = Reference,
            CompareOp = (Refresh.CompareOp)CompareOp,
            DepthWriteEnable = DepthWriteEnable ? 1 : 0,
            StencilTestEnable = StencilTestEnable ? 1 : 0
        };
    }
}

public struct BufferBinding(RawBuffer buffer, uint offset)
{
    public RawBuffer Buffer = buffer;
    public uint Offset = offset;

    public Refresh.BufferBinding ToSDLGpu() 
    {
        return new Refresh.BufferBinding() 
        {
            Buffer = Buffer.Handle,
            Offset = Offset
        };
    }
}

public struct TextureSamplerBinding(Texture texture, Sampler sampler)
{
    public Texture Texture = texture;
    public Sampler Sampler = sampler;

    public Refresh.TextureSamplerBinding ToSDLGpu() 
    {
        return new Refresh.TextureSamplerBinding() 
        {
            Texture = Texture.Handle,
            Sampler = Sampler.Handle
        };
    }
}

public struct TextureTransferInfo(TransferBuffer transferBuffer, uint offset = 0, uint imagePitch = 0, uint imageHeight = 0)
{
	public TransferBuffer TransferBuffer = transferBuffer;
	public uint Offset = offset;
	public uint ImagePitch = imagePitch;
	public uint ImageHeight = imageHeight;

	public Refresh.TextureTransferInfo ToSDLGpu()
	{
		return new Refresh.TextureTransferInfo
		{
			TransferBuffer = TransferBuffer.Handle,
			Offset = Offset,
			ImagePitch = ImagePitch,
			ImageHeight = ImageHeight
		};
	}
}

#endregion