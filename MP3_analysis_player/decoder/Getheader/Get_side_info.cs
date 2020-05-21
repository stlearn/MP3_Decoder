using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using MP3_analysis_player.decoder.format_definition;

namespace MP3_analysis_player.decoder.Getheader
{
    /// <summary>
    /// 获取side information的类
    /// </summary>
    class Get_side_info
    {
        //输入帧数据
        private readonly byte[] data;

        //数据帧头信息
        private readonly Data_Frame_Header_Info _dataFrameHeaderInfo;

        //定义一个用来返回边信息的边信息对象
        private Side_Infomation sideInfomation = new Side_Infomation();

        public Get_side_info(byte[] _data, Data_Frame_Header_Info _dataFrameHeaderInfo)
        {
            data = _data;
            this._dataFrameHeaderInfo = _dataFrameHeaderInfo;
        }

        /// <summary>
        /// 获取一个数据帧的边信息
        /// </summary>
        /// <returns>边信息类side_information</returns>
        public Side_Infomation GetSideInfomation()
        {
            //求side infomation
            sideInfomation.main_data_begin = data[0];
            sideInfomation.main_data_begin = (ushort) ((sideInfomation.main_data_begin << 1) + (data[1] >> 7));

            //单声道
            if (_dataFrameHeaderInfo.track_mode == 2)
            {
                GetSideInfomationSingle();
            }
            //双声道
            else
            {
                GetSideInfomationDouble();
            }

            return sideInfomation;
        }

        /// <summary>
        /// 获取单声道边信息内容
        /// </summary>
        private void GetSideInfomationSingle()
        {
            //求private_bits
            sideInfomation.private_bits = (byte) ((data[1] >> 2) & 0x1F);
            //求scfsi
            sideInfomation.scfsi[0][0] = (byte) (data[1] >> 1 & 0x01);
            sideInfomation.scfsi[0][1] = (byte) (data[1] & 0x01);
            sideInfomation.scfsi[0][2] = (byte) (data[2] >> 7 & 0x01);
            sideInfomation.scfsi[0][3] = (byte) (data[2] >> 6 & 0x01);

            //第三(索引2)字节还剩6位
            sideInfomation.granule0 = GetGranuleSingle0();
            sideInfomation.granule1 = GetGranuleSingle1();
        }

        /// <summary>
        /// 获取双声道边信息内容
        /// </summary>
        private void GetSideInfomationDouble()
        {
            //求private_bits
            sideInfomation.private_bits = (byte) ((data[1] >> 4) & 0x07);
            
            //求声道1scfsi
            sideInfomation.scfsi[0][0] = (byte) (data[1] >> 3 & 0x01);
            sideInfomation.scfsi[0][1] = (byte) (data[1] >> 2 & 0x01);
            sideInfomation.scfsi[0][2] = (byte) (data[1] >> 1 & 0x01);
            sideInfomation.scfsi[0][3] = (byte) (data[1] & 0x01);

            //从data[2]开始
            sideInfomation.granule0 = GetGranuleDouble0();

            //求声道2scfsi 从data[16] 剩两位
            sideInfomation.scfsi[1][0] = (byte)(data[16] >> 1 & 0x01);
            sideInfomation.scfsi[1][1] = (byte)(data[16] & 0x01);
            sideInfomation.scfsi[1][2] = (byte)(data[17] >> 7 & 0x01);
            sideInfomation.scfsi[1][3] = (byte)(data[17] >> 6 & 0x01);

            //从data[17] 剩6位
            sideInfomation.granule1 = GetGranuleDouble1();
        }

