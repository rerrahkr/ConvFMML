using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML
{
    public class MML
    {
        public List<Part> PartList { get; }
        public string Title { get; }
        public int CountsPerWholeNote { get; }
        public MMLStyle Style { get; }

        public int PartSize
        {
            get
            {
                return PartList.Count;
            }
        }

        public MML(List<Part> partList, string title, int countsPerWholeNote, MMLStyle style)
        {
            PartList = partList;
            Title = title;
            CountsPerWholeNote = countsPerWholeNote;
            Style = style;
        }
    }
}
