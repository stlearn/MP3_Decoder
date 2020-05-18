using System.IO;
using MP3_analysis_player.decoder.format_definition;
using MP3_analysis_player.decoder.table;

namespace MP3_analysis_player.decoder.header
{
    /// <summary>
    ///定义和获取数据帧头部的类
    /// </summary>
    class Data_Frame_Header
    {
        //内部
        /// <summary>
        /// 输入文件流
        /// </summary>
        private Stream input;

        /// <summary>
        /// 找到同步后读入4字节
        /// </summary>
        private byte[] header = new byte[4];

        private Data_Frame_Header_Info Header_info = new Data_Frame_Header_Info();

        public static bool begin_of_file = true;

        /// <summary>
        /// 传入文件流
        /// </summary>
        /// <param name="b">数组大小为4</param>
        public Data_Frame_Header(Stream _input)
        {
            input = _input;
        }

        private bool FindSyn()
        {
            while (input.Position < input.Length)
            {
                if (input.CanRead)
                {
                    input.Read(header, 0, 4);
                    Header_info.syn = header[0];
                    Header_info.syn = (ushort) ((Header_info.syn << 8) + header[1]);
                    Header_info.syn = (ushort) (Header_info.syn >> 4);

                    //找到帧同步头
                    if (Header_info.syn == 0x0FFF)
                    {
                        return true;
                    }
                    else
                    {
                        if (input.CanSeek && input.Position < input.Length)
                        {
                            input.Seek(-3, SeekOrigin.Current);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }

            return false;
        }


        /// <summary>
        /// 获取一个数据帧头部，如果这是第一次获取并且返回null请检查类属性begin_of_file,如果是true则可以判断文件内容错误，无法打开
        /// </summary>
        /// <returns></returns>
        public Data_Frame_Header_Info getHeaderInfo()
        {
            if (FindSyn())
            {
                Header_info.version = (byte)((header[1] >> 3) & 0x01);
                Header_info.layer = (byte)((header[1] >> 1) & 0x03);
                Header_info.if_crc = (byte)((header[1]) & 0x01);
                Header_info.bitrate_index = (byte)((header[2] >> 4) & 0x0F);
                Header_info.sampling_frequency = (byte)((header[2] >> 2) & 0x03);
                Header_info.padding = (byte)((header[2] >> 1) & 0x01);
                Header_info.extension = (byte)((header[2]) & 0x01);
                Header_info.track_mode = (byte)((header[3] >> 6) & 0x03);
                Header_info.stereo_mode_ext = (byte)((header[3] >> 4) & 0x03);
                Header_info.copyright = (byte)((header[3] >> 3) & 0x01);
                Header_info.original = (byte)((header[3] >> 2) & 0x01);
                Header_info.emphasis = (byte)((header[3] >> 1) & 0x03);

                int v=0,l=0;//版本和层转化成索引值
                switch (Header_info.version)
                {
                    case 0: v = 2;
                        break;
                    case 1: v = 1;
                        break;
                }
                switch (Header_info.layer)
                {
                    case 0:
                        l = 1;break;
                    case 1:
                        l = 3;break;
                    case 2:
                        l = 2;break;
                    case 3:
                        l = 1;break;
                    

                }



                //自动以数据的求取
                Header_info.bitrate = Table.bitrates[v-1][l-1][Header_info.bitrate_index];
                Header_info.frequency = Table.frequencies[v-1][Header_info.frequency];
                Header_info.sample_number = Table.sample_number[v-1][l-1];

                Header_info.frame_length = l == 1
                    ? ((Header_info.sample_number / 8 * Header_info.bitrate) / Header_info.frequency +
                       Header_info.padding * 4)
                    : ((Header_info.sample_number / 8 * Header_info.bitrate) / Header_info.frequency +
                       Header_info.padding);

                //此处先用来抹去CRC校验部分的影响
                if (Header_info.if_crc == 0)
                {
                    input.Seek(2, SeekOrigin.Current);
                }

                begin_of_file = false;

                return Header_info;
            }
            else
            {
                return null;
            }
        }
    }
}
