const string ROOT_PATH = @"D:\\Dropbox\\Creative\\Guitaraoke\\";

var videoPath = args[0];
var duration = 0f;
if (args.Any(arg => Single.TryParse(arg, out duration))) {
	Console.WriteLine(String.Join(',', args));
	Console.WriteLine($"Duration {duration}s (via argument)");
} else {
	duration = Single.MaxValue;
}
var draft = args.Any(arg => arg == "draft");
if (String.IsNullOrEmpty(videoPath) || !File.Exists(videoPath)) {
	var fsw = new FileSystemWatcher(ROOT_PATH, "*.txt") {
		IncludeSubdirectories = true
	};
	fsw.Changed += FileDidAThing;
	fsw.Created += FileDidAThing;
	fsw.Renamed += FileDidAThing;
	fsw.EnableRaisingEvents = true;
	Console.WriteLine("Watching for files. Press a key to exit");
	Console.ReadKey(true);
}

void FileDidAThing(object sender, FileSystemEventArgs e) {
	Console.WriteLine(e.ChangeType);
	Console.WriteLine(e.FullPath);
	Console.WriteLine(String.Empty.PadRight(40, '='));
}

//var engine = new ChordMakerEngine();
//await engine.MakeChords(videoPath, draft, duration);
