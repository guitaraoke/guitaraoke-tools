import { describe, test, expect } from 'vitest'
import { Song } from '../song'
import { Chord } from '@tonaljs/tonal';

const A7 = Chord.get("A7");
const Bb = Chord.get("Bb");
const Dm = Chord.get("Dm");
const C = Chord.get("C");

describe('Song parsing code', () => {
    test('extracts one beat for each line', () => {
        let lines = `A\n\nD\n\nE\n\nA\n`;
        var song = Song.parseLines(lines);
        expect(song.beats.length).toBe(8);
    });
});

describe('Song manipulation', () => {
    test('appending a beat', () => {
        let song = new Song();
        expect(song.beats.length).toBe(0);
        song.appendBeat();
        expect(song.beats.length).toBe(1);
    });
    test('appending a beat with a chord', () => {
        let song = new Song();
        song.appendBeat("A7");
        expect(song.beats[0].chord).toMatchObject(Chord.get("A7"));
    });
    test('appending a beat with a lyric', () => {
        let song = new Song();
        song.appendBeat("A7", "hello world");
        expect(song.beats[0].lyric).toBe("hello world");
    });

    test('inserting a beat', () => {
        let song = new Song();
        song.appendBeat("A7");
        song.insertBeat(0, "Dm");
        expect(song.beats[0].chord).toMatchObject(Dm);
    });
    test('removing a beat', () => {
        let song = new Song();
        song.appendBeat("A7");
        song.appendBeat("Dm");
        song.removeBeat(0);
        expect(song.beats[0].chord).toMatchObject(Dm);
    });
});
