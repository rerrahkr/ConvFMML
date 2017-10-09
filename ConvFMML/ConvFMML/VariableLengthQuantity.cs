using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML
{
    public class VariableLengthQuantity
    {
        public uint Value { get; }
        public int BytesLength { get; }

        public VariableLengthQuantity(byte[] bs, int startIndex)
        {
            byte b;
            int index = startIndex;
            Value = 0;

            while (((b = bs[index++]) & 0x80) != 0)
            {
                Value |= (b & 0x7fu);
                Value <<= 7;
            }
            Value |= b;

            BytesLength = index - startIndex;
        }
    }
}
