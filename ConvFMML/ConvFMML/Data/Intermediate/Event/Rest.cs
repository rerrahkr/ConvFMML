using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.Intermediate.Event
{
    public class Rest : NoteRest
    {
        public Rest(Position start, Position end) : base(start, end) { }

        public override NoteRest Clone()
        {
            return new Rest(Start.Clone(), End?.Clone());
        }
    }
}
