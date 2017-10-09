using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command.Custom
{
    public class CustomVolume : Volume
    {
        public CustomVolume(int value, MMLCommandRelation relation) : base(value, relation) { }

        protected override string GenerateString(Settings settings, SoundModule module)
        {
            int newValue;
            Settings.ControlCommand.Volume volumeSettings = settings.controlCommand.volume;

            if (volumeSettings.RangeCustom != 0)
            {
                newValue = Value * (int)volumeSettings.RangeCustom / 127;
            }
            else
            {
                newValue = Value;
            }

            return volumeSettings.CommandCustom + newValue;
        }
    }
}
