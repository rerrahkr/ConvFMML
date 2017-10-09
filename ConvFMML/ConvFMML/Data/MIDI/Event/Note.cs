using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MIDI.Event
{
    public abstract class Note : MIDIEvent
    {
        public int Number { get; }
        public int Velocity { get; }

        protected Note(uint deltaTime, int channel, int number, int velocity) : base(deltaTime, channel)
        {
            Number = number;
            Velocity = velocity;
        }

        protected abstract override string GenerateString();
    }
}
