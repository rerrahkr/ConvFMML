using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MIDI.Event
{
    public class Volume : ControlChange
    {
        public Volume(uint deltaTime, int channel, int value) : base(deltaTime, channel, value) { }

        protected override string GenerateString()
        {
            return $"{DeltaTime}:\tVolume\t{Channel}, {Value}";
        }
    }
}
