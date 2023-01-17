using FFMpegCore;
using FFMpegCore.Extend;
using FFMpegCore.Pipes;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing;

// FontCollection collection = new();
// var family = collection.Add("fonts/FreeSans.ttf");
// var font = family.CreateFont(12);

static IEnumerable<IVideoFrame> CreateFramesSD(int count, int width, int height) {
    DrawingOptions options = new() {
        GraphicsOptions = new() {
            ColorBlendingMode = PixelColorBlendingMode.Multiply
        }
    };
    IBrush brush = Brushes.Horizontal(Color.FromRgba(255,255,0,127), Color.FromRgba(0,0,255,63));
    IPen pen = Pens.DashDot(Color.Green, 5);
    
    for (var i = 0; i < count; i++) {
        using Image<Rgba32> image = new(width, height, Color.Transparent);
        IPath yourPolygon = new Star(x: 640.0f, y: 36.0f, prongs: 5, innerRadii: 100.0f, outerRadii: (float) (i+1));
        image.Mutate(x => x.Fill(options, brush, yourPolygon)
            .Draw(options, pen, yourPolygon));
        using ImageVideoFrameWrapper<Rgba32> wrapper = new(image);
        Console.WriteLine($"Frame {i}");
        yield return wrapper;
    }
}

var frames = CreateFramesSD(count: 24 * 30, width: 1280, height: 720);
var videoFramesSource = new RawVideoPipeSource(frames) { FrameRate = 24 };
var success = FFMpegArguments
    .FromPipeInput(videoFramesSource)
    .OutputToFile("output.webm", overwrite: true, options => options.WithVideoCodec("libvpx-vp9"))
    .ProcessSynchronously();
Console.WriteLine("Done");
