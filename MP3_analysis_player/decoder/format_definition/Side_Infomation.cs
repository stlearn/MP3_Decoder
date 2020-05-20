using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_analysis_player.decoder.format_definition
{
    /// <summary>
    /// side infomation 定义类
    /// 单声道17字节
    /// 双声道32字节
    /// </summary>
    class Side_Infomation
    {
        /// <summary>
        /// 主数据开始
        /// 单双声道都是9bit
        /// </summary>
        public ushort main_data_begin;

        /// <summary>
        /// 私有位（两个粒度公用）
        /// 单声道5位
        /// 双声道3位
        /// </summary>
        public byte private_bits;

        /// <summary>
        /// 缩放因子选择信息（两个粒度公用）
        /// 单声道4位，双声道8位
        /// </summary>
        public byte[][] scfsi = new byte[2][]{new byte[4],new byte[4]};

        /// <summary>
        /// 粒度组0的边信息
        /// 单声道59位
        /// 双声道118
        /// </summary>
        public Granule granule0;
        
        /// <summary>
        /// 粒度组1的边信息
        /// 单声道59位
        /// 双声道118
        /// </summary>
        public Granule granule1;
    }
}
