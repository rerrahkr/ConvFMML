using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MIDI.Event
{
    public class TimeSignature : MetaEvent
    {
        public int Numerator { get; }
        public int DenominatorBitShift { get; }
        public int MIDIClockPerMetronomeTick { get; }
        public int NumberOfNotesPerClocks { get; }

        public TimeSignature(uint deltaTime, int numerator, int denominatorBitShift, int midiClockPerMetronomeTick, int numberOfNotesPerClocks) : base(deltaTime)
        {
            Numerator = numerator;
            DenominatorBitShift = denominatorBitShift;
            MIDIClockPerMetronomeTick = midiClockPerMetronomeTick;
            NumberOfNotesPerClocks = numberOfNotesPerClocks;
        }

        protected override string GenerateString()
        {
            return $"{DeltaTime}:\tTime Signature\t{Numerator}, {DenominatorBitShift}, {MIDIClockPerMetronomeTick}, {NumberOfNotesPerClocks}";
        }
    }
}
