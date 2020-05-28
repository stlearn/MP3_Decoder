using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_analysis_player.decoder.process
{
    internal class SBI
    {
        public int[] l;
        public int[] s;

        public SBI()
        {
            l = new int[23];
            s = new int[14];
        }

        public SBI(int[] thel, int[] thes)
        {
            l = thel;
            s = thes;
        }
    }
}
