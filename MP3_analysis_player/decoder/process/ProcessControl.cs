using System;
using System.Collections.Generic;
using System.IO;
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

        private GetBit getBit = new GetBit();
        //缩放因子
        private ScaleFac[] scalefac = new[] {new ScaleFac(), new ScaleFac()};

        //缩放因子起始处
        private int part2_start;

        //哈夫曼解码变量
        internal int[] v = { 0 };
        internal int[] w = { 0 };
        internal int[] x = { 0 };
        internal int[] y = { 0 };
        private readonly int sfreq;
        private readonly SBI[] sfBandIndex;
        //声道数
        private int nch = 0;
        //哈夫曼结果
        private int[] Haffman_res = new int[576];
        //零值填充起始处记录
        private int[] nonzero = new []{576,576};

        //反量化变量
        private const int SSLIMIT = 18;
        private const int SBLIMIT = 32;
        public static readonly int[] pretab = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 3, 3, 3, 2, 0 };
        public static readonly float[] t_43;
        public static readonly float[] two_to_negative_half_pow =
        {
            1.0000000000e+00f, 7.0710678119e-01f, 5.0000000000e-01f, 3.5355339059e-01f, 2.5000000000e-01f,
            1.7677669530e-01f, 1.2500000000e-01f, 8.8388347648e-02f, 6.2500000000e-02f, 4.4194173824e-02f,
            3.1250000000e-02f, 2.2097086912e-02f, 1.5625000000e-02f, 1.1048543456e-02f, 7.8125000000e-03f,
            5.5242717280e-03f, 3.9062500000e-03f, 2.7621358640e-03f, 1.9531250000e-03f, 1.3810679320e-03f,
            9.7656250000e-04f, 6.9053396600e-04f, 4.8828125000e-04f, 3.4526698300e-04f, 2.4414062500e-04f,
            1.7263349150e-04f, 1.2207031250e-04f, 8.6316745750e-05f, 6.1035156250e-05f, 4.3158372875e-05f,
            3.0517578125e-05f, 2.1579186438e-05f, 1.5258789062e-05f, 1.0789593219e-05f, 7.6293945312e-06f,
            5.3947966094e-06f, 3.8146972656e-06f, 2.6973983047e-06f, 1.9073486328e-06f, 1.3486991523e-06f,
            9.5367431641e-07f, 6.7434957617e-07f, 4.7683715820e-07f, 3.3717478809e-07f, 2.3841857910e-07f,
            1.6858739404e-07f, 1.1920928955e-07f, 8.4293697022e-08f, 5.9604644775e-08f, 4.2146848511e-08f,
            2.9802322388e-08f, 2.1073424255e-08f, 1.4901161194e-08f, 1.0536712128e-08f, 7.4505805969e-09f,
            5.2683560639e-09f, 3.7252902985e-09f, 2.6341780319e-09f, 1.8626451492e-09f, 1.3170890160e-09f,
            9.3132257462e-10f, 6.5854450798e-10f, 4.6566128731e-10f, 3.2927225399e-10f
        };
        //逆量化结果
        private readonly float[][][] dequantize_res;


        //立体声处理变量
        private readonly float[][] k;
        public static readonly float[][] io =
        {
            new[]
            {
                1.0000000000e+00f, 8.4089641526e-01f, 7.0710678119e-01f, 5.9460355751e-01f, 5.0000000001e-01f,
                4.2044820763e-01f, 3.5355339060e-01f, 2.9730177876e-01f, 2.5000000001e-01f, 2.1022410382e-01f,
                1.7677669530e-01f, 1.4865088938e-01f, 1.2500000000e-01f, 1.0511205191e-01f, 8.8388347652e-02f,
                7.4325444691e-02f, 6.2500000003e-02f, 5.2556025956e-02f, 4.4194173826e-02f, 3.7162722346e-02f,
                3.1250000002e-02f, 2.6278012978e-02f, 2.2097086913e-02f, 1.8581361173e-02f, 1.5625000001e-02f,
                1.3139006489e-02f, 1.1048543457e-02f, 9.2906805866e-03f, 7.8125000006e-03f, 6.5695032447e-03f,
                5.5242717285e-03f, 4.6453402934e-03f
            },
            new[]
            {
                1.0000000000e+00f, 7.0710678119e-01f, 5.0000000000e-01f, 3.5355339060e-01f, 2.5000000000e-01f,
                1.7677669530e-01f, 1.2500000000e-01f, 8.8388347650e-02f, 6.2500000001e-02f, 4.4194173825e-02f,
                3.1250000001e-02f, 2.2097086913e-02f, 1.5625000000e-02f, 1.1048543456e-02f, 7.8125000002e-03f,
                5.5242717282e-03f, 3.9062500001e-03f, 2.7621358641e-03f, 1.9531250001e-03f, 1.3810679321e-03f,
                9.7656250004e-04f, 6.9053396603e-04f, 4.8828125002e-04f, 3.4526698302e-04f, 2.4414062501e-04f,
                1.7263349151e-04f, 1.2207031251e-04f, 8.6316745755e-05f, 6.1035156254e-05f, 4.3158372878e-05f,
                3.0517578127e-05f, 2.1579186439e-05f
            }
        };
        //立体声处理结果
        private readonly float[][][] stereo_res;
        private int[] is_pos = new int[576];
        private float[] is_ratio = new float[576];
        public static readonly float[] TAN12 =
        {
            0.0f, 0.26794919f, 0.57735027f, 1.0f, 1.73205081f, 3.73205081f, 9.9999999e10f, -3.73205081f, -1.73205081f,
            -1.0f, -0.57735027f, -0.26794919f, 0.0f, 0.26794919f, 0.57735027f, 1.0f
        };

        //重排序变量
        private static int[][] reorder_table;
        private readonly float[] out_1d;

        /// <summary>
        /// 静态初始化函数
        /// </summary>
        static ProcessControl(){
            t_43 = create_t_43();
        }

        public ProcessControl(Data_Frame_Header_Info dataFrameHeaderInfo,Side_Infomation sideInfomation,byte[] mainDatas,string filename)
        {
            _dataFrameHeaderInfo = dataFrameHeaderInfo;
            _sideInfomation = sideInfomation;
            _mainDatas = mainDatas;
            _filename = filename;

            foreach (var b in _mainDatas)
            {
                getBit.hputbuf(b);
            }

            Huffman.Initialize();
            sfBandIndex = new SBI[9];
            int[] l0 =
            {
                0, 6, 12, 18, 24, 30, 36, 44, 54, 66, 80, 96, 116, 140, 168, 200, 238, 284, 336, 396, 464, 522,
                576
            };
            int[] s0 = { 0, 4, 8, 12, 18, 24, 32, 42, 56, 74, 100, 132, 174, 192 };
            int[] l1 =
            {
                0, 6, 12, 18, 24, 30, 36, 44, 54, 66, 80, 96, 114, 136, 162, 194, 232, 278, 330, 394, 464, 540,
                576
            };
            int[] s1 = { 0, 4, 8, 12, 18, 26, 36, 48, 62, 80, 104, 136, 180, 192 };
            int[] l2 =
            {
                0, 6, 12, 18, 24, 30, 36, 44, 54, 66, 80, 96, 116, 140, 168, 200, 238, 284, 336, 396, 464, 522,
                576
            };
            int[] s2 = { 0, 4, 8, 12, 18, 26, 36, 48, 62, 80, 104, 134, 174, 192 };

            int[] l3 =
            {
                0, 4, 8, 12, 16, 20, 24, 30, 36, 44, 52, 62, 74, 90, 110, 134, 162, 196, 238, 288, 342, 418, 576
            };
            int[] s3 = { 0, 4, 8, 12, 16, 22, 30, 40, 52, 66, 84, 106, 136, 192 };
            int[] l4 =
            {
                0, 4, 8, 12, 16, 20, 24, 30, 36, 42, 50, 60, 72, 88, 106, 128, 156, 190, 230, 276, 330, 384, 576
            };
            int[] s4 = { 0, 4, 8, 12, 16, 22, 28, 38, 50, 64, 80, 100, 126, 192 };
            int[] l5 =
            {
                0, 4, 8, 12, 16, 20, 24, 30, 36, 44, 54, 66, 82, 102, 126, 156, 194, 240, 296, 364, 448, 550,
                576
            };
            int[] s5 = { 0, 4, 8, 12, 16, 22, 30, 42, 58, 78, 104, 138, 180, 192 };

            int[] l6 =
            {
                0, 6, 12, 18, 24, 30, 36, 44, 54, 66, 80, 96, 116, 140, 168, 200, 238, 284, 336, 396, 464, 522,
                576
            };
            int[] s6 = { 0, 4, 8, 12, 18, 26, 36, 48, 62, 80, 104, 134, 174, 192 };
            int[] l7 =
            {
                0, 6, 12, 18, 24, 30, 36, 44, 54, 66, 80, 96, 116, 140, 168, 200, 238, 284, 336, 396, 464, 522,
                576
            };
            int[] s7 = { 0, 4, 8, 12, 18, 26, 36, 48, 62, 80, 104, 134, 174, 192 };
            int[] l8 =
            {
                0, 12, 24, 36, 48, 60, 72, 88, 108, 132, 160, 192, 232, 280, 336, 400, 476, 566, 568, 570, 572,
                574, 576
            };
            int[] s8 = { 0, 8, 16, 24, 36, 52, 72, 96, 124, 160, 162, 164, 166, 192 };

            sfBandIndex[0] = new SBI(l0, s0);
            sfBandIndex[1] = new SBI(l1, s1);
            sfBandIndex[2] = new SBI(l2, s2);

            sfBandIndex[3] = new SBI(l3, s3);
            sfBandIndex[4] = new SBI(l4, s4);
            sfBandIndex[5] = new SBI(l5, s5);

            sfBandIndex[6] = new SBI(l6, s6);
            sfBandIndex[7] = new SBI(l7, s7);
            sfBandIndex[8] = new SBI(l8, s8);

            sfreq = _dataFrameHeaderInfo.sampling_frequency +
                    ((_dataFrameHeaderInfo.version == 1) ? 3 : 0);

            dequantize_res = new float[2][][];
            for (int i = 0; i < 2; i++)
            {
                dequantize_res[i] = new float[SBLIMIT][];
                for (int i2 = 0; i2 < SBLIMIT; i2++)
                {
                    dequantize_res[i][i2] = new float[SSLIMIT];
                }
            }

            k = new float[2][];
            for (int i6 = 0; i6 < 2; i6++)
            {
                k[i6] = new float[SBLIMIT * SSLIMIT];
            }

            stereo_res = new float[2][][];
            for (int i3 = 0; i3 < 2; i3++)
            {
                stereo_res[i3] = new float[SBLIMIT][];
                for (int i4 = 0; i4 < SBLIMIT; i4++)
                {
                    stereo_res[i3][i4] = new float[SSLIMIT];
                }
            }
            if (reorder_table == null)
            {
                reorder_table = new int[9][];
                for (int i = 0; i < 9; i++)
                    reorder_table[i] = Reorder(sfBandIndex[i].s);
            }

            out_1d = new float[SBLIMIT * SSLIMIT];
        }

        private static float[] create_t_43()
        {
            float[] t43 = new float[8192];
            double d43 = (4.0 / 3.0);

            for (int i = 0; i < 8192; i++)
            {
                t43[i] = (float)Math.Pow(i, d43);
            }
            return t43;
        }

    /// <summary>
    /// 开始解码并输出为文件
    /// </summary>
    public void Start()
        {
            //单声道
            if(_dataFrameHeaderInfo.track_mode == 3)
            {
                nch = 1;
            }
            //双声道
            else
            {
                nch = 2;
            }

            //主要控制结构
            for (int gr = 0; gr < 2; gr++)
            {
                for (int ch = 0; ch < nch; ch++)
                {
                    part2_start = getBit.hsstell();

                    ReadScaleFactors(ch, gr);

                    //哈夫曼解码
                    HuffmanDecode(ch, gr);

                    //逆量化
                    dequantize_sample(dequantize_res[ch], ch, gr);
                }

                //立体声处理
                stereo(gr);

                // if ((which_channels == OutputChannels.DOWNMIX_CHANNELS) && (nch > 1))
                //     doDownMix();
                for (int ch = 0; ch < nch; ch++)
                {
                    //重排序
                    Reorder(stereo_res[ch], ch, gr);
                    /*
                    Antialias(ch, gr);
                    //for (int hb = 0;hb<576;hb++) CheckSumOut1d = CheckSumOut1d + out_1d[hb];
                    //System.out.println("CheckSumOut1d = "+CheckSumOut1d);

                    Hybrid(ch, gr);

                    //for (int hb = 0;hb<576;hb++) CheckSumOut1d = CheckSumOut1d + out_1d[hb];
                    //System.out.println("CheckSumOut1d = "+CheckSumOut1d);

                    for (sb18 = 18; sb18 < 576; sb18 += 36)
                        // Frequency inversion
                        for (ss = 1; ss < SSLIMIT; ss += 2)
                            out_1d[sb18 + ss] = -out_1d[sb18 + ss];

                    if ((ch == 0) || (which_channels == OutputChannels.RIGHT_CHANNEL))
                    {
                        for (ss = 0; ss < SSLIMIT; ss++)
                        {
                            // Polyphase synthesis
                            sb = 0;
                            for (sb18 = 0; sb18 < 576; sb18 += 18)
                            {
                                samples1[sb] = out_1d[sb18 + ss];
                                //filter1.input_sample(out_1d[sb18+ss], sb);
                                sb++;
                            }
                            //buffer.appendSamples(0, samples1);
                            //Console.WriteLine("Adding samples right into output buffer");
                            filter1.WriteAllSamples(samples1);
                            filter1.calculate_pcm_samples(buffer);
                        }
                    }
                    else
                    {
                        for (ss = 0; ss < SSLIMIT; ss++)
                        {
                            // Polyphase synthesis
                            sb = 0;
                            for (sb18 = 0; sb18 < 576; sb18 += 18)
                            {
                                samples2[sb] = out_1d[sb18 + ss];
                                //filter2.input_sample(out_1d[sb18+ss], sb);
                                sb++;
                            }
                            //buffer.appendSamples(1, samples2);
                            //Console.WriteLine("Adding samples right into output buffer");
                            filter2.WriteAllSamples(samples2);
                            filter2.calculate_pcm_samples(buffer);
                        }
                    }*/
                }
                // channels
            }
        }

        /// <summary>
        /// 获取比例因子
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="gr"></param>
        private void ReadScaleFactors(int ch, int gr)
        {
            int sfb, window;
            Granule gr_info;

            if (gr == 0)
            {
                gr_info = _sideInfomation.granule0;
            }
            else
            {
                gr_info = _sideInfomation.granule1;
            }
            int scale_comp = gr_info.scale_fac_compress[ch];
            int length0 = gr_info.slen1[ch];
            int length1 = gr_info.slen2[ch];

            if ((gr_info.window_switching_flag[ch] != 0) && (gr_info.block_type[ch] == 2))
            {
                if ((gr_info.mixed_block_flag[ch]) != 0)
                {
                    //混合块
                    for (sfb = 0; sfb < 8; sfb++)
                        scalefac[ch].l[sfb] = getBit.ReadBits(length0);
                    for (sfb = 3; sfb < 6; sfb++)
                        for (window = 0; window < 3; window++)
                            scalefac[ch].s[window][sfb] = getBit.ReadBits(length0);
                    for (sfb = 6; sfb < 12; sfb++)
                        for (window = 0; window < 3; window++)
                            scalefac[ch].s[window][sfb] = getBit.ReadBits(length1);
                    for (sfb = 12, window = 0; window < 3; window++)
                        scalefac[ch].s[window][sfb] = 0;
                }
                else
                {
                    //短块

                    scalefac[ch].s[0][0] = getBit.ReadBits(length0);
                    scalefac[ch].s[1][0] = getBit.ReadBits(length0);
                    scalefac[ch].s[2][0] = getBit.ReadBits(length0);
                    scalefac[ch].s[0][1] = getBit.ReadBits(length0);
                    scalefac[ch].s[1][1] = getBit.ReadBits(length0);
                    scalefac[ch].s[2][1] = getBit.ReadBits(length0);
                    scalefac[ch].s[0][2] = getBit.ReadBits(length0);
                    scalefac[ch].s[1][2] = getBit.ReadBits(length0);
                    scalefac[ch].s[2][2] = getBit.ReadBits(length0);
                    scalefac[ch].s[0][3] = getBit.ReadBits(length0);
                    scalefac[ch].s[1][3] = getBit.ReadBits(length0);
                    scalefac[ch].s[2][3] = getBit.ReadBits(length0);
                    scalefac[ch].s[0][4] = getBit.ReadBits(length0);
                    scalefac[ch].s[1][4] = getBit.ReadBits(length0);
                    scalefac[ch].s[2][4] = getBit.ReadBits(length0);
                    scalefac[ch].s[0][5] = getBit.ReadBits(length0);
                    scalefac[ch].s[1][5] = getBit.ReadBits(length0);
                    scalefac[ch].s[2][5] = getBit.ReadBits(length0);
                    scalefac[ch].s[0][6] = getBit.ReadBits(length1);
                    scalefac[ch].s[1][6] = getBit.ReadBits(length1);
                    scalefac[ch].s[2][6] = getBit.ReadBits(length1);
                    scalefac[ch].s[0][7] = getBit.ReadBits(length1);
                    scalefac[ch].s[1][7] = getBit.ReadBits(length1);
                    scalefac[ch].s[2][7] = getBit.ReadBits(length1);
                    scalefac[ch].s[0][8] = getBit.ReadBits(length1);
                    scalefac[ch].s[1][8] = getBit.ReadBits(length1);
                    scalefac[ch].s[2][8] = getBit.ReadBits(length1);
                    scalefac[ch].s[0][9] = getBit.ReadBits(length1);
                    scalefac[ch].s[1][9] = getBit.ReadBits(length1);
                    scalefac[ch].s[2][9] = getBit.ReadBits(length1);
                    scalefac[ch].s[0][10] = getBit.ReadBits(length1);
                    scalefac[ch].s[1][10] = getBit.ReadBits(length1);
                    scalefac[ch].s[2][10] = getBit.ReadBits(length1);
                    scalefac[ch].s[0][11] = getBit.ReadBits(length1);
                    scalefac[ch].s[1][11] = getBit.ReadBits(length1);
                    scalefac[ch].s[2][11] = getBit.ReadBits(length1);
                    scalefac[ch].s[0][12] = 0;
                    scalefac[ch].s[1][12] = 0;
                    scalefac[ch].s[2][12] = 0;
                }
            }
            else
            {
                // 长块 0,1,3

                if ((_sideInfomation.scfsi[ch][0] == 0) || (gr == 0))
                {
                    scalefac[ch].l[0] = getBit.ReadBits(length0);
                    scalefac[ch].l[1] = getBit.ReadBits(length0);
                    scalefac[ch].l[2] = getBit.ReadBits(length0);
                    scalefac[ch].l[3] = getBit.ReadBits(length0);
                    scalefac[ch].l[4] = getBit.ReadBits(length0);
                    scalefac[ch].l[5] = getBit.ReadBits(length0);
                }
                if ((_sideInfomation.scfsi[ch][1] == 0) || (gr == 0))
                {
                    scalefac[ch].l[6] = getBit.ReadBits(length0);
                    scalefac[ch].l[7] = getBit.ReadBits(length0);
                    scalefac[ch].l[8] = getBit.ReadBits(length0);
                    scalefac[ch].l[9] = getBit.ReadBits(length0);
                    scalefac[ch].l[10] = getBit.ReadBits(length0);
                }
                if ((_sideInfomation.scfsi[ch][2] == 0) || (gr == 0))
                {
                    scalefac[ch].l[11] = getBit.ReadBits(length1);
                    scalefac[ch].l[12] = getBit.ReadBits(length1);
                    scalefac[ch].l[13] = getBit.ReadBits(length1);
                    scalefac[ch].l[14] = getBit.ReadBits(length1);
                    scalefac[ch].l[15] = getBit.ReadBits(length1);
                }
                if ((_sideInfomation.scfsi[ch][3] == 0) || (gr == 0))
                {
                    scalefac[ch].l[16] = getBit.ReadBits(length1);
                    scalefac[ch].l[17] = getBit.ReadBits(length1);
                    scalefac[ch].l[18] = getBit.ReadBits(length1);
                    scalefac[ch].l[19] = getBit.ReadBits(length1);
                    scalefac[ch].l[20] = getBit.ReadBits(length1);
                }

                scalefac[ch].l[21] = 0;
                scalefac[ch].l[22] = 0;
            }
        }

        /// <summary>
        /// 哈夫曼解码
        /// </summary>
        /// <param name="ch">声道</param>
        /// <param name="gr">粒度</param>
        private void HuffmanDecode(int ch, int gr)
        {
            x[0] = 0;
            y[0] = 0;
            v[0] = 0;
            w[0] = 0;

            Granule gr_info;

            if (gr == 0)
            {
                gr_info = _sideInfomation.granule0;
            }
            else
            {
                gr_info = _sideInfomation.granule1;
            }

            int part2_3_end = (int)(part2_start +gr_info.part2_3_length[ch]);
            int num_bits;
            int region1Start;
            int region2Start;
            int index;

            int buf, buf1;

            Huffman h;

            if (((gr_info.window_switching_flag[ch]) != 0) && (gr_info.block_type[ch] == 2))
            {
                region1Start = (sfreq == 8) ? 72 : 36; 
                region2Start = 576; 
            }
            else
            {
                buf = gr_info.region0_count[ch] + 1;
                buf1 = buf + gr_info.region1_count[ch] + 1;

                if (buf1 > sfBandIndex[sfreq].l.Length - 1)
                    buf1 = sfBandIndex[sfreq].l.Length - 1;

                region1Start = sfBandIndex[sfreq].l[buf];
                region2Start = sfBandIndex[sfreq].l[buf1];
            }

            index = 0;
            // 读大值区
            for (int i = 0; i < (gr_info.big_values[ch] << 1); i += 2)
            {
                if (i < region1Start)
                    h = Huffman.ht[gr_info.table_select[ch][0]];
                else if (i < region2Start)
                    h = Huffman.ht[gr_info.table_select[ch][1]];
                else
                    h = Huffman.ht[gr_info.table_select[ch][2]];

                Huffman.Decode(h, x, y, v, w, getBit);

                Haffman_res[index++] = x[0];
                Haffman_res[index++] = y[0];
            }

            // 读count1区
            h = Huffman.ht[gr_info.count1table_select[ch] + 32];
            num_bits = getBit.hsstell();

            while ((num_bits < part2_3_end) && (index < 576))
            {
                Huffman.Decode(h, x, y, v, w, getBit);

                Haffman_res[index++] = v[0];
                Haffman_res[index++] = w[0];
                Haffman_res[index++] = x[0];
                Haffman_res[index++] = y[0];
                num_bits = getBit.hsstell();
            }

            if (num_bits > part2_3_end)
            {
                getBit.RewindStreamBits(num_bits - part2_3_end);
                index -= 4;
            }

            num_bits =getBit.hsstell();

            // Dismiss stuffing bits
            if (num_bits < part2_3_end)
                getBit.ReadBits(part2_3_end - num_bits);

            // Zero out rest

            if (index < 576)
                nonzero[ch] = index;
            else
                nonzero[ch] = 576;

            if (index < 0)
                index = 0;

            //填充0区
            for (; index < 576; index++)
                Haffman_res[index] = 0;
        }

        /// <summary>
        /// 逆量化
        /// </summary>
        /// <param name="xr"></param>
        /// <param name="ch"></param>
        /// <param name="gr"></param>
        private void dequantize_sample(float[][] xr, int ch, int gr)
        {

            Granule gr_info;
            if (gr == 0)
            {
                gr_info = _sideInfomation.granule0;
            }
            else
            {
                gr_info = _sideInfomation.granule1;
            }

            int cb = 0;
            int next_cb_boundary;
            int cb_begin = 0;
            int cb_width = 0;
            int index = 0, t_index, j;
            float g_gain;
            float[][] res = xr;

            // 确定当前缩放因子带，初始化边界

            if ((gr_info.window_switching_flag[ch] != 0) && (gr_info.block_type[ch] == 2))
            {
                if (gr_info.mixed_block_flag[ch] != 0)
                    next_cb_boundary = sfBandIndex[sfreq].l[1];
                // 长块: 0,1,3
                else
                {
                    cb_width = sfBandIndex[sfreq].s[1];
                    next_cb_boundary = (cb_width << 2) - cb_width;
                    cb_begin = 0;
                }
            }
            else
            {
                next_cb_boundary = sfBandIndex[sfreq].l[1]; // 长块: 0,1,3
            }

            // 计算全局缩放因子

            g_gain = (float)Math.Pow(2.0, (0.25 * (gr_info.global_gain[ch] - 210.0)));

            for (j = 0; j < nonzero[ch]; j++)
            {
                int reste = j % SSLIMIT;
                int quotien = (j - reste) / SSLIMIT;
                if (Haffman_res[j] == 0)
                    res[quotien][reste] = 0.0f;
                else
                {
                    int abv = Haffman_res[j];
                    if (Haffman_res[j] > 0)
                        res[quotien][reste] = g_gain * t_43[abv];
                    else
                        res[quotien][reste] = -g_gain * t_43[-abv];
                }
            }

            // 通过块类型，求逆量化公式

            for (j = 0; j < nonzero[ch]; j++)
            {
                int reste = j % SSLIMIT;
                int quotien = (j - reste) / SSLIMIT;

                if (index == next_cb_boundary)
                {
                    if ((gr_info.window_switching_flag[ch] != 0) && (gr_info.block_type[ch] == 2))
                    {
                        if (gr_info.mixed_block_flag[ch] != 0)
                        {
                            if (index == sfBandIndex[sfreq].l[8])
                            {
                                next_cb_boundary = sfBandIndex[sfreq].s[4];
                                next_cb_boundary = (next_cb_boundary << 2) - next_cb_boundary;
                                cb = 3;
                                cb_width = sfBandIndex[sfreq].s[4] - sfBandIndex[sfreq].s[3];

                                cb_begin = sfBandIndex[sfreq].s[3];
                                cb_begin = (cb_begin << 2) - cb_begin;
                            }
                            else if (index < sfBandIndex[sfreq].l[8])
                            {
                                next_cb_boundary = sfBandIndex[sfreq].l[(++cb) + 1];
                            }
                            else
                            {
                                next_cb_boundary = sfBandIndex[sfreq].s[(++cb) + 1];
                                next_cb_boundary = (next_cb_boundary << 2) - next_cb_boundary;

                                cb_begin = sfBandIndex[sfreq].s[cb];
                                cb_width = sfBandIndex[sfreq].s[cb + 1] - cb_begin;
                                cb_begin = (cb_begin << 2) - cb_begin;
                            }
                        }
                        else
                        {
                            next_cb_boundary = sfBandIndex[sfreq].s[(++cb) + 1];
                            next_cb_boundary = (next_cb_boundary << 2) - next_cb_boundary;

                            cb_begin = sfBandIndex[sfreq].s[cb];
                            cb_width = sfBandIndex[sfreq].s[cb + 1] - cb_begin;
                            cb_begin = (cb_begin << 2) - cb_begin;
                        }
                    }
                    else
                    {
                        //长块 blocks

                        next_cb_boundary = sfBandIndex[sfreq].l[(++cb) + 1];
                    }
                }

                if ((gr_info.window_switching_flag[ch] != 0) &&
                    (((gr_info.block_type[ch] == 2) && (gr_info.mixed_block_flag[ch] == 0)) ||
                     ((gr_info.block_type[ch] == 2) && (gr_info.mixed_block_flag[ch] != 0) && (j >= 36))))
                {
                    t_index = (index - cb_begin) / cb_width;
                    /*xr[sb][ss] *= pow(2.0, ((-2.0 * gr_info.subblock_gain[t_index])
					-(0.5 * (1.0 + gr_info.scalefac_scale)
					* scalefac[ch].s[t_index][cb]))); */
                    int idx = scalefac[ch].s[t_index][cb] << gr_info.scalefac_scale[ch];
                    idx += (int)(gr_info.subblock_gain[ch][t_index] << 2);

                    res[quotien][reste] *= two_to_negative_half_pow[idx];
                }
                else
                {
                    /*xr[sb][ss] *= pow(2.0, -0.5 * (1.0+gr_info.scalefac_scale)
					* (scalefac[ch].l[cb]
					+ gr_info.preflag * pretab[cb])); */
                    int idx = scalefac[ch].l[cb];

                    if (gr_info.preflag[ch] != 0)
                        idx += pretab[cb];

                    idx = idx << gr_info.scalefac_scale[ch];
                    res[quotien][reste] *= two_to_negative_half_pow[idx];
                }
                index++;
            }

            for (j = nonzero[ch]; j < 576; j++)
            {
                int reste = j % SSLIMIT;
                int quotien = (j - reste) / SSLIMIT;
                if (reste < 0)
                    reste = 0;
                if (quotien < 0)
                    quotien = 0;
                res[quotien][reste] = 0.0f;
            }
        }


        private void i_stereo_k_values(int is_pos, int io_type, int i)
        {
            if (is_pos == 0)
            {
                k[0][i] = 1.0f;
                k[1][i] = 1.0f;
            }
            else if ((is_pos & 1) != 0)
            {
                k[0][i] = io[io_type][Shift.URShift((is_pos + 1), 1)];
                k[1][i] = 1.0f;
            }
            else
            {
                k[0][i] = 1.0f;
                k[1][i] = io[io_type][Shift.URShift(is_pos, 1)];
            }
        }
        /// <summary>
        /// 立体声处理
        /// </summary>
        /// <param name="gr">粒度</param>
        private void stereo(int gr)
        {
            int sb, ss;

            //单声道
            if (nch == 1)
            {
                for (sb = 0; sb < SBLIMIT; sb++)
                    for (ss = 0; ss < SSLIMIT; ss += 3)
                    {
                        stereo_res[0][sb][ss] = dequantize_res[0][sb][ss];
                        stereo_res[0][sb][ss + 1] = dequantize_res[0][sb][ss + 1];
                        stereo_res[0][sb][ss + 2] = dequantize_res[0][sb][ss + 2];
                    }
            }
            else
            {
                Granule gr_info;
                if (gr == 0)
                {
                    gr_info = _sideInfomation.granule0;
                }
                else
                {
                    gr_info = _sideInfomation.granule1;
                }

                int mode_ext = _dataFrameHeaderInfo.stereo_mode_ext;
                int sfb;
                int i;
                int lines, temp, temp2;

                bool ms_stereo = ((_dataFrameHeaderInfo.track_mode == 1) && ((mode_ext & 0x2) != 0));
                bool i_stereo = ((_dataFrameHeaderInfo.track_mode == 1) && ((mode_ext & 0x1) != 0));
                bool lsf = false;

                int io_type = (gr_info.scale_fac_compress[0] & 1);

                // 初始化

                for (i = 0; i < 576; i++)
                {
                    is_pos[i] = 7;

                    is_ratio[i] = 0.0f;
                }

                if (i_stereo)
                {
                    if ((gr_info.window_switching_flag[0] != 0) && (gr_info.block_type[0] == 2))
                    {
                        if (gr_info.mixed_block_flag[0] != 0)
                        {
                            int max_sfb = 0;

                            for (int j = 0; j < 3; j++)
                            {
                                int sfbcnt;
                                sfbcnt = 2;
                                for (sfb = 12; sfb >= 3; sfb--)
                                {
                                    i = sfBandIndex[sfreq].s[sfb];
                                    lines = sfBandIndex[sfreq].s[sfb + 1] - i;
                                    i = (i << 2) - i + (j + 1) * lines - 1;

                                    while (lines > 0)
                                    {
                                        if (dequantize_res[1][i / 18][i % 18] != 0.0f)
                                        {
                                            sfbcnt = sfb;
                                            sfb = -10;
                                            lines = -10;
                                        }

                                        lines--;
                                        i--;
                                    }
                                }

                                sfb = sfbcnt + 1;

                                if (sfb > max_sfb)
                                    max_sfb = sfb;

                                while (sfb < 12)
                                {
                                    temp = sfBandIndex[sfreq].s[sfb];
                                    sb = sfBandIndex[sfreq].s[sfb + 1] - temp;
                                    i = (temp << 2) - temp + j * sb;

                                    for (; sb > 0; sb--)
                                    {
                                        is_pos[i] = scalefac[1].s[j][sfb];
                                        if (is_pos[i] != 7)
                                            if (lsf)
                                                i_stereo_k_values(is_pos[i], io_type, i);
                                            else
                                                is_ratio[i] = TAN12[is_pos[i]];

                                        i++;
                                    }

                                    sfb++;
                                } 

                                sfb = sfBandIndex[sfreq].s[10];
                                sb = sfBandIndex[sfreq].s[11] - sfb;
                                sfb = (sfb << 2) - sfb + j * sb;
                                temp = sfBandIndex[sfreq].s[11];
                                sb = sfBandIndex[sfreq].s[12] - temp;
                                i = (temp << 2) - temp + j * sb;

                                for (; sb > 0; sb--)
                                {
                                    is_pos[i] = is_pos[sfb];

                                    if (lsf)
                                    {
                                        k[0][i] = k[0][sfb];
                                        k[1][i] = k[1][sfb];
                                    }
                                    else
                                    {
                                        is_ratio[i] = is_ratio[sfb];
                                    }

                                    i++;
                                }

                            }

                            if (max_sfb <= 3)
                            {
                                i = 2;
                                ss = 17;
                                sb = -1;
                                while (i >= 0)
                                {
                                    if (dequantize_res[1][i][ss] != 0.0f)
                                    {
                                        sb = (i << 4) + (i << 1) + ss;
                                        i = -1;
                                    }
                                    else
                                    {
                                        ss--;
                                        if (ss < 0)
                                        {
                                            i--;
                                            ss = 17;
                                        }
                                    }

                                }

                                i = 0;
                                while (sfBandIndex[sfreq].l[i] <= sb)
                                    i++;
                                sfb = i;
                                i = sfBandIndex[sfreq].l[i];
                                for (; sfb < 8; sfb++)
                                {
                                    sb = sfBandIndex[sfreq].l[sfb + 1] - sfBandIndex[sfreq].l[sfb];
                                    for (; sb > 0; sb--)
                                    {
                                        is_pos[i] = scalefac[1].l[sfb];
                                        if (is_pos[i] != 7)
                                            if (lsf)
                                                i_stereo_k_values(is_pos[i], io_type, i);
                                            else
                                                is_ratio[i] = TAN12[is_pos[i]];
                                        i++;
                                    }

                                }

                            }

                        }
                        else
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                int sfbcnt;
                                sfbcnt = -1;
                                for (sfb = 12; sfb >= 0; sfb--)
                                {
                                    temp = sfBandIndex[sfreq].s[sfb];
                                    lines = sfBandIndex[sfreq].s[sfb + 1] - temp;
                                    i = (temp << 2) - temp + (j + 1) * lines - 1;

                                    while (lines > 0)
                                    {
                                        if (dequantize_res[1][i / 18][i % 18] != 0.0f)
                                        {
                                            sfbcnt = sfb;
                                            sfb = -10;
                                            lines = -10;
                                        }

                                        lines--;
                                        i--;
                                    } 
                                }

                                sfb = sfbcnt + 1;
                                while (sfb < 12)
                                {
                                    temp = sfBandIndex[sfreq].s[sfb];
                                    sb = sfBandIndex[sfreq].s[sfb + 1] - temp;
                                    i = (temp << 2) - temp + j * sb;
                                    for (; sb > 0; sb--)
                                    {
                                        is_pos[i] = scalefac[1].s[j][sfb];
                                        if (is_pos[i] != 7)
                                            if (lsf)
                                                i_stereo_k_values(is_pos[i], io_type, i);
                                            else
                                                is_ratio[i] = TAN12[is_pos[i]];
                                        i++;
                                    }

                                    sfb++;
                                }

                                temp = sfBandIndex[sfreq].s[10];
                                temp2 = sfBandIndex[sfreq].s[11];
                                sb = temp2 - temp;
                                sfb = (temp << 2) - temp + j * sb;
                                sb = sfBandIndex[sfreq].s[12] - temp2;
                                i = (temp2 << 2) - temp2 + j * sb;

                                for (; sb > 0; sb--)
                                {
                                    is_pos[i] = is_pos[sfb];

                                    if (lsf)
                                    {
                                        k[0][i] = k[0][sfb];
                                        k[1][i] = k[1][sfb];
                                    }
                                    else
                                    {
                                        is_ratio[i] = is_ratio[sfb];
                                    }

                                    i++;
                                }

                            }

                        }

                    }
                    else
                    {
                        i = 31;
                        ss = 17;
                        sb = 0;
                        while (i >= 0)
                        {
                            if (dequantize_res[1][i][ss] != 0.0f)
                            {
                                sb = (i << 4) + (i << 1) + ss;
                                i = -1;
                            }
                            else
                            {
                                ss--;
                                if (ss < 0)
                                {
                                    i--;
                                    ss = 17;
                                }
                            }
                        }

                        i = 0;
                        while (sfBandIndex[sfreq].l[i] <= sb)
                            i++;

                        sfb = i;
                        i = sfBandIndex[sfreq].l[i];
                        for (; sfb < 21; sfb++)
                        {
                            sb = sfBandIndex[sfreq].l[sfb + 1] - sfBandIndex[sfreq].l[sfb];
                            for (; sb > 0; sb--)
                            {
                                is_pos[i] = scalefac[1].l[sfb];
                                if (is_pos[i] != 7)
                                    if (lsf)
                                        i_stereo_k_values(is_pos[i], io_type, i);
                                    else
                                        is_ratio[i] = TAN12[is_pos[i]];
                                i++;
                            }
                        }

                        sfb = sfBandIndex[sfreq].l[20];
                        for (sb = 576 - sfBandIndex[sfreq].l[21]; (sb > 0) && (i < 576); sb--)
                        {
                            is_pos[i] = is_pos[sfb]; // error here : i >=576

                            if (lsf)
                            {
                                k[0][i] = k[0][sfb];
                                k[1][i] = k[1][sfb];
                            }
                            else
                            {
                                is_ratio[i] = is_ratio[sfb];
                            }

                            i++;
                        }

                    }

                }

                i = 0;
                for (sb = 0; sb < SBLIMIT; sb++)
                for (ss = 0; ss < SSLIMIT; ss++)
                {
                    if (is_pos[i] == 7)
                    {
                        if (ms_stereo)
                        {
                            stereo_res[0][sb][ss] =
                                (dequantize_res[0][sb][ss] + dequantize_res[1][sb][ss]) * 0.707106781f;
                            stereo_res[1][sb][ss] =
                                (dequantize_res[0][sb][ss] - dequantize_res[1][sb][ss]) * 0.707106781f;
                        }
                        else
                        {
                            stereo_res[0][sb][ss] = dequantize_res[0][sb][ss];
                            stereo_res[1][sb][ss] = dequantize_res[1][sb][ss];
                        }
                    }
                    else if (i_stereo)
                    {
                        if (lsf)
                        {
                            stereo_res[0][sb][ss] = dequantize_res[0][sb][ss] * k[0][i];
                            stereo_res[1][sb][ss] = dequantize_res[0][sb][ss] * k[1][i];
                        }
                        else
                        {
                            stereo_res[1][sb][ss] = dequantize_res[0][sb][ss] / (1 + is_ratio[i]);
                            stereo_res[0][sb][ss] = stereo_res[1][sb][ss] * is_ratio[i];
                        }
                    }

                    i++;
                }
            }
        }

        /// <summary>
        /// 重排序
        /// </summary>
        /// <param name="xr"></param>
        /// <param name="ch"></param>
        /// <param name="gr"></param>
        private void Reorder(float[][] xr, int ch, int gr)
        {

            Granule gr_info;
            if (gr == 0)
            {
                gr_info = _sideInfomation.granule0;
            }
            else
            {
                gr_info = _sideInfomation.granule1;
            }

            int freq, freq3;
            int index;
            int sfb, sfb_start, sfb_lines;
            int src_line, des_line;
            float[][] xr_1d = xr;

            if ((gr_info.window_switching_flag[ch] != 0) && (gr_info.block_type[ch] == 2))
            {
                for (index = 0; index < 576; index++)
                    out_1d[index] = 0.0f;

                if (gr_info.mixed_block_flag[ch] != 0)
                {
                    // NO REORDER FOR LOW 2 SUBBANDS
                    for (index = 0; index < 36; index++)
                    {
                        // Modif E.B 02/22/99
                        int reste = index % SSLIMIT;
                        int quotien = (index - reste) / SSLIMIT;
                        out_1d[index] = xr_1d[quotien][reste];
                    }
                    // REORDERING FOR REST SWITCHED SHORT
                    for (sfb = 3, sfb_start = sfBandIndex[sfreq].s[3], sfb_lines = sfBandIndex[sfreq].s[4] - sfb_start;
                        sfb < 13;
                        sfb++, sfb_start = sfBandIndex[sfreq].s[sfb],
                            sfb_lines = sfBandIndex[sfreq].s[sfb + 1] - sfb_start)
                    {
                        int sfb_start3 = (sfb_start << 2) - sfb_start;

                        for (freq = 0, freq3 = 0; freq < sfb_lines; freq++, freq3 += 3)
                        {
                            src_line = sfb_start3 + freq;
                            des_line = sfb_start3 + freq3;
                            // Modif E.B 02/22/99
                            int reste = src_line % SSLIMIT;
                            int quotien = (src_line - reste) / SSLIMIT;

                            out_1d[des_line] = xr_1d[quotien][reste];
                            src_line += sfb_lines;
                            des_line++;

                            reste = src_line % SSLIMIT;
                            quotien = (src_line - reste) / SSLIMIT;

                            out_1d[des_line] = xr_1d[quotien][reste];
                            src_line += sfb_lines;
                            des_line++;

                            reste = src_line % SSLIMIT;
                            quotien = (src_line - reste) / SSLIMIT;

                            out_1d[des_line] = xr_1d[quotien][reste];
                        }
                    }
                }
                else
                {
                    // pure short
                    for (index = 0; index < 576; index++)
                    {
                        int j = reorder_table[sfreq][index];
                        int reste = j % SSLIMIT;
                        int quotien = (j - reste) / SSLIMIT;
                        out_1d[index] = xr_1d[quotien][reste];
                    }
                }
            }
            else
            {
                // long blocks
                for (index = 0; index < 576; index++)
                {
                    // Modif E.B 02/22/99
                    int reste = index % SSLIMIT;
                    int quotien = (index - reste) / SSLIMIT;
                    out_1d[index] = xr_1d[quotien][reste];
                }
            }
        }

        /// <summary>
        /// 重排序辅助函数
        /// </summary>
        /// <param name="scalefac_band"></param>
        /// <returns></returns>
        internal static int[] Reorder(int[] scalefac_band)
        {
            // SZD: converted from LAME
            int j = 0;
            int[] ix = new int[576];
            for (int sfb = 0; sfb < 13; sfb++)
            {
                int start = scalefac_band[sfb];
                int end = scalefac_band[sfb + 1];
                for (int window = 0; window < 3; window++)
                for (int i = start; i < end; i++)
                    ix[3 * i + window] = j++;
            }
            return ix;
        }

    }

}
