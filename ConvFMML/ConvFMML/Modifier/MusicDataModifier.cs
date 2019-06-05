using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConvFMML.Data.Intermediate;
using ConvFMML.Data.Intermediate.Event;

namespace ConvFMML.Modifier
{
    public abstract class MusicDataModifier
    {
        private class CommandPartSet
        {
            public LinkedList<ChangeEventSet> InstrumentSetList { get; set; }
            public LinkedList<ChangeEventSet> VolumeSetList { get; set; }
            public LinkedList<ChangeEventSet> PanSetList { get; set; }
        }

        public static MusicDataModifier Factory(MMLStyle mmlStyle)
        {
            MusicDataModifier instance;

            switch (mmlStyle)
            {
                case MMLStyle.Custom:
                    instance = new CustomMusicDataModifier();
                    break;
                case MMLStyle.FMP:
                    instance = new FMPMusicDataModifier();
                    break;
                case MMLStyle.FMP7:
                    instance = new FMP7MusicDataModifier();
                    break;
                case MMLStyle.MXDRV:
                    instance = new MXDRVMusicDataModifier();
                    break;
                case MMLStyle.NRTDRV:
                    instance = new NRTDRVMusicDataModifier();
                    break;
                case MMLStyle.PMD:
                    instance = new PMDMusicDataModifier();
                    break;
                case MMLStyle.MUCOM88:
                    instance = new MUCOM88MusicDataModifier();
                    break;
                case MMLStyle.Mml2vgm:
                    instance = new Mml2vgmMusicDataModifier();
                    break;
                default:
                    instance = null;
                    break;
            }

            return instance;
        }

        public Intermediate Modify(Intermediate src, Settings settings, List<NotesStatus> statusList)
        {
            Intermediate mod = ConstructPartFromTrack(src, settings, statusList);

            mod = InsertRests(mod);

            LinkedList<TempoSet> tempoList = CreateTempoSetList(mod.TempoList);
            List<CommandPartSet> commandPartList = CreateCommandPartSetList(mod.TrackList);


            if (settings.controlCommand.generic.Invalid == 1)
            {
                DeleteInvalidControlCommands(mod, commandPartList);
            }

            if (settings.controlCommand.generic.SamePosition == 1)
            {
                ModifySamePositionControlCommands(mod, tempoList, commandPartList);
            }

            if (settings.controlCommand.generic.Predeclared == 1)
            {
                DeletePredeclaredControlCommands(mod, tempoList, commandPartList, settings, statusList);
            }

            mod = ConvertTempoSetList(mod, tempoList);
            mod = ConvertCommandPartSetList(mod, commandPartList);


            mod = CutNoteByControlCommands(mod);

            if (settings.noteRest.CutByBar != 0)
            {
                mod = CutNoteByBar(mod);
            }


            return mod;
        }

        private Intermediate ConstructPartFromTrack(Intermediate src, Settings settings, List<NotesStatus> statusList)
        {
            try
            {
                Settings.ControlCommand comSettings = settings.controlCommand;
                var newTrackList = new List<Track>();

                foreach (NotesStatus ns in statusList)
                {
                    if (ns.Printable)
                    {
                        Track track = src.TrackList.Find(x => x.Number == ns.TrackNumber);

                        var newNotesList = new List<Notes>();
                        if (track.Polyphony > 0)
                        {
                            Notes notes = track.NotesList.Find(x => x.NumberInTrack == ns.NumberInTrack);
                            newNotesList.Add(notes);
                        }

                        var newInstList = new LinkedList<Instrument>();
                        if (comSettings.programChange.Enable)
                        {
                            foreach (Instrument inst in track.InstrumentList)
                            {
                                newInstList.AddLast(inst.Clone());
                            }
                        }

                        var newVolumeList = new LinkedList<Volume>();
                        if (comSettings.volume.Enable)
                        {
                            foreach (Volume vol in track.VolumeList)
                            {
                                newVolumeList.AddLast(vol.Clone());
                            }
                        }

                        var newPanList = new LinkedList<Pan>();
                        newPanList = ClonePanList(newPanList, track.PanList, settings, ns.SoundModule);


                        newTrackList.Add(new Track(newNotesList, newInstList, newVolumeList, newPanList, track.Number, track.Length, ns.Name));
                    }
                }

                LinkedList<Tempo> newTempoList;
                if (comSettings.tempo.Enable)
                {
                    newTempoList = src.TempoList;
                }
                else
                {
                    newTempoList = new LinkedList<Tempo>();
                }

                return new Intermediate(newTrackList, src.TimeSignatureList, src.KeySignatureList, newTempoList, src.Title, src.CountsPerWholeNote);
            }
            catch (Exception ex)
            {
                throw new Exception("パートの再構築に失敗しました。", ex);
            }
        }

