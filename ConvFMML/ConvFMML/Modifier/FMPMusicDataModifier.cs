using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConvFMML.Data.Intermediate.Event;
using ConvFMML.Data.MML.Command.FMP;

namespace ConvFMML.Modifier
{
    public class FMPMusicDataModifier : MusicDataModifier
    {
        protected override LinkedList<Pan> ClonePanList(LinkedList<Pan> newList, LinkedList<Pan> srcList, Settings settings, SoundModule module)
        {
            if (settings.mmlExpression.ExtensionFMP == 2)
            {
                return base.ClonePanList(newList, srcList, settings, module);
            }
            else
            {
                return newList;
            }
        }

        protected override ChangeEventSet CreateInstrumentSet(Instrument i)
        {
            return new ChangeEventSet(i, new FMPInstrument((i.Value + 1), MMLCommandRelation.Clear));
        }

        protected override ChangeEventSet CreatePanSet(Pan p)
        {
            return new ChangeEventSet(p, new FMPPan(p.Value, MMLCommandRelation.Clear));
        }

        protected override TempoSet CreateTempoSet(Tempo t)
        {
            return new TempoSet(t, new FMPTempo(t.Value, MMLCommandRelation.Clear));
        }

        protected override ChangeEventSet CreateVolumeSet(Volume v)
        {
            return new ChangeEventSet(v, new FMPVolume(v.Value, MMLCommandRelation.Clear));
        }
    }
}
