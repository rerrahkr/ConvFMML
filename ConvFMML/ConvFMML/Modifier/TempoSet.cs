using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Modifier
{
    public class TempoSet
    {
        private Data.Intermediate.Event.Tempo intermediate;
        private Data.MML.Command.Tempo mml;

        public Data.Intermediate.Event.Tempo Data
        {
            get
            {
                return intermediate;
            }
        }

        public Data.Intermediate.Position Position
        {
            get
            {
                return intermediate.Position;
            }
        }

        public TempoSet(Data.Intermediate.Event.Tempo intermediate, Data.MML.Command.Tempo mml)
        {
            this.intermediate = intermediate;
            this.mml = mml;
        }

        public string ToString(Settings settings, SoundModule module)
        {
            return mml.ToString(settings, module);
        }
    }
}
