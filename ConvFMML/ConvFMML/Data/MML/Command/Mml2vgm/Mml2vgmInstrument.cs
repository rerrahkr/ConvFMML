using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command.Mml2vgm
{
    public class Mml2vgmInstrument : Instrument
    {
        public Mml2vgmInstrument(int value, MMLCommandRelation relation) : base(value, relation) { }

        protected override string GenerateString(Settings settings, SoundModule module)
        {
            return "@" + ((module == SoundModule.SSG) ? "E" : "") + Value;
        }
    }
}
