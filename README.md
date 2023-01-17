# guitaraoke-tools
Tools for running Guitaraoke

# The Guitaraoke Workflow.

#### Download the source files

Create a folder called **`Guitaraoke\1 Sources\{Artist} - {Title}`**

Download the files from karaoke-version.co.uk:

* **Karaoke.mp4** - the karaoke video with lyrics
* **Backing.mp3** - the backing track minus guitars and bass
* **Lead.mp3** - "lead" guitar
* **Rhythm.mp3** - "rhythm" guitar
* **Bass.mp3** - bass guitar

#### Render the 5.1 video source

Using Adobe Premiere:

1. Open the `Guitaraoke - Template.proj`
2. Save As... `Guitaraoke\1 Sources\{Artist} - {Title}\{Artist} - {Title} - Guitaraoke.prproj`
3. Rename the project sequence to `Georgia Satellites - Battleship Chains - Guitaraoke`
4. Add the project tracks:
   1. Karaoke.mp4 - onto the Video track. Mute the audio.
   2. Backing.mp4 - split the audio into two mono sources, add this to Backing (Left) and Backing (Right)
   3. Lead, Bass, Rhythm onto their own tracks
   4. Make sure everything is synced up.
   5. Unlink the Karaoke.mp4 audio and video, cut the clip at the black frame after the title "slide", Rate Stretch the title backwards until it starts at the beginning of the count-in clip.
5. Export Media, H.264 video, AAC Audio, 48000 Hz, 5.1 channels, 320kbps

Export to **`Guitaraoke\2 Mixes\{Artist} - {Title}`**

#### Make the chord chart

I make chords using the **Rechorder** tool which is part of my **guitaraoke-tools** repo.











