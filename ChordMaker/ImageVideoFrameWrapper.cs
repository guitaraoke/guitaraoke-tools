using System.Runtime.CompilerServices;
using FFMpegCore.Pipes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ChordMaker;
public class ImageVideoFrameWrapper<T> : IVideoFrame, IDisposable where T : unmanaged, IPixel<T> {
    public int Width => Source.Width;
    public int Height => Source.Height;
    public string Format => "rgba";
    public Image<T> Source { get; private set; }
    public ImageVideoFrameWrapper(Image<T> source) {
        Source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public void Serialize(Stream stream) {
        byte[] pixelBytes = new byte[Source.Width * Source.Height * Unsafe.SizeOf<Rgba32>()];
        Source.CopyPixelDataTo(pixelBytes);
        stream.Write(pixelBytes, 0, pixelBytes.Length);
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