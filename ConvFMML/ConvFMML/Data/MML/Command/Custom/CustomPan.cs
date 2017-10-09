using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command.Custom
{
    public class CustomPan : Pan
    {
        public CustomPan(int value, MMLCommandRelation relation) : base(value, relation) { }

        protected override string GenerateString(Settings settings, SoundModule module)
        {
            string sign;
            int newValue;
            Settings.ControlCommand.Pan panSettings = settings.controlCommand.pan;

            if (panSettings.CommandCustom == 0)
            {
                sign = panSettings.MIDICommandCustom;
                newValue = Value;
            }
            else
            {
                newValue = -1;
                if (Value <= panSettings.BorderLeft)
                {
                    sign = panSettings.LeftCommandCustom;
                }
                else if (panSettings.BorderRight <= Value)
                {
                    sign = panSettings.RightCommandCustom;
                }
                else
                {
                    sign = panSettings.CenterCommandCustom;
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
