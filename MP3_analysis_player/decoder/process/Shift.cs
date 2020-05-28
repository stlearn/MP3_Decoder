using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_analysis_player.decoder.process
{
    /// <summary>
    /// 执行移位操作
    /// </summary>
    internal class Shift
    {
        public static int URShift(int number, int bits)
        {
            if (number >= 0)
                return number >> bits;
            return (number >> bits) + (2 << ~bits);
        }

        public static int URShift(int number, long bits)
        {
            return URShift(number, (int)bits);
        }

        public static long URShift(long number, int bits)
        {
            if (number >= 0)
                return number >> bits;
            return (number >> bits) + (2L << ~bits);
        }

        public static long URShift(long number, long bits)
        {
            return URShift(number, (int)bits);
        }
    }
}
