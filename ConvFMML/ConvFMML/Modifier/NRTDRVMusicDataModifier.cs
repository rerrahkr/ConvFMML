using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConvFMML.Data.Intermediate.Event;
using ConvFMML.Data.MML.Command.NRTDRV;

namespace ConvFMML.Modifier
{
    public class NRTDRVMusicDataModifier : MusicDataModifier
    {
        protected override ChangeEventSet CreateInstrumentSet(Instrument i)
        {
            return new ChangeEventSet(i, new NRTDRVInstrument((i.Value + 1), MMLCommandRelation.Clear));
        }

        protected override ChangeEventSet CreatePanSet(Pan p)
        {
            return new ChangeEventSet(p, new NRTDRVPan(p.Value, MMLCommandRelation.Clear));
        }

        protected override TempoSet CreateTempoSet(Tempo t)
        {
            return new TempoSet(t, new NRTDRVTempo(t.Value, MMLCommandRelation.Clear));
        }

        protected override ChangeEventSet CreateVolumeSet(Volume v)
        {
            return new ChangeEventSet(v, new NRTDRVVolume(v.Value, MMLCommandRelation.Clear));
        }
    }
}
