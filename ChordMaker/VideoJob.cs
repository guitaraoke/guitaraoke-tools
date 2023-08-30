using SixLabors.ImageSharp;

public class VideoJob {

	public VideoJob(string sourceFilePath) {

		SourceFilePath = sourceFilePath;
		var bareFileName = Path.GetFileNameWithoutExtension(sourceFilePath);
		var tokens = bareFileName.Split(" - ");
		Artist = tokens[0];
		Title = tokens[1];
	}

	public Color OverlayColor = Color.FromRgba(0, 0, 0, 127);
	public string SourceFilePath { get; set; }
	public string Artist { get; init; }
	public string Title { get; init; }
	public string OutputFilePath => FilePathWrangler.GetOutputFilePath(SourceFilePath);
	public string OverlayFilePath => FilePathWrangler.GetOverlayFilePath(SourceFilePath);
	public string ChordTimesFilePath => FilePathWrangler.GetChordTimesTextFilePath(SourceFilePath);
}
