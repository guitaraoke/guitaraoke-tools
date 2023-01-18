using System.Diagnostics;
using ChordMaker;
using FFMpegCore;
using FFMpegCore.Pipes;
using SixLabors.ImageSharp;
using FFProbe = ChordMaker.ffmpeg.FFProbe;

public class ChordMakerEngine {
	const int FPS_MULTIPLIER = 2;

	public async Task MakeChords(string videoPath, bool draft, float duration) {

		var stats = await FFProbe.GetVideoStats(videoPath);
		var job = new VideoJob(videoPath, draft);

		var fps = (float) Math.Round(stats.FPS, draft ? 1 : FPS_MULTIPLIER);
		var width = draft ? 640 : stats.Width;
		var height = draft ? 360 : stats.Height;

		Console.WriteLine($"Artist: {job.Artist}");
		Console.WriteLine($"Title: {job.Title}");
		Console.WriteLine($"Size: {width}x{height}");
		Console.WriteLine($"FPS: {fps}");
		Console.WriteLine($"r_frame_rate: {stats.RFrameRate}");
		Console.WriteLine(String.Empty.PadRight(40, '-'));
		Console.WriteLine($"Source:  {videoPath}");
		Console.WriteLine($"Chords:  {job.ChordTimesFilePath}");
		Console.WriteLine($"Overlay: {job.OverlayFilePath}");
		Console.WriteLine($"Output:  {job.OutputFilePath}");

		var chords = (await File.ReadAllLinesAsync(job.ChordTimesFilePath)).Select(line => new Chord(line))
			.Where(chord => chord.Time >= 0)
			.ToList();

		for (var i = 1; i < chords.Count; i++) {
			chords[i - 1].Duration = chords[i].Time - chords[i - 1].Time;
		}

		var shortestChord = chords.Where(chord => chord.Duration > 0).OrderBy(chord => chord.Duration).First();
		const float MIN_CHORD_WIDTH = 160f;
		const int SPEED = 120;
		const float LINE_SPLIT_THRESHOLD = MIN_CHORD_WIDTH / SPEED;

		for (var i = 1; i < chords.Count; i++) {
			if (!(chords[i - 1].Duration < LINE_SPLIT_THRESHOLD)) continue;
			chords[i++].TextLine++;
		}

		duration = Math.Min(duration, stats.Duration);

		Console.WriteLine($"Shortest chord: {shortestChord.Time} {shortestChord.Name} {shortestChord.Duration}");
		Console.WriteLine($"Speed: {SPEED}");
		Console.WriteLine($"Line Split Threshold: {LINE_SPLIT_THRESHOLD}");
		Console.WriteLine($"Duration: {duration}");

		var frameCount = (int) (duration * fps * FPS_MULTIPLIER);
		var fm = new FrameMaker(width, height, fps * FPS_MULTIPLIER);
		var frames = fm.CreateFrames(frameCount, chords, SPEED, job.OverlayColor);

		var videoFramesSource = new RawVideoPipeSource(frames) { FrameRate = fps * FPS_MULTIPLIER };
		var sw = new Stopwatch();
		sw.Start();
		FFMpegArguments
			.FromPipeInput(videoFramesSource)
			.OutputToFile($"{job.OverlayFilePath}", overwrite: true, options => options.WithVideoCodec("libvpx-vp9"))
			.ProcessSynchronously();
		sw.Stop();
		Console.WriteLine($"Generated {frameCount} frames in {sw.ElapsedMilliseconds} ms");

		var startInfo = new ProcessStartInfo();
		startInfo.CreateNoWindow = false;
		startInfo.UseShellExecute = false;
		startInfo.FileName = @"ffmpeg";
		var ffmpegArguments = $"-i \"{job.SourceFilePath}\" -c:v libvpx-vp9"
		                      + $" -r {fps * FPS_MULTIPLIER}"
		                      + $" -i \"{job.OverlayFilePath}\""
		                      + $" -metadata artist=\"{job.Artist}\""
		                      + $" -metadata title=\"{job.Title} (Guitaraoke Backing)\""
		                      + $" -metadata album=\"Guitaraoke\""
		                      + $" -filter_complex \"[0:0][1:0]overlay\""
		                      + $" -c:v libx264"
		                      + $" -b:v 3200k "
		                      + $" -y ";
		if (duration < 60) ffmpegArguments += $" -ss 00:00:00 -t 00:00:{duration}";
		ffmpegArguments += $" \"{job.OutputFilePath}\"";

		startInfo.Arguments = ffmpegArguments;
		Console.WriteLine(ffmpegArguments);
		using (var ffmpeg = Process.Start(startInfo)) ffmpeg.WaitForExit();

		Console.WriteLine("Done");
		var color = Console.ForegroundColor;
		Console.ForegroundColor = ConsoleColor.Magenta;
		Console.WriteLine(@"                                                                                                                                                                                   
#############               #########       ########        ########  ######################   ### 
#::::::::::::###          ##:::::::::##     #:::::::#       #::::::#  #::::::::::::::::::::#  ##:##
#:::::::::::::::##      ##:::::::::::::##   #::::::::#      #::::::#  #::::::::::::::::::::#  #:::#
###:::::#####:::::#    #:::::::###:::::::#  #:::::::::#     #::::::#  ##::::::#########::::#  #:::#
  #:::::#    #:::::#   #::::::#   #::::::#  #::::::::::#    #::::::#    #:::::#       ######  #:::#
  #:::::#     #:::::#  #:::::#     #:::::#  #:::::::::::#   #::::::#    #:::::#               #:::#
  #:::::#     #:::::#  #:::::#     #:::::#  #:::::::#::::#  #::::::#    #::::::##########     #:::#
  #:::::#     #:::::#  #:::::#     #:::::#  #::::::# #::::# #::::::#    #:::::::::::::::#     #:::#
  #:::::#     #:::::#  #:::::#     #:::::#  #::::::#  #::::#:::::::#    #:::::::::::::::#     #:::#
  #:::::#     #:::::#  #:::::#     #:::::#  #::::::#   #:::::::::::#    #::::::##########     #:::#
  #:::::#     #:::::#  #:::::#     #:::::#  #::::::#    #::::::::::#    #:::::#               ##:##
  #:::::#    #:::::#   #::::::#   #::::::#  #::::::#     #:::::::::#    #:::::#       ######   ### 
###:::::#####:::::#    #:::::::###:::::::#  #::::::#      #::::::::#  ##::::::########:::::#       
#:::::::::::::::##      ##:::::::::::::##   #::::::#       #:::::::#  #::::::::::::::::::::#   ### 
#::::::::::::###          ##:::::::::##     #::::::#        #::::::#  #::::::::::::::::::::#  ##:##
#############               #########       ########         #######  ######################   ###");
		Console.ForegroundColor = color;
		Console.WriteLine();
		Console.WriteLine();
		Console.WriteLine($"{job.OutputFilePath}");
	}

}
