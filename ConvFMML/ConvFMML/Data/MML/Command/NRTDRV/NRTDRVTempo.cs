using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command.NRTDRV
{
    public class NRTDRVTempo : Tempo
    {
        public NRTDRVTempo(int value, MMLCommandRelation relation) : base(value, relation) { }

        protected override string GenerateString(Settings settings, SoundModule module)
        {
            return "t" + Value;
        }
    }
}
