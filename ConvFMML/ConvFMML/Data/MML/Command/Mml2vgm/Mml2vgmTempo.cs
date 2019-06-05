using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command.Mml2vgm
{
    public class Mml2vgmTempo : Tempo
    {
        public Mml2vgmTempo(int value, MMLCommandRelation relation) : base(value, relation) { }

        protected override string GenerateString(Settings settings, SoundModule module)
        {
            return "T" + Value;
        }
    }
}
