using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using MP3_analysis_player.decoder.process;
using NAudio.Wave;
using Application = System.Windows.Application;

namespace MP3_analysis_player.player
{
    class PcmPlayer
    {
        private readonly PcmDatas _pcmDatas;
        private readonly MainWindow _window;
        private WaveOut pcmOut;
        private WaveFormat waveFormat;
        private BufferedWaveProvider providerbuffer;

        private Thread t;
        //存放pcm数据
        private byte[] data;

        public PcmPlayer(PcmDatas pcmDatas,MainWindow window)
        {
            _pcmDatas = pcmDatas;
            _window = window;
            data = _pcmDatas.GetPcmData().ToArray();
            waveFormat = new WaveFormat(pcmDatas.Frequency, 16, pcmDatas.Channels);
            providerbuffer = new BufferedWaveProvider(waveFormat);
            pcmOut = new WaveOut();
        }

        public void play()
        {
            t = new Thread(playThread);
            t.Priority = ThreadPriority.Highest;
            t.Start();
        }

        public void stop()
        {
            if (t != null)
            {
                t.Abort();
                t = null;
            }
        }

        public void playThread()
        {
            byte[] tdata = data;
            //记录数据位置
            int count = 0;
            while (count < tdata.Length)
            {
                try
                {
                    BufferedWaveProvider provider = new BufferedWaveProvider(waveFormat);
                    if ((tdata.Length - count) < 88200)
                    {
                        provider.AddSamples(tdata, count, tdata.Length - count - 1);
                    }
                    else
                    {
                        provider.AddSamples(tdata, count, 882000);
                    }

                    pcmOut.Init(provider);
                    pcmOut.Play();
                    Thread.Sleep((int) provider.BufferDuration.TotalMilliseconds - 50);
                    count += 882000;
                    Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() =>
                            _window.ProgressBar.Value = (int) (100f * ((float) count / (float) tdata.Length))));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

            }
        }

    }
}
