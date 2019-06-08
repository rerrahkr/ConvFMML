using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConvFMML.Properties;

namespace ConvFMML.Converter
{
    public abstract class IntermediateToMMLConverter
    {
        public static IntermediateToMMLConverter Factory(MMLStyle mmlStyle)
        {
            IntermediateToMMLConverter instance;

            switch (mmlStyle)
            {
                case MMLStyle.Custom:
                    instance = new IntermediateToCustomMMLConverter();
                    break;
                case MMLStyle.FMP:
                    instance = new IntermediateToFMPMMLConverter();
                    break;
                case MMLStyle.FMP7:
                    instance = new IntermediateToFMP7MMLConverter();
                    break;
                case MMLStyle.MXDRV:
                    instance = new IntermediateToMXDRVMMLConverter();
                    break;
                case MMLStyle.NRTDRV:
                    instance = new IntermediateToNRTDRVMMLConverter();
                    break;
                case MMLStyle.PMD:
                    instance = new IntermediateToPMDMMLConverter();
                    break;
                case MMLStyle.MUCOM88:
                    instance = new IntermediateToMUCOM88MMLConverter();
                    break;
                case MMLStyle.Mml2vgm:
                    instance = new IntermediateToMml2vgmMMLConverter();
                    break;
                default:
                    instance = null;
                    break;
            }

            return instance;
        }

        public Data.MML.MML Convert(Data.Intermediate.Intermediate intermediate, Settings settings, List<Data.Intermediate.NotesStatus> statusList)
        {
            try
            {
                List<Data.MML.Part> partlist = CreatePartList(intermediate, settings, statusList);

                return CreateMMLInstance(partlist, intermediate.Title, intermediate.CountsPerWholeNote);
            }
            catch (Exception ex)
            {
                throw new Exception(Resources.ErrorConverterFailedToMML, ex);
            }
        }