        /// <summary>
        /// 获取单声道第一个粒度
        /// </summary>
        /// <returns></returns>
        private Granule GetGranuleSingle0()
        {
            Granule granule = new Granule();
            granule.part2_3_length[0] = (uint) (data[2] & 0x3F);
            granule.part2_3_length[0] = (uint) ((granule.part2_3_length[0] << 6) + (data[3] >> 2));

            granule.big_values[0] = (ushort) (data[3] & 0x3);
            granule.big_values[0] = (ushort) ((granule.big_values[0] << 7) + (data[4] >> 1));

            granule.global_gain[0] = (ushort) (data[4] & 0x01);
            granule.global_gain[0] = (ushort) ((granule.global_gain[0] << 7) + (data[5] >> 1));

            granule.scale_fac_compress[0] = (byte) (data[5] & 0x01);
            granule.scale_fac_compress[0] = (byte) ((granule.scale_fac_compress[0] << 3) + data[6] >> 5);

            granule.window_switching_flag[0] = (byte) ((data[6] >> 4) & 0x01);
            //当上面这个为1时
            if (granule.window_switching_flag[0] == 1)
            {
                granule.block_type[0] = (byte) ((data[6] >> 2) & 0x03);
                granule.mixed_block_flag[0] = (byte) ((data[6] >> 1) & 0x01);

                granule.table_select[0][0] = (uint) (data[6] & 0x01);
                granule.table_select[0][0] = (uint) ((granule.table_select[0][0] << 4) + data[7] >> 4);

                granule.table_select[0][1] = (uint) (((data[7] & 0x0f) << 1) + (data[8] >> 7));

                granule.subblock_gain[0][0] = (uint) ((data[8] >> 4) & 0x07);
                granule.subblock_gain[0][1] = (uint) ((data[8] >> 1) & 0x07);
                granule.subblock_gain[0][2] = (uint) (((data[8] & 0x01) << 2) + (data[9] >> 6));
            }
            //当上面这个为1时
            else
            {
                granule.table_select[0][0] = (uint) (((data[6] & 0x0F) << 1) + (data[7] >> 7));
                granule.table_select[0][1] = (uint) ((data[7] >> 2) & 0x1f);
                granule.table_select[0][2] = (uint) (((data[7] & 0x03) << 3) + (data[8] >> 5));

                granule.region0_count[0] = (byte) ((data[8] >> 1) & 0x0f);

                granule.region1_count[0] = (byte) (data[8] & 0x01);
                granule.region1_count[0] = (byte) ((granule.region1_count[0] << 2) + (data[9] >> 6));
            }

            granule.preflag[0] = (byte) ((data[9] >> 5) & 0x01);
            granule.scalefac_scale[0] = (byte) ((data[9] >> 4) & 0x01);
            granule.count1table_select[0] = (byte) ((data[9] >> 3) & 0x01);

            return granule;
        }

        /// <summary>
        /// 获取单声道第二个粒度
        /// </summary>
        /// <returns></returns>
        private Granule GetGranuleSingle1()
        {
            Granule granule = new Granule();

            granule.part2_3_length[0] = (uint)(data[9] & 0x07);
            granule.part2_3_length[0] = (uint)((granule.part2_3_length[0] << 8) + data[10]);
            granule.part2_3_length[0] = (uint) ((granule.part2_3_length[0] << 1) + (data[11] >> 7));

            granule.big_values[0] = (ushort)(data[11] & 0x7f);
            granule.big_values[0] = (ushort)((granule.big_values[0] << 2) + (data[12] >> 6));

            granule.global_gain[0] = (ushort)(data[12] & 0x3f);
            granule.global_gain[0] = (ushort)((granule.global_gain[0] << 2) + (data[13] >> 6));

            granule.scale_fac_compress[0] = (byte)((data[13]>>2) & 0x0f);

            granule.window_switching_flag[0] = (byte)((data[13] >> 1) & 0x01);
            //当上面这个为0时
            if (granule.window_switching_flag[0] == 1)
            {
                granule.block_type[0] = (byte)(((data[13] & 0x01)<<1)+(data[14]>>7));

                granule.mixed_block_flag[0] = (byte)((data[14] >> 6) & 0x01);

                granule.table_select[0][0] = (uint)((data[14]>>1) & 0x1f);

                granule.table_select[0][1] = (uint)(((data[14] & 0x01) << 4) + (data[15] >> 4));

                granule.subblock_gain[0][0] = (uint) ((data[15] >> 1) & 0x07);
                granule.subblock_gain[0][1] = (uint)(((data[15]& 0x01) << 2)+(data[16]>>6));
                granule.subblock_gain[0][2] = (uint) ((data[16] >> 3) & 0x07);
            }
            //当上面这个为1时
            else
            {
                granule.table_select[0][0] = (uint)(((data[13] & 0x01) << 4) + (data[14] >> 4));
                granule.table_select[0][1] = (uint) (((data[14] & 0x0f) << 1) + (data[15] >> 7));
                granule.table_select[0][2] = (uint)((data[15] >> 2) & 0x1f);

                granule.region0_count[0] = (byte)(((data[15] & 0x03) << 2)+(data[16]>>6));

                granule.region1_count[0] = (byte)((data[16]>>3) & 0x07);
            }

            granule.preflag[0] = (byte)((data[16] >> 2) & 0x01);
            granule.scalefac_scale[0] = (byte)((data[16] >> 1) & 0x01);
            granule.count1table_select[0] = (byte)(data[16] & 0x01);

            return granule;
        }

