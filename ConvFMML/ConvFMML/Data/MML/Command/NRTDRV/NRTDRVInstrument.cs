using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command.NRTDRV
{
    public class NRTDRVInstrument : Instrument
    {
        public NRTDRVInstrument(int value, MMLCommandRelation relation) : base(value, relation) { }

        protected override string GenerateString(Settings settings, SoundModule module)
        {
            return "@" + Value;
        }
    }
}
