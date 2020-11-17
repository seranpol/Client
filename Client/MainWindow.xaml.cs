using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Execute1_Click(object sender, RoutedEventArgs e)
        {
            var sw = Stopwatch.StartNew();

            var t1 = DoWorkAsync(sw, Colors.Red, 0, 100);
            var t2 = DoWorkAsync(sw, Colors.Green, 1, 200);
            var t3 = DoWorkAsync(sw, Colors.Blue, 2, 300);

            await Task.WhenAll(t1, t2, t3);

            sw.Stop();

            Status.Text = $"Finished after { sw.ElapsedMilliseconds } ms";
        }



        public async Task DoWorkAsync(Stopwatch sw, Color color, int column, int delay)
        {
            for (int i = 0; i < 5; i++)
            {
                //await Task.Delay(0);
                //await Task.Delay(0).ConfigureAwait(false);
                //await Task.Delay(100);
                await Task.Delay(100).ConfigureAwait(false);

                var startTime = sw.ElapsedMilliseconds;

                while (sw.ElapsedMilliseconds < startTime + delay) { /* just hang around do to important computation !! */ }

                var endtime = sw.ElapsedMilliseconds;

                var nevermind = this.Dispatcher.BeginInvoke((Action)(() => DrawTimeRectangle(startTime, endtime, color, column)));
            }
        }

        public void DrawTimeRectangle(long startTime, long endTime, Color color, int column)
        {
            int scalingFactorHack = 7;
            startTime = startTime / scalingFactorHack;
            endTime = endTime / scalingFactorHack;

            int numberOfColumns = 3;
            var pxPrColumn = DrawArea.ActualWidth / numberOfColumns;

            var rectangle = new Rectangle
            {
                Width = pxPrColumn + 50,
                Height = endTime - startTime,
                Fill = new SolidColorBrush(color),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1,
            };

            this.DrawArea.Children.Add(rectangle);
            Canvas.SetTop(rectangle, startTime);
            Canvas.SetLeft(rectangle, pxPrColumn * column);

        }

        public static List<string> ctxs = new List<string>();

        private async void Execute2_Click(object sender, RoutedEventArgs e)
        {
            ctxs.Add(TaskScheduler.FromCurrentSynchronizationContext().GetType().FullName);
            ctxs.Add(TaskScheduler.Current?.GetType().FullName);

            Status.Text = await GetStringAsync();
        }

        private static async Task<string> GetStringAsync() => await MyExternalAsyncStringFetcher.GetString();
    }

    public static class MyExternalAsyncStringFetcher
    {
        public static async Task<string> GetString() => await Task.Run(() =>
        {
            MainWindow.ctxs.Add(TaskScheduler.Current?.GetType().FullName);
            return "foobar";
            });
    }
}
