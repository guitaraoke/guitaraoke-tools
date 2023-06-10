namespace Hydra.Player2;

public class TrackEventArgs : EventArgs {
	public float Volume { get; set; }
	public float Pan { get; set; }
	public TrackEventArgs(float volume, float pan) {
		this.Volume = volume;
		this.Pan = pan;
	}
}
