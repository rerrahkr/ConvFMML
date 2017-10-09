using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.Intermediate.Event
{
    public class KeySignature : ICloneable
    {
        public Position Position { get; set; }
        public Key Key { get; set; }

        public KeySignature(Position position, Key key)
        {
            Position = position;
            Key = key;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public KeySignature Clone()
        {
            var clone = (KeySignature)MemberwiseClone();
            clone.Position = Position.Clone();
            return clone;
        }
    }
}
