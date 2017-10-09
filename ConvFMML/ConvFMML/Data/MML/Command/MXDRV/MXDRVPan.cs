using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command.MXDRV
{
    public class MXDRVPan : Pan
    {
        public MXDRVPan(int value, MMLCommandRelation relation) : base(value, relation) { }

        protected override string GenerateString(Settings settings, SoundModule module)
        {
            string sign;
            Settings.ControlCommand.Pan panSettings = settings.controlCommand.pan;

            if (Value <= panSettings.BorderLeft)
            {
                sign = "p1";
            }
            else if (panSettings.BorderRight <= Value)
            {
                sign = "p2";
            }
            else
            {
                sign = "p3";
            }

            return sign;
        }
    }
}
