using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML
{
    public static class LittleEndianConverter
    {
        public static byte[] Convert(byte[] bs)
        {
            if (BitConverter.IsLittleEndian == true)
            {
                return bs.Reverse().ToArray();
            }
            else
            {
                return bs;
            }
        }

        private static byte[] SubArray(byte[] src, int startIndex, int count)
        {
            byte[] ret = new byte[count];
            Array.Copy(src, startIndex, ret, 0, count);
            return ret;
        }

        public static short ToInt16(byte[] value, int startIndex)
        {
            byte[] sub = SubArray(value, startIndex, sizeof(short));
            return BitConverter.ToInt16(Convert(sub), 0);
        }

        public static ushort ToUInt16(byte[] value, int startIndex)
        {
            byte[] sub = SubArray(value, startIndex, sizeof(ushort));
            return BitConverter.ToUInt16(Convert(sub), 0);
        }

        public static uint ToUInt32(byte[] value, int startIndex)
        {
            byte[] sub = SubArray(value, startIndex, sizeof(uint));
            return BitConverter.ToUInt32(Convert(sub), 0);
        }
    }
}
