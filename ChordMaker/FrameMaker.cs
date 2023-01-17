using ChordMaker;
using FFMpegCore.Pipes;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

public class FrameMaker {

	private const float PLAYHEAD_POSITION_AS_PROPORTION = 8f;

	public int Width { get; init; }
    public int Height { get; init; }
    public float Fps { get; init; }

    public FrameMaker(int width, int height, float fps) {
	    Width = width;
	    Height = height;
	    Fps = fps;
    }

    private FontFamily family = new FontCollection().Add(System.IO.Path.Combine("fonts", "ttf", "FreeSansBold.ttf"));

    private readonly DrawingOptions options = new() {
        GraphicsOptions = new() {
            ColorBlendingMode = PixelColorBlendingMode.Normal
        }
    };

    readonly SolidBrush white = Brushes.Solid(Color.White);

    public IEnumerable<IVideoFrame> CreateDebugFrames(int count) {
        var point = new PointF(20, 20);
        var font = family.CreateFont(32);
        using Image<Rgba32> emptyFrame = new(Width, Height, Color.Transparent);
        for (var frameNumber = 0; frameNumber < count; frameNumber++) {
            Console.WriteLine($"Debug Frame {frameNumber}");
            using var frame = emptyFrame.Clone(x => x.DrawText(frameNumber.ToString(), font, white, point));
            using ImageVideoFrameWrapper<Rgba32> wrapper = new(frame);
            yield return wrapper;
        }
    }

