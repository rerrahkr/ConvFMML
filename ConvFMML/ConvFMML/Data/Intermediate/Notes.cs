using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.Intermediate
{
    public class Notes : ICloneable
    {
        public LinkedList<Event.NoteRest> NoteList { get; set; }
        public int NumberInTrack { get; set; }

        public Notes(LinkedList<Event.NoteRest> noteList, int numberInTrack)
        {
            NoteList = noteList;
            NumberInTrack = numberInTrack;
        }

        public void ModifyPositionsByCountsPerWholenote(int curCountsPerWholeNote, int newCountsPerWholeNote)
        {
            double ratio = (double)newCountsPerWholeNote / (double)curCountsPerWholeNote;
            var newList = new LinkedList<Event.NoteRest>();

            foreach (Event.NoteRest note in NoteList)
            {
                note.Start = Position.ConvertByTimeDivisionRatio(note.Start, ratio);
                note.End = Position.ConvertByTimeDivisionRatio(note.End, ratio);

                if (note.Start.CompareTo(note.End) != 0)
                {
                    newList.AddLast(note);
                }
            }

            NoteList = newList;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public Notes Clone()
        {
            var clone = (Notes)MemberwiseClone();

            clone.NoteList = new LinkedList<Event.NoteRest>();
            foreach (Event.NoteRest nt in NoteList)
            {
                clone.NoteList.AddLast(nt.Clone());
            }

            return clone;
        }
    }
}
