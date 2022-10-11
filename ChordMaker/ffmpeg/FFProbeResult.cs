using ChordMaker.ffmpeg;

namespace ChordMaker;
public class FFProbeResult {
    public FFProbeFormat Format { get; set; } = new();
    public FFProbeStream[] Streams { get; set; } = {}; 
}