        private List<Data.MML.Part> CreatePartList(Data.Intermediate.Intermediate intermediate, Settings settings, List<Data.Intermediate.NotesStatus> statusList)
        {
            int[][] lentable = CreateLengthTable((uint)settings.mmlExpression.TimeBase, settings.noteRest);


            var partList = new List<Data.MML.Part>();

            LinkedListNode<Data.Intermediate.Event.Tempo> tempoNode = intermediate.TempoList.First;
            foreach (Data.Intermediate.Track track in intermediate.TrackList)
            {
                Data.Intermediate.Notes notes = track.NotesList[0];
                Data.Intermediate.NotesStatus status = statusList.Find(x => (x.TrackNumber == track.Number && x.NumberInTrack == notes.NumberInTrack));

                LinkedListNode<Data.Intermediate.Event.NoteRest> noteNode = notes.NoteList.First;
                LinkedListNode<Data.Intermediate.Event.Instrument> instNode = track.InstrumentList.First;
                LinkedListNode<Data.Intermediate.Event.Volume> volNode = track.VolumeList.First;
                LinkedListNode<Data.Intermediate.Event.Pan> panNode = track.PanList.First;
                LinkedListNode<Data.Intermediate.Event.KeySignature> ksNode = intermediate.KeySignatureList.First;
                LinkedListNode<Data.Intermediate.Event.TimeSignature> tsNode = intermediate.TimeSignatureList.First;

                int barCnt = 1;
                Key key = Key.CMaj;
                var barList = new List<Data.MML.Bar>();

                for (int i = 1; i <= track.Length.Bar; i++)
                {
                    var comList = new LinkedList<Data.MML.Command.Command>();
                    bool prevTied = false;

                    MMLCommandRelation relation = MMLCommandRelation.Clear;
                    while (true)
                    {
                        string addCommandName;
                        if (intermediate.TrackList.IndexOf(track) == 0)
                        {
                            addCommandName = GetEarliestCommandName(ksNode?.Value, tempoNode?.Value, instNode?.Value, volNode?.Value, panNode?.Value, noteNode?.Value, i, prevTied, settings);

                        }
                        else
                        {
                            addCommandName = GetEarliestCommandName(ksNode?.Value, null, instNode?.Value, volNode?.Value, panNode?.Value, noteNode?.Value, i, prevTied, settings);
                        }

                        Data.MML.Command.Command addCommand = null;
                        if (addCommandName == typeof(Data.Intermediate.Event.KeySignature).Name)
                        {
                            key = ksNode.Value.Key;
                            ksNode = ksNode.Next;
                            continue;
                        }
                        else if (addCommandName == typeof(Data.Intermediate.Event.Tempo).Name)
                        {
                            if (comList.Count > 0)
                            {
                                comList.Last.Value.CommandRelation |= MMLCommandRelation.NextControl;
                            }

                            addCommand = CreateTempoInstance(tempoNode.Value.Value, relation);

                            relation |= MMLCommandRelation.PrevControl;
                            tempoNode = tempoNode.Next;
                        }
                        else if (addCommandName == typeof(Data.Intermediate.Event.Instrument).Name)
                        {
                            if (comList.Count > 0)
                            {
                                comList.Last.Value.CommandRelation |= MMLCommandRelation.NextControl;
                            }

                            addCommand = CreateInstrumentInstance(instNode.Value.Value, relation);

                            relation |= MMLCommandRelation.PrevControl;
                            instNode = instNode.Next;
                        }
                        else if (addCommandName == typeof(Data.Intermediate.Event.Volume).Name)
                        {
                            if (comList.Count > 0)
                            {
                                comList.Last.Value.CommandRelation |= MMLCommandRelation.NextControl;
                            }

                            addCommand = CreateVolumeInstance(volNode.Value.Value, relation);

                            relation |= MMLCommandRelation.PrevControl;
                            volNode = volNode.Next;
                        }
                        else if (addCommandName == typeof(Data.Intermediate.Event.Pan).Name)
                        {
                            if (comList.Count > 0)
                            {
                                comList.Last.Value.CommandRelation |= MMLCommandRelation.NextControl;
                            }

                            addCommand = CreatePanInstance(panNode.Value.Value, relation);

                            relation |= MMLCommandRelation.PrevControl;
                            panNode = panNode.Next;
                        }
                        else if (addCommandName == typeof(Data.Intermediate.Event.Note).Name)
                        {
                            if (comList.Count > 0)
                            {
                                comList.Last.Value.CommandRelation &= ~MMLCommandRelation.NextControl;
                            }

                            var note = (Data.Intermediate.Event.Note)noteNode.Value;
                            Data.Intermediate.Position poslen = CalculateLength(note.Start, note.End, intermediate.TimeSignatureList, (int)settings.mmlExpression.TimeBase);
                            List<int> length = ConvertLengthFormat(poslen, lentable);
                            MMLCommandRelation tempRel = relation;
                            if (note.TieFlag)
                            {
                                tempRel |= MMLCommandRelation.TieAfter;
                                relation |= MMLCommandRelation.TieBefore;
                            }
                            else
                            {
                                tempRel &= ~MMLCommandRelation.TieAfter;
                                relation &= ~MMLCommandRelation.TieBefore;
                            }

                            addCommand = CreateNoteInstance((note.KeyNumber / 12 - 1), Tables.NoteNameDictionary[key][note.KeyNumber % 12], length, tempRel);

                            relation &= ~MMLCommandRelation.PrevControl;
                            prevTied = note.TieFlag;
                            noteNode = noteNode.Next;
                        }
                        else if (addCommandName == typeof(Data.Intermediate.Event.Rest).Name)
                        {
                            if (comList.Count > 0)
                            {
                                comList.Last.Value.CommandRelation &= ~MMLCommandRelation.NextControl;
                            }

                            var rest = (Data.Intermediate.Event.Rest)noteNode.Value;
                            Data.Intermediate.Position poslen = CalculateLength(rest.Start, rest.End, intermediate.TimeSignatureList, (int)settings.mmlExpression.TimeBase);
                            List<int> length = ConvertLengthFormat(poslen, lentable);

                            MMLCommandRelation tempRel = relation;
                            if (rest.TieFlag)
                            {
                                tempRel |= MMLCommandRelation.TieAfter;
                                relation |= MMLCommandRelation.TieBefore;
                            }
                            else
                            {
                                tempRel &= ~MMLCommandRelation.TieAfter;
                                relation &= ~MMLCommandRelation.TieBefore;
                            }

                            addCommand = CreateRestInstance(length, tempRel);

                            relation &= ~MMLCommandRelation.PrevControl;
                            prevTied = rest.TieFlag;
                            noteNode = noteNode.Next;
                        }
                        else
                        {
                            break;
                        }

                        comList.AddLast(addCommand);
                    }


                    if (settings.mmlExpression.NewBlockByBar == 1 && i == tsNode.Value.Position.Bar)
                    {
                        barCnt = 1;
                    }
                    else
                    {
                        barCnt++;
                    }

                    uint nextTSBar = (tsNode.Next?.Value.Position.Bar ?? 0);
                    if (i + 1 == nextTSBar)
                    {
                        tsNode = tsNode.Next;
                    }
                    string seperateSign = GetSeperateSign(i, barCnt, nextTSBar, settings.mmlExpression);


                    barList.Add(new Data.MML.Bar(comList, i, seperateSign));
                }

                if (track.Length.Tick == 0)
                {
                    Data.MML.Bar lastBar = barList.Last();
                    if (lastBar.CommandList.Count == 0)
                    {
                        barList.Remove(lastBar);
                    }
                }


                partList.Add(new Data.MML.Part(barList, status.SoundModule, track.Name));
            }


            return partList;
        }

