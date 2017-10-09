using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Converter
{
    public class LengthElement : ICloneable
    {
        public int Length { get; }
        public int Gate { get; }
        public bool TripletFlag { get; }

        public LengthElement(int length, int gate, bool tripletFlag)
        {
            Length = length;
            Gate = gate;
            TripletFlag = tripletFlag;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public LengthElement Clone()
        {
            return (LengthElement)MemberwiseClone();
        }
    }
}
