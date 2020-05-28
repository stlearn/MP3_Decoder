using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_analysis_player.decoder
{
    internal sealed class GetBit
    {
        /// <summary>
        /// 总位数
        /// </summary>
        private const int BUFSIZE = 4096 * 8;

        private static readonly int BUFSIZE_MASK = BUFSIZE - 1;

        private int[] buf;
        private int offset, totbit, buf_byte_idx;

        internal GetBit()
        {
            InitBlock();

            offset = 0;
            totbit = 0;
            buf_byte_idx = 0;
        }

        private void InitBlock()
        {
            buf = new int[BUFSIZE];
        }

        public int hsstell()
        {
            return (totbit);
        }

        /// <summary>
        /// 读取N位
        /// </summary>
        public int ReadBits(int N)
        {
            totbit += N;

            int val = 0;

            int pos = buf_byte_idx;
            if (pos + N < BUFSIZE)
            {
                while (N-- > 0)
                {
                    val <<= 1;
                    val |= ((buf[pos++] != 0) ? 1 : 0);
                }
            }
            else
            {
                while (N-- > 0)
                {
                    val <<= 1;
                    val |= ((buf[pos] != 0) ? 1 : 0);
                    pos = (pos + 1) & BUFSIZE_MASK;
                }
            }
            buf_byte_idx = pos;
            return val;
        }

        /// <summary>
        ///读取一位
        /// </summary>
        public int ReadOneBit()
        {
            totbit++;
            int val = buf[buf_byte_idx];
            buf_byte_idx = (buf_byte_idx + 1) & BUFSIZE_MASK;
            return val;
        }

        /// <summary>
        /// 写入一个字节
        /// </summary>
        public void hputbuf(int val)
        {
            int ofs = offset;
            buf[ofs++] = val & 0x80;
            buf[ofs++] = val & 0x40;
            buf[ofs++] = val & 0x20;
            buf[ofs++] = val & 0x10;
            buf[ofs++] = val & 0x08;
            buf[ofs++] = val & 0x04;
            buf[ofs++] = val & 0x02;
            buf[ofs++] = val & 0x01;

            if (ofs == BUFSIZE)
                offset = 0;
            else
                offset = ofs;
        }

        /// <summary>
        /// 返回n bit
        /// </summary>
        public void RewindStreamBits(int bitCount)
        {
            totbit -= bitCount;
            buf_byte_idx -= bitCount;
            if (buf_byte_idx < 0)
                buf_byte_idx += BUFSIZE;
        }

        /// <summary>
        /// 返回n byte
        /// </summary>
        public void RewindStreamBytes(int byteCount)
        {
            int bits = (byteCount << 3);
            totbit -= bits;
            buf_byte_idx -= bits;
            if (buf_byte_idx < 0)
                buf_byte_idx += BUFSIZE;
        }
    }
}
