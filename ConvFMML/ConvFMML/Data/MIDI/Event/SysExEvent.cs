using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MIDI.Event
{
    public class SysExEvent : Event
    {
        public SysExEvent(uint deltaTime) : base(deltaTime) { }

        protected override string GenerateString()
        {
            return $"{DeltaTime}:\tSysEx Event";
        }
    }
}
