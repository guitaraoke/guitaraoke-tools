namespace ChordMaker.ffmpeg;
public class FFProbeStream {
    public int Index { get; set; }
    public string CodecName { get; set; } = "";
    public string CodecLongName { get; set; } = "";
    public string Profile { get; set; } = "";
    public string CodecType { get; set; } = "";
    public string CodecTagString { get; set; } = "";
    public string CodecTag { get; set; } = "";
    public int Width { get; set; }
    public int Height { get; set; }
    private string rFrameRate = "";
    public string NBFrames { get; set; } = "";
    public int Frames => Int32.Parse(NBFrames);
    public int RFrameRateNumerator { get; private set; }
    public int RFrameRateDenominator { get; private set; }
    public string RFrameRate {
        get => rFrameRate;
        set {
            rFrameRate = value;
            var tokens = rFrameRate.Split("/");
            if (tokens.Length != 2) throw new Exception($"Could not calculate FPS from RFrameRate {RFrameRate}");
            RFrameRateNumerator = Int32.Parse(tokens[0]);
            RFrameRateDenominator = Int32.Parse(tokens[1]);
            FPS = RFrameRateNumerator / (float) RFrameRateDenominator;
        }
    }

    public string AvgFrameRate { get; set; } = "";
    public int DurationTs { get; set; }
    public float Duration { get; set; }
    public int BitRate { get; set; }
    public float FPS { get; private set; }
}
