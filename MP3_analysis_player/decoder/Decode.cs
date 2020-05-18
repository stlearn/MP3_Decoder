using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MP3_analysis_player.decoder.format_definition;

namespace MP3_analysis_player.decoder
{
    /// <summary>
    /// 解码一帧的流程控制
    /// </summary>
    class Decode
    {
        /// <summary>
        /// 这一帧的数据帧头部信息和自定义信息的计算值
        /// </summary>
        private readonly Data_Frame_Header_Info frameHeaderInfo;
        /// <summary>
        /// 帧除去头部和CRC的部分
        /// </summary>
        private readonly byte[] frame_data;

        public Decode(Data_Frame_Header_Info _frameHeaderInfo, byte[] _frameData)
        {
            frameHeaderInfo = _frameHeaderInfo;
            frame_data = _frameData;
        }


    }
}
