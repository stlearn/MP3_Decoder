using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_analysis_player.decoder.format_definition
{
    /// <summary>
    /// 定义缩放因子
    /// </summary>
    class ScaleFac
    {// <summary>
            /// 存储长窗缩放因子
            /// </summary>
            public int[] l = new int[32];

            /// <summary>
            /// 存储短窗缩放因子
            /// </summary>
            public int[][] s = new int[3][]
            {
                new int[13],
                new int[13], 
                new int[13]
            };
    }
}
