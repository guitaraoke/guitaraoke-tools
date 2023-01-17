public class Folder {
	public const string MIXES = "2 Mixes";
	public const string CHORDS = "3 Chords";
	public const string OVERLAYS = "4 Overlays";
	public const string OUTPUT = "5 Output";
}

public class FilePathWrangler {

	public string RootPath { get; }

	public FilePathWrangler(string rootPath) => RootPath = rootPath;

	private string QualifyFileName(string folder, string filePath, string suffix)
		=> Path.Combine(RootPath, folder, Path.GetFileNameWithoutExtension(filePath) + " - " + suffix);


	public string GetChordNamesTextFilePath(string filePath)
		=> QualifyFileName(Folder.CHORDS, filePath, "chord names.txt");

	public string GetChordTimesTextFilePath(string filePath)
		=> QualifyFileName(Folder.CHORDS, filePath, "chord times.txt");

	public string GetOverlayFilePath(string filePath)
		=> QualifyFileName(Folder.OVERLAYS, filePath, "chords.webm");

	public string GetOutputFilePath(string filePath)
		=> QualifyFileName(Folder.OUTPUT, filePath, "with chords.mp4");
}
