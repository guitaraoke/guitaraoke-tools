function ChordPlayer(ac) {
    this.piano = Synth.createInstrument('piano');
    Synth.setVolume(0.33);
    this.playChord = function(chordName) {
        let parts = chordName.split(/\//);
        let chord = parts[0];
        const chordNotes = Tonal.Chord.get(chord, { sharps: true }).notes;
        const notes = chordNotes.map(note => ({
            note : note == 'E#' ? 'F' : note == 'B#' ? 'C' : note,
            octave: (note >= 'C' ? 5 : 4)
        }));
        notes.unshift({note: chordNotes[0], octave: 3 });
        if (parts.length == 2) {
            let root = parts[1];
            notes.push(root + '2');
        }
        console.log(notes);
        //piano.play('C', 4, 2); // plays C4 for 2s using the 'piano' sound profile
        notes.forEach(note => {
            this.piano.play(note.note, note.octave, 2);
        });
    }
}

function ensurePlayerCreated() {
    if (!window.chordPlayer) {
        window.chordPlayer = new ChordPlayer(new (window.AudioContext || window.webkitAudioContext));
    }
    return window.chordPlayer;
}

function rechorder(file) {

    document.getElementById("chord-names-div").addEventListener("click", function (event) {
        var player = ensurePlayerCreated();
        if (event.target.tagName == 'SPAN') {
            player.playChord(event.target.innerText);
            event.cancelBubble = true;
            event.preventDefault();
            return (false);
        }
        this.style.zIndex = 2;
        let ta = document.getElementById("chord-names-textarea");
        ta.value = this.innerText;
        ta.style.zIndex = 3;
        ta.focus();
        this.scrollTop = ta.scrollTop;
    });

    document.getElementById("chord-names-textarea").addEventListener("blur", function () {
        this.style.zIndex = 2;
        let div = document.getElementById("chord-names-div");
        div.innerText = this.value;
        div.style.zIndex = 3;
        highlightAllChords(div);
        this.scrollTop = div.scrollTop;
    });

    Object.defineProperty(HTMLMediaElement.prototype, 'playing', {
        get: function () {
            return !!(this.currentTime > 0 && !this.paused && !this.ended && this.readyState > 2);
        }
    });
	var flats = {
		"A#": "Bb",
		"B#": "C",
		"C#": "Db",
		"D#": "Eb",
		"E#": "F",
		"F#": "Gb",
		"G#": "Ab"
	};

    const video = document.querySelector("video");
    const clock = document.getElementById('clock');
    window.setInterval(function () {
        clock.innerText = (Math.floor(video.currentTime * 1000) / 1000).toFixed(3);
    }, 100);
    video.playbackRate = 1.0;

    document.querySelectorAll("#playback-speed-selector input").forEach(radio => radio.addEventListener("change", function (event) {
        video.playbackRate = this.value;
    }));
    const chordsTextArea = document.getElementById("chord-names-textarea");
    const chordTimesTextArea = document.getElementById('chord-times-textarea');

    let chord = 0;
    let text = chordsTextArea.value;
    let chords = extractChords(chordsTextArea.value);
    let div = document.getElementById("chord-names-div");
    highlightAllChords(div);

    document.getElementById('go-button').addEventListener("keydown", function (event) {
        var player = ensurePlayerCreated();
        console.log(player);
        switch (event.key) {
            case 'm':
                var line = video.currentTime.toFixed(4) + " ???";
                chordTimesTextArea.value += line + "\n";
                chordTimesTextArea.scrollTop = chordTimesTextArea.scrollHeight;
                return;
            case 'ArrowLeft':
                chord--;
                if (chord < 0) chord = 0;
                highlightChord(text, chords[chord].index, chords[chord][0].trim().length);
                if (!video.playing) player.playChord(chords[chord][0].trim());
                return;
            case 'ArrowRight':
                chord++;
                highlightChord(text, chords[chord].index, chords[chord][0].trim().length);
                if (!video.playing) player.playChord(chords[chord][0].trim());
                return;
        }
    });

    document.getElementById('go-button').addEventListener("click", function (evt) {
		console.log('yep');
        var player = ensurePlayerCreated();
        if (video.playing) {
            if (!(evt.ctrlKey)) {
                // player.playChord(chords[chord][0].trim());
                var line = video.currentTime.toFixed(4) + " " + chords[chord][0].trim();
                chordTimesTextArea.value += line + "\n";
                chordTimesTextArea.scrollTop = chordTimesTextArea.scrollHeight;
            }
            chord++;
            highlightChord(text, chords[chord].index, chords[chord][0].trim().length);
			drawDataBars();
        } else {
            chord = 0;
            text = chordsTextArea.value;
            chords = extractChords(chordsTextArea.value);
            highlightChord(text, chords[chord].index, chords[chord][0].trim().length);
            video.play();
        }
	});

	document.getElementById('go-button').focus();

	function flattenChords(text) {
		//for (const [key, value] of Object.entries(flats)) {
		//	text = text.replaceAll(key, value);
		//}
		return text;
	}

    function highlightAllChords(element) {
        let text = flattenChords(element.innerText);
        var chords = extractChords(text);
        let html = '';
        let index = 0;
        for (var i = 0; i < chords.length; i++) {
            html += text.substring(index, chords[i].index);
            html += '<span>';
            html += text.substring(chords[i].index, chords[i].index + chords[i][0].trim().length);
            html += '</span>';
            index = chords[i].index + chords[i][0].trim().length;
        }
        html += text.substring(index);
        element.innerHTML = html;
    }

    function highlightChord(text, index, length) {
        let div = document.getElementById("chord-names-div");
        let html = text.substring(0, index) + '<span>'
            + text.substring(index, index + length) + '</span>' + text.substring(index + length);
        div.innerHTML = html;
        span = div.querySelector("span");
        span.scrollIntoView({ behavior: "smooth", block: "center" });
    }

    function extractChords(text) {
        let chords = [];
        const regexp = /\b(NC|[A-G][b#]?(M|m|maj|aug|dim)?[5679]?(?:(b|add)(4|5|6|9|11|13))?(?:sus[24]|dim|aug)?(?:\/[A-G][b#]?)?)\s/g;
        while (match = regexp.exec(text)) chords.push(match);
        return chords;
    }

    document.getElementById("chord-names-textarea").addEventListener("keyup", function () {
        var data = {
            file: file,
            chordNames: this.value
        };
        console.log(data);
        $.post('/Home/ChordNames', data, function() {
            console.log("Updated chord names");
        });
        extractChords(this.value);
    });

    function drawDataBars() {
        var chordTimesList = document.getElementById("chord-times-textarea").value;
        var chords = chordTimesList.split(/\n/)
            .filter(c => c.split)
            .map(c => c.split(/[: ]+/))
            .map(pair => ({ name: pair[1], time: pair[0] }));
        for(var i = 1; i < chords.length; i++) {
             chords[i-1].duration = chords[i].time - chords[i-1].time;
         }
        var max = Math.max.apply(null, chords.map(c => c.duration || 0));
        var container = document.querySelector("div#chord-times div div");
        container.innerHTML = "";
        var textarea = document.getElementById("chord-times-textarea")
        var lineHeight = parseInt(window.getComputedStyle(textarea)["line-height"]);
        for(var i = 0; i < chords.length; i++) {
            var span = document.createElement('span');
            span.style.top = (4 + (i * lineHeight)) + "px";
            span.style.width = (100 * (chords[i].duration / max)) + "px";
            container.appendChild(span);
        }
    }

    document.getElementById("chord-times-textarea").addEventListener("keyup", function () {
        drawDataBars();
        var data = {
            file: file,
            chordTimes: this.value
        };
        console.log(data);
        $.post('/Home/ChordTimes', data);
    });
    drawDataBars();

}
