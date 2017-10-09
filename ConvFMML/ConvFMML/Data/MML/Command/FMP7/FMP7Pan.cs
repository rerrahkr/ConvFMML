using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command.FMP7
{
    public class FMP7Pan : Pan
    {
        public FMP7Pan(int value, MMLCommandRelation relation) : base(value, relation) { }

        protected override string GenerateString(Settings settings, SoundModule module)
        {
            string sign;
            int jud;
            int newValue;
            Settings.ControlCommand.Pan panSettings = settings.controlCommand.pan;

            if (Value == 64)
            {
                jud = 2;
                newValue = 128;
            }
            else
            {
                if (Value > 64)
                {
                    jud = 0;
                    newValue = (Value - 64) * 127 / 63;
                }
                else
                {
                    jud = 1;
                    newValue = (64 - Value) * 127 / 64;
                }
            }

            if (panSettings.CommandFMP7 == 0)
            {
                sign = "P";
                switch (jud)
                {
                    case 0:
                        newValue = 128 + newValue;
                        break;
                    case 1:
                        newValue = 128 - newValue;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (jud)
                {
                    case 0:
                        sign = "PR";
                        break;
                    case 1:
                        sign = "PL";
                        break;
                    default:
                        sign = "PC";
                        newValue = -1;
                        break;
                }
            }

            if (newValue < 0)
            {
                return sign;
            }
            else
            {
                return sign + newValue;
            }
        }
    }
}
