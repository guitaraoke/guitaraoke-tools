//var builder = WebApplication.CreateBuilder(args);
//var app = builder.Build();
//app.UseHttpsRedirection();
//app.MapGet("/wave", (string path, int channel) => {
//	var result = new {
//		path,
//		channel
//	};
//	return result;
//});
//app.Run();

using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

var reader = new AudioFileReader(
	@"C:\Users\dylan\Dropbox\Creative\Guitaraoke\5 Output\Faith No More - Everything's Ruined - Guitaraoke - with chords.mp4");
Console.WriteLine(reader);
var sp = reader.ToSampleProvider();
var slice = reader.WaveFormat.SampleRate;
var buffer = new float[slice];
var read = 1;
var i = 0;
var imageWidth = (int) (reader.Length / slice);
var imageHeight = 100 * reader.WaveFormat.Channels;
var image = new Image<Rgba32>(imageWidth, imageHeight);
var offset = 0;
var brush = new SolidBrush(Color.Red);
while (0 < (read = sp.Read(buffer, 0, buffer.Length))) {
	if (i++ % reader.WaveFormat.Channels == 0) {
		offset = 0;
		Console.WriteLine();
		Console.Write(i / 6);
		Console.Write(":  ");
	}
	var mean = (int)(1000 * buffer.Sum(x => Math.Abs((double)x)) / reader.WaveFormat.SampleRate);
	image.Mutate(x => x.Fill(Color.Red, new RectangleF(i, offset, i + 1, offset + mean)));
	offset += 100;
}
image.Save("waveform.jpg", new JpegEncoder());
Console.WriteLine("waveform done");
