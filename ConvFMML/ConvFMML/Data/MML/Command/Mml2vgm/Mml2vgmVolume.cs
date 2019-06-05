using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command.Mml2vgm
{
    public class Mml2vgmVolume : Volume
    {
        public Mml2vgmVolume(int value, MMLCommandRelation relation) : base(value, relation) { }

        protected override string GenerateString(Settings settings, SoundModule module)
        {
            Settings.ControlCommand.Volume volumeSettings = settings.controlCommand.volume;

            int newValue;
            if (module == SoundModule.SSG)
            {
                double temp = Value * 15.0 / 127.0;
                newValue = (int)Math.Round(temp, MidpointRounding.AwayFromZero);
            }
            else
            {
                newValue = Value;
            }

            return "v" + newValue;
        }
    }
}
