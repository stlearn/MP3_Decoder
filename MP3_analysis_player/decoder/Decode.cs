using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MP3_analysis_player.decoder.format_definition;
using MP3_analysis_player.decoder.Getheader;
using MP3_analysis_player.decoder.process;

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
        /// 这一帧的边信息
        /// </summary>
        private Side_Infomation sideInfomation;

        /// <summary>
        /// 帧除去头部和CRC的部分
        /// </summary>
        private readonly byte[] frame_data;

        /// <summary>
        /// 主数据
        /// </summary>
        private byte[] mainDatas;

        /// <summary>
        /// 文件输入流
        /// </summary>
        private readonly Stream input;

        private readonly string filename;
        private readonly WriteToFile _write;

        /// <summary>
        /// 主数据位数
        /// </summary>
        private uint mainDataBit = 0;


        public static bool isFirstFrame = true;

        public Decode(Data_Frame_Header_Info _frameHeaderInfo, byte[] _frameData,Stream _input,string _filename,WriteToFile write)
        {
            frameHeaderInfo = _frameHeaderInfo;
            frame_data = _frameData;
            input = _input;
            filename = _filename;
            _write = write;
        }

        /// <summary>
        /// 控制一帧解码流程的主函数
        /// </summary>
        public void Start()
        {
            //判断第一帧是vbr还是cbr info xing
            if (isFirstFrame)
            {
                isFirstFrame = false;
                return;
            }

            //获取边信息
            Get_side_info getSideInfo = new Get_side_info(frame_data,frameHeaderInfo);
            sideInfomation = getSideInfo.GetSideInfomation();
            
            //提取主数据
            mainDataBit += (sideInfomation.granule0.part2_3_length[0] + sideInfomation.granule1.part2_3_length[0]+
                            sideInfomation.granule0.part2_3_length[1] + sideInfomation.granule1.part2_3_length[1]);
            mainDatas = new byte[(int)Math.Ceiling((float)mainDataBit/8)];

            byte[] data1 = new byte[sideInfomation.main_data_begin > (int)Math.Ceiling((float)mainDataBit / 8) ? (int)Math.Ceiling((float)mainDataBit / 8) : sideInfomation.main_data_begin];
            byte[] data2;
            if ((int) Math.Ceiling((float) mainDataBit / 8) - sideInfomation.main_data_begin >= 0)
            { 
                data2 = new byte[(int)Math.Ceiling((float)mainDataBit / 8) - sideInfomation.main_data_begin];
            }
            //数据都在前一帧
            else
            {
                data2 = new byte[0];
            }

            input.Seek(-(sideInfomation.main_data_begin+frameHeaderInfo.frame_length), SeekOrigin.Current);
            input.Read(data1, 0,data1.Length);
            if (frameHeaderInfo.track_mode == 11)
            {
                if (frameHeaderInfo.if_crc == 1)
                {
                    input.Seek(21, SeekOrigin.Current);
                }
                else
                {
                    input.Seek(23, SeekOrigin.Current);
                }
            }
            else
            {
                if (frameHeaderInfo.if_crc == 1)
                {
                    input.Seek(36, SeekOrigin.Current);
                }
                else
                {
                    input.Seek(38, SeekOrigin.Current);
                }
            }

            input.Read(data2, 0, data2.Length);

            //还原位置
            if (frameHeaderInfo.track_mode == 11)
            {
                if (frameHeaderInfo.if_crc == 1)
                {
                    input.Seek(frameHeaderInfo.frame_length-21- ((int)Math.Ceiling((float)mainDataBit / 8) - sideInfomation.main_data_begin), SeekOrigin.Current);
                }
                else
                {
                    input.Seek(frameHeaderInfo.frame_length - 23- ((int)Math.Ceiling((float)mainDataBit / 8) - sideInfomation.main_data_begin), SeekOrigin.Current);
                }
            }
            else
            {
                if (frameHeaderInfo.if_crc == 1)
                {
                    input.Seek(frameHeaderInfo.frame_length - 36- ((int)Math.Ceiling((float)mainDataBit / 8) - sideInfomation.main_data_begin), SeekOrigin.Current);
                }
                else
                {
                    input.Seek(frameHeaderInfo.frame_length - 38- ((int)Math.Ceiling((float)mainDataBit / 8) - sideInfomation.main_data_begin), SeekOrigin.Current);
                }
            }

            //复制主数据
            for(int i = 0;i< data1.Length;i++)
            {
                mainDatas[i] = data1[i];
            }

            for (int j = 0; j < data2.Length; j++)
            {
                mainDatas[data1.Length + j] = data2[j];
            }

            ProcessControl process = new ProcessControl(frameHeaderInfo,sideInfomation,mainDatas,filename,_write);
            //进入解码流程
            process.Start();
            Console.WriteLine(input.Position);
        }

    }
}
