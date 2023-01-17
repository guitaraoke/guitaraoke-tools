using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ChordMaker.ffmpeg;

public static class FFProbe {
	public static async Task<FFProbeStream> GetVideoStats(string videoFilePath) {
		JsonConvert.DefaultSettings = () => new() {
			ContractResolver = new DefaultContractResolver {
				NamingStrategy = new SnakeCaseNamingStrategy()
			}
		};
		var startInfo = new ProcessStartInfo {
			CreateNoWindow = false,
			UseShellExecute = false,
			FileName = @"ffprobe",
			RedirectStandardOutput = true
		};
		var arguments = $"-v quiet -print_format json -show_format -show_streams \"{videoFilePath}\"";
		startInfo.Arguments = arguments;
		Console.WriteLine($"ffprobe {arguments}");
		using var ffprobe = Process.Start(startInfo);
		if (ffprobe == null) throw new($"Couldn't read FPS from {videoFilePath}");
		var stdout = new StringBuilder();
		await ConsumeReader(ffprobe.StandardOutput, stdout);
		await ffprobe.WaitForExitAsync();
		var json = stdout.ToString();
		var result = JsonConvert.DeserializeObject<FFProbeResult>(json);
		return result?.Streams.FirstOrDefault(s => s.CodecType == "video") ?? throw new($"Couldn't read FPS from {videoFilePath}");
	}

	private static async Task ConsumeReader(TextReader reader, StringBuilder sb) {
		while (await reader.ReadLineAsync() is { } text) sb.AppendLine(text);
	}
}
