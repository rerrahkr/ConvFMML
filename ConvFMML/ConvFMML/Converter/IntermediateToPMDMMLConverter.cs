﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConvFMML.Data.MML;
using ConvFMML.Data.MML.Command;
using ConvFMML.Data.MML.Command.PMD;

namespace ConvFMML.Converter
{
    public class IntermediateToPMDMMLConverter : IntermediateToMMLConverter
    {
        protected override Tempo CreateTempoInstance(int value, MMLCommandRelation relation)
        {
            return new PMDTempo(value, relation);
        }

        protected override Instrument CreateInstrumentInstance(int value, MMLCommandRelation relation)
        {
            return new PMDInstrument((value + 1), relation);
        }

        protected override Volume CreateVolumeInstance(int value, MMLCommandRelation relation)
        {
            return new PMDVolume(value, relation);
        }

        protected override Pan CreatePanInstance(int value, MMLCommandRelation relation)
        {
            return new PMDPan(value, relation);
        }

        protected override Note CreateNoteInstance(int octave, string name, List<int> length, MMLCommandRelation relation)
        {
            return new PMDNote(octave, name, length, relation);
        }

        protected override Rest CreateRestInstance(List<int> length, MMLCommandRelation relation)
        {
            return new PMDRest(length, relation);
        }

        protected override MML CreateMMLInstance(List<Part> partList, string title, int countsPerWholeNote)
        {
            return new MML(partList, title, countsPerWholeNote, MMLStyle.PMD);
        }
    }
}
