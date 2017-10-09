using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MIDI.Event
{
    public abstract class Event
    {
        public uint DeltaTime { get; }

        protected Event(uint deltaTime)
        {
            DeltaTime = deltaTime;
        }

        public override string ToString()
        {
            return GenerateString();
        }

        protected abstract string GenerateString();
    }
}
