using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MIDI.Event
{
    public class ProgramChange : MIDIEvent
    {
        public int Number { get; }

        public ProgramChange(uint deltaTime, int channel, int number) : base(deltaTime, channel)
        {
            Number = number;
        }

        protected override string GenerateString()
        {
            return $"{DeltaTime}:\tProgram Change\t{Channel}, {Number}";
        }
    }
}
