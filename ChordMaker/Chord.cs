using System.Text.RegularExpressions;

namespace ChordMaker;

public class Chord {
	public string Name { get; set; } = String.Empty;
	public string ExtraBit { get; set; } = String.Empty;
	public float Time { get; set; } = 0f;

	public int TextLine { get; set; } = 0;
	public float Duration { get; set; } = 0f;
	public string PrettyName => Name.Prettify();
	public string PrettyExtraBit => ExtraBit.Prettify();
	public string FullPrettyNameWithoutBass => (PrettyName + PrettyExtraBit).Split('/').First();
	private readonly Regex regex = new("[ :]+");

	public bool HasExtraBit => String.IsNullOrEmpty(ExtraBit) == false;

	public Chord(string line) {
		var tokens = regex.Split(line.Trim());
		if (tokens.Length <= 1) return;
		Time = Single.Parse(tokens[0].Trim());
		(Name, ExtraBit) = ParseChord(tokens[1].Trim());
	}

	private (string, string) ParseChord(string chord) {
		return chord switch {
			"NC" => ("×", ""),
			"n.c" => ("×", ""),
			_ => (chord[..1], chord[1..])
		};
	}
}

public static class StringExtensions {
	public static string Prettify(this string s)
		=> s.Replace("b", "♭").Replace("#", "♯");
}