    public IEnumerable<IVideoFrame> CreateFrames(int frameCount, List<Chord> chords, float speed, Color overlayColor) {

        var chordList = chords
            .GroupBy(c => c.FullPrettyNameWithoutBass)
            .OrderByDescending(group => group.Sum(c => c.Duration))
            .Select(c => c.First())
	        .ToList();

        Console.WriteLine("Chords:");
        foreach (var freq in chordList) Console.WriteLine($"* {freq.FullPrettyNameWithoutBass}");

        var howManyLines = chords.Max(chord => chord.TextLine) + 1;

        var spf = 1f / Fps; // seconds per frame

        var playheadPosition = Width / PLAYHEAD_POSITION_AS_PROPORTION;
        var chordBarHeight = Height * howManyLines switch {
            2 => 0.2f,
            _ => 0.16f
        };


        var fontSize = chordBarHeight * howManyLines switch {
            2 => 0.4f,
            _ => 0.7f
        };

        var chordBarTop = Height * 0.8f;
        var largeFont = family.CreateFont(fontSize);
        TextOptions largeTextOptions = new(largeFont);
        var smallFont = family.CreateFont(fontSize * 0.7f);
        var tinyFont = family.CreateFont(12f);
        TextOptions smallTextOptions = new(smallFont);
        TextOptions tinyFontOptions = new(tinyFont);
        var chordBarTopPadding = howManyLines switch {
            2 => (chordBarHeight - fontSize) / 9,
            _ => (chordBarHeight - fontSize) / 3
        };
        var chordTextTop = chordBarTop + chordBarTopPadding;
        var chordBarColor = Brushes.Solid(Color.FromRgba(0, 0, 0, 200));
        var overlayBrush = Brushes.Solid(overlayColor);

        var chordBar = new RectangularPolygon(0, chordBarTop, Width, chordBarHeight);
        var chordOverlay = new RectangularPolygon(0, chordBarTop, playheadPosition, chordBarHeight);
        var playhead = new RectangularPolygon(playheadPosition - 2f, chordBarTop - 1f, 4, chordBarHeight + 2f);
        var journeyTime = Width / speed;
        var timeFromPlayheadToZero = (Width / PLAYHEAD_POSITION_AS_PROPORTION) / speed;
        Console.WriteLine($"Time from playhead to zero: {timeFromPlayheadToZero}");

        var releaseTime = 2 * timeFromPlayheadToZero;

        // Chords on screen are actually a tiny bit earlier than real time
        // because brains are weird and are used to seeing things before we hear them.
        const float CHORD_NUDGE_IN_PIXELS = 48f;

        using Image<Rgba32> emptyFrame = new(Width, Height, Color.Transparent);
        emptyFrame.Mutate(x => x
            .Fill(options, chordBarColor, chordBar)
            .Fill(options, overlayBrush, chordOverlay)
            .Fill(options, white, playhead)
        );
        ImageVideoFrameWrapper<Rgba32> emptyFrameWrapper = new(emptyFrame);
        var assemblyInfo = getAssemblyInfo();
        var assemblyInfoRect = MeasureText(assemblyInfo, tinyFontOptions);
        var assemblyInfoPoint = new PointF(((float) Width) - assemblyInfoRect.Width, ((float) Height) - assemblyInfoRect.Height);
        var stampFrame = emptyFrame.Clone(x => x.DrawText(assemblyInfo, tinyFont, white, assemblyInfoPoint));
        for (var frame = 0; frame < frameCount; frame++) {
            var echo = (frame % 10) == 0;
            if (echo) {
                Console.WriteLine();
                Console.Write($"Frame {frame}/{frameCount} ({100 * frame / frameCount}%): ");
            }
            var time = frame * spf;
            var firstFrameTime = time - releaseTime;
            var finalFrameTime = time + journeyTime;
            var frameChords = chords.Where(c => c.Time >= firstFrameTime && c.Time <= finalFrameTime).ToList();
            const int FADE = 5;
            if (time < FADE || frameChords.Any() || frame < Fps) {
                using var image = (frame < Fps ? stampFrame : emptyFrame).Clone();
                if (time < FADE) { // Draw list of chords for FADE seconds
                    var alpha = (byte) (Math.Sqrt(((FADE - time) / FADE)) * 255);
                    var fadingWhite = Brushes.Solid(Color.FromRgba(255, 255, 255, alpha));
                    var fadingBlack = Brushes.Solid(Color.FromRgba(0, 0, 0, alpha));
                    var label = "Chords: " + String.Join(", ", chordList.Select(c => c.FullPrettyNameWithoutBass).ToArray());
                    var labelRect = MeasureText(label, largeTextOptions);
                    var labelFont = largeFont;
                    if (labelRect.Width > (Width * 0.9)) {
                        labelRect = MeasureText(label, smallTextOptions);
                        labelFont = smallFont;
                    }
                    var point = new PointF((Width - labelRect.Width) / 2.0f, Height / 20);
                    var padding = 10f;
                    var fillRect = new RectangularPolygon(0, point.Y - padding, Width, labelRect.Height + padding);
                    image.Mutate(x => x.Fill(options, fadingBlack, fillRect));
                    image.Mutate(x => x.DrawText(label, labelFont, fadingWhite, point));
                }
                
                if (frameChords.Any() || frame < Fps) {
                    foreach (var chord in frameChords) {
                        var offset = playheadPosition + (speed * (chord.Time - time)) - CHORD_NUDGE_IN_PIXELS;
                        var point = new PointF(offset, chordTextTop + fontSize * chord.TextLine);
                        if (echo) {
                            Console.Write($" {chord.Name}");
                            if (chord.HasExtraBit) Console.Write(chord.ExtraBit);
                        }
                        image.Mutate(x => x.DrawText(chord.PrettyName, largeFont, white, point));
                        if (chord.HasExtraBit) {
                            var largeRect = MeasureText(chord.PrettyName, largeTextOptions);
                            var smallRect = MeasureText(chord.ExtraBit, smallTextOptions);
                            point.X += largeRect.Width;
                            point.Y += (largeRect.Height - (1.12f * smallRect.Height));
                            image.Mutate(x => x.DrawText(chord.PrettyExtraBit, smallFont, white, point));
                        }
                    }
                    image.Mutate(x => x
                        .Fill(options, overlayBrush, chordOverlay)
                        .Fill(options, white, playhead)
                    );
                }
                using ImageVideoFrameWrapper<Rgba32> wrapper = new(image);
                yield return wrapper;
            } else {
                if (echo) Console.Write($"<empty>");
                yield return emptyFrameWrapper;
            }
        }
    }

    private string getAssemblyInfo() {
        var file = new FileInfo(this.GetType().Assembly.Location);
        return $"{file.Name} {file.LastWriteTime:O}";
    }
    private Dictionary<TextOptions, Dictionary<string, FontRectangle>> textSizeCache = new();
    private FontRectangle MeasureText(string text, TextOptions options) {
        if (!textSizeCache.ContainsKey(options)) textSizeCache[options] = new();
        if (textSizeCache[options].ContainsKey(text)) return textSizeCache[options][text];
        var rect = TextMeasurer.Measure(text, options);
        textSizeCache[options].Add(text, rect);
        return rect;
    }
}
