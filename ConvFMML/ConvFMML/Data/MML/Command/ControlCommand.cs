using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command
{
    public abstract class ControlCommand : Command
    {
        public int Value { get; }

        protected ControlCommand(int value, MMLCommandRelation relation) : base(relation)
        {
            Value = value;
        }

        protected abstract override string GenerateString(Settings settings, SoundModule module);
    }
}
