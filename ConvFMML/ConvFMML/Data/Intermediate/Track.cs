using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.Intermediate
{
    public class Track : ICloneable
    {
        public List<Notes> NotesList { get; set; }
        public LinkedList<Event.Instrument> InstrumentList { get; set; }
        public LinkedList<Event.Volume> VolumeList { get; set; }
        public LinkedList<Event.Pan> PanList { get; set; }
        public int Number { get; set; }
        public Position Length { get; set; }
        public string Name { get; set; }

        public int Polyphony
        {
            get
            {
                return NotesList.Count;
            }
        }

        public Track(
            List<Notes> notesList,
            LinkedList<Event.Instrument> instrumentList,
            LinkedList<Event.Volume> volumeList,
            LinkedList<Event.Pan> panList,
            int number,
            Position length,
            string name
            )
        {
            NotesList = notesList;
            InstrumentList = instrumentList;
            VolumeList = volumeList;
            PanList = panList;
            Number = number;
            Length = length;
            Name = name;
        }

        public void ModifyPositionsByCountsPerWholenote(int curCountsPerWholeNote, int newCountsPerWholeNote)
        {
            var tempList = new List<Event.NoteRest>();
            foreach (Notes ns in NotesList)
            {
                ns.ModifyPositionsByCountsPerWholenote(curCountsPerWholeNote, newCountsPerWholeNote);
                tempList.AddRange(ns.NoteList.ToList());
            }
            tempList = tempList.OrderBy(x => x.Start).ToList();

            var newNotesList = new List<Notes>();
            foreach (Event.NoteRest addNote in tempList)
            {
                ReorderNotes(newNotesList, addNote);
            }
            NotesList = newNotesList;

            double ratio = (double)newCountsPerWholeNote / (double)curCountsPerWholeNote;

            foreach (Event.Instrument inst in InstrumentList)
            {
                inst.Position = Position.ConvertByTimeDivisionRatio(inst.Position, ratio);
            }

            foreach (Event.Volume vol in VolumeList)
            {
                vol.Position = Position.ConvertByTimeDivisionRatio(vol.Position, ratio);
            }

            foreach (Event.Pan pan in PanList)
            {
                pan.Position = Position.ConvertByTimeDivisionRatio(pan.Position, ratio);
            }
        }

        private void ReorderNotes(List<Notes> newNotesList, Event.NoteRest addNote)
        {
            foreach (Notes notes in newNotesList)
            {
                LinkedList<Event.NoteRest> listInNotes = notes.NoteList;
                if (listInNotes.Last.Value.End.CompareTo(addNote.Start) <= 0)
                {
                    listInNotes.AddLast(addNote);
                    return;
                }
            }

            var newListInNotes = new LinkedList<Event.NoteRest>();
            newListInNotes.AddLast(addNote);
            newNotesList.Add(new Notes(newListInNotes, newNotesList.Count));
        }

        public List<NotesStatus> GetPartStatusList()
        {
            var list = new List<NotesStatus>();
            if (NotesList.Count == 0)
            {
                list.Add(new NotesStatus(Number, Name, 0, true));
            }
            else
            {
                foreach (Notes notes in NotesList)
                {
                    list.Add(new NotesStatus(Number, Name, notes.NumberInTrack, false));
                }
            }
            return list;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public Track Clone()
        {
            var clone = (Track)MemberwiseClone();

            clone.NotesList = new List<Notes>();
            NotesList.ForEach(x => clone.NotesList.Add(x.Clone()));

            clone.InstrumentList = new LinkedList<Event.Instrument>();
            foreach (Event.Instrument inst in InstrumentList)
            {
                clone.InstrumentList.AddLast(inst.Clone());
            }

            clone.VolumeList = new LinkedList<Event.Volume>();
            foreach (Event.Volume vol in VolumeList)
            {
                clone.VolumeList.AddLast(vol.Clone());
            }

            clone.PanList = new LinkedList<Event.Pan>();
            foreach (Event.Pan pan in PanList)
            {
                clone.PanList.AddLast(pan.Clone());
            }

            return clone;
        }
    }
}
