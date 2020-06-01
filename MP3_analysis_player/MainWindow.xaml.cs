using System;
using System.Collections.Generic;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using MP3_analysis_player.decoder;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using MP3_analysis_player.decoder.format_definition;
using MP3_analysis_player.player;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using MessageBox = System.Windows.MessageBox;
using MessageBoxOptions = System.Windows.MessageBoxOptions;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace MP3_analysis_player
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //音频pcm数据
        private readonly PcmDatas _pcmDatas;
        //pcm player
        private PcmPlayer player;


        public MainWindow()
        {
            InitializeComponent();
            _pcmDatas = new PcmDatas();
        }

        private void Button_Click_Stop(object sender, RoutedEventArgs e)
        {
            //防止正在播放
            if (player != null)
            {
                player.stop();
            }
        }

        private void Button_Click_Play(object sender, RoutedEventArgs e)
        {
            //防止正在播放
            if (player != null)
            {
                player.stop();
            }
            player = new PcmPlayer(_pcmDatas,this);
            player.play();
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }

        private void Open_file_Click(object sender, RoutedEventArgs e)
        {
            //防止正在播放
            if (player != null)
            {
                player.stop();
            }
            
            //处理文件弹窗和文件名
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "音频文件|*.mp3";
            openFile.ShowDialog();
            char[] sp = new []{'\\'};
            String[] a= openFile.FileName.Split(sp,100);

            //定义输入流
            Stream input;

            //是否是未选择文件就关闭文件框的处理
            if (openFile.FileName.Length>0)
            {
                 input= openFile.OpenFile();
            }
            else
            {
                return;
            }

            //开始解码
            Decoder decoder = new Decoder(input,a[a.Length-1],this,_pcmDatas);
            if (!decoder.Start())
            {
                //清空文件名文本框显示
                Filename.Text = null;
                MessageBox.Show("文件格式错误", "提示", MessageBoxButton.OK);
                _pcmDatas.Clear();
            }
            else
            {
                //设置文件名文本框显示
                Filename.Text = a[a.Length - 1];

                MessageBox.Show("解码成功,可以播放了", "提示", MessageBoxButton.OK);

                IEnumerable<short> left = _pcmDatas.getLeft();
                IEnumerable<short> right = _pcmDatas.getRight();

                //抽样步长
                int len = (int) (left.LongCount() / 1000);
                //抽样样本
                IList<short> l = left.ToList();
                IList<short> r = right.ToList();

                ChartValues<short> ll = new ChartValues<short>();
                ChartValues<short> rr = new ChartValues<short>();

                for (int i = 0; i < 1000; i++)
                {
                    ll.Add(l[i*len]);
                    rr.Add(r[i*len]);
                }



                pcm_chart_left.Series = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "左声道",
                        Values = ll
                    }
                };
                pcm_chart_right.Series = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "右声道",
                        Values = rr
                    }
                };
            };

        }

        private void Close_file_Click(object sender, RoutedEventArgs e)
        {
            //防止正在播放
            if (player != null)
            {
                player.stop();
            }
            //清空pcm数据
            _pcmDatas.Clear();
            //清空显示框
            Filename.Text = null;
            Channels.Text = null;
            SampleMode.Text = null;
        }

        private void pcm_chart_left_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
