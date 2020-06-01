using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using LiveCharts.Wpf;

namespace MP3_analysis_player.player
{
    class ChartViewThread
    {
        private readonly PcmDatas _pcmDatas;
        private readonly MainWindow _window;

        public ChartViewThread(PcmDatas pcmDatas,MainWindow window)
        {
            _pcmDatas = pcmDatas;
            _window = window;
        }

        public void start()
        {
            //创建线程
            Thread charThread = new Thread(threadProcess);
            //开启线程
            charThread.Start();
        }

        private void threadProcess()
        {
            IEnumerable<short> left = _pcmDatas.getLeft();
            IEnumerable<short> right = _pcmDatas.getRight();

            //抽样步长
            int len = (int)(left.LongCount() / 1000);
            //抽样样本
            IList<short> l = left.ToList();
            IList<short> r = right.ToList();

            ChartValues<short> ll = new ChartValues<short>();
            ChartValues<short> rr = new ChartValues<short>();

            for (int i = 0; i < 1000; i++)
            {
                ll.Add(l[i * len]);
                rr.Add(r[i * len]);
            }

            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    _window.pcm_chart_left.Series = new SeriesCollection
                    {
                        new LineSeries
                        {
                            Title = "左声道",
                            Values = ll
                        }
                    };
                    _window.pcm_chart_right.Series = new SeriesCollection
                    {
                        new LineSeries
                        {
                            Title = "右声道",
                            Values = rr
                        }
                    };
                }));
        }

    }
}
