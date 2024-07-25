using System;
using RefreshCS;

namespace Riateu.Graphics;

public class CopyPass : IPassPool
{
    public IntPtr Handle { get; internal set; }

    public void UploadToBuffer(in TransferBufferLocation source, in BufferRegion destination, bool cycle) 
    {
        Refresh.Refresh_UploadToBuffer(Handle, source.ToSDLGpu(), destination.ToSDLGpu(), cycle ? 1 : 0);
    }

    public void UploadToBuffer(TransferBuffer source, RawBuffer destination, bool cycle) 
    {
        UploadToBuffer(
            new TransferBufferLocation(source),
            new BufferRegion(destination, 0, destination.Size),
            cycle
        );
    }

	public void UploadToTexture(TransferBuffer source, Texture destination, bool cycle) 
    {
		UploadToTexture(new TextureTransferInfo(source), new TextureRegion(destination), cycle);
	}

	public void UploadToTexture(in TextureTransferInfo source, in TextureRegion destination, bool cycle) 
    {
		Refresh.Refresh_UploadToTexture(Handle, source.ToSDLGpu(), destination.ToSDLGpu(), cycle ? 1 : 0);
	}

    public void Obtain(nint handle)
    {
        Handle = handle;
    }

    public void Reset()
    {
        Handle = IntPtr.Zero;
    }
}