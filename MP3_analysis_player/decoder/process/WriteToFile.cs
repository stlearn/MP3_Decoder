using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MP3_analysis_player.player;

namespace MP3_analysis_player.decoder.process
{
    class WriteToFile
    {
        private readonly PcmDatas _pcmDatas;
        private static FileStream file;

        public WriteToFile(string filename,PcmDatas pcmDatas)
        {
            _pcmDatas = pcmDatas;
            char[] sp = new[] { '.' };
            string[] _f = filename.Split(sp,2); 
            file = new FileStream($"{_f[0]}.pcm", FileMode.Create);
        }

        public void write(int length, byte[] data)
        {
            file.Write(data, 0, length);
            _pcmDatas.AddPcmData(data);
        }
        ~WriteToFile()
        {
            file.Close();
        }
    }
}