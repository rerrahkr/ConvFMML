using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.Intermediate.Event
{
    public abstract class ChangeEvent
    {
        public Position Position { get; set; }
        public int Value { get; set; }

        protected ChangeEvent(Position position, int value)
        {
            Position = position;
            Value = value;
        }

        public ChangeEvent Clone()
        {
            return (ChangeEvent)MemberwiseClone();
        }
    }
}
