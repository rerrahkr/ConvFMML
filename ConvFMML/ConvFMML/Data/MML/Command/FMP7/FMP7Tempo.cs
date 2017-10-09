using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command.FMP7
{
    public class FMP7Tempo : Tempo
    {
        public FMP7Tempo(int value, MMLCommandRelation relation) : base(value, relation) { }

        protected override string GenerateString(Settings settings, SoundModule module)
        {
            return "T" + Value;
        }
    }
}
