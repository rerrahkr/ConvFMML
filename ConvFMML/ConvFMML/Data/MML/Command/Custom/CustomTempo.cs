using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command.Custom
{
    public class CustomTempo : Tempo
    {
        public CustomTempo(int value, MMLCommandRelation relation) : base(value, relation) { }

        protected override string GenerateString(Settings settings, SoundModule module)
        {
            Settings.ControlCommand.Tempo tempoSettings = settings.controlCommand.tempo;

            return tempoSettings.CommandCustom + Value;
        }
    }
}