        /// <summary>
        /// 获取双声道第一个声道的两个粒度信息
        /// </summary>
        /// <returns></returns>
        private Granule GetGranuleDouble0()
        {
            Granule granule = new Granule();

            //第一个粒度
            granule.part2_3_length[0] = (uint)data[2];
            granule.part2_3_length[0] = (uint)((granule.part2_3_length[0] << 4) + (data[3]>>4));

            granule.big_values[0] = (ushort)(data[3] & 0x0F);
            granule.big_values[0] = (ushort)((granule.big_values[0] << 5) + (data[4]>>3));

            granule.global_gain[0] = (ushort)(data[4]&0x07);
            granule.global_gain[0] = (ushort) ((granule.global_gain[0] << 5) + (data[5] >> 3));

            granule.scale_fac_compress[0] = (byte)(data[5] & 0x07);
            granule.scale_fac_compress[0] = (byte) ((granule.scale_fac_compress[0] << 1) + (data[6] >> 7));

            granule.window_switching_flag[0] = (byte)((data[6] >> 6) & 0x01);
            //当上面这个为0时
            if (granule.window_switching_flag[0] == 1)
            {
                granule.block_type[0] = (byte) ((data[6] >> 4) & 0x03);

                granule.mixed_block_flag[0] = (byte)((data[6] >> 3) & 0x01);

                granule.table_select[0][0] = (uint)(data[6] & 0x07);
                granule.table_select[0][0] = (uint) ((granule.table_select[0][0] << 2) + (data[7] >> 6));

                granule.table_select[0][1] = (uint)((data[7] >> 1) &0x1f);

                granule.subblock_gain[0][0] = (uint)(data[7] & 0x01);
                granule.subblock_gain[0][0] = (uint) ((granule.subblock_gain[0][0] << 2) + (data[8] >> 6));
                granule.subblock_gain[0][1] = (uint)((data[8] >> 3) & 0x07);
                granule.subblock_gain[0][2] = (uint)(data[8] & 0x07);
            }
            //当上面这个为1时
            else
            {
                granule.table_select[0][0] = (uint)((data[6] >> 1) & 0x1f);
                granule.table_select[0][1] = (uint)(data[6] & 0x01);
                granule.table_select[0][1] = (uint) ((granule.table_select[0][1] << 4) + (data[7] >> 4));
                granule.table_select[0][2] = (uint)(data[7] & 0x0f);
                granule.table_select[0][2] = (uint) ((granule.table_select[0][2] << 1) + (data[8] >> 7));

                granule.region0_count[0] = (byte) ((data[8] >> 3) & 0x0f);

                granule.region1_count[0] = (byte)(data[8] & 0x07);
            }

            granule.preflag[0] = (byte)((data[9] >> 7) & 0x01);
            granule.scalefac_scale[0] = (byte)((data[9] >> 6) & 0x01);
            granule.count1table_select[0] = (byte)((data[9] >>5)& 0x01);

            //第二个粒度
            granule.part2_3_length[1] = (uint)(data[9] & 0x1f);
            granule.part2_3_length[1] = (uint)((granule.part2_3_length[1] << 7) + (data[10]>>1));

            granule.big_values[1] = (ushort)(data[10] & 0x01);
            granule.big_values[1] = (ushort)((granule.big_values[1] << 8) + data[11]);

            granule.global_gain[1] = (ushort)(data[12]);

            granule.scale_fac_compress[1] = (byte)((data[13] >> 4) & 0x0f);

            granule.window_switching_flag[1] = (byte)((data[13] >> 3) & 0x01);
            //当上面这个为0时
            if (granule.window_switching_flag[1] == 1)
            {
                granule.block_type[1] = (byte) ((data[13] >> 1) & 0x03);

                granule.mixed_block_flag[1] = (byte)(data[13] & 0x01);

                granule.table_select[1][0] = (uint)((data[14] >> 3) & 0x1f);

                granule.table_select[1][1] = (uint)(((data[14] & 0x07) << 2) + (data[15] >> 6));

                granule.subblock_gain[1][0] = (uint)((data[15] >> 3) & 0x07);
                granule.subblock_gain[1][1] = (uint) (data[15] & 0x07);
                granule.subblock_gain[1][2] = (uint)((data[16] >> 5) & 0x07);
            }
            //当上面这个为1时
            else
            {
                granule.table_select[1][0] = (uint)((data[13] & 0x07) << 2 + (data[14] >>6));
                granule.table_select[1][1] = (uint)((data[14]>>1) & 0x1f);
                granule.table_select[1][2] = (uint)(((data[14] & 0x01) << 4)+(data[15]>>4));

                granule.region0_count[1] = (byte)(data[15] & 0x0f);

                granule.region1_count[1] = (byte)((data[16] >> 5) & 0x07);
            }

            granule.preflag[1] = (byte)((data[16] >> 4) & 0x01);
            granule.scalefac_scale[1] = (byte)((data[16] >> 3) & 0x01);
            granule.count1table_select[1] = (byte)(data[16] >>2 & 0x01);

            return granule;
        }

