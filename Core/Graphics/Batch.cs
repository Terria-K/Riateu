using System.Collections.Generic;
using System.Runtime.InteropServices;
using MoonWorks;
using MoonWorks.Graphics;
using MoonWorks.Math.Float;

namespace Riateu.Graphics;

public class Batch : System.IDisposable
{
    private static readonly float[] CornerOffsetX = { 0.0f, 0.0f, 1.0f, 1.0f };
    private static readonly float[] CornerOffsetY = { 0.0f, 1.0f, 0.0f, 1.0f };  
    private const int MaxTextures = 8192;
    private GraphicsDevice device;
    private PositionColorTextureVertex[] vertices;
    private uint[] indices;
    private TextureSamplerBinding[] textures;
    private TextureSamplerBinding[] fragmentSampler;
    private uint textureCount;

    private Buffer vertexBuffer;
    private Buffer indexBuffer;
    private Stack<TransformVertexUniform> Matrices;

    public TransformVertexUniform Matrix;

    public Batch(GraphicsDevice device, int width, int height) 
    {
        Matrices = new();
        this.device = device;
        textures = new TextureSamplerBinding[MaxTextures];
        vertices = new PositionColorTextureVertex[MaxTextures * 4];
        indices = GenerateIndexArray(MaxTextures * 6);

        fragmentSampler = new TextureSamplerBinding[1];
        vertexBuffer = Buffer.Create<PositionColorTextureVertex>(device, BufferUsageFlags.Vertex, (uint)vertices.Length);
        indexBuffer = Buffer.Create<uint>(device, BufferUsageFlags.Index, (uint)indices.Length);

        TransformVertexUniform uniform;
        var model = Matrix4x4.CreateScale(1) *
            Matrix4x4.CreateRotationZ(0) *
            Matrix4x4.CreateTranslation(0, 0, 0);
        var view = Matrix4x4.CreateTranslation(0, 0, 0);
        var projection = Matrix4x4.CreateOrthographicOffCenter(0, width, 0, height, -1, 1);
        uniform = new TransformVertexUniform(
            model * view * projection
        );
        Matrix = uniform;
    }

    public static uint[] GenerateIndexArray(uint maxIndices)
    {
        var result = new uint[maxIndices];
        for (uint i = 0, j = 0; i < maxIndices; i += 6, j += 4)
        {
            result[i] = j;
            result[i + 1] = j + 1;
            result[i + 2] = j + 2;
            result[i + 3] = j + 2;
            result[i + 4] = j + 1;
            result[i + 5] = j + 3;
        }

        return result;
    }

    public void PushMatrix(in Matrix4x4 matrix) 
    {
        var transformUniform = new TransformVertexUniform(matrix);
        Matrices.Push(transformUniform);
        Matrix = transformUniform;
    }

    public void PushMatrix(in Camera camera) 
    {
        PushMatrix(camera.Transform);
    }

    public void PopMatrix() 
    {
        if (Matrices.Count == 0) 
        {
            Logger.LogError("Use of PopMatrix while there is no matrix had pushed yet");
            return;
        }
        Matrix = Matrices.Pop();
    }

    public void PushVertex(in CommandBuffer cmdBuf) 
    {
        if (textureCount == 0) 
        {
            return;
        }

        cmdBuf.SetBufferData(indexBuffer, indices, 0, 0, textureCount * 6);
        cmdBuf.SetBufferData(vertexBuffer, vertices, 0, 0, textureCount * 4);
    }

    public void Draw(in CommandBuffer cmdBuf) 
    {
        Draw(cmdBuf, Matrix);
    }

    public void Draw(in CommandBuffer cmdBuf, TransformVertexUniform viewProjection) 
    {
        if (textureCount == 0) 
        {
            return;
        }
        var vertexOffset = cmdBuf.PushVertexShaderUniforms(viewProjection);

        cmdBuf.BindVertexBuffers(vertexBuffer);
        cmdBuf.BindIndexBuffer(indexBuffer, IndexElementSize.ThirtyTwo);
        fragmentSampler[0] = textures[0];

        var offset = 0u;
        for (uint i = 1; i < textureCount; i++) 
        {
            var texture = textures[i];
            if (texture.Sampler.Handle == fragmentSampler[0].Sampler.Handle && 
                texture.Texture.Handle == fragmentSampler[0].Texture.Handle) 
            {
                continue;
            }
            cmdBuf.BindFragmentSamplers(fragmentSampler);
            cmdBuf.DrawIndexedPrimitives(offset * 4u, 0u, (i - offset) * 2u, vertexOffset, 0u);
            fragmentSampler[0] = texture;
            offset = i;
        }

        cmdBuf.BindFragmentSamplers(fragmentSampler);
        cmdBuf.DrawIndexedPrimitives(offset * 4u, 0u, (textureCount - offset) * 2u, vertexOffset, 0u);

        textureCount = 0;
    }

    public void Add(
        Texture texture, Sampler sampler, Vector2 position, Matrix3x2 transform,
        FlipMode flipMode = FlipMode.None, float layerDepth = 0) 
    {
        Add(new SpriteTexture(texture), texture, sampler, position, transform, flipMode, layerDepth);
    }

