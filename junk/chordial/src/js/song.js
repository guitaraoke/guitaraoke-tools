import { Chord, Interval } from '@tonaljs/tonal';

class Beat {
    uglify = chordName => chordName.replace(/♭/g, 'b').replace(/♯/g, '#');
    
    constructor(chord, lyric) {
        this.chord = Chord.get(this.uglify(chord ?? ""));
        this.lyric = lyric ?? "";
    }

    isSameAs(that) {
        if (!that) return(false);
        if (this.lyric != that.lyric) return(false);
        if (this.chord && !that.chord) return(false);
        if (that.chord && !this.chord) return(false);
        return (this.chord.symbol == that.chord.symbol);
    }
}

class Song {
    constructor(beats) {
        this.beats = beats ?? new Array();
    }
    
    insertBeat(index, chord, lyric) {
        this.beats.splice(index, 0, new Beat(chord, lyric));
    }
    
    appendBeat(chord, lyric) {
        this.beats.push(new Beat(chord, lyric))
    }
    
    removeBeat(index) {
        this.beats.splice(index, 1);
    }

    updateBeat(index, chord, lyric) {
        var newBeat = new Beat(chord, lyric);
        var oldBeat = this.beats[index];
        if (newBeat.isSameAs(oldBeat)) return(false); 
        this.beats[index] =  newBeat;
        return(true);
    }

    static parseScore(score) {
        var beats = score.trim().split(/\n\s*\n/).map(Song.parseScoreLine).flat();
        var song = new Song(beats);
        return song;
    }
    static parseScoreLine(line) {
        let [chords, lyrics] = line.split('\n');
        lyrics = lyrics || '';
        chords = chords.replace(/(\s+)(\S+)/g, "$1|$2");
        console.log(chords);
        chords = chords.split('|');
        if (! chords.length) return [];
        let lyricIndex = 0;
        let beats = chords.map((chord, index) => {
            let lyric = (index+1 == chords.length ? lyrics : lyrics.substring(0, chord.length));
            lyrics = lyrics.slice(chord.length);
            chord = chord.trim();
            return new Beat(chord, lyric);
        });
        return beats;        
    }

    asciify(beatsPerLine) {
        beatsPerLine ||= 16;
        let output = new Array();
        for (var i = 0; i < this.beats.length; i+=beatsPerLine) {
            let chonk = this.beats.slice(i, i+beatsPerLine);
            let chords = "";
            let lyrics = "";
            for(var j = 0; j < chonk.length; j++) {
                let beat = chonk[j];
                var len = Math.max(beat.lyric.length,beat.chord.symbol.length+2);
                if (len < 4) len = 4;
                lyrics += chonk[j].lyric.padEnd(len, ' ');
                chords += (chonk[j].chord.symbol || '/').padEnd(len, ' ');
            }            
            output.push(chords);
            output.push(lyrics);
            output.push("");
        }
        return output.join('\n');
    }

    static SillyGames = Song.parseScore(`
F      / / / Am  / / / Am  / / / Bb  / / / 
(intro)                                    

Bbm  / / / F  / / / C7  / / G7  C  / / / 
                                            

F  /         /       /   Am     /   /         /    Am  / /    / Bb    / /       /     
   I've been wanting you    for so  long, it's a shame,     oh, baby,     every time I

Bm   /    /    / F  /       /    /  Bb     /      Am       Gm  F   / / / 
hear your name      Oh, the pain      boy, how it hurts me in- side      

F      /     /       /    Am       /        /    /  Am     /         /      /        Bb / /        /             
 Cause every time we meet  We play hide and seek       I'm wondering what I should do     should I dear come 

Bbm /  /       /   F     /      /    / C7      /    /       G7 C7   / / /
Up  to you and say  "How do you do?"      Would you turn me a- way?    


Ab /         /       /     Cm          /        /        /
   You're as much to blame    'Cause I know you feel the same

C /   /     /      Dm   /   /   /
      I can see it in your eyes

Bb    /        C      /   F    /    Dm  /
  But I've got no time to live this lie 

Bb    /        C      /   C    /    /     /  Fm7    Fm / /  Bb / /  /
  No, I've got no time to play your silly ga-a-ames               Silly
 
Fm7 Fm  / /  Bb / / /
ga-          -a-mes 

F      /     /      /   Am       /         /      /    Am      /       /       /     Bb    /  /  /     
  Yet, in my mind I say   "If he makes his move today     I'll just pretend to be shocked"
`)

    static WordUp = Song.parseScore(`
F#m  /      /      /  Esus4 /   /      /     
Yoh, pretty ladies,  around the world, got a 

D     /        E         /       F#      /        /      /        
weird thing to show you, so tell all the boys and girls, tell your

F#m     /     /       /        Esus4 /    /     /       
brother, your sister, and your momma  too, 'cos we're a-

D       /  E     /       F#        /       /    /        
bout to go down, and you know just what to do.  Wave your

F#m   /      /    /    Esus4 /       / /
hands in the air, like you   don't care

D     /      E      /       F#      /        /      /        
Glide by the people as they stop to look and stare,   do your

F#m   /        /     /  Esus4 /     /        /  
dance, do your dance, do your dance quick, a mo-

D    /        E     /       F#    /    /    /
-mma come on, baby, tell me what's the word?
    `)
}

export { Beat, Song };