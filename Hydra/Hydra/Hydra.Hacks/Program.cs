using NAudio.Wave;

foreach (var file in Directory.GetFiles(@"D:\Dropbox\Creative\Guitaraoke\1 Sources\Abba - Waterloo\", "*.mp3")) {
	Console.WriteLine(file);
	var ftx = new FileTrackExtractor(file);
	foreach (var track in ftx.SampleProviders) {
		Console.WriteLine(track.Key);
	}
}