        /// <summary>
        /// 获取双声道第二个声道的两个粒度信息
        /// </summary>
        /// <returns></returns>
        private Granule GetGranuleDouble1()
        {
            Granule granule = new Granule();

            //第一个粒度
            granule.part2_3_length[0] = (uint)(data[17] & 0x3f);
            granule.part2_3_length[0] = (uint)((granule.part2_3_length[0] << 6) + (data[18]>>2));

            granule.big_values[0] = (ushort)(data[18] & 0x03);
            granule.big_values[0] = (ushort)((granule.big_values[0] << 7) + (data[19] >> 1));

            granule.global_gain[0] = (ushort)(data[19] & 0x01);
            granule.global_gain[0] = (ushort)((granule.global_gain[0] << 7) + (data[20] >> 1));

            granule.scale_fac_compress[0] = (byte)(data[20] & 0x01);
            granule.scale_fac_compress[0] = (byte) ((granule.scale_fac_compress[0] << 3) + (data[21] >> 5));

            granule.window_switching_flag[0] = (byte)((data[21] >> 4) & 0x01);
            //当上面这个为0时
            if (granule.window_switching_flag[0] == 1)
            {
                granule.block_type[0] = (byte)((data[21] >> 2) & 0x03);

                granule.mixed_block_flag[0] = (byte)((data[21] >> 1) & 0x01);

                granule.table_select[0][0] = (uint)(data[21] & 0x01);
                granule.table_select[0][0] = (uint) ((granule.table_select[0][0] << 4) + (data[22] >> 4));

                granule.table_select[0][1] = (uint)(((data[22] & 0x0f) << 1) + (data[23] >> 7));

                granule.subblock_gain[0][0] = (uint)((data[23] >> 4) & 0x07);
                granule.subblock_gain[0][1] = (uint)((data[23] >> 1) & 0x07);
                granule.subblock_gain[0][2] = (uint)(data[23] & 0x01);
                granule.subblock_gain[0][2] = (uint) ((granule.subblock_gain[0][2] << 2) + (data[24] >> 6));
            }
            //当上面这个为1时
            else
            {
                granule.table_select[0][0] = (uint)(((data[21] & 0x0f) << 1) + (data[22] >> 7));
                granule.table_select[0][1] = (uint) ((data[22] >> 2) & 0x1f);
                granule.table_select[0][2] = (uint)(data[22] & 0x03);
                granule.table_select[0][2] = (uint) ((granule.table_select[0][2] << 3) + (data[23] >> 5));

                granule.region0_count[0] = (byte)((data[23] >> 1) & 0x0f);

                granule.region1_count[0] = (byte)(data[23] & 0x01);
                granule.region1_count[0] = (byte) ((granule.region1_count[0] << 2) + (data[24] >> 6));
            }

            granule.preflag[0] = (byte)((data[24] >> 5) & 0x01);
            granule.scalefac_scale[0] = (byte)((data[24] >> 4) & 0x01);
            granule.count1table_select[0] = (byte) ((data[24] >> 3) & 0x01);

            //第二个粒度
            granule.part2_3_length[1] = (uint)(data[24] & 0x07);
            granule.part2_3_length[1] = (uint)((granule.part2_3_length[1] << 8) + data[25]);
            granule.part2_3_length[1] = (uint)((granule.part2_3_length[1] << 1) + (data[26] >> 7));

            granule.big_values[1] = (ushort)(data[26] & 0x7f);
            granule.big_values[1] = (ushort)((granule.big_values[1] << 2) + (data[27] >> 6));

            granule.global_gain[1] = (ushort)(data[27] & 0x3f);
            granule.global_gain[1] = (ushort)((granule.global_gain[1] << 2) + (data[28] >> 6));

            granule.scale_fac_compress[1] = (byte)((data[28] >> 2) & 0x0f);

            granule.window_switching_flag[1] = (byte)((data[28] >> 1) & 0x01);
            //当上面这个为1时
            if (granule.window_switching_flag[1] == 1)
            {
                granule.block_type[1] = (byte)(((data[28] & 0x01) << 1) + (data[29] >> 7));

                granule.mixed_block_flag[1] = (byte)((data[29] >> 6) & 0x01);

                granule.table_select[1][0] = (uint)((data[29] >> 1) & 0x1f);

                granule.table_select[1][1] = (uint)(((data[29] & 0x01) << 4) + (data[30] >> 4));

                granule.subblock_gain[1][0] = (uint)((data[30] >> 1) & 0x07);
                granule.subblock_gain[1][1] = (uint)(((data[30] & 0x01) << 2) + (data[31] >> 6));
                granule.subblock_gain[1][2] = (uint)((data[31] >> 3) & 0x07);
            }
            //当上面这个为1时
            else
            {
                granule.table_select[1][0] = (uint)(((data[28] & 0x01) << 4) + (data[29] >> 4));
                granule.table_select[1][1] = (uint)(((data[29] & 0x0f) << 1) + (data[30] >> 7));
                granule.table_select[1][2] = (uint)((data[30] >> 2) & 0x1f);

                granule.region0_count[1] = (byte)(((data[30] & 0x03) << 2) + (data[31] >> 6));

                granule.region1_count[1] = (byte)((data[31] >> 3) & 0x07);
            }

            granule.preflag[1] = (byte)((data[31] >> 2) & 0x01);
            granule.scalefac_scale[1] = (byte)((data[31] >> 1) & 0x01);
            granule.count1table_select[1] = (byte)(data[31] & 0x01);

            return granule;
        }
    }

}
