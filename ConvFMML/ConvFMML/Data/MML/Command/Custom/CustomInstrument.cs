using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command.Custom
{
    public class CustomInstrument : Instrument
    {
        public CustomInstrument(int value, MMLCommandRelation relation) : base(value, relation) { }

        protected override string GenerateString(Settings settings, SoundModule module)
        {
            Settings.ControlCommand.ProgramChange pcSettings = settings.controlCommand.programChange;

            return pcSettings.CommandCustom + Value;
        }
    }
}
