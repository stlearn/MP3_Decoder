using System.IO;
using System.Text;
using MP3_analysis_player.decoder.format_definition;

namespace MP3_analysis_player.decoder.header
{
    /// <summary>
    /// IDV3V2分析
    /// 完成MP3文件id3v2标签分析
    /// </summary>
    class Header
    {
        //输入文件流
        private Stream input;

        //id3v2类
        private ID3V2 id3V2 = new ID3V2();

        public Header(Stream _input)
        {
            input = _input;
        }

        private byte readByte()
        {
            byte[] b=new byte[1];
            input.Read(b,0,1);
            return b[0];
        }

        /// <summary>
        /// 返回输入文件流的ID3V2头部信息,没有此头部则返回null
        /// </summary>
        /// <returns>ID3V2</returns>
        public ID3V2 getID3V2()
        {
            //read header
            byte[] b = new byte[3];
            input.Read(b, 0, 3);
            id3V2.Header[0] = (char)b[0];
            id3V2.Header[1] = (char)b[1];
            id3V2.Header[2] = (char)b[2];

            //如果文件没有ID3头部，则返回NULL
            if (id3V2.Header[0] != 'I' || id3V2.Header[1] != 'D' || id3V2.Header[2] != '3')
            {
                return null;
            }

            //read version
            byte[] c = new byte[1];
            input.Read(c, 0, 1);
            id3V2.Ver = (char) c[0];

            //read revision
            b = new byte[1];
            input.Read(b, 0, 1);
            id3V2.Revision = (char) b[0];

            //read flag;
            b = new byte[1];
            input.Read(b, 0, 1);
            id3V2.Flag = (char)b[0];

            //read size
            b = new byte[4];
            input.Read(b, 0, 4);
            id3V2.Size[0] = (char) b[0];
            id3V2.Size[1] = (char) b[1];
            id3V2.Size[2] = (char) b[2];
            id3V2.Size[3] = (char) b[3];

            
            //读取标签帧
            while (input.Position < id3V2.getSize())
            {
                //初始化
                ID3V2.Label_Frame lf = new ID3V2.Label_Frame();
                lf.FrameID = new StringBuilder();
                lf.info = new StringBuilder();
                lf.Flags = new StringBuilder();

                //read frameid
                lf.FrameID.Append((char) readByte(), 1);
                lf.FrameID.Append((char) readByte(), 1);
                lf.FrameID.Append((char) readByte(), 1);
                lf.FrameID.Append((char) readByte(), 1);

                //read size
                char[] Size=new char[4];
                Size[0] = (char) readByte();
                Size[1] = (char) readByte();
                Size[2] = (char) readByte();
                Size[3] = (char) readByte();

                lf.Size = Size[0] * 0x1000000 + Size[1] * 0x10000 + Size[2] * 0x100 + Size[3];

                //read flags
                lf.Flags.Append((char) readByte(),1);
                lf.Flags.Append((char) readByte(),1);

                //read info
                for (int i = 0; i < lf.Size; i++)
                {
                    //一般帧内容的开始是一个‘\0’
                    if (i == 0)
                    {
                        char first = (char) readByte();
                        if (first != '\0')
                        {
                            lf.info.Append(first, 1);
                        }
                    }
                    else
                    {
                        lf.info.Append((char)readByte(), 1);
                    }
                }

                id3V2.Label_Frame_list.Add(lf);
            }

            return id3V2;
        }

    }
}
