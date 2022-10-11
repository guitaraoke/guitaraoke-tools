using System.Text.RegularExpressions;

namespace ChordMaker;

public class Chord {
    public int TextLine { get; set; } = 0;
    public float Time { get; set; }
    public string Name { get; set; }
    public string ExtraBit { get; set; }
    public float Duration { get; set; } = 0f;
    public string PrettyName => Name.Replace("b", "♭").Replace("#", "♯");
    public string PrettyExtraBit => ExtraBit.Replace("b", "♭").Replace("#", "♯");
    Regex regex = new Regex("[ :]+");

    public bool HasExtraBit => String.IsNullOrEmpty(ExtraBit) == false;

    public Chord(string line) {
        Time = 0f;
        Name = String.Empty;
        ExtraBit = String.Empty;
        var tokens = regex.Split(line);
        if (tokens.Length > 1) {
            Time = float.Parse(tokens[0].Trim());
            (Name, ExtraBit) = ParseChord(tokens[1].Trim());
        }
    }
    private (string, string) ParseChord(string chord) {
        return chord switch {
            "NC" => ("×", ""),
            "n.c" => ("×", ""),
            _ => (chord.Substring(0,1), chord.Substring(1))
        };
    }
}
