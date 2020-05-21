using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MP3_analysis_player.decoder.format_definition;
using MP3_analysis_player.decoder.Getheader;

namespace MP3_analysis_player.decoder
{
    /// <summary>
    /// 解码器主控制类
    /// </summary>
    class Decoder
    {

        /// <summary>
        /// 文件数据流
        /// </summary>
        private Stream input;

        private readonly string _filename;

        public Decoder(Stream _input,string filename)
        {
            input = _input;
            _filename = filename;
        }

        /// <summary>
        /// 控制整个解码流程的主函数
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            //文件头
            Header mp3_header = new Header(input);
            ID3V2 id3V2 = mp3_header.getID3V2();


            Data_Frame_Header_Info headerInfo = new Data_Frame_Header_Info();

            while (headerInfo!=null)
            {
                //数据帧头
                Data_Frame_Header df = new Data_Frame_Header(input);
                headerInfo = df.getHeaderInfo();

                //文件找不到数据帧或者频率或比特率有问题
                if ((headerInfo == null && Data_Frame_Header.begin_of_file) || (headerInfo!=null && headerInfo.frequency == 1) || (headerInfo!=null && headerInfo.bitrate == 0))
                {
                    return false;
                }

                
                if (headerInfo != null)
                {
                    //获取一帧的数据
                    byte[] besides_header = new byte[headerInfo.frame_length - 4];
                    input.Read(besides_header, 0, besides_header.Length);

                    //进行这一帧的解码
                    Decode decode = new Decode(headerInfo,besides_header,input,_filename);
                    decode.Start();
                }
            }
            return true;
        }
    }
}
