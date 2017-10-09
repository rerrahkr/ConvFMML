using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConvFMML.Data.MIDI;
using ConvFMML.Data.MIDI.Event;

namespace ConvFMML
{
    public class MIDIReader
    {
        private class DataSet   // Inner class for collecting data to make MIDI class
        {
            public int Format { set; get; }
            public int TrackSize { set; get; }
            public int TimeDivision { set; get; }
            public List<Track> TrackList { set; get; }
        }

        public string MIDIPath { get; set; }

        public MIDI Read(string midiPath, int format)
        {
            MIDI data = Read(midiPath);
            if (data.Format != format && format == 1)
            {
                return ConvertToFormat1(data);
            }
            else
            {
                return data;
            }
        }

        public MIDI Read(string midiPath)
        {
            MIDIPath = midiPath;
            return Read();
        }

        public MIDI Read()
        {
            byte[] bs = InputData();
            try
            {
                var set = new DataSet();
                ReadHeaderChunk(bs, set);
                ReadTrackChunk(bs, set);
                return new MIDI(set.TrackList, set.Format, set.TrackSize, set.TimeDivision);
            }
            catch (Exception ex)
            {
                throw new Exception("ファイル '" + MIDIPath + "' は、データが壊れているため読み込めませんでした。" + ex.Message, ex);
            }
        }

