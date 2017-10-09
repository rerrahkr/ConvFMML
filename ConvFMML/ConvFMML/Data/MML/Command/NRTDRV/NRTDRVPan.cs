using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command.NRTDRV
{
    public class NRTDRVPan : Pan
    {
        public NRTDRVPan(int value, MMLCommandRelation relation) : base(value, relation) { }

        protected override string GenerateString(Settings settings, SoundModule module)
        {
            string sign;
            Settings.ControlCommand.Pan panSettings = settings.controlCommand.pan;

            if (Value <= panSettings.BorderLeft)
            {
                sign = "P1";
            }
            else if (panSettings.BorderRight <= Value)
            {
                sign = "P2";
            }
            else
            {
                sign = "P3";
            }

            return sign;
        }
    }
}
