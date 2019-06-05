using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command.MUCOM88
{
    public class MUCOM88Tempo : Tempo
    {
        public MUCOM88Tempo(int value, MMLCommandRelation relation) : base(value, relation) { }

        protected override string GenerateString(Settings settings, SoundModule module)
        {
            return "T" + Value;
        }
    }
}
