using Shouldly;

namespace GuitaraokeTools.Tests;

public class FilePathWranglerTests {
	[Fact]
	public void FileWranglerGetsChordNamesCorrect() {
		var rootPath = "D:\\Dropbox\\Creative\\Guitaraoke";
		var original = "D:\\Dropbox\\Creative\\Guitaraoke\\2 Mixes\\Georgia Satellites - Battleship Chains - Guitaraoke.mp4";
		var expected = "D:\\Dropbox\\Creative\\Guitaraoke\\3 Chords\\Georgia Satellites - Battleship Chains - Guitaraoke - chord names.txt";

		var fpw = new FilePathWrangler(rootPath);
		var output = fpw.GetChordNamesTextFilePath(original);
		output.ShouldBe(expected);
	}

	[Fact]
	public void FileWranglerGetsOverlayFileCorrect() {
		var rootPath = "D:\\Dropbox\\Creative\\Guitaraoke";
		var original = "D:\\Dropbox\\Creative\\Guitaraoke\\2 Mixes\\Georgia Satellites - Battleship Chains - Guitaraoke.mp4";
		var expected = "D:\\Dropbox\\Creative\\Guitaraoke\\3 Chords\\Georgia Satellites - Battleship Chains - Guitaraoke - chord names.txt";
		var fpw = new FilePathWrangler(rootPath);
		var output = fpw.GetOverlayFilePath(original);
		output.ShouldBe(expected);
	}


	[Fact]
	public void FileWranglerGetsOutputFileCorrect() {
		var rootPath = "D:\\Dropbox\\Creative\\Guitaraoke";
		var original = "D:\\Dropbox\\Creative\\Guitaraoke\\2 Mixes\\Georgia Satellites - Battleship Chains - Guitaraoke.mp4";
		var expected = "D:\\Dropbox\\Creative\\Guitaraoke\\3 Chords\\Georgia Satellites - Battleship Chains - Guitaraoke - chord names.txt";

		var fpw = new FilePathWrangler(rootPath);
		var output = fpw.GetOutputFilePath(original);
		output.ShouldBe(expected);
	}
}
