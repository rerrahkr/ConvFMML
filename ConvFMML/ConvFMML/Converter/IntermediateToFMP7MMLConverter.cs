using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConvFMML.Data.MML;
using ConvFMML.Data.MML.Command;
using ConvFMML.Data.MML.Command.FMP7;

namespace ConvFMML.Converter
{
    public class IntermediateToFMP7MMLConverter : IntermediateToMMLConverter
    {
        protected override Tempo CreateTempoInstance(int value, MMLCommandRelation relation)
        {
            return new FMP7Tempo(value, relation);
        }

        protected override Instrument CreateInstrumentInstance(int value, MMLCommandRelation relation)
        {
            return new FMP7Instrument((value + 1), relation);
        }

        protected override Volume CreateVolumeInstance(int value, MMLCommandRelation relation)
        {
            return new FMP7Volume(value, relation);
        }

        protected override Pan CreatePanInstance(int value, MMLCommandRelation relation)
        {
            return new FMP7Pan(value, relation);
        }

        protected override Note CreateNoteInstance(int octave, string name, List<int> length, MMLCommandRelation relation)
        {
            return new FMP7Note(octave, name, length, relation);
        }

        protected override Rest CreateRestInstance(List<int> length, MMLCommandRelation relation)
        {
            return new FMP7Rest(length, relation);
        }

        protected override MML CreateMMLInstance(List<Part> partList, string title, int countsPerWholeNote)
        {
            return new MML(partList, title, countsPerWholeNote, MMLStyle.FMP7);
        }
    }
}
