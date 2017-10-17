using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.Intermediate
{
    public class Intermediate : ICloneable
    {
        public List<Track> TrackList { get; set; }
        public LinkedList<Event.TimeSignature> TimeSignatureList { get; set; }
        public LinkedList<Event.KeySignature> KeySignatureList { get; set; }
        public LinkedList<Event.Tempo> TempoList { get; set; }
        public string Title { get; set; }
        private int _countsPerWholeNote;
        public int CountsPerWholeNote
        {
            get
            {
                return _countsPerWholeNote;
            }
            set
            {
                int prev = _countsPerWholeNote;
                _countsPerWholeNote = value;
                if (prev != 0)
                {
                    ModifyPositionsByCountsPerWholenote(prev, value);
                }
            }
        }

        public Intermediate(
            List<Track> trackList,
            LinkedList<Event.TimeSignature> timeSignatureList,
            LinkedList<Event.KeySignature> keysignatureList,
            LinkedList<Event.Tempo> tempoList,
            string title,
            int countsPerWholeNote
            )
        {
            TrackList = trackList;
            TimeSignatureList = timeSignatureList;
            KeySignatureList = keysignatureList;
            TempoList = tempoList;
            Title = title;
            CountsPerWholeNote = countsPerWholeNote;
        }

        public List<NotesStatus> GetNotesStatusList()
        {
            var list = new List<NotesStatus>();
            foreach (Track track in TrackList)
            {
                list.AddRange(track.GetPartStatusList());
            }
            return list;
        }

        public void ModifyPositionsByCountsPerWholenote(int countsPerWholeNote)
        {
            ModifyPositionsByCountsPerWholenote(_countsPerWholeNote, countsPerWholeNote);
        }

        private void ModifyPositionsByCountsPerWholenote(int curCountsPerWholeNote, int newCountsPerWholeNote)
        {
            double ratio = (double)newCountsPerWholeNote / (double)curCountsPerWholeNote;

            foreach (Event.TimeSignature ts in TimeSignatureList)
            {
                ts.TickPerBar = (uint)(ts.TickPerBar * ratio);
                ts.Position = Position.ConvertByTimeDivisionRatio(ts.Position, ratio, null);
                ts.PrevSignedPosition = Position.ConvertByTimeDivisionRatio(ts.PrevSignedPosition, ratio, TimeSignatureList);
            }

            foreach (Event.KeySignature ks in KeySignatureList)
            {
                ks.Position = Position.ConvertByTimeDivisionRatio(ks.Position, ratio, TimeSignatureList);
            }

            foreach (Event.Tempo tp in TempoList)
            {
                tp.Position = Position.ConvertByTimeDivisionRatio(tp.Position, ratio, TimeSignatureList);
            }

            foreach (Track trk in TrackList)
            {
                trk.ModifyPositionsByCountsPerWholenote(curCountsPerWholeNote, newCountsPerWholeNote, TimeSignatureList);
            }
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public Intermediate Clone()
        {
            var clone = (Intermediate)MemberwiseClone();

            clone.TrackList = new List<Track>();
            TrackList.ForEach(x => clone.TrackList.Add(x.Clone()));

            clone.TimeSignatureList = new LinkedList<Event.TimeSignature>();
            foreach (Event.TimeSignature ts in TimeSignatureList)
            {
                clone.TimeSignatureList.AddLast(ts.Clone());
            }

            clone.KeySignatureList = new LinkedList<Event.KeySignature>();
            foreach (Event.KeySignature ks in KeySignatureList)
            {
                clone.KeySignatureList.AddLast(ks.Clone());
            }

            clone.TempoList = new LinkedList<Event.Tempo>();
            foreach (Event.Tempo tp in TempoList)
            {
                clone.TempoList.AddLast(tp.Clone());
            }

            return clone;
        }
    }
}
