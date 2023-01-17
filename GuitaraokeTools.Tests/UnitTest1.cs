using Shouldly;

namespace GuitaraokeTools.Tests;

public class FilePathWranglerTests {
	[Fact]
	public void FileWranglerGetsChordNamesCorrect() {
		var original = "D:\\Dropbox\\Creative\\Guitaraoke\\2 Mixes\\Georgia Satellites - Battleship Chains - Guitaraoke.mp4";
		var expected = "D:\\Dropbox\\Creative\\Guitaraoke\\3 Chords\\Georgia Satellites - Battleship Chains - Guitaraoke - chord names.txt";
		var output = FilePathWrangler.GetChordNamesTextFilePath(original);
		output.ShouldBe(expected);
	}

	[Fact]
	public void FileWranglerGetsOverlayFileCorrect() {
		var original = "D:\\Dropbox\\Creative\\Guitaraoke\\2 Mixes\\Georgia Satellites - Battleship Chains - Guitaraoke.mp4";
		var expected = "D:\\Dropbox\\Creative\\Guitaraoke\\4 Overlays\\Georgia Satellites - Battleship Chains - Guitaraoke - chords.webm";
		var output = FilePathWrangler.GetOverlayFilePath(original);
		output.ShouldBe(expected);
	}


	[Fact]
	public void FileWranglerGetsOutputFileCorrect() {
		var original = "D:\\Dropbox\\Creative\\Guitaraoke\\2 Mixes\\Georgia Satellites - Battleship Chains - Guitaraoke.mp4";
		var expected = "D:\\Dropbox\\Creative\\Guitaraoke\\5 Output\\Georgia Satellites - Battleship Chains - Guitaraoke - with chords.mp4";
		var output = FilePathWrangler.GetOutputFilePath(original);
		output.ShouldBe(expected);
	}
}
