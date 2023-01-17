using System;
using SixLabors.ImageSharp;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using FFMpegCore.Pipes;
using SixLabors.ImageSharp.PixelFormats;

public class ImageVideoFrameWrapper<T> : IVideoFrame, IDisposable where T : unmanaged, IPixel<T> {
    public int Width => Source.Width;

    public int Height => Source.Height;

    public string Format => "rgba";

    public Image<T> Source { get; private set; }

    public ImageVideoFrameWrapper(Image<T> source)  {
        Source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public void Serialize(Stream stream) {
        byte[] pixelBytes = new byte[Source.Width * Source.Height * Unsafe.SizeOf<Rgba32>()];
        Source.CopyPixelDataTo(pixelBytes);
        stream.Write(pixelBytes, 0, pixelBytes.Length);
        // var data = Source.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, Source.PixelFormat);
        //
        // try {
        //     var buffer = new byte[data.Stride * data.Height];
        //     Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
        //     stream.Write(buffer, 0, buffer.Length);
        // }
        // finally {
        //     Source.UnlockBits(data);
        // }
    }

    public async Task SerializeAsync(Stream stream, CancellationToken token) {
        var pixelBytes = new byte[Source.Width * Source.Height * Unsafe.SizeOf<Rgba32>()];
        Source.CopyPixelDataTo(pixelBytes);
        await stream.WriteAsync(pixelBytes, 0, pixelBytes.Length);
    }

    public void Dispose() { 
        Source.Dispose();
    }
}
