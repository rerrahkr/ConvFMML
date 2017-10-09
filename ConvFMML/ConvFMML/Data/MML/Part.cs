using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML
{
    public class Part
    {
        public List<Bar> BarList { get; }
        public SoundModule SoundModule { get; }
        public string Name { get; }

        public int Length
        {
            get
            {
                return BarList.Count;
            }
        }

        public Part(List<Bar> barList, SoundModule module, string name)
        {
            BarList = barList;
            SoundModule = module;
            Name = name;
        }
    }
}
