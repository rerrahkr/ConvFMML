using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.Intermediate.Event
{
    public class TimeSignature : ICloneable
    {
        public Position Position { get; set; }
        public Position PrevSignedPosition { get; set; }
        public int Numerator { get; set; }
        public int Denominator { get; set; }
        public uint TickPerBar { get; set; }

        public TimeSignature(Position position, Position prevSignedPosition, int numerator, int denominator, uint tickPerBar)
        {
            Position = position;
            PrevSignedPosition = prevSignedPosition;
            Numerator = numerator;
            Denominator = denominator;
            TickPerBar = tickPerBar;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public TimeSignature Clone()
        {
            var clone = (TimeSignature)MemberwiseClone();
            clone.Position = Position.Clone();
            clone.PrevSignedPosition = PrevSignedPosition.Clone();
            return clone;
        }
    }
}
