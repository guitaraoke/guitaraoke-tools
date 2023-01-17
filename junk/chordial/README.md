# Chordial
Chordial is a web app for building chord "charts" for creating guitaraoke backing videos.

A chart is a single line of text, with each beat in the song separate by a tab character. Here's the first bar of "Word Up", with tab characters represented as `⇒`:

```
F♯m⇒⇒⇒⇒E⇒⇒⇒⇒D⇒⇒E⇒⇒F♯m⇒⇒⇒⇒
```

Chordial's interface is a weird cross between a music notation app and a spreadsheet. Each beat of the song is represented by a cell. A cell can be empty, or can contain a chord.

### Keys:

`Tab`, `Right Arrow`: move one beat towards the end of the song. If you're on the final beat, add another beat and give it focus.

`Left Arrow`: move one beat towards the beginning of the song.

`Ins`: insert a new beat before the current beat.

`Ctrl+Del`: delete the current beat.

`Shift+arrow` (or mouse drag): select a range of cells. Ranges must be linear.

Copy: copy the select range to the clipboard, as a tab-separated string of formatted chords.

Paste: parses the clipboard, splits it on tab characters, formats each chord, and inserts the result at the focus. 

### Entering chords:

Type into the cell, or play chord shapes on a MIDI controller.

Typing into cells will format/validate them onblur, and replace them with blank space if they're not valid.

### FAQ

**What if I need two chords in a single beat?**

Double the tempo so each beat is e.g. an eighth note instead of a quarter note.





