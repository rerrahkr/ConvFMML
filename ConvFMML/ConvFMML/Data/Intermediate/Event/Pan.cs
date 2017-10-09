using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.Intermediate.Event
{
    public class Pan : ChangeEvent, ICloneable
    {
        public Pan(Position position, int value) : base(position, value) { }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public new Pan Clone()
        {
            var clone = (Pan)MemberwiseClone();
            clone.Position = Position.Clone();
            return clone;
        }
    }
}