        private int[][] CreateLengthTable(uint countsPerWholeNote, Settings.NoteRest settings)
        {
            // Create length elements
            var elements = new List<LengthElement>();
            int div = 1;
            int divtri;
            while (true)
            {
                if (countsPerWholeNote % div == 0)
                {
                    elements.Add(new LengthElement(div, ((int)countsPerWholeNote / div), false));
                    divtri = div * 3;
                    if (countsPerWholeNote % divtri == 0)
                    {
                        elements.Add(new LengthElement(divtri, ((int)countsPerWholeNote / divtri), true));
                    }
                    div <<= 1;
                }
                else
                {
                    break;
                }
            }
            elements = elements.OrderBy(x => x.Length).ToList();

            // Create length table
            var table = new LengthContainer[countsPerWholeNote];
            table[0] = new LengthContainer();
            table[0].AddLengthElement(elements.Find(x => x.Length == 1));
            elements.Remove(elements.Find(x => x.Length == 1));     // Remove whole note

            for (int i = 0; i < elements.Count; i++)
            {
                FillLengthTableLoop(table, elements, i, new LengthContainer(), settings);
            }

            return table.Select(x => x.GetLength(settings)).ToArray();
        }

        private void FillLengthTableLoop(LengthContainer[] table, List<LengthElement> elements, int n, LengthContainer container, Settings.NoteRest settings)
        {
            if (elements.Count == n) return;

            LengthContainer newContainer = container.Clone();
            int loopcnt = (elements[n].TripletFlag ? 2 : 1);

            for (int i = 0; i < loopcnt; i++)
            {
                newContainer.AddLengthElement(elements[n]);
                if (newContainer.Gate >= table.Length) return;

                if (table[newContainer.Gate] == null)
                {
                    table[newContainer.Gate] = newContainer;
                }
                else
                {
                    if (settings.LengthStyle == 0)
                    {
                        if (newContainer.Count < table[newContainer.Gate].Count)
                        {
                            table[newContainer.Gate] = newContainer;
                        }
                        else if (newContainer.Count == table[newContainer.Gate].Count)
                        {
                            if (newContainer.TripletCount < table[newContainer.Gate].TripletCount)
                            {
                                table[newContainer.Gate] = newContainer;
                            }
                        }
                    }
                    else
                    {
                        if (newContainer.TripletCount < table[newContainer.Gate].TripletCount)
                        {
                            table[newContainer.Gate] = newContainer;
                        }
                    }
                }

                for (int j = n + 1; j < elements.Count; j++)
                {
                    FillLengthTableLoop(table, elements, j, newContainer, settings);
                }

                if (elements[n].TripletFlag)
                {
                    newContainer = container.Clone();
                }
            }
        }

