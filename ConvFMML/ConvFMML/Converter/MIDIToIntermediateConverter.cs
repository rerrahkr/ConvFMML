using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Converter
{
    public class MIDIToIntermediateConverter
    {
        private class DataSet
        {
            public List<Data.Intermediate.Track> TrackList { set; get; }
            public LinkedList<Data.Intermediate.Event.TimeSignature> TimeSignatureList { set; get; }
            public LinkedList<Data.Intermediate.Event.KeySignature> KeySignatureList { set; get; }
            public LinkedList<Data.Intermediate.Event.Tempo> TempoList { set; get; }
            public string Title { set; get; }
        }

        public Data.Intermediate.Intermediate Convert(Data.MIDI.MIDI midi)
        {
            try
            {
                var set = new DataSet();
                set.TrackList = new List<Data.Intermediate.Track>();

                ConvertConductorTrack(midi.TrackList[0], set, midi.TimeDivision);
                for (int i = 1; i < midi.TrackSize; i++)
                {
                    set.TrackList.Add(ConvertTrack(midi.TrackList[i], set.TimeSignatureList.First, i));
                }

                return new Data.Intermediate.Intermediate(set.TrackList, set.TimeSignatureList, set.KeySignatureList, set.TempoList, set.Title, (midi.TimeDivision * 4));
            }
            catch (Exception ex)
            {
                throw new Exception("分解能の変換に失敗しました。", ex);
            }
        }

        private void ConvertConductorTrack(Data.MIDI.Track midiTrack, DataSet set, int timeDivision)
        {
            LinkedList<Data.MIDI.Event.Event> events = midiTrack.EventList;

            set.TimeSignatureList = ConvertTimeSignature(events, timeDivision);

            var pos = new Data.Intermediate.Position(1, 0);
            LinkedListNode<Data.Intermediate.Event.TimeSignature> tsNode = set.TimeSignatureList.First;

            var ksl = new LinkedList<Data.Intermediate.Event.KeySignature>();
            ksl.AddFirst(new Data.Intermediate.Event.KeySignature(pos.Clone(), Key.CMaj));      // Set default key signature
            var tl = new LinkedList<Data.Intermediate.Event.Tempo>();
            tl.AddFirst(new Data.Intermediate.Event.Tempo(pos.Clone(), 120));       // Set default tempo
            string title = String.Empty;
            var length = new Data.Intermediate.Position(1, 0);

            foreach (Data.MIDI.Event.Event ev in events)
            {
                pos = new Data.Intermediate.Position(pos, ev.DeltaTime, tsNode);
                while (tsNode.Next != null && pos.CompareTo(tsNode.Next.Value.Position) >= 0)       // Go to proper node
                {
                    tsNode = tsNode.Next;
                }

                if (ev is Data.MIDI.Event.SetTempo)
                {
                    var st = (Data.MIDI.Event.SetTempo)ev;
                    var newST = new Data.Intermediate.Event.Tempo(pos.Clone(), (60000000 / st.Value));
                    if (tl.Last.Value.Position.CompareTo(newST.Position) == 0)
                    {
                        tl.RemoveLast();
                    }
                    tl.AddLast(newST);
                }
                else if (ev is Data.MIDI.Event.KeySignature)
                {
                    var ks = (Data.MIDI.Event.KeySignature)ev;
                    var newKS = new Data.Intermediate.Event.KeySignature(pos.Clone(), Tables.KeyTable[ks.MinorFlagNumber, ks.SignatureNumber]);
                    if (ksl.Last.Value.Position.CompareTo(newKS.Position) == 0)
                    {
                        ksl.RemoveLast();
                    }
                    ksl.AddLast(newKS);
                }
                else if (ev is Data.MIDI.Event.SequenceTrackName)
                {
                    var stn = (Data.MIDI.Event.SequenceTrackName)ev;
                    title = stn.Name;
                }
                else if (ev is Data.MIDI.Event.EndOfTrack)
                {
                    length = pos.Clone();
                }
            }

            set.TrackList.Add(new Data.Intermediate.Track(
                new List<Data.Intermediate.Notes>(),
                new LinkedList<Data.Intermediate.Event.Instrument>(),
                new LinkedList<Data.Intermediate.Event.Volume>(),
                new LinkedList<Data.Intermediate.Event.Pan>(),
                0,
                length,
                "Conductor Track"
                ));
            set.TempoList = tl;
            set.KeySignatureList = ksl;
            set.Title = title;
        }

        private LinkedList<Data.Intermediate.Event.TimeSignature> ConvertTimeSignature(LinkedList<Data.MIDI.Event.Event> events, int timeDivision)
        {
            var pos = new Data.Intermediate.Position(1, 0);

            var tsl = new LinkedList<Data.Intermediate.Event.TimeSignature>();
            tsl.AddFirst(new Data.Intermediate.Event.TimeSignature(pos.Clone(), pos.Clone(), 4, 4, ((uint)timeDivision * 4)));    // Set defalt time signature (4/4)

            foreach (Data.MIDI.Event.Event ev in events)
            {
                pos = new Data.Intermediate.Position(pos, ev.DeltaTime, tsl.Last);

                if (ev is Data.MIDI.Event.TimeSignature)
                {
                    var newEv = (Data.MIDI.Event.TimeSignature)ev;

                    Data.Intermediate.Position oldPos = pos.Clone();
                    if (pos.Tick != 0)  // skip if pos(x : 0)
                    {
                        pos = new Data.Intermediate.Position((pos.Bar + 1), 0);
                    }

                    var newTS = new Data.Intermediate.Event.TimeSignature(pos.Clone(), oldPos, newEv.Numerator, (1 << newEv.DenominatorBitShift), ((uint)(timeDivision >> (newEv.DenominatorBitShift - 2)) * (uint)newEv.Numerator));
                    if (tsl.Last.Value.Position.CompareTo(newTS.PrevSignedPosition) == 0)
                    {
                        tsl.RemoveLast();
                    }
                    tsl.AddLast(newTS);
                }
            }

            return tsl;
        }

        private Data.Intermediate.Track ConvertTrack(Data.MIDI.Track midiTrack, LinkedListNode<Data.Intermediate.Event.TimeSignature> tsNode, int trackNumber)
        {
            var pos = new Data.Intermediate.Position(1, 0);

            var nl = new List<Data.Intermediate.Notes>();
            var il = new LinkedList<Data.Intermediate.Event.Instrument>();
            var vl = new LinkedList<Data.Intermediate.Event.Volume>();
            var pl = new LinkedList<Data.Intermediate.Event.Pan>();
            string name = String.Empty;
            var length = new Data.Intermediate.Position(1, 0);

            foreach (Data.MIDI.Event.Event ev in midiTrack.EventList)
            {
                pos = new Data.Intermediate.Position(pos, ev.DeltaTime, tsNode);
                while (tsNode.Next != null && pos.CompareTo(tsNode.Next.Value.Position) >= 0)       // Go to proper node
                {
                    tsNode = tsNode.Next;
                }

                if (ev is Data.MIDI.Event.NoteOn)
                {
                    FuncNoteOn(nl, pos.Clone(), (Data.MIDI.Event.NoteOn)ev);
                }
                else if (ev is Data.MIDI.Event.NoteOff)
                {
                    FuncNoteOff(nl, pos.Clone(), (Data.MIDI.Event.NoteOff)ev);
                }
                else if (ev is Data.MIDI.Event.ProgramChange)
                {
                    var inst = (Data.MIDI.Event.ProgramChange)ev;
                    il.AddLast(new Data.Intermediate.Event.Instrument(pos.Clone(), inst.Number));
                }
                else if (ev is Data.MIDI.Event.Volume)
                {
                    var vol = (Data.MIDI.Event.Volume)ev;
                    vl.AddLast(new Data.Intermediate.Event.Volume(pos.Clone(), vol.Value));
                }
                else if (ev is Data.MIDI.Event.Pan)
                {
                    var pan = (Data.MIDI.Event.Pan)ev;
                    pl.AddLast(new Data.Intermediate.Event.Pan(pos.Clone(), pan.Value));
                }
                else if (ev is Data.MIDI.Event.SequenceTrackName)
                {
                    var stn = (Data.MIDI.Event.SequenceTrackName)ev;
                    name = stn.Name;
                }
                else if (ev is Data.MIDI.Event.EndOfTrack)
                {
                    length = pos.Clone();
                }
            }

            return new Data.Intermediate.Track(nl, il, vl, pl, trackNumber, length, name);
        }

        private void FuncNoteOn(List<Data.Intermediate.Notes> nl, Data.Intermediate.Position position, Data.MIDI.Event.NoteOn ev)
        {
            foreach (Data.Intermediate.Notes notes in nl)
            {
                LinkedList<Data.Intermediate.Event.NoteRest> list = notes.NoteList;
                if (list.Last.Value.End != null)        // Check note overlapping
                {
                    list.AddLast(new Data.Intermediate.Event.Note(position, null, ev.Number, ev.Velocity));
                    return;
                }
            }

            // Create new notelist
            var newList = new LinkedList<Data.Intermediate.Event.NoteRest>();
            newList.AddFirst(new Data.Intermediate.Event.Note(position, null, ev.Number, ev.Velocity));
            nl.Add(new Data.Intermediate.Notes(newList, nl.Count));
        }

        private void FuncNoteOff(List<Data.Intermediate.Notes> nl, Data.Intermediate.Position position, Data.MIDI.Event.NoteOff ev)
        {
            foreach (Data.Intermediate.Notes notes in nl)
            {
                Data.Intermediate.Event.Note note = (Data.Intermediate.Event.Note)notes.NoteList.Last.Value;
                if (note.End == null && note.KeyNumber == ev.Number)
                {
                    note.End = position;
                    return;
                }
            }
        }
    }
}
