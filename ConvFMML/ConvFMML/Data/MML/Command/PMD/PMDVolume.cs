using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command.PMD
{
    public class PMDVolume : Volume
    {
        public PMDVolume(int value, MMLCommandRelation relation) : base(value, relation) { }

        protected override string GenerateString(Settings settings, SoundModule module)
        {
            string sign;
            int newValue;
            Settings.ControlCommand.Volume volumeSettings = settings.controlCommand.volume;

            if (volumeSettings.CommandPMD == 0)
            {
                sign = "v";
                if (module == SoundModule.SSG)
                {
                    newValue = Value * 15 / 127;
                }
                else
                {
                    newValue = Value * 16 / 127;
                }
            }
            else
            {
                sign = "V";
                if (module == SoundModule.SSG)
                {
                    newValue = Value * 15 / 127;
                }
                else
                {
                    newValue = Value;
                }
            }

            return sign + newValue;
        }
    }
}
