using System;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using MP3_analysis_player.decoder;
using System.IO;
using System.Windows.Forms;
using MP3_analysis_player.decoder.format_definition;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_Stop(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_Play(object sender, RoutedEventArgs e)
        {

        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }

        private void Open_file_Click(object sender, RoutedEventArgs e)
        {
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
            Decoder decoder = new Decoder(input,a[a.Length-1]);
            if (!decoder.Start())
            {
                //清空文件名文本框显示
                Filename.Text = null;
                MessageBox.Show("文件格式错误", "提示", MessageBoxButton.OK);
            }
            else
            {
                //设置文件名文本框显示
                Filename.Text = a[a.Length - 1];

                MessageBox.Show("解码成功,可以播放了", "提示", MessageBoxButton.OK);
            };

        }

        private void Close_file_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
