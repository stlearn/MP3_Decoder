using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_analysis_player.decoder.process
{
    abstract class ABuffer
    {
        public const int OBUFFERSIZE = 2 * 1152; //2 * 1152个样本
        public const int MAXCHANNELS = 2; //最大声道数

        public abstract void Append(int channel, short valueRenamed);


        public virtual void AppendSamples(int channel, float[] f)
        {
            for (int i = 0; i < 32; i++)
            {
                Append(channel, Clip((f[i])));
            }
        }

        private static short Clip(float sample)
        {
            return ((sample > 32767.0f) ? (short)32767 : ((sample < -32768.0f) ? (short)-32768 : (short)sample));
        }

        public abstract void WriteBuffer(int val);

        public abstract void Close();

        public abstract void ClearBuffer();

        public abstract void SetStopFlag();
    }
}
