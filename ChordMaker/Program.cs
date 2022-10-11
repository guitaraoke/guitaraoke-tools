using System.Diagnostics;
using ChordMaker;
using FFMpegCore;
using FFMpegCore.Pipes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SixLabors.ImageSharp;

int fpsMultiplier = 2;
float duration = Int32.MaxValue;
if (args.Length > 1) float.TryParse(args[1], out duration);
var draft = (args.Length > 2 && args[2] == "draft");
const int MAX_CHORD_SPEED = 120; // Maximum chord speed in pixels per second
const int MIN_CHORD_SPEED = 80; //
string videoPath = args[0];
var stats = GetVideoStats(videoPath);
var FPS = (float)Math.Round((double)stats.FPS, (draft ? 1 : fpsMultiplier));
var width = draft ? 320 : stats.Width;
var height = draft ? 180 : stats.Height;
var directory = System.IO.Path.GetDirectoryName(videoPath);
var filename = System.IO.Path.GetFileNameWithoutExtension(videoPath);

string chordListFilePath = Path.Combine(directory, "_chords", filename + " - chord times.txt");
string chordWebmFilePath = Path.Combine(directory, "_chords", "output", filename + " - chords.webm");
string outputFilePath = Path.Combine(directory, "_chords", "output", filename + " - with chords.mp4");

var lines = File.ReadAllLines(chordListFilePath);
var bareFileName = System.IO.Path.GetFileNameWithoutExtension(videoPath);
var tokens = bareFileName.Split(" - ");
var artist = tokens[0];
var title = tokens[1];
var (r, g, b) = GetColorForSong(bareFileName);
var overlayColor = Color.FromRgba(r, g, b, 127);
Console.WriteLine($"Artist: {artist}");
Console.WriteLine($"Title: {title}");
Console.WriteLine($"Color: ({r},{g},{b})");
Console.WriteLine($"Size: {width}x{height}");
Console.WriteLine($"FPS: {FPS}");
Console.WriteLine($"r_frame_rate: {stats.RFrameRate}");
Console.WriteLine(String.Empty.PadRight(40, '-'));
Console.WriteLine($"Source:  {videoPath}");
Console.WriteLine($"Chords:  {chordListFilePath}");
Console.WriteLine($"Overlay: {chordWebmFilePath}");
Console.WriteLine($"Output:  {outputFilePath}");

var chords = File.ReadAllLines(chordListFilePath).Select(line => new Chord(line)).ToList();
for (var i = 1; i < chords.Count; i++) {
    chords[i - 1].Duration = chords[i].Time - chords[i - 1].Time;
}

var shortestChord = chords.Where(chord => chord.Duration > 0).OrderBy(chord => chord.Duration).First();
var minChordWidth = 160f;
var speed = minChordWidth / shortestChord.Duration;
if (speed > MAX_CHORD_SPEED) speed = MAX_CHORD_SPEED; 
if (speed < MIN_CHORD_SPEED) speed = MIN_CHORD_SPEED;

float lineSplitThreshold = minChordWidth/speed;

for(var i = 1; i < chords.Count; i++) {
    if (chords[i-1].Duration < lineSplitThreshold) {
        chords[i].TextLine++;
        i++;
    }
}

Console.WriteLine($"Shortest chord: {shortestChord.Time} {shortestChord.Name} {shortestChord.Duration}");
Console.WriteLine($"Speed: {speed}");
Console.WriteLine($"Line Split Threshold: {lineSplitThreshold}");

duration = Math.Min((float)duration, stats.Duration);

Console.WriteLine($"Duration: {duration}");

var frameCount = (int)(duration * FPS * fpsMultiplier); // stats.Frames * 2; //  (int) ((chords.Max(pair => pair.Time) + 5) * FPS);

var fm = new FrameMaker {    
    Width = width,
    Height = height,
    Fps = FPS * fpsMultiplier
};

var frames = fm.CreateFramesSD(frameCount, chords, speed, overlayColor);
// var frames = fm.CreateDebugFrames(frameCount);
var videoFramesSource = new RawVideoPipeSource(frames) { FrameRate = FPS * fpsMultiplier };
var sw = new Stopwatch();
sw.Start();
FFMpegArguments
    .FromPipeInput(videoFramesSource)
    .OutputToFile($"{chordWebmFilePath}", overwrite: true, options => options.WithVideoCodec("libvpx-vp9"))
    // .OutputToFile($"overlay.webm", overwrite: true, options => options.WithVideoCodec("libvpx-vp9"))
    .ProcessSynchronously();
