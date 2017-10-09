using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.Intermediate.Event
{
    public class Instrument : ChangeEvent, ICloneable
    {
        public Instrument(Position position, int value) : base(position, value) { }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public new Instrument Clone()
        {
            var clone = (Instrument)MemberwiseClone();
            clone.Position = Position.Clone();
            return clone;
        }
    }
}
