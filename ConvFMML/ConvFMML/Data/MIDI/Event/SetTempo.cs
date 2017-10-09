using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MIDI.Event
{
    public class SetTempo : MetaEvent
    {
        public int Value { get; }

        public SetTempo(uint deltaTime, int value) : base(deltaTime)
        {
            Value = value;
        }

        protected override string GenerateString()
        {
            return $"{DeltaTime}:\tSet Tempo\t{Value}";
        }
    }
}
