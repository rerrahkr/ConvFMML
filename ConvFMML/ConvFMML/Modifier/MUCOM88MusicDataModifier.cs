using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConvFMML.Data.Intermediate.Event;
using ConvFMML.Data.MML.Command.MUCOM88;

namespace ConvFMML.Modifier
{
    public class MUCOM88MusicDataModifier : MusicDataModifier
    {
        protected override ChangeEventSet CreateInstrumentSet(Instrument i)
        {
            return new ChangeEventSet(i, new MUCOM88Instrument((i.Value + 1), MMLCommandRelation.Clear));
        }

        protected override ChangeEventSet CreatePanSet(Pan p)
        {
            return new ChangeEventSet(p, new MUCOM88Pan(p.Value, MMLCommandRelation.Clear));
        }

        protected override TempoSet CreateTempoSet(Tempo t)
        {
            return new TempoSet(t, new MUCOM88Tempo(t.Value, MMLCommandRelation.Clear));
        }

        protected override ChangeEventSet CreateVolumeSet(Volume v)
        {
            return new ChangeEventSet(v, new MUCOM88Volume(v.Value, MMLCommandRelation.Clear));
        }
    }
}
