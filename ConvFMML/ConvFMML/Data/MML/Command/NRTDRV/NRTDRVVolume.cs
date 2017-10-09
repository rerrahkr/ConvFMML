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
                if (module == SoundModule.FM)
                {
                    newValue = Value * (int)volumeSettings.VStep / 127;
                }
                else
                {
                    newValue = Value * 15 / 127;
                }
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
                    newValue = Value * 15 / 127;
                }
            }

            return sign + newValue;
        }
    }
}
