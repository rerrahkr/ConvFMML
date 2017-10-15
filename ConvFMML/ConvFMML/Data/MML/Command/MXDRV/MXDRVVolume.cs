using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command.MXDRV
{
    public class MXDRVVolume : Volume
    {
        public MXDRVVolume(int value, MMLCommandRelation relation) : base(value, relation) { }

        protected override string GenerateString(Settings settings, SoundModule module)
        {
            string sign;
            int newValue;
            Settings.ControlCommand.Volume volumeSettings = settings.controlCommand.volume;

            if (volumeSettings.CommandMXDRV == 0)
            {
                sign = "v";
                double temp = Value * 15.0 / 127.0;
                newValue = (int)Math.Round(temp, MidpointRounding.AwayFromZero);
            }
            else
            {
                sign = "@v";
                newValue = Value;
            }

            return sign + newValue;
        }
    }
}
