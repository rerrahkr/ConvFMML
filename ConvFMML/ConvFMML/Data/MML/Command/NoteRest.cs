using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command
{
    public abstract class NoteRest : Command
    {
        public List<int> Length { get; }

        protected NoteRest(List<int> length, MMLCommandRelation relation) : base(relation)
        {
            Length = length;
        }

        protected abstract override string GenerateString(Settings settings, SoundModule module);
    }
}