sw.Stop();
Console.WriteLine($"Generated {frameCount} frames in {sw.ElapsedMilliseconds} ms");
// return;

ProcessStartInfo startInfo = new ProcessStartInfo();
startInfo.CreateNoWindow = false;
startInfo.UseShellExecute = false;
startInfo.FileName = @"ffmpeg";
var srcSetpts = $"{stats.RFrameRateDenominator}*N*{fpsMultiplier}/{stats.RFrameRateNumerator}/TB";
var ovlSetpts = $"{stats.RFrameRateDenominator}*N/{stats.RFrameRateNumerator}/TB";
var ffmpegArguments = $"-i \"{videoPath}\" -c:v libvpx-vp9"
    + $" -r { FPS * fpsMultiplier }" // double up framerate of source video
    + $" -i \"{chordWebmFilePath}\""
    //+ $" -r { FPS * FPS_MULTIPLIER }" // double up framerate of source video
    + $" -metadata artist=\"{artist}\""
    + $" -metadata title=\"{title} (Guitaraoke Backing)\""
    + $" -metadata album=\"Guitaraoke\""
    // + $@" -filter_complex ""
    //     [0:0]setpts={srcSetpts}[src];
    //     [1:0]setpts={ovlSetpts}[ovl];
    //     [src][ovl]overlay
    // """
    + $@" -filter_complex ""[0:0][1:0]overlay"""
    + $" -c:v libx264"
    + $" -b:v 3200k "
    + $" -y ";
    if (duration < 60) ffmpegArguments += $" -ss 00:00:00 -t 00:00:{duration}"; 
//    + $" composite.mp4";
    ffmpegArguments += $" \"{outputFilePath}\"";

startInfo.Arguments = ffmpegArguments;
Console.WriteLine(ffmpegArguments);
using (Process? ffmpeg = Process.Start(startInfo)) {
    if (ffmpeg != null) ffmpeg.WaitForExit();
}

Console.WriteLine("Done");

static ChordMaker.ffmpeg.FFProbeStream GetVideoStats(string videoFilePath) {
    JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
        ContractResolver = new DefaultContractResolver {
            NamingStrategy = new SnakeCaseNamingStrategy()
        }
    };
    ProcessStartInfo startInfo = new ProcessStartInfo();
    startInfo.CreateNoWindow = false;
    startInfo.UseShellExecute = false;
    startInfo.FileName = @"ffprobe";
    startInfo.RedirectStandardOutput = true;
    var ffprobeArguments = $"-v quiet -print_format json -show_format -show_streams "
        + $" \"{videoFilePath}\"";

    startInfo.Arguments = ffprobeArguments;
    Console.WriteLine($"ffprobe {ffprobeArguments}");
    using (Process? ffprobe = Process.Start(startInfo)) {
        if (ffprobe != null) {
            ffprobe.WaitForExit();
            var json = ffprobe.StandardOutput.ReadToEnd();
            Console.WriteLine(json);
            var result = JsonConvert.DeserializeObject<FFProbeResult>(json);
            if (result != null) {
                return result.Streams.FirstOrDefault(s => s.CodecType == "video");                
            }
        }
    }
    throw new Exception($"Couldn't read FPS from {videoFilePath}");
}


static (byte, byte, byte) GetColorForSong(string bareFileName) {
    Console.WriteLine($"finding colour for {bareFileName}");
    var colors = File.ReadAllLines("colors.txt");
    var color = colors.FirstOrDefault(c => c.StartsWith(bareFileName, StringComparison.OrdinalIgnoreCase));
    if (color == null) return (80, 80, 80);
    var tokens = color.Split(":");
    if (tokens.Length < 2) return (80, 80, 80);
    var rgb = tokens[1].Split(",").Select(byte.Parse).ToList();
    if (rgb.Count < 3) return (80, 80, 80);
    var r = rgb[0];
    var g = rgb[1];
    var b = rgb[2];
    return (r, g, b);
}
