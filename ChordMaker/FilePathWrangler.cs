public class Folder {
	public const string SOURCES = "1 Sources";
	public const string MIXES = "2 Mixes";
	public const string CHORDS = "3 Chords";
	public const string OVERLAYS = "4 Overlays";
	public const string OUTPUT = "5 Output";
}

public class FilePathWrangler {


	private const string RootPath = "D:\\Dropbox\\Creative\\Guitaraoke";

	private static string QualifyFileName(string folder, string filePath, string suffix)
		=> Path.Combine(RootPath, folder, Path.GetFileNameWithoutExtension(filePath) + " - " + suffix);

	public static string GetChordNamesTextFilePath(string filePath)
		=> QualifyFileName(Folder.CHORDS, filePath, "chord names.txt");

	public static string GetChordTimesTextFilePath(string filePath)
		=> QualifyFileName(Folder.CHORDS, filePath, "chord times.txt");

	public static string GetOverlayFilePath(string filePath)
		=> QualifyFileName(Folder.OVERLAYS, filePath, "chords.webm");

	public static string GetOutputFilePath(string filePath)
		=> QualifyFileName(Folder.OUTPUT, filePath, "with chords.mp4");
}
