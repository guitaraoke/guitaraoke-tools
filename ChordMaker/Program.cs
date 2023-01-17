var videoPath = args[0];
var duration = 0f;
if (args.Any(arg => Single.TryParse(arg, out duration))) {
	Console.WriteLine($"Duration {duration}s (via argument)");
} else {
	duration = Single.MaxValue;
}
var draft = args.Any(arg => arg == "draft");
var wrangler = new FilePathWrangler("D:\\Dropbox\\Creative\\Guitaraoke");
var engine = new ChordMakerEngine(wrangler);
engine.MakeChords(videoPath, draft, duration);