        private byte[] InputData()
        {
            try
            {
                using (var fs = new System.IO.FileStream(MIDIPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    byte[] bs = new byte[fs.Length];
                    fs.Read(bs, 0, bs.Length);
                    return bs;
                }
            }
            catch (Exception ex) when (!(ex is System.IO.FileNotFoundException))
            {
                throw new Exception("ファイル '" + MIDIPath + "' の読み込みに失敗しました。", ex);
            }
        }

        private void ReadHeaderChunk(byte[] bs, DataSet set)
        {
            if (Encoding.ASCII.GetString(bs.Take(4).ToArray()) != "MThd") throw new Exception("Couldn't read 'MThd'");

            set.Format = LittleEndianConverter.ToUInt16(bs, 8);
            set.TrackSize = LittleEndianConverter.ToUInt16(bs, 10);
            set.TimeDivision = LittleEndianConverter.ToInt16(bs, 12);
        }

        private void ReadTrackChunk(byte[] bs, DataSet set)
        {
            set.TrackList = new List<Track>();

            uint cur = 14;   // First track chunk byte
            for (int i = 0; i < set.TrackSize; i++)
            {
                if (Encoding.ASCII.GetString(bs.Skip((int)cur).Take(4).ToArray()) != "MTrk") throw new Exception("Couldn't read 'MTrk'");
                cur += 4;

                uint dataLength = LittleEndianConverter.ToUInt32(bs, (int)cur);
                cur += 4;

                set.TrackList.Add(InstantiateEvent(bs.Skip((int)cur).Take((int)dataLength).ToArray()));
                cur += dataLength;
            }
        }

        private Track InstantiateEvent(byte[] bs)
        {
            var eventList = new LinkedList<Event>();

            for (int i = 0; i < bs.Length; i++)
            {
                byte b, status = 0;

                var deltaTime = new VariableLengthQuantity(bs, i);
                i += deltaTime.BytesLength;

                if (((b = bs[i]) & 0x80) != 0)   // skip when using running status
                {
                    status = b;
                    i++;
                }

                switch (status & 0xf0)
                {
                    case 0x80:  // Note Off
                        eventList.AddLast(new NoteOff(deltaTime.Value, (status & 0x0f), bs[i], bs[i + 1]));
                        i++;
                        break;

                    case 0x90:  // Note On
                        if (bs[i + 1] == 0)   // 0x90 Note Off (velocity: 0)
                        {
                            eventList.AddLast(new NoteOff(deltaTime.Value, (status & 0x0f), bs[i], bs[i + 1]));
                        }
                        else
                        {
                            eventList.AddLast(new NoteOn(deltaTime.Value, (status & 0x0f), bs[i], bs[i + 1]));
                        }
                        i++;
                        break;

                    case 0xa0:  // Polyphonic Key Pressure
                        eventList.AddLast(new MIDIEvent(deltaTime.Value, (status & 0x0f)));
                        i++;
                        break;

                    case 0xb0:  // Control Change
                        b = bs[i++];
                        switch (b)
                        {
                            case 0x07:  // Volume
                                eventList.AddLast(new Volume(deltaTime.Value, (status & 0x0f), bs[i]));
                                break;
                            case 0x0a:  // Pan
                                eventList.AddLast(new Pan(deltaTime.Value, (status & 0x0f), bs[i]));
                                break;
                            default:    // Other Control Change
                                eventList.AddLast(new ControlChange(deltaTime.Value, (status & 0x0f), bs[i]));
                                break;
                        }
                        break;

                    case 0xc0:  // Program Change
                        eventList.AddLast(new ProgramChange(deltaTime.Value, (status & 0x0f), bs[i]));
                        break;

                    case 0xd0:  // Channel Pressure
                        eventList.AddLast(new MIDIEvent(deltaTime.Value, (status & 0x0f)));
                        break;

                    case 0xe0:  // Pitch Bend
                        eventList.AddLast(new MIDIEvent(deltaTime.Value, (status & 0x0f)));
                        i++;
                        break;

                    case 0xf0:
                        switch (status)
                        {
                            case 0xf0:
                            case 0xf7:  // SysEx Event
                                {
                                    var vlq = new VariableLengthQuantity(bs, i);
                                    i += vlq.BytesLength + (int)vlq.Value - 1;  // Jump to last byte
                                    eventList.AddLast(new SysExEvent(deltaTime.Value));
                                    break;
                                }

                            case 0xff:  // Meta Event
                                b = bs[i++];
                                switch (b)
                                {
                                    case 0x01:  // Text Event
                                        {
                                            var vlq = new VariableLengthQuantity(bs, i);
                                            i += vlq.BytesLength + (int)vlq.Value - 1;  // Jump to last byte
                                            eventList.AddLast(new MetaEvent(deltaTime.Value));
                                            break;
                                        }

                                    case 0x02:  // Copyright Notice
                                        {
                                            var vlq = new VariableLengthQuantity(bs, i);
                                            i += vlq.BytesLength + (int)vlq.Value - 1;  // Jump to last byte
                                            eventList.AddLast(new MetaEvent(deltaTime.Value));
                                            break;
                                        }

                                    case 0x03:  // Sequence/Track Name
                                        {
                                            var vlq = new VariableLengthQuantity(bs, i);
                                            i += vlq.BytesLength;   // Value 0 has 1 byte
                                            string text;
                                            if (vlq.Value == 0)
                                            {
                                                text = String.Empty;
                                            }
                                            else
                                            {
                                                text = Encoding.GetEncoding("shift_jis").GetString(bs.Skip(i).Take((int)vlq.Value).ToArray());
                                            }
                                            eventList.AddLast(new SequenceTrackName(deltaTime.Value, text));
                                            i += (int)vlq.Value - 1;    // Unmove cursor if value 0, else jump to last byte
                                            break;
                                        }

                                    case 0x04:  // Instrument Name
                                        {
                                            var vlq = new VariableLengthQuantity(bs, i);
                                            i += vlq.BytesLength + (int)vlq.Value - 1;  // Jump to last byte
                                            eventList.AddLast(new MetaEvent(deltaTime.Value));
                                            break;
                                        }

                                    case 0x05:  // Lyric
                                        {
                                            var vlq = new VariableLengthQuantity(bs, i);
                                            i += vlq.BytesLength + (int)vlq.Value - 1;  // Jump to last byte
                                            eventList.AddLast(new MetaEvent(deltaTime.Value));
                                            break;
                                        }

                                    case 0x06:  // Marker
                                        {
                                            var vlq = new VariableLengthQuantity(bs, i);
                                            i += vlq.BytesLength + (int)vlq.Value - 1;  // Jump to last byte
                                            eventList.AddLast(new MetaEvent(deltaTime.Value));
                                            break;
                                        }

                                    case 0x07:  // Cue Point
                                        {
                                            var vlq = new VariableLengthQuantity(bs, i);
                                            i += vlq.BytesLength + (int)vlq.Value - 1;  // Jump to last byte
                                            eventList.AddLast(new MetaEvent(deltaTime.Value));
                                            break;
                                        }

                                    case 0x08:  // Program Name
                                        {
                                            var vlq = new VariableLengthQuantity(bs, i);
                                            i += vlq.BytesLength + (int)vlq.Value - 1;  // Jump to last byte
                                            eventList.AddLast(new MetaEvent(deltaTime.Value));
                                            break;
                                        }

                                    case 0x09:  // Device Name
                                        {
                                            var vlq = new VariableLengthQuantity(bs, i);
                                            i += vlq.BytesLength + (int)vlq.Value - 1;  // Jump to last byte
                                            eventList.AddLast(new MetaEvent(deltaTime.Value));
                                            break;
                                        }

                                    case 0x20:  // MIDI Channel Prefix
                                        eventList.AddLast(new MetaEvent(deltaTime.Value));
                                        i++;
                                        break;

                                    case 0x21:  // MIDI Port Prefix
                                        eventList.AddLast(new MetaEvent(deltaTime.Value));
                                        i++;
                                        break;

                                    case 0x2f:  // End of Track
                                        eventList.AddLast(new EndOfTrack(deltaTime.Value));
                                        break;

                                    case 0x51:  // Set Tempo
                                        i++;
                                        int tempoValue = 0;
                                        for (int j = 0; j < 3; j++)
                                        {
                                            tempoValue <<= 8;
                                            tempoValue |= bs[i + j];
                                        }
                                        eventList.AddLast(new SetTempo(deltaTime.Value, tempoValue));
                                        i += 2;
                                        break;

                                    case 0x54:  // SMTPE Offset
                                        eventList.AddLast(new MetaEvent(deltaTime.Value));
                                        i += 5;
                                        break;

                                    case 0x58:  // Time Signature
                                        i++;
                                        eventList.AddLast(new TimeSignature(deltaTime.Value, bs[i], bs[i + 1], bs[i + 2], bs[i + 3]));
                                        i += 3;
                                        break;

                                    case 0x59:  // Key Signature
                                        i++;
                                        eventList.AddLast(new KeySignature(deltaTime.Value, (sbyte)bs[i], bs[i + 1]));
                                        i++;
                                        break;

                                    case 0x7f:  // Sequencer Specific Meta Event
                                        {
                                            var vlq = new VariableLengthQuantity(bs, i);
                                            i += vlq.BytesLength + (int)vlq.Value - 1;  // Jump to last byte
                                            eventList.AddLast(new MetaEvent(deltaTime.Value));
                                            break;
                                        }

                                    default:
                                        throw new Exception("Unknown Meta Event");
                                }
                                break;

                            default:
                                throw new Exception("Unknown 0xfx Event");
                        }
                        break;

                    default:
                        throw new Exception("Unknown Event");
                }
            }   // for

            return new Track(eventList);
        }

        private MIDI ConvertToFormat1(MIDI src)
        {
            try
            {
                Track srcTrack = src.TrackList[0];
                var newTracks = new List<Track>();
                int cnt = 0;        // event counter

                var eventlist = new LinkedList<Event>();
                uint deltaTime = 0;


                // Create Conductor track
                foreach (Event ev in srcTrack.EventList)
                {
                    deltaTime += ev.DeltaTime;

                    if (ev is MetaEvent)
                    {
                        MetaEvent modEv;
                        if (ev is SetTempo)
                        {
                            var st = (SetTempo)ev;
                            modEv = new SetTempo(deltaTime, st.Value);
                        }
                        else if (ev is TimeSignature)
                        {
                            var ts = (TimeSignature)ev;
                            modEv = new TimeSignature(deltaTime, ts.Numerator, ts.DenominatorBitShift, ts.MIDIClockPerMetronomeTick, ts.NumberOfNotesPerClocks);
                        }
                        else if (ev is KeySignature)
                        {
                            var ks = (KeySignature)ev;
                            modEv = new KeySignature(deltaTime, ks.SignatureNumber, ks.MinorFlagNumber);
                        }
                        else if (ev is SequenceTrackName)
                        {
                            var stn = (SequenceTrackName)ev;
                            modEv = new SequenceTrackName(deltaTime, stn.Name);
                        }
                        else if (ev is EndOfTrack)
                        {
                            modEv = new EndOfTrack(deltaTime);
                        }
                        else
                        {
                            modEv = new MetaEvent(deltaTime);
                        }
                        eventlist.AddLast(modEv);

                        deltaTime = 0;

                        if (!(ev is EndOfTrack))
                        {
                            cnt++;
                        }
                    }
                }
                newTracks.Add(new Track(eventlist));

                eventlist = new LinkedList<Event>();
                deltaTime = 0;


                // Create System Setup track
                foreach (Event ev in srcTrack.EventList)
                {
                    deltaTime += ev.DeltaTime;

                    if (ev is SysExEvent)
                    {
                        eventlist.AddLast(new SysExEvent(deltaTime));

                        deltaTime = 0;
                        cnt++;
                    }
                    else if (ev is EndOfTrack)
                    {
                        eventlist.AddLast(new EndOfTrack(deltaTime));
                    }
                }
                newTracks.Add(new Track(eventlist));


                // Create Notes track
                for (int ch = 0; cnt + 1 < srcTrack.EventList.Count; ch++)
                {
                    eventlist = new LinkedList<Event>();
                    deltaTime = 0;

                    foreach (Event ev in srcTrack.EventList)
                    {
                        deltaTime += ev.DeltaTime;

                        if (ev is MIDIEvent)
                        {
                            var midiEv = (MIDIEvent)ev;
                            if (midiEv.Channel == ch)
                            {
                                MIDIEvent modEv;
                                if (midiEv is NoteOn)
                                {
                                    var nton = (NoteOn)midiEv;
                                    modEv = new NoteOn(deltaTime, nton.Channel, nton.Number, nton.Velocity);
                                }
                                else if (midiEv is NoteOff)
                                {
                                    var ntoff = (NoteOff)midiEv;
                                    modEv = new NoteOff(deltaTime, ntoff.Channel, ntoff.Number, ntoff.Velocity);
                                }
                                else if (midiEv is ProgramChange)
                                {
                                    var pc = (ProgramChange)midiEv;
                                    modEv = new ProgramChange(deltaTime, pc.Channel, pc.Number);
                                }
                                else if (midiEv is Volume)
                                {
                                    var vol = (Volume)midiEv;
                                    modEv = new Volume(deltaTime, vol.Channel, vol.Value);
                                }
                                else if (midiEv is Pan)
                                {
                                    var pan = (Pan)midiEv;
                                    modEv = new Pan(deltaTime, pan.Channel, pan.Value);
                                }
                                else if (midiEv is ControlChange)
                                {
                                    var cc = (ControlChange)midiEv;
                                    modEv = new ControlChange(deltaTime, cc.Channel, cc.Value);
                                }
                                else
                                {
                                    var me = (MIDIEvent)midiEv;
                                    modEv = new MIDIEvent(deltaTime, me.Channel);
                                }
                                eventlist.AddLast(modEv);

                                deltaTime = 0;
                                cnt++;
                            }
                        }
                        else if (ev is EndOfTrack)
                        {
                            eventlist.AddLast(new EndOfTrack(deltaTime));
                        }
                    }
                    newTracks.Add(new Track(eventlist));
                }


                return new MIDI(newTracks, 1, newTracks.Count, src.TimeDivision);
            }
            catch (Exception ex)
            {
                throw new Exception("MIDIデータのフォーマット1への変換に失敗しました。", ex);
            }
        }
    }
}
