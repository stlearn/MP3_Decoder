using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_analysis_player.player
{
    class PcmDatas
    {
        private List<byte> Pcm_data;
        public int Channels{ get; set; }
        public int Frequency { get; set; }

        public PcmDatas()
        {
            Pcm_data = new List<byte>();
            Channels = 1;
            Frequency = 1;
        }

        public void AddPcmData(byte[] datas)
        {
            foreach (var data in datas)
            {
                Pcm_data.Add(data);
            }

        }

        public List<byte> GetPcmData()
        {
            return Pcm_data;
        }

        public IEnumerable<short> getLeft()
        {
            List<short> left = new List<short>();

            for (int i = 0; i < Pcm_data.Count; i += 4)
            {
                short tmp;
                tmp = Pcm_data[i];
                tmp = (short)((tmp << 8) + Pcm_data[i + 1]);
                left.Add(tmp);
            }
            return left.AsEnumerable();
        }
        
        public IEnumerable<short> getRight()
        {
            List<short> right = new List<short>();

            for (int i = 2; i < Pcm_data.Count; i += 4)
            {
                short tmp;
                tmp = Pcm_data[i];
                tmp = (short)((tmp << 8) + Pcm_data[i + 1]);
                right.Add(tmp);
            }
            return right.AsEnumerable();
        }
        

        public void Clear()
        {
            Pcm_data.Clear();
            Channels = 0;
            Frequency = 0;
        }
    }
}
