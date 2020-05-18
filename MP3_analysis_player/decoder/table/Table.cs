using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_analysis_player.decoder.table
{
    class Table
    {
        /// <summary>
        /// 采样频率
        /// 顺序1：v1 v2
        /// 顺序2：按头信息值
        /// </summary>
        public static readonly int[][] frequencies =
        {
            new[] {44100, 48000, 32000, 1},
            new[] { 22050, 24000, 16000, 1 }
        };

        /// <summary>
        /// 采样个数
        /// 顺序1：v1-v2
        /// 顺序2：l1-l3
        /// </summary>
        public static readonly int[][] sample_number =
        {
            new[] {384, 1152, 1152},
            new[] {384, 1152, 576 }
        };

        /// <summary>
        /// 数据帧比特率表
        /// 顺序1：v1-v2
        /// 顺序2：l1-l3
        /// </summary>
        public static readonly int[][][] bitrates =
        {
            new[]
            {
                new[]
                {
                    0, 32000, 64000, 96000, 128000, 160000, 192000, 224000, 256000, 288000, 320000, 352000, 384000,
                    416000,
                    448000, 0
                },
                new[]
                {
                    0, 32000, 48000, 56000, 64000, 80000, 96000, 112000, 128000, 160000, 192000, 224000, 256000, 320000,
                    384000, 0
                },
                new[]
                {
                    0, 32000, 40000, 48000, 56000, 64000, 80000, 96000, 112000, 128000, 160000, 192000, 224000, 256000,
                    320000, 0
                }
            },
            new[]
            {
                new[]
                {
                    0, 32000, 48000, 56000, 64000, 80000, 96000, 112000, 128000, 144000, 160000, 176000, 192000, 224000,
                    256000, 0
                },
                new[]
                {
                    0, 8000, 16000, 24000, 32000, 40000, 48000, 56000, 64000, 80000, 96000, 112000, 128000, 144000,
                    160000,
                    0
                },
                new[]
                {
                    0, 8000, 16000, 24000, 32000, 40000, 48000, 56000, 64000, 80000, 96000, 112000, 128000, 144000,
                    160000,
                    0
                }
            }
        };

        /// <summary>
        /// 数据帧比特率文字表
        /// 顺序1：v1-v1
        /// 顺序2：l1-l3
        /// </summary>
        public static readonly string[][][] bitrate_str =
        {
            new[]
            {
                new[]
                {
                    "free format", "32 kbit/s", "64 kbit/s", "96 kbit/s", "128 kbit/s", "160 kbit/s", "192 kbit/s",
                    "224 kbit/s", "256 kbit/s", "288 kbit/s", "320 kbit/s", "352 kbit/s", "384 kbit/s", "416 kbit/s",
                    "448 kbit/s", "forbidden"
                },
                new[]
                {
                    "free format", "32 kbit/s", "48 kbit/s", "56 kbit/s", "64 kbit/s", "80 kbit/s", "96 kbit/s",
                    "112 kbit/s", "128 kbit/s", "160 kbit/s", "192 kbit/s", "224 kbit/s", "256 kbit/s", "320 kbit/s",
                    "384 kbit/s", "forbidden"
                },
                new[]
                {
                    "free format", "32 kbit/s", "40 kbit/s", "48 kbit/s", "56 kbit/s", "64 kbit/s", "80 kbit/s",
                    "96 kbit/s", "112 kbit/s", "128 kbit/s", "160 kbit/s", "192 kbit/s", "224 kbit/s", "256 kbit/s",
                    "320 kbit/s", "forbidden"
                }
            },
            new[]
            {
                new[]
                {
                    "free format", "32 kbit/s", "48 kbit/s", "56 kbit/s", "64 kbit/s", "80 kbit/s", "96 kbit/s",
                    "112 kbit/s", "128 kbit/s", "144 kbit/s", "160 kbit/s", "176 kbit/s", "192 kbit/s", "224 kbit/s",
                    "256 kbit/s", "forbidden"
                },
                new[]
                {
                    "free format", "8 kbit/s", "16 kbit/s", "24 kbit/s", "32 kbit/s", "40 kbit/s", "48 kbit/s",
                    "56 kbit/s",
                    "64 kbit/s", "80 kbit/s", "96 kbit/s", "112 kbit/s", "128 kbit/s", "144 kbit/s", "160 kbit/s",
                    "forbidden"
                },
                new[]
                {
                    "free format", "8 kbit/s", "16 kbit/s", "24 kbit/s", "32 kbit/s", "40 kbit/s", "48 kbit/s",
                    "56 kbit/s",
                    "64 kbit/s", "80 kbit/s", "96 kbit/s", "112 kbit/s", "128 kbit/s", "144 kbit/s", "160 kbit/s",
                    "forbidden"
                }
            }
        };


    }
}
