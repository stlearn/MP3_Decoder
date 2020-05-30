using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_analysis_player.decoder.process
{
    internal class Buffer16BitStereo : ABuffer
    {
        //声道数
        private static readonly int CHANNELS = 2;
        //存储pcm数据
        public byte[] m_Buffer = new byte[OBUFFERSIZE * 2];
        public int[] m_Bufferp = new int[MAXCHANNELS];
        private int m_End;
        private int m_Offset;

        public Buffer16BitStereo()
        {
            ClearBuffer();
        }

        public byte[] getData()
        {
            return m_Buffer;
        }
        public int BytesLeft
        {
            get
            {
                return m_End - m_Offset;
            }
        }

        public int Read(byte[] bufferOut, int offset, int count)
        {
            if (bufferOut == null)
            {
                throw new ArgumentNullException("bufferOut");
            }
            if ((count + offset) > bufferOut.Length)
            {
                throw new ArgumentException("The sum of offset and count is larger than the buffer length");
            }
            int remaining = BytesLeft;
            int copySize;
            if (count > remaining)
            {
                copySize = remaining;
            }
            else
            {
                // Copy an even number of sample frames
                int remainder = count % (2 * CHANNELS);
                copySize = count - remainder;
            }

            Array.Copy(m_Buffer, m_Offset, bufferOut, offset, copySize);

            m_Offset += copySize;
            return copySize;
        }

        public override void Append(int channel, short sampleValue)
        {
            m_Buffer[m_Bufferp[channel]] = (byte)(sampleValue & 0xff);
            m_Buffer[m_Bufferp[channel] + 1] = (byte)(sampleValue >> 8);

            m_Bufferp[channel] += CHANNELS * 2;
        }

        public override void AppendSamples(int channel, float[] samples)
        {
            if (samples == null)
            {
                throw new ArgumentNullException("samples");
            }
            if (samples.Length < 32)
            {
                throw new ArgumentException("samples must have 32 values");
            }
            int pos = m_Bufferp[channel];

            for (int i = 0; i < 32; i++)
            {
                float fs = samples[i];
                if (fs > 32767.0f)
                    fs = 32767.0f;
                else if (fs < -32767.0f)
                    fs = -32767.0f;

                int sample = (int)fs;
                m_Buffer[pos] = (byte)(sample & 0xff);
                m_Buffer[pos + 1] = (byte)(sample >> 8);

                pos += CHANNELS * 2;
            }

            m_Bufferp[channel] = pos;
        }

        public override sealed void ClearBuffer()
        {
            m_Offset = 0;
            m_End = 0;

            for (int i = 0; i < CHANNELS; i++)
                m_Bufferp[i] = i * 2;
        }

        public override void SetStopFlag()
        {
        }

        public override void WriteBuffer(int val)
        {
            m_Offset = 0;

            m_End = m_Bufferp[0];
        }

        public override void Close()
        {
        }
    }
}
