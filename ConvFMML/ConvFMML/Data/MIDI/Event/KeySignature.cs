using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MIDI.Event
{
    public class KeySignature : MetaEvent
    {
        public int SignatureNumber { get; }
        public int MinorFlagNumber { get; }

        public KeySignature(uint deltaTime, int signatureNumber, int minorFlagNumber) : base(deltaTime)
        {
            SignatureNumber = signatureNumber;
            MinorFlagNumber = minorFlagNumber;
        }

        protected override string GenerateString()
        {
            return $"{DeltaTime}:\tKey Signature\t{SignatureNumber}, {MinorFlagNumber}";
        }
    }
}