        private string GetEarliestCommandName
            (
            Data.Intermediate.Event.KeySignature ks,
            Data.Intermediate.Event.Tempo tempo,
            Data.Intermediate.Event.Instrument inst,
            Data.Intermediate.Event.Volume volume,
            Data.Intermediate.Event.Pan pan,
            Data.Intermediate.Event.NoteRest nr,
            int barNumber,
            bool prevTied,
            Settings settings
            )
        {
            Data.Intermediate.Position pos;
            string name;


            if (ks == null)
            {
                pos = null;
                name = string.Empty;
            }
            else
            {
                if (barNumber < ks.Position.Bar)
                {
                    if ((settings.noteRest.TieStyle == 1 ||
                        (settings.noteRest.TieStyle == 0 && settings.noteRest.CutByBar == 1 && !settings.noteRest.NewBlockInCutted)) &&
                        prevTied)
                    {
                        pos = ks.Position;
                        name = ks.GetType().Name;
                    }
                    else
                    {
                        pos = null;
                        name = string.Empty;
                    }
                }
                else
                {
                    pos = ks.Position;
                    name = ks.GetType().Name;
                }
            }

            if (tempo != null)
            {
                if (barNumber == tempo.Position.Bar ||
                    (settings.noteRest.TieStyle == 1 ||
                    (settings.noteRest.TieStyle == 0 && settings.noteRest.CutByBar == 1 && !settings.noteRest.NewBlockInCutted)) &&
                    prevTied)
                {
                    if (pos == null || pos.CompareTo(tempo.Position) > 0)
                    {
                        pos = tempo.Position;
                        name = tempo.GetType().Name;
                    }
                }
            }

            if (inst != null)
            {
                if (barNumber == inst.Position.Bar ||
                    (settings.noteRest.TieStyle == 1 ||
                    (settings.noteRest.TieStyle == 0 && settings.noteRest.CutByBar == 1 && !settings.noteRest.NewBlockInCutted)) &&
                    prevTied)
                {
                    if (pos == null || pos.CompareTo(inst.Position) > 0)
                    {
                        pos = inst.Position;
                        name = inst.GetType().Name;
                    }
                }
            }

            if (volume != null)
            {
                if (barNumber == volume.Position.Bar ||
                    (settings.noteRest.TieStyle == 1 ||
                    (settings.noteRest.TieStyle == 0 && settings.noteRest.CutByBar == 1 && !settings.noteRest.NewBlockInCutted)) &&
                    prevTied)
                {
                    if (pos == null || pos.CompareTo(volume.Position) > 0)
                    {
                        pos = volume.Position;
                        name = volume.GetType().Name;
                    }
                }
            }

            if (pan != null)
            {
                if (barNumber == pan.Position.Bar ||
                    (settings.noteRest.TieStyle == 1 ||
                    (settings.noteRest.TieStyle == 0 && settings.noteRest.CutByBar == 1 && !settings.noteRest.NewBlockInCutted)) &&
                    prevTied)
                {
                    if (pos == null || pos.CompareTo(pan.Position) > 0)
                    {
                        pos = pan.Position;
                        name = pan.GetType().Name;
                    }
                }
            }

            if (nr != null)
            {
                if (barNumber == nr.Start.Bar ||
                    (settings.noteRest.TieStyle == 1 ||
                    (settings.noteRest.TieStyle == 0 && settings.noteRest.CutByBar == 1 && !settings.noteRest.NewBlockInCutted)) &&
                    prevTied)
                {
                    if (pos == null || pos.CompareTo(nr.Start) > 0)
                    {
                        name = nr.GetType().Name;
                    }
                }
            }


            return name;
        }

