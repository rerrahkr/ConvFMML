using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command
{
    public abstract class Note : NoteRest
    {
        public int Octave { get; }
        public string Name { get; }

        public Note(int octave, string name, List<int> length, MMLCommandRelation relation) : base(length, relation)
        {
            Octave = octave;
            Name = name;
        }

        protected abstract override string GenerateString(Settings settings, SoundModule module);
    }
}
