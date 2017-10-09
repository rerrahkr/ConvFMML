using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.Intermediate.Event
{
    public class Note : NoteRest
    {
        public int KeyNumber { get; set; }
        public int Velocity { get; set; }

        public Note(Position start, Position end, int keyNumber, int velocity) : base(start, end)
        {
            KeyNumber = keyNumber;
            Velocity = velocity;
        }

        public override NoteRest Clone()
        {
            var clone = (Note)MemberwiseClone();
            clone.Start = Start.Clone();
            clone.End = End?.Clone();
            return clone;
        }
    }
}
