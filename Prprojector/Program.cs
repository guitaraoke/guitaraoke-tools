const string ROOT_PATH = @"D:\Dropbox\Creative\Guitaraoke\";

var templateFileXml = File.ReadAllText(Path.Combine(ROOT_PATH, "Artist - Title - Guitaraoke.prproj"));
var sourcePath = Path.Combine(ROOT_PATH, Folder.SOURCES);
foreach (var dir in Directory.GetDirectories(sourcePath)) {
	var files = Directory.GetFiles(dir);
	if (files.Any(f => f.EndsWith(".prproj"))) continue;
	Console.WriteLine($"{dir} does not have a PRPROJ file - create it now?");
	if ((Console.ReadKey(true).KeyChar | 32) == 'y') {
		// do a replace on the template XML, populate artist and title, and save it.
		var tokens = Path.GetFileName(dir).Split(" - ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
		var (artist, title) = (tokens[0], tokens[1]);
		var modifiedFileXml = templateFileXml.Replace("Artist - Title", $"{artist} - {title}");
		var outputFilePath = Path.Combine(dir, $"{artist} - {title} - Guitaraoke.prproj");
		File.WriteAllText(outputFilePath, modifiedFileXml);
		Console.WriteLine($"Created {outputFilePath}");
	}
}
