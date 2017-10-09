using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.Intermediate.Event
{
    public abstract class NoteRest : ICloneable
    {
        public Position Start { get; set; }
        public Position End { get; set; }
        public bool TieFlag { get; set; } = false;

        protected NoteRest(Position start, Position end)
        {
            Start = start;
            End = end;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public abstract NoteRest Clone();
    }
}
