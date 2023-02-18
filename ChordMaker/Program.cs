const string ROOT_PATH = @"D:\\Dropbox\\Creative\\Guitaraoke\\";
var engine = new ChordMakerEngine();

var videoPath = args.Length > 0 ? args[0] : String.Empty;
var duration = 0f;
if (args.Any(arg => Single.TryParse(arg, out duration))) {
	Console.WriteLine(String.Join(',', args));
	Console.WriteLine($"Duration {duration}s (via argument)");
} else {
	duration = Single.MaxValue;
}
var draft = args.Any(arg => arg == "draft");

var songs = new List<string>();

if (String.IsNullOrEmpty(videoPath) || !File.Exists(videoPath)) {
	var fsw = new FileSystemWatcher(ROOT_PATH, "*.txt") {
		IncludeSubdirectories = true
	};
	fsw.Changed += FileDidAThing;
	fsw.Created += FileDidAThing;
	fsw.Renamed += FileDidAThing;
	fsw.EnableRaisingEvents = true;
	Console.WriteLine("Watching for files. Press a key to exit");
	while (true) {
		// press the key 1: byte = ASCII 49. In binary: 00110001
		// Then we're going to do a bitwise AND:        00001111
		//										result: 00000001
		var key = Console.ReadKey(true).KeyChar & 15;
		Console.WriteLine(key);
		if (key < songs.Count) {
			videoPath = Path.Combine(ROOT_PATH, Folder.MIXES, $"{songs[key]} - Guitaraoke.mp4");
			Console.WriteLine($"Encoding videoPath:");
			await engine.MakeChords(videoPath, draft, duration);
			songs.Remove(songs[key]);
			DrawMenu();
		}
	}
}

void FileDidAThing(object sender, FileSystemEventArgs e) {
	var fileName = Path.GetFileNameWithoutExtension(e.FullPath);
	var tokens = fileName.Split(" - ");
	var song = $"{tokens[0]} - {tokens[1]}";
	if (songs.Contains(song)) return;
	songs.Add(song);
	DrawMenu();
}

void DrawMenu() {
	Console.WriteLine(String.Empty.PadRight(40, '='));
	for (var i = 0; i < songs.Count; i++) {
		Console.WriteLine($"{i}: {songs[i]}");
	}
	Console.WriteLine("Press a number to encode a song, or Ctrl-C to quit!");


}

