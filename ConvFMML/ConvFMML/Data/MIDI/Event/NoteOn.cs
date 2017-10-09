using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MIDI.Event
{
    public class NoteOn : Note
    {
        public NoteOn(uint deltaTime, int channel, int number, int velocity) : base(deltaTime, channel, number, velocity) { }

        protected override string GenerateString()
        {
            return $"{DeltaTime}:\tNote On\t{Channel}, {Number}, {Velocity}";
        }
    }
}
