using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command
{
    public abstract class Rest : NoteRest
    {
        public Rest(List<int> length, MMLCommandRelation relation) : base(length, relation) { }

        protected abstract override string GenerateString(Settings settings, SoundModule module);
    }
}