        private Data.Intermediate.Position CalculateLength
            (
            Data.Intermediate.Position start,
            Data.Intermediate.Position end,
            LinkedList<Data.Intermediate.Event.TimeSignature> tsList,
            int countsPerWholeNote
            )
        {
            // Search current time signature
            LinkedListNode<Data.Intermediate.Event.TimeSignature> curTSNode = tsList.First;
            while (true)
            {
                if (curTSNode.Value.Position.CompareTo(start) < 0)
                {
                    if (curTSNode.Next == null)
                    {
                        break;
                    }
                    else
                    {
                        curTSNode = curTSNode.Next;
                    }
                }
                else if (curTSNode.Value.Position.CompareTo(start) == 0)
                {
                    break;
                }
                else
                {
                    curTSNode = curTSNode.Previous;
                    break;
                }
            }

            // Calculate length data by Position
            Data.Intermediate.Position len = new Data.Intermediate.Position(0, 0);
            Data.Intermediate.Position tempStart = start;
            LinkedListNode<Data.Intermediate.Event.TimeSignature> nextTSNode = curTSNode.Next;

            while (true)
            {
                if (nextTSNode == null)
                {
                    Data.Intermediate.Position sub = end.Subtract(tempStart, curTSNode.Value.TickPerBar);
                    Data.Intermediate.Position sub2 = Data.Intermediate.Position.ConvertByTicksPerBar(sub, curTSNode.Value.TickPerBar, (uint)countsPerWholeNote);
                    len = len.Add(sub2, (uint)countsPerWholeNote);
                    break;
                }
                else
                {
                    if (nextTSNode.Value.PrevSignedPosition.CompareTo(end) >= 0)
                    {
                        Data.Intermediate.Position sub = end.Subtract(tempStart, curTSNode.Value.TickPerBar);
                        Data.Intermediate.Position sub2 = Data.Intermediate.Position.ConvertByTicksPerBar(sub, curTSNode.Value.TickPerBar, (uint)countsPerWholeNote);
                        len = len.Add(sub2, (uint)countsPerWholeNote);
                        break;
                    }
                    else
                    {
                        Data.Intermediate.Position sub = nextTSNode.Value.PrevSignedPosition.Subtract(tempStart, curTSNode.Value.TickPerBar);
                        Data.Intermediate.Position sub2 = Data.Intermediate.Position.ConvertByTicksPerBar(sub, curTSNode.Value.TickPerBar, (uint)countsPerWholeNote);
                        len = len.Add(sub2, (uint)countsPerWholeNote);
                        tempStart = nextTSNode.Value.Position;
                        curTSNode = curTSNode.Next;
                        nextTSNode = nextTSNode.Next;
                    }
                }
            }

            return len;
        }

        private List<int> ConvertLengthFormat(Data.Intermediate.Position posdata, int[][] lentable)
        {
            var lenList = new List<int>();
            uint bar = posdata.Bar;
            uint tick = posdata.Tick;

            for (; bar > 0; bar--)
            {
                lenList.AddRange(lentable[0]);
            }

            if (tick > 0)
            {
                lenList.AddRange(lentable[tick]);
            }

            return lenList;
        }

        protected abstract Data.MML.Command.Tempo CreateTempoInstance(int value, MMLCommandRelation relation);

        protected abstract Data.MML.Command.Instrument CreateInstrumentInstance(int value, MMLCommandRelation relation);

        protected abstract Data.MML.Command.Volume CreateVolumeInstance(int value, MMLCommandRelation relation);

        protected abstract Data.MML.Command.Pan CreatePanInstance(int value, MMLCommandRelation relation);

        protected abstract Data.MML.Command.Note CreateNoteInstance(int octave, string name, List<int> length, MMLCommandRelation relation);

        protected abstract Data.MML.Command.Rest CreateRestInstance(List<int> length, MMLCommandRelation relation);

        private string GetSeperateSign(int curBar, int barCnt, uint tsBar, Settings.MMLExpression settings)
        {
            if (settings.NewLineByTimeSignature == 1 && curBar + 1 == tsBar)
            {
                return Environment.NewLine;
            }

            if (settings.NewBlockByBar != 0)
            {
                if (settings.NewLineBarCount != 0 && barCnt % settings.NewLineBarCount == 0)
                {
                    return Environment.NewLine;
                }
                else
                {
                    switch (settings.NewBlockByBar)
                    {
                        case 1:
                            return " ";
                        case 2:
                            return "\t";
                        default:
                            return string.Empty;
                    }
                }
            }
            else
            {
                return string.Empty;
            }
        }

        protected abstract Data.MML.MML CreateMMLInstance(List<Data.MML.Part> partList, string title, int countsPerWholenote);
    }
}
