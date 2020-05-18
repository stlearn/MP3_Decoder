using System.Collections;
using System.Text;

namespace MP3_analysis_player.decoder.format_definition
{
    /// <summary>
    /// ID3V2头部的定义
    /// </summary>
    class ID3V2
    {
        //标签头
        public char[] Header = new char[3]; //必须为ID3,否则无标签
        public char Ver;    //如果是ID3V2.3 就是3
        public char Revision; //此版本为0
        public char Flag; /*标志字节一般为0，定义如下(abc000000B) a：表示是否使用Unsynchronisation b：表示是否有扩展头部，一般没有，所以一般也不设置 c：表示是否为测试标签，99.99%的标签都不是测试标签，不设置*/
        public char[] Size = new char[4];//标签大小共四个字节，每个字节只使用低7位，最高位不使用恒为0，计算时将最高位去掉，得到28bit的数据，计算公式如下：

        //标签帧
        public ArrayList Label_Frame_list = new ArrayList();

        /// <summary>
        ///         每一个标签帧格式
        /// </summary>
        public struct Label_Frame
        {
            public StringBuilder FrameID;   /*用四个字符标识一个帧,说明其内容,稍后有常用的标识对照表*/
            public int Size;    /*标签帧帧内容的大小,不包括帧头,不得小于 1*/
            public StringBuilder Flags;    /*存放标志,只定义了 6 位,稍后详细解说*/
            public StringBuilder info;
        }

        /// <summary>
        /// ID3V2整个头部大小
        /// </summary>
        /// <returns>size</returns>
        public int getSize()
        {
            int size = (Size[0] & 0x7F) * 0x200000 + (Size[1] & 0x7F) * 0x400 + (Size[2] & 0x7F) * 0x80 +
                       (Size[3] & 0x7F);
            return size;
        }

    }
}
