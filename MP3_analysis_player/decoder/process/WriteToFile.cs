using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_analysis_player.decoder.process
{
    class WriteToFile
    {
        private static FileStream file = new FileStream("a.pcm",FileMode.OpenOrCreate | FileMode.Append);
        public void write(int length,byte[] data)
        {
            file.Write(data,0,length);
        }
        ~WriteToFile()
        {
            file.Close();
        }
    }
}
