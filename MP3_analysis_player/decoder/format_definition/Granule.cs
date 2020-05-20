using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_analysis_player.decoder.format_definition
{
    /// <summary>
    /// 粒度信息的定义类
    /// </summary>
    class Granule
    {
        /// <summary>
        /// 主数据长度
        /// 单声道12位
        /// 双声道24位
        /// </summary>
        public uint[] part2_3_length = new uint[2];

        /// <summary>
        /// 大值区长度
        /// 单声道9
        /// 双声道18
        /// </summary>
        public ushort[] big_values = new ushort[2];          

        /// <summary>
        /// 全局增益因子
        /// 表示编码器采用的量化步长
        /// 单声道8
        /// 双声道16位
        /// </summary>
        public ushort[] global_gain = new ushort[2];           

        /// <summary>
        /// 比例因子压缩系数
        /// 单声道4
        /// 双声道8位
        /// </summary>
        public byte[] scale_fac_compress = new byte[2];     //比例因子压缩系数 

        /// <summary>
        /// 加窗标志位
        /// 单声道1位
        /// 双声道2位
        /// 0表示没有特殊窗
        /// 1表示有特殊窗
        /// </summary>
        public byte[] window_switching_flag = new byte[2];

        /// <summary>
        /// window_switching_flag ===1 时
        /// 窗类型
        /// 单声道2位
        /// 双声道4位
        /// 00默认 01起始窗（特殊的长窗） 10短窗 11结束窗（特殊的长窗）
        /// window_switching_flag ===0 时
        /// 00 
        /// </summary>
        public byte[] block_type = new byte[2];             

        /// <summary>
        /// window_switching_flag ===1 时
        /// 混合窗标志
        /// 单声道1位
        /// 双声道2位
        /// </summary>
        public byte[] mixed_block_flag = new byte[2];


        /// <summary>
        /// window_switching_flag ===1 时
        /// 单声道10（拆成2*5） 双声道20（拆两个2*5）
        ///window_switching_flag ===0 时
        /// 单声道15（拆成3*5） 双声道30（拆两个3*5） 
        /// </summary>
        public uint[][] table_select = new uint[2][]{new uint[3],new uint[3]};         //霍夫曼解码表选择 
        
        /// <summary>
        /// 子块增益
        /// window_switching_flag ===1 时
        /// 短块中的缩放因子
        /// 单声道9 双声道18
        /// 是3*3的需要拆开
        /// </summary>
        public uint[][] subblock_gain = new uint[2][] { new uint[3], new uint[3] };      //子块增益 

        /// <summary>
        /// 大值区 0 号区大小
        /// 单4双8
        /// </summary>
        public byte[] region0_count = new byte[2];

        /// <summary>
        /// 大值区 1 号区大小
        /// 单3双6
        /// </summary>
        public byte[] region1_count = new byte[2];   
        
        /// <summary>
        /// 预标志
        /// 单1双2
        /// 反量化的时候用
        /// </summary>
        public byte[] preflag = new byte[2];

        /// <summary>
        /// 比例因子缩放
        /// 单1双2
        /// 反量化的时候用
        /// </summary>
        public byte[] scalefac_scale = new byte[2];

        /// <summary>
        /// 霍夫曼小值区解码表
        /// 单1双2
        /// </summary>
        public byte[] count1table_select = new byte[2];

    }
}
