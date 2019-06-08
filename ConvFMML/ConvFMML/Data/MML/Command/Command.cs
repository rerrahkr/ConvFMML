using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command
{
    public class Command
    {
        public MMLCommandRelation CommandRelation { get; set; }

        protected Command(MMLCommandRelation relation)
        {
            CommandRelation = relation;
        }

        public string ToString(Settings settings, SoundModule module)
        {
            return GenerateString(settings, module);
        }

        protected virtual string GenerateString(Settings settings, SoundModule module)
        {
            return string.Empty;
        }
    }
}
