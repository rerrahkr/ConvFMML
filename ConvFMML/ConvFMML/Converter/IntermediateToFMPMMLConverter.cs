using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConvFMML.Data.MML;
using ConvFMML.Data.MML.Command;
using ConvFMML.Data.MML.Command.FMP;

namespace ConvFMML.Converter
{
    public class IntermediateToFMPMMLConverter : IntermediateToMMLConverter
    {
        protected override Tempo CreateTempoInstance(int value, MMLCommandRelation relation)
        {
            return new FMPTempo(value, relation);
        }

        protected override Instrument CreateInstrumentInstance(int value, MMLCommandRelation relation)
        {
            return new FMPInstrument((value + 1), relation);
        }

        protected override Volume CreateVolumeInstance(int value, MMLCommandRelation relation)
        {
            return new FMPVolume(value, relation);
        }

        protected override Pan CreatePanInstance(int value, MMLCommandRelation relation)
        {
            return new FMPPan(value, relation);
        }

        protected override Note CreateNoteInstance(int octave, string name, List<int> length, MMLCommandRelation relation)
        {
            return new FMPNote(octave, name, length, relation);
        }

        protected override Rest CreateRestInstance(List<int> length, MMLCommandRelation relation)
        {
            return new FMPRest(length, relation);
        }

        protected override MML CreateMMLInstance(List<Part> partList, string title, int countsPerWholeNote)
        {
            return new MML(partList, title, countsPerWholeNote, MMLStyle.FMP);
        }
    }
}
