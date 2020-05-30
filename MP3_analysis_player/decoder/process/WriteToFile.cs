using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MP3_analysis_player.decoder.process
{
    class WriteToFile
    {
        private static FileStream file;

        public WriteToFile(string filename)
        {
            char[] sp = new[] { '.' };
            string[] _f = filename.Split(sp,2); 
            file = new FileStream($"{_f[0]}.pcm", FileMode.Create);
        }

        public void write(int length, byte[] data)
        {
            file.Write(data, 0, length);
        }
        ~WriteToFile()
        {
            file.Close();
        }
    }
}