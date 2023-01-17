import styles from '../css/chordial.scss';
import { Chord } from '@tonaljs/tonal';

const COLUMNS = 16;
export class Renderer {
    get offset() {
        return (COLUMNS * 2) - 1;
    }

    constructor(editor, dom) {
        this.editor = editor;
        this.dom = dom;
        var chartDivAttributes = { 
            'class': 'chart',
            'style': `grid-template-columns: ${new Array(COLUMNS).fill("1fr").join(" ")}`
        };
        this.chartDiv = this.createElement('div', chartDivAttributes);     
        this.dom.appendChild(this.chartDiv);
        this.index = 0;
    }

    get beatIndex() {
        return Math.floor(this.index/2);
    }

    createElement(tagName, attributes, innerText) {
        const element = document.createElement(tagName);
        for (const [key, value] of Object.entries(attributes || {})) {
            element.setAttribute(key, value);
        }
        if (innerText) element.innerText = innerText;
        return element;
    }

    moveFocusLeft() {
        if (this.index > 0) this.index -= 2;
    }

    moveFocusRight() {
        this.index += 2;
    }
    moveFocusUp() {
        let input = this.dom.querySelectorAll("input")[this.index];
        if (input.className == "lyric") {
            this.index -= 1;
        } else {
            if (this.index > this.offset) this.index -= this.offset;
        }
    }
    moveFocusDown() {
        let input = this.dom.querySelectorAll("input")[this.index];
        if (input.className == "chord") {
            this.index += 1;
        } else {
            this.index += this.offset;
        }
    }

    render(song) {
        var style = this.createElement('style', {});
        style.innerHTML = styles;
        this.dom.appendChild(style);
        this.renderSong(song);
    }
    renderTextForPremiere(song) {        
        let chords = song.beats.map(beat => this.beautify(beat.chord.symbol)).join("\t");        
        let lyrics = song.beats.map(beat => beat.lyric).join("\t");
        return [chords, lyrics].join("\n");
    }

    renderSong(song) {
        this.chartDiv.innerHTML = '';
        this.divs = song.beats.map((beat, index) => {
            let beatDiv = this.renderBeat(beat, index);
            this.chartDiv.appendChild(beatDiv);
            return beatDiv;
        });
        let input = this.dom.querySelectorAll("input")[this.index];
        if (input) input.focus();
    }
    beautify(chord) {
        if (! chord.replace) return(chord);
        return chord
            .replace(/b/g, '♭')
            .replace(/#/g, '♯')
            .replace(/dim/, 'o')
            .replace(/aug/g, '+')
            .replace(/maj7/g, 'Δ7')
            .replace(/7/g, '⁷');
    }

    renderBeat(beat, index) {
        let beatDiv = this.createElement('div');
        let chordInput = this.createElement('input', { 'class': 'chord' });
        chordInput.beatIndex = index;
        chordInput.value = this.beautify(beat.chord.tonic ? beat.chord.symbol : '');
        chordInput.addEventListener("focus", this.onInputFocus.bind(this));
        chordInput.addEventListener("blur", this.onInputBlur.bind(this));
        beatDiv.appendChild(chordInput);
        let lyricInput = this.createElement('input', { 'class': 'lyric' });
        lyricInput.addEventListener("focus", this.onInputFocus.bind(this));
        lyricInput.addEventListener("blur", this.onInputBlur.bind(this));
        lyricInput.value = beat.lyric.trimEnd().replace(/ /g, '·');
        lyricInput.beatIndex = index;
        beatDiv.appendChild(lyricInput);
        return beatDiv;
    }

    onInputFocus(event) {
        let input = event.target;
        let inputs = this.dom.querySelectorAll("input");
        this.index = [].indexOf.call(inputs, input);
        input.select();

    }
    onInputBlur(event) {
        let index = event.target.beatIndex;
        let lyric, chord;
        switch (event.target.className) {
            case "lyric":                
                lyric = event.target.value;
                chord = event.target.previousElementSibling.value;
                break;
            case "chord":
                lyric = event.target.nextElementSibling.value;
                chord = event.target.value;
                break;
        }
        console.log("UPDATE", lyric, chord, index);
        this.editor.updateBeat(index, chord, lyric);
    }
}
