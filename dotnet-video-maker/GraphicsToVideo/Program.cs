using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Extend;
using FFMpegCore.Pipes;

namespace GraphicsToVideo;

public static class Program {
    public static void Main() {
        // mpeg4 (.mp4) worked in media player but not chrome
        // libx264 for (.mp4) but in chrome but not media player
        // libvpx-vp9 for (.webm) worked everywhere

        string outputPath = Path.GetFullPath("output.mp4");

        var frames = CreateFramesSD(200, 400, 300);
        var videoFramesSource = new RawVideoPipeSource(frames) { FrameRate = 24 };
        var success = FFMpegArguments
            .FromPipeInput(videoFramesSource)
            .OutputToFile(outputPath, overwrite: true, options => options.WithVideoCodec("libx264"))// vpx-vp9"))
            .ProcessSynchronously();

        Console.WriteLine($"Saved: {outputPath}");
    }

    static IEnumerable<IVideoFrame> CreateFramesSD(int count, int w, int h) {
        for (int i = 0; i < count; i++) {
            Console.CursorLeft = 0;
            Console.Write($"Encoding: frame {i + 1} of {count} ...");
            if (i == count - 1)
                Console.WriteLine("");
            yield return new FakeVideoFrame((byte) (i % 255));
        }
    }
}

public class FakeVideoFrame : IVideoFrame {
    private byte value;

    public int Width => 1280;

    public int Height => 720;

    public string Format => "rgba";

    public FakeVideoFrame(byte value) {
        this.value = value;
    }

    public void Serialize(Stream pipe) {
        byte[] bytes = new byte[Width * Height * 4];
        for (var i = 0; i < bytes.Length; i++) bytes[i] = value;
        pipe.Write(bytes, 0, bytes.Length);
    }

    public async Task SerializeAsync(Stream pipe, CancellationToken token) {
        byte[] bytes = new byte[Width * Height * 4];
        for (var i = 0; i < bytes.Length; i++) bytes[i] = value;
        await pipe.WriteAsync(bytes, 0, bytes.Length);
    }
}