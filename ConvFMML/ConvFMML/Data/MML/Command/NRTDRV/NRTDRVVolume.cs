using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command.NRTDRV
{
    public class NRTDRVVolume : Volume
    {
        public NRTDRVVolume(int value, MMLCommandRelation relation) : base(value, relation) { }

        protected override string GenerateString(Settings settings, SoundModule module)
        {
            string sign;
            int newValue;
            Settings.ControlCommand.Volume volumeSettings = settings.controlCommand.volume;

            if (volumeSettings.CommandNRTDRV == 0)
            {
                sign = "v";
                double temp;
                if (module == SoundModule.FM)
                {
                    temp = Value * (double)volumeSettings.VStep / 127.0;
                }
                else
                {
                    temp = Value * 15.0 / 127.0;
                }
                newValue = (int)Math.Round(temp, MidpointRounding.AwayFromZero);
            }
            else
            {
                sign = "V";
                if (module == SoundModule.FM)
                {
                    newValue = Value;
                }
                else
                {
                    double temp = Value * 15.0 / 127.0;
                    newValue = (int)Math.Round(temp, MidpointRounding.AwayFromZero);
                }
            }

            return sign + newValue;
        }
    }
}
