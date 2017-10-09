using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MIDI.Event
{
    public class MIDIEvent : Event
    {
        public int Channel { get; }

        public MIDIEvent(uint deltaTime, int channel) : base(deltaTime)
        {
            Channel = channel;
        }

        protected override string GenerateString()
        {
            return $"{DeltaTime}:\tOther MIDI Event\t{Channel}";
        }
    }
}
