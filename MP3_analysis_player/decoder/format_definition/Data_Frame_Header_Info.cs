using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_analysis_player.decoder.format_definition
{
    /// <summary>
    /// 四字节数据帧头部
    /// </summary>
    class Data_Frame_Header_Info
    {
        //数据帧头部内容
        /// <summary>
        /// 同步位，12位
        /// </summary>
        public ushort syn;

        /// <summary>
        /// 版本1位，0：MPEG2 1：MPEG1
        /// </summary>
        public byte version;

        /// <summary>
        /// layer描述2位， 00保留；01layer3；02layer2；03layer1 
        /// </summary>
        public byte layer;

        /// <summary>
        /// 校验位 1没校验 0紧跟帧头有两字节的校验码
        /// </summary>
        public byte if_crc;

        /// <summary>
        /// 位率索引4位
        /// </summary>
        public byte bitrate_index;

        /// <summary>
        ///采样频率2位
        /// </summary>
        public byte sampling_frequency;

        /// <summary>
        ///填充位1位 0无填充 1有填充
        /// </summary>
        public byte padding;

        /// <summary>
        /// 私有位1位
        /// </summary>
        public byte extension;

        /// <summary>
        /// 声道模式2位 00立体声 01联合立体声 10双声道 11单声道
        /// </summary>
        public byte track_mode;

        /// <summary>
        /// 联合立体声扩展模式2位
        /// </summary>
        public byte stereo_mode_ext;

        /// <summary>
        /// 版权1位 0没有 1有版权
        /// </summary>
        public byte copyright;

        /// <summary>
        /// 原创1位   0拷贝 1原创
        /// </summary>
        public byte original;

        /// <summary>
        /// 强调2位
        /// </summary>
        public byte emphasis;

        /// <summary>
        /// 数据帧比特率
        /// </summary>
        public int bitrate;

        /// <summary>
        /// 采样频率
        /// </summary>
        public int frequency;

        /// <summary>
        /// 采样个数
        /// </summary>
        public int sample_number;

        /// <summary>
        /// 数据帧长。包含数据帧头部和CRC的两个字节（有可能没有）
        /// </summary>
        public long frame_length;
    }
}