        protected virtual LinkedList<Pan> ClonePanList(LinkedList<Pan> newList, LinkedList<Pan> srcList, Settings settings, SoundModule module)
        {
            if (settings.controlCommand.pan.Enable && module == SoundModule.FM)
            {
                foreach (Pan pan in srcList)
                {
                    newList.AddLast(pan.Clone());
                }
            }
            return newList;
        }

        private Intermediate InsertRests(Intermediate src)
        {
            var query = src.TrackList.Select(x => x.Length);
            Position newLength = query.Max();

            var newTrackList = new List<Track>();
            foreach (Track track in src.TrackList)
            {
                Notes notes = track.NotesList[0];
                var newNoteList = new LinkedList<NoteRest>();

                if (notes.NoteList.Count > 0)
                {
                    LinkedListNode<NoteRest> noteNode = notes.NoteList.First;
                    var prevEnd = new Position(1, 0);

                    for (; noteNode != null; noteNode = noteNode.Next)
                    {
                        if (noteNode.Value.Start.CompareTo(prevEnd) > 0)
                        {
                            newNoteList.AddLast(new Rest(prevEnd.Clone(), noteNode.Value.Start.Clone()));
                        }
                        newNoteList.AddLast(noteNode.Value);
                        prevEnd = noteNode.Value.End;
                    }

                    if (newNoteList.Last.Value.End.CompareTo(newLength) < 0)
                    {
                        newNoteList.AddLast(new Rest(newNoteList.Last.Value.End.Clone(), newLength.Clone()));
                    }
                }
                else
                {
                    newNoteList.AddLast(new Rest(new Position(1, 0), newLength.Clone()));
                }

                var newNotesList = new List<Notes>();
                newNotesList.Add(new Notes(newNoteList, notes.NumberInTrack));
                newTrackList.Add(new Track(newNotesList, track.InstrumentList, track.VolumeList, track.PanList, track.Number, newLength, track.Name));
            }

            return new Intermediate(newTrackList, src.TimeSignatureList, src.KeySignatureList, src.TempoList, src.Title, src.CountsPerWholeNote);
        }

        private LinkedList<TempoSet> CreateTempoSetList(LinkedList<Tempo> tempoList)
        {
            var list = new LinkedList<TempoSet>();
            foreach (Tempo t in tempoList)
            {
                list.AddLast(CreateTempoSet(t));
            }
            return list;
        }

        private List<CommandPartSet> CreateCommandPartSetList(List<Track> trackList)
        {
            var list = new List<CommandPartSet>();

            foreach (Track track in trackList)
            {
                var set = new CommandPartSet();

                var instlist = new LinkedList<ChangeEventSet>();
                foreach (Instrument i in track.InstrumentList)
                {
                    instlist.AddLast(CreateInstrumentSet(i));
                }
                set.InstrumentSetList = instlist;

                var vollist = new LinkedList<ChangeEventSet>();
                foreach (Volume v in track.VolumeList)
                {
                    vollist.AddLast(CreateVolumeSet(v));
                }
                set.VolumeSetList = vollist;

                var panlist = new LinkedList<ChangeEventSet>();
                foreach (Pan p in track.PanList)
                {
                    panlist.AddLast(CreatePanSet(p));
                }
                set.PanSetList = panlist;

                list.Add(set);
            }

            return list;
        }

        protected abstract TempoSet CreateTempoSet(Tempo t);

        protected abstract ChangeEventSet CreateInstrumentSet(Instrument i);

        protected abstract ChangeEventSet CreateVolumeSet(Volume v);

        protected abstract ChangeEventSet CreatePanSet(Pan p);

