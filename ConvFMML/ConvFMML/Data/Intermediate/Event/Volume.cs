using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.Intermediate.Event
{
    public class Volume : ChangeEvent, ICloneable
    {
        public Volume(Position position, int value) : base(position, value) { }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public new Volume Clone()
        {
            var clone = (Volume)MemberwiseClone();
            clone.Position = Position.Clone();
            return clone;
        }
    }
}