    public void Add(
        SpriteTexture sTexture, Texture texture, Sampler sampler, Vector2 position, Matrix3x2 transform,
        FlipMode flipMode = FlipMode.None, float layerDepth = 0) 
    {
        if (texture.IsDisposed) 
        {
            throw new System.ObjectDisposedException(nameof(texture));
        }

        if (textureCount == textures.Length) 
        {
            int maxTextures = (int)(textureCount + 2048);
            System.Array.Resize(ref textures, maxTextures);
            System.Array.Resize(ref vertices, vertices.Length + textures.Length * 4);

            indices = GenerateIndexArray((uint)(textures.Length * 6));

            vertexBuffer.Dispose();
            vertexBuffer = Buffer.Create<PositionColorTextureVertex>(
                device, BufferUsageFlags.Vertex, (uint)vertices.Length);

            indexBuffer.Dispose();
            indexBuffer = Buffer.Create<uint>(
                device, BufferUsageFlags.Index, (uint)vertices.Length
            );
        }

        textures[textureCount].Texture = texture;
        textures[textureCount].Sampler = sampler;

        float width = sTexture.Source.W;
        float height = sTexture.Source.H;
        
        var topLeft = new Vector2(position.X, position.Y);
        var topRight = new Vector2(position.X + width, position.Y);
        var bottomLeft = new Vector2(position.X, position.Y + height);
        var bottomRight = new Vector2(position.X + width, position.Y + height);

        var vertexOffset = textureCount * 4;

        vertices[vertexOffset].Position = new Vector3(Vector2.Transform(topLeft, transform), layerDepth);
        vertices[vertexOffset + 1].Position = new Vector3(Vector2.Transform(bottomLeft, transform), layerDepth);
        vertices[vertexOffset + 2].Position = new Vector3(Vector2.Transform(topRight, transform), layerDepth);
        vertices[vertexOffset + 3].Position = new Vector3(Vector2.Transform(bottomRight, transform), layerDepth);

        vertices[vertexOffset].Color = Color.White;
        vertices[vertexOffset + 1].Color = Color.White;
        vertices[vertexOffset + 2].Color = Color.White;
        vertices[vertexOffset + 3].Color = Color.White;

        vertices[vertexOffset].TexCoord = sTexture.UV.TopLeft;
        vertices[vertexOffset + 1].TexCoord = sTexture.UV.BottomLeft;
        vertices[vertexOffset + 2].TexCoord = sTexture.UV.TopRight;
        vertices[vertexOffset + 3].TexCoord = sTexture.UV.BottomRight;

        var flipByte = (byte)(flipMode & (FlipMode.Horizontal | FlipMode.Vertical));
        vertices[vertexOffset].TexCoord.X = CornerOffsetX[0 ^ flipByte] * sTexture.UV.Dimensions.X + sTexture.UV.Position.X;
        vertices[vertexOffset].TexCoord.Y = CornerOffsetY[0 ^ flipByte] * sTexture.UV.Dimensions.Y + sTexture.UV.Position.Y;
        vertices[vertexOffset + 1].TexCoord.X = CornerOffsetX[1 ^ flipByte] * sTexture.UV.Dimensions.X + sTexture.UV.Position.X;
        vertices[vertexOffset + 1].TexCoord.Y = CornerOffsetY[1 ^ flipByte] * sTexture.UV.Dimensions.Y + sTexture.UV.Position.Y;
        vertices[vertexOffset + 2].TexCoord.X = CornerOffsetX[2 ^ flipByte] * sTexture.UV.Dimensions.X + sTexture.UV.Position.X;
        vertices[vertexOffset + 2].TexCoord.Y = CornerOffsetY[2 ^ flipByte] * sTexture.UV.Dimensions.Y + sTexture.UV.Position.Y;
        vertices[vertexOffset + 3].TexCoord.X = CornerOffsetX[3 ^ flipByte] * sTexture.UV.Dimensions.X + sTexture.UV.Position.X;
        vertices[vertexOffset + 3].TexCoord.Y = CornerOffsetY[3 ^ flipByte] * sTexture.UV.Dimensions.Y + sTexture.UV.Position.Y;
        textureCount++;
    }

    public void Dispose()
    {
        vertexBuffer.Dispose();
        indexBuffer.Dispose();
    }
}


[StructLayout(LayoutKind.Sequential)]
public struct PositionColorVertex(Vector3 position, Color color) : IVertexType
{
    public Vector3 Position = position;
    public Color Color = color;

    public static VertexElementFormat[] Formats { get; } = new VertexElementFormat[2] 
    {
        VertexElementFormat.Vector3,
        VertexElementFormat.Color
    };
}

[StructLayout(LayoutKind.Sequential)]
public struct PositionColorTextureVertex(Vector3 position, Color color, Vector2 texCoord) : IVertexType
{
    public Vector3 Position = position;
    public Color Color = color;
    public Vector2 TexCoord = texCoord;

    public static VertexElementFormat[] Formats { get; } = [
        VertexElementFormat.Vector3,
        VertexElementFormat.Color,
        VertexElementFormat.Vector2
    ];
}

public struct TransformVertexUniform(Matrix4x4 viewProjection) 
{
    public Matrix4x4 ViewProjection = viewProjection;
}