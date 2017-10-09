using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.Intermediate.Event
{
    public class Tempo : ICloneable
    {
        public Position Position { get; set; }
        public int Value { get; set; }

        public Tempo(Position position, int value)
        {
            Position = position;
            Value = value;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public Tempo Clone()
        {
            var clone = (Tempo)MemberwiseClone();
            clone.Position = Position.Clone();
            return clone;
        }
    }
}
