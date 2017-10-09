using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MIDI.Event
{
    public class NoteOff : Note
    {
        public NoteOff(uint deltaTime, int channel, int number, int velocity) : base(deltaTime, channel, number, velocity) { }

        protected override string GenerateString()
        {
            return $"{DeltaTime}:\tNote Off\t{Channel}, {Number}, {Velocity}";
        }
    }
}
