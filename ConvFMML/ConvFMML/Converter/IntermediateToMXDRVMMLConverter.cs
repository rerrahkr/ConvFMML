using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConvFMML.Data.MML;
using ConvFMML.Data.MML.Command;
using ConvFMML.Data.MML.Command.MXDRV;

namespace ConvFMML.Converter
{
    public class IntermediateToMXDRVMMLConverter : IntermediateToMMLConverter
    {
        protected override Tempo CreateTempoInstance(int value, MMLCommandRelation relation)
        {
            return new MXDRVTempo(value, relation);
        }

        protected override Instrument CreateInstrumentInstance(int value, MMLCommandRelation relation)
        {
            return new MXDRVInstrument((value + 1), relation);
        }

        protected override Volume CreateVolumeInstance(int value, MMLCommandRelation relation)
        {
            return new MXDRVVolume(value, relation);
        }

        protected override Pan CreatePanInstance(int value, MMLCommandRelation relation)
        {
            return new MXDRVPan(value, relation);
        }

        protected override Note CreateNoteInstance(int octave, string name, List<int> length, MMLCommandRelation relation)
        {
            return new MXDRVNote(octave, name, length, relation);
        }

        protected override Rest CreateRestInstance(List<int> length, MMLCommandRelation relation)
        {
            return new MXDRVRest(length, relation);
        }

        protected override MML CreateMMLInstance(List<Part> partList, string title, int countsPerWholeNote)
        {
            return new MML(partList, title, countsPerWholeNote, MMLStyle.MXDRV);
        }
    }
}
