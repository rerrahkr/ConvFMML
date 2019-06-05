using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command.MUCOM88
{
    public class MUCOM88Volume : Volume
    {
        public MUCOM88Volume(int value, MMLCommandRelation relation) : base(value, relation) { }

        protected override string GenerateString(Settings settings, SoundModule module)
        {
            double temp = Value * 15.0 / 127.0;
            return "v" + (int)Math.Round(temp, MidpointRounding.AwayFromZero);
        }
    }
}
