namespace ChordMaker.ffmpeg;
public class FFProbeFormat {
    public string Filename { get; set; } = "";
    public int NbStreams { get; set; }
    public int NbPrograms { get; set; }
    public int BitRate { get; set; }
}
