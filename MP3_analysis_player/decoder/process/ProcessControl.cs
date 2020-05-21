using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MP3_analysis_player.decoder.format_definition;

namespace MP3_analysis_player.decoder.process
{
    /// <summary>
    /// 一帧的解码控制
    /// </summary>
    class ProcessControl
    {
        private readonly Data_Frame_Header_Info _dataFrameHeaderInfo;
        private readonly Side_Infomation _sideInfomation;
        private readonly byte[] _mainDatas;
        private readonly string _filename;

        public ProcessControl(Data_Frame_Header_Info dataFrameHeaderInfo,Side_Infomation sideInfomation,byte[] mainDatas,string filename)
        {
            _dataFrameHeaderInfo = dataFrameHeaderInfo;
            _sideInfomation = sideInfomation;
            _mainDatas = mainDatas;
            _filename = filename;
        }

        /// <summary>
        /// 开始解码并输出为文件
        /// </summary>
        public void Start()
        {

        }
    }
}
