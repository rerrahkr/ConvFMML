using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MIDI.Event
{
    public class SequenceTrackName : MetaEvent
    {
        public string Name { get; }

        public SequenceTrackName(uint deltaTime, string name) : base(deltaTime)
        {
            Name = name;
        }

        protected override string GenerateString()
        {
            return $"{DeltaTime}:\tSequence Track Name\t{Name}";
        }
    }
}
