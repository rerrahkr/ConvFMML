using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MIDI.Event
{
    public class ControlChange : MIDIEvent
    {
        public int Value { get; }

        public ControlChange(uint deltaTime, int channel, int value) : base(deltaTime, channel)
        {
            Value = value;
        }

        protected override string GenerateString()
        {
            return $"{DeltaTime}:\tOther Control Change";
        }
    }
}
