using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConvFMML.Data.Intermediate.Event;
using ConvFMML.Data.MML.Command.PMD;

namespace ConvFMML.Modifier
{
    public class PMDMusicDataModifier : MusicDataModifier
    {
        protected override ChangeEventSet CreateInstrumentSet(Instrument i)
        {
            return new ChangeEventSet(i, new PMDInstrument((i.Value + 1), MMLCommandRelation.Clear));
        }

        protected override ChangeEventSet CreatePanSet(Pan p)
        {
            return new ChangeEventSet(p, new PMDPan(p.Value, MMLCommandRelation.Clear));
        }

        protected override TempoSet CreateTempoSet(Tempo t)
        {
            return new TempoSet(t, new PMDTempo(t.Value, MMLCommandRelation.Clear));
        }

        protected override ChangeEventSet CreateVolumeSet(Volume v)
        {
            return new ChangeEventSet(v, new PMDVolume(v.Value, MMLCommandRelation.Clear));
        }
    }
}
