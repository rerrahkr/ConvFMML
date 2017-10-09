using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Modifier
{
    public class ChangeEventSet
    {
        private Data.Intermediate.Event.ChangeEvent intermediate;
        private Data.MML.Command.ControlCommand mml;

        public Data.Intermediate.Event.ChangeEvent Data
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

        public ChangeEventSet(Data.Intermediate.Event.ChangeEvent intermediate, Data.MML.Command.ControlCommand mml)
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
