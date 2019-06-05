using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConvFMML.Data.MML;
using ConvFMML.Data.MML.Command;
using ConvFMML.Data.MML.Command.MUCOM88;

namespace ConvFMML.Converter
{
    public class IntermediateToMUCOM88MMLConverter : IntermediateToMMLConverter
    {
        protected override Tempo CreateTempoInstance(int value, MMLCommandRelation relation)
        {
            return new MUCOM88Tempo(value, relation);
        }

        protected override Instrument CreateInstrumentInstance(int value, MMLCommandRelation relation)
        {
            return new MUCOM88Instrument((value + 1), relation);
        }

        protected override Volume CreateVolumeInstance(int value, MMLCommandRelation relation)
        {
            return new MUCOM88Volume(value, relation);
        }

        protected override Pan CreatePanInstance(int value, MMLCommandRelation relation)
        {
            return new MUCOM88Pan(value, relation);
        }

        protected override Note CreateNoteInstance(int octave, string name, List<int> length, MMLCommandRelation relation)
        {
            return new MUCOM88Note(octave, name, length, relation);
        }

        protected override Rest CreateRestInstance(List<int> length, MMLCommandRelation relation)
        {
            return new MUCOM88Rest(length, relation);
        }

        protected override MML CreateMMLInstance(List<Part> partList, string title, int countsPerWholeNote)
        {
            return new MML(partList, title, countsPerWholeNote, MMLStyle.MUCOM88);
        }
    }
}
