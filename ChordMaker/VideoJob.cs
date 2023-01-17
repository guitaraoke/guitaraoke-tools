using SixLabors.ImageSharp;

public class VideoJob {

	public VideoJob(string sourceFilePath, bool draft) {
		
		SourceFilePath = sourceFilePath;
		Draft = draft;
		var bareFileName = Path.GetFileNameWithoutExtension(sourceFilePath);
		var tokens = bareFileName.Split(" - ");
		Artist = tokens[0];
		Title = tokens[1];
	}

	public Color OverlayColor = Color.FromRgba(0, 0, 0, 127);

	public string SourceFilePath { get; set; }
	public bool Draft { get; }
	public string Artist { get; init; }
	public string Title { get; init; }
	public string OutputFilePath => FilePathWrangler.GetOutputFilePath(SourceFilePath);
	public string OverlayFilePath => FilePathWrangler.GetOverlayFilePath(SourceFilePath);
	public string ChordTimesFilePath => FilePathWrangler.GetChordTimesTextFilePath(SourceFilePath);
}