        private void DeleteInvalidControlCommands(Intermediate src, List<CommandPartSet> partSet)
        {
            try
            {
                int i = 0;
                foreach (Track track in src.TrackList)
                {
                    LinkedListNode<NoteRest> noteNode = track.NotesList[0].NoteList.First;
                    var newInstrumentSetList = new LinkedList<ChangeEventSet>();
                    LinkedListNode<ChangeEventSet> instNode = partSet[i].InstrumentSetList.First;

                    while (noteNode != null && instNode != null)
                    {
                        NoteRest curNote = noteNode.Value;
                        ChangeEventSet curInst = instNode.Value;

                        if (curNote.End.CompareTo(curInst.Position) <= 0)
                        {
                            noteNode = noteNode.Next;
                            continue;
                        }

                        if (curNote is Rest)
                        {
                            if (instNode.Next != null)
                            {
                                if (instNode.Next.Value.Position.CompareTo(curNote.End) > 0)
                                {
                                    newInstrumentSetList.AddLast(curInst);
                                }
                            }
                            else
                            {
                                if (noteNode.Next != null)
                                {
                                    newInstrumentSetList.AddLast(curInst);
                                }
                            }
                        }
                        else
                        {
                            newInstrumentSetList.AddLast(curInst);
                        }

                        instNode = instNode.Next;
                    }
                    partSet[i].InstrumentSetList = newInstrumentSetList;


                    noteNode = track.NotesList[0].NoteList.First;
                    var newVolumeSetList = new LinkedList<ChangeEventSet>();
                    LinkedListNode<ChangeEventSet> volNode = partSet[i].VolumeSetList.First;

                    while (noteNode != null && volNode != null)
                    {
                        NoteRest curNote = noteNode.Value;
                        ChangeEventSet curVol = volNode.Value;

                        if (curNote.End.CompareTo(curVol.Position) <= 0)
                        {
                            noteNode = noteNode.Next;
                            continue;
                        }

                        if (curNote is Rest)
                        {
                            if (volNode.Next != null)
                            {
                                if (volNode.Next.Value.Position.CompareTo(curNote.End) > 0)
                                {
                                    newVolumeSetList.AddLast(curVol);
                                }
                            }
                            else
                            {
                                if (noteNode.Next != null)
                                {
                                    newVolumeSetList.AddLast(curVol);
                                }
                            }
                        }
                        else
                        {
                            newVolumeSetList.AddLast(curVol);
                        }

                        volNode = volNode.Next;
                    }
                    partSet[i].VolumeSetList = newVolumeSetList;


                    noteNode = track.NotesList[0].NoteList.First;
                    var newPansetList = new LinkedList<ChangeEventSet>();
                    LinkedListNode<ChangeEventSet> panNode = partSet[i].PanSetList.First;

                    while (noteNode != null && panNode != null)
                    {
                        NoteRest curNote = noteNode.Value;
                        ChangeEventSet curPan = panNode.Value;

                        if (curNote.End.CompareTo(curPan.Position) <= 0)
                        {
                            noteNode = noteNode.Next;
                            continue;
                        }

                        if (curNote is Rest)
                        {
                            if (panNode.Next != null)
                            {
                                if (panNode.Next.Value.Position.CompareTo(curNote.End) > 0)
                                {
                                    newPansetList.AddLast(curPan);
                                }
                            }
                            else
                            {
                                if (noteNode.Next != null)
                                {
                                    newPansetList.AddLast(curPan);
                                }
                            }
                        }
                        else
                        {
                            newPansetList.AddLast(curPan);
                        }

                        panNode = panNode.Next;
                    }
                    partSet[i].PanSetList = newPansetList;

                    i++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("音符に影響を与えないコマンドの削除に失敗しました。", ex);
            }
        }

        private void ModifySamePositionControlCommands(Intermediate src, LinkedList<TempoSet> tempoSetList, List<CommandPartSet> partSet)
        {
            try
            {
                foreach (CommandPartSet set in partSet)
                {
                    var newInstrumentSetList = new LinkedList<ChangeEventSet>();
                    LinkedListNode<ChangeEventSet> curInstNode = set.InstrumentSetList.First;
                    while (curInstNode != null)
                    {
                        if (curInstNode.Previous != null)
                        {
                            if (curInstNode.Value.Position.CompareTo(curInstNode.Previous.Value.Position) == 0)
                            {
                                newInstrumentSetList.RemoveLast();
                            }
                        }
                        newInstrumentSetList.AddLast(curInstNode.Value);
                        curInstNode = curInstNode.Next;
                    }
                    set.InstrumentSetList = newInstrumentSetList;

                    var newVolumeSetList = new LinkedList<ChangeEventSet>();
                    LinkedListNode<ChangeEventSet> curVolNode = set.VolumeSetList.First;
                    while (curVolNode != null)
                    {
                        if (curVolNode.Previous != null)
                        {
                            if (curVolNode.Value.Position.CompareTo(curVolNode.Previous.Value.Position) == 0)
                            {
                                newVolumeSetList.RemoveLast();
                            }
                        }
                        newVolumeSetList.AddLast(curVolNode.Value);
                        curVolNode = curVolNode.Next;
                    }
                    set.VolumeSetList = newVolumeSetList;

                    var newPanSetList = new LinkedList<ChangeEventSet>();
                    LinkedListNode<ChangeEventSet> curPanNode = set.PanSetList.First;
                    while (curPanNode != null)
                    {
                        if (curPanNode.Previous != null)
                        {
                            if (curPanNode.Value.Position.CompareTo(curPanNode.Previous.Value.Position) == 0)
                            {
                                newPanSetList.RemoveLast();
                            }
                        }
                        newPanSetList.AddLast(curPanNode.Value);
                        curPanNode = curPanNode.Next;
                    }
                    set.PanSetList = newPanSetList;
                }

                var newTempoSetList = new LinkedList<TempoSet>();
                LinkedListNode<TempoSet> curTempoNode = tempoSetList.First;
                while (curTempoNode != null)
                {
                    if (curTempoNode.Previous != null)
                    {
                        if (curTempoNode.Value.Position.CompareTo(curTempoNode.Previous.Value.Position) == 0)
                        {
                            newTempoSetList.RemoveLast();
                        }
                    }
                    newTempoSetList.AddLast(curTempoNode.Value);
                    curTempoNode = curTempoNode.Next;
                }
                tempoSetList = newTempoSetList;
            }
            catch (Exception ex)
            {
                throw new Exception("同位置同種のコマンドの処理に失敗しました。", ex);
            }
        }

        private void DeletePredeclaredControlCommands(Intermediate src, LinkedList<TempoSet> tempoSetList, List<CommandPartSet> partSet, Settings settings, List<NotesStatus> statusList)
        {
            try
            {
                NotesStatus status = null;
                int i = 0;
                foreach (CommandPartSet set in partSet)
                {
                    NotesStatus sts = statusList.Find(x => (x.TrackNumber == src.TrackList[i].Number && x.NumberInTrack == src.TrackList[i].NotesList[0].NumberInTrack));
                    if (i == 0)
                    {
                        status = sts;
                    }

                    var newInstrumentSetList = new LinkedList<ChangeEventSet>();
                    LinkedListNode<ChangeEventSet> curInstNode = set.InstrumentSetList.First;
                    while (curInstNode != null)
                    {
                        if (curInstNode.Previous == null ||
                            curInstNode.Value.ToString(settings, sts.SoundModule) != curInstNode.Previous.Value.ToString(settings, sts.SoundModule))
                        {
                            newInstrumentSetList.AddLast(curInstNode.Value);
                        }
                        curInstNode = curInstNode.Next;
                    }
                    set.InstrumentSetList = newInstrumentSetList;

                    var newVolumeSetList = new LinkedList<ChangeEventSet>();
                    LinkedListNode<ChangeEventSet> curVolNode = set.VolumeSetList.First;
                    while (curVolNode != null)
                    {
                        if (curVolNode.Previous == null ||
                            curVolNode.Value.ToString(settings, sts.SoundModule) != curVolNode.Previous.Value.ToString(settings, sts.SoundModule))
                        {
                            newVolumeSetList.AddLast(curVolNode.Value);
                        }
                        curVolNode = curVolNode.Next;
                    }
                    set.VolumeSetList = newVolumeSetList;

                    var newPanSetList = new LinkedList<ChangeEventSet>();
                    LinkedListNode<ChangeEventSet> curPanNode = set.PanSetList.First;
                    while (curPanNode != null)
                    {
                        if (curPanNode.Previous == null ||
                            curPanNode.Value.ToString(settings, sts.SoundModule) != curPanNode.Previous.Value.ToString(settings, sts.SoundModule))
                        {
                            newPanSetList.AddLast(curPanNode.Value);
                        }
                        curPanNode = curPanNode.Next;
                    }
                    set.PanSetList = newPanSetList;

                    i++;
                }

                var newTempoSetList = new LinkedList<TempoSet>();
                LinkedListNode<TempoSet> curTempoNode = tempoSetList.First;
                while (curTempoNode != null)
                {
                    if (curTempoNode.Previous == null ||
                        curTempoNode.Value.ToString(settings, status.SoundModule) != curTempoNode.Previous.Value.ToString(settings, status.SoundModule))
                    {
                        newTempoSetList.AddLast(curTempoNode.Value);
                    }
                    curTempoNode = curTempoNode.Next;
                }
                tempoSetList = newTempoSetList;
            }
            catch (Exception ex)
            {
                throw new Exception("宣言済みコマンドの削除に失敗しました。", ex);
            }
        }

        private Intermediate ConvertTempoSetList(Intermediate src, LinkedList<TempoSet> tempoSetList)
        {
            return new Intermediate(src.TrackList, src.TimeSignatureList, src.KeySignatureList, ConvertTempoSetList(tempoSetList), src.Title, src.CountsPerWholeNote);
        }

        private Intermediate ConvertCommandPartSetList(Intermediate src, List<CommandPartSet> commandPartList)
        {
            var newTrackList = new List<Track>();

            int i = 0;
            foreach (Track track in src.TrackList)
            {
                CommandPartSet set = commandPartList[i];
                newTrackList.Add(new Track(track.NotesList, ConvertInstrumentSetList(set.InstrumentSetList), ConvertVolumeSetList(set.VolumeSetList), ConvertPanSetList(set.PanSetList), track.Number, track.Length, track.Name));
                i++;
            }

            return new Intermediate(newTrackList, src.TimeSignatureList, src.KeySignatureList, src.TempoList, src.Title, src.CountsPerWholeNote);
        }

        private LinkedList<Tempo> ConvertTempoSetList(LinkedList<TempoSet> setList)
        {
            var list = new LinkedList<Tempo>();
            setList.ToList().ForEach(x => list.AddLast(x.Data));
            return list;
        }

        private LinkedList<Instrument> ConvertInstrumentSetList(LinkedList<ChangeEventSet> setList)
        {
            var list = new LinkedList<Instrument>();
            setList.ToList().ForEach(x => list.AddLast((Instrument)x.Data));
            return list;
        }

        private LinkedList<Volume> ConvertVolumeSetList(LinkedList<ChangeEventSet> setList)
        {
            var list = new LinkedList<Volume>();
            setList.ToList().ForEach(x => list.AddLast((Volume)x.Data));
            return list;
        }

        private LinkedList<Pan> ConvertPanSetList(LinkedList<ChangeEventSet> setList)
        {
            var list = new LinkedList<Pan>();
            setList.ToList().ForEach(x => list.AddLast((Pan)x.Data));
            return list;
        }

        private Intermediate CutNoteByControlCommands(Intermediate src)
        {
            try
            {
                foreach (Track track in src.TrackList)
                {
                    LinkedListNode<NoteRest> noteNode = track.NotesList[0].NoteList.First;
                    if (src.TrackList.IndexOf(track) == 0)
                    {
                        LinkedListNode<Tempo> tempoNode = src.TempoList.First;

                        while (tempoNode != null)
                        {
                            NoteRest curNote = noteNode.Value;
                            Tempo curTempo = tempoNode.Value;

                            if (curNote.End.CompareTo(curTempo.Position) <= 0)
                            {
                                noteNode = noteNode.Next;
                                continue;
                            }

                            if (curNote.Start.CompareTo(curTempo.Position) < 0)
                            {
                                NoteRest newNote;
                                if (curNote is Note)
                                {
                                    var nt = (Note)curNote;
                                    newNote = new Note(curTempo.Position.Clone(), nt.End.Clone(), nt.KeyNumber, nt.Velocity);
                                }
                                else
                                {
                                    var rst = (Rest)curNote;
                                    newNote = new Rest(curTempo.Position.Clone(), rst.End.Clone());
                                }
                                track.NotesList[0].NoteList.AddAfter(noteNode, newNote);

                                curNote.End = curTempo.Position.Clone();
                                curNote.TieFlag = true;
                            }

                            tempoNode = tempoNode.Next;
                        }
                    }


                    noteNode = track.NotesList[0].NoteList.First;
                    LinkedListNode<Instrument> instNode = track.InstrumentList.First;

                    while (instNode != null)
                    {
                        NoteRest curNote = noteNode.Value;
                        Instrument curInst = instNode.Value;

                        if (curNote.End.CompareTo(curInst.Position) <= 0)
                        {
                            noteNode = noteNode.Next;
                            continue;
                        }

                        if (curNote.Start.CompareTo(curInst.Position) < 0)
                        {
                            NoteRest newNote;
                            if (curNote is Note)
                            {
                                var nt = (Note)curNote;
                                newNote = new Note(curInst.Position.Clone(), nt.End.Clone(), nt.KeyNumber, nt.Velocity);
                            }
                            else
                            {
                                var rst = (Rest)curNote;
                                newNote = new Rest(curInst.Position.Clone(), rst.End.Clone());
                            }
                            track.NotesList[0].NoteList.AddAfter(noteNode, newNote);

                            curNote.End = curInst.Position.Clone();
                            curNote.TieFlag = true;
                        }

                        instNode = instNode.Next;
                    }


                    noteNode = track.NotesList[0].NoteList.First;
                    LinkedListNode<Volume> volNode = track.VolumeList.First;

                    while (volNode != null)
                    {
                        NoteRest curNote = noteNode.Value;
                        Volume curVol = volNode.Value;

                        if (curNote.End.CompareTo(curVol.Position) <= 0)
                        {
                            noteNode = noteNode.Next;
                            continue;
                        }

                        if (curNote.Start.CompareTo(curVol.Position) < 0)
                        {
                            NoteRest newNote;
                            if (curNote is Note)
                            {
                                var nt = (Note)curNote;
                                newNote = new Note(curVol.Position.Clone(), nt.End.Clone(), nt.KeyNumber, nt.Velocity);
                            }
                            else
                            {
                                var rst = (Rest)curNote;
                                newNote = new Rest(curVol.Position.Clone(), rst.End.Clone());
                            }
                            track.NotesList[0].NoteList.AddAfter(noteNode, newNote);

                            curNote.End = curVol.Position.Clone();
                            curNote.TieFlag = true;
                        }

                        volNode = volNode.Next;
                    }


                    noteNode = track.NotesList[0].NoteList.First;
                    LinkedListNode<Pan> panNode = track.PanList.First;

                    while (panNode != null)
                    {
                        NoteRest curNote = noteNode.Value;
                        Pan curPan = panNode.Value;

                        if (curNote.End.CompareTo(curPan.Position) <= 0)
                        {
                            noteNode = noteNode.Next;
                            continue;
                        }

                        if (curNote.Start.CompareTo(curPan.Position) < 0)
                        {
                            NoteRest newNote;
                            if (curNote is Note)
                            {
                                var nt = (Note)curNote;
                                newNote = new Note(curPan.Position.Clone(), nt.End.Clone(), nt.KeyNumber, nt.Velocity);
                            }
                            else
                            {
                                var rst = (Rest)curNote;
                                newNote = new Rest(curPan.Position.Clone(), rst.End.Clone());
                            }
                            track.NotesList[0].NoteList.AddAfter(noteNode, newNote);

                            curNote.End = curPan.Position.Clone();
                            curNote.TieFlag = true;
                        }

                        panNode = panNode.Next;
                    }
                }


                return src;
            }
            catch (Exception ex)
            {
                throw new Exception("音符・休符のコマンドによるカットに失敗しました。", ex);
            }
        }

        private Intermediate CutNoteByBar(Intermediate src)
        {
            try
            {
                foreach (Track track in src.TrackList)
                {
                    LinkedListNode<NoteRest> noteNode = track.NotesList[0].NoteList.First;
                    for (; noteNode != null; noteNode = noteNode.Next)
                    {
                        NoteRest nr = noteNode.Value;
                        if (nr.Start.Bar < nr.End.Bar &&
                            (nr.End.Bar != nr.Start.Bar + 1 || nr.End.Tick != 0))
                        {
                            NoteRest newNr;
                            var newPos = new Position(nr.Start.Bar + 1, 0);

                            if (nr is Note)
                            {
                                var note = (Note)nr;
                                newNr = new Note(newPos.Clone(), note.End.Clone(), note.KeyNumber, note.Velocity);
                            }
                            else
                            {
                                var rest = (Rest)nr;
                                newNr = new Rest(newPos.Clone(), rest.End.Clone());
                            }
                            track.NotesList[0].NoteList.AddAfter(noteNode, newNr);

                            nr.End = newPos.Clone();
                            nr.TieFlag = true;
                        }
                    }
                }

                return src;
            }
            catch (Exception ex)
            {
                throw new Exception("小節による音符・休符のカットに失敗しました。", ex);
            }
        }
    }
}
