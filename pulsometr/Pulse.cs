using AForge.Imaging.Filters;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using AForge.Imaging;

using AForge.Math;
using System.Drawing;
using Windows.Graphics.Imaging;
using System.IO;
using System.Collections.Generic;
using Windows.Storage.Streams;
using System.Threading.Tasks;

namespace pulsometr
{
    class Pulse
    {
        System.Drawing.Bitmap image;
        public List<double> RedMeanList= new List<double>();
        List<Windows.UI.Xaml.Media.Imaging.WriteableBitmap> MapList = new List<Windows.UI.Xaml.Media.Imaging.WriteableBitmap>();


        public void start(IRandomAccessStream steramImage)
        {

            Task task = new Task(async () => await Transform(steramImage));
            task.RunSynchronously();

        }

        public async Task<int> Transform(IRandomAccessStream steramImage)
        {
            
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(steramImage);
            PixelDataProvider pixelProvider = await decoder.GetPixelDataAsync();
            byte[] pixels = pixelProvider.DetachPixelData();
            Windows.UI.Xaml.Media.Imaging.WriteableBitmap source = new Windows.UI.Xaml.Media.Imaging.WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
             using (Stream stream = source.PixelBuffer.AsStream())
            {
                await stream.WriteAsync(pixels, 0,pixels.Length);
            }
            source.Invalidate();
            this.image = (System.Drawing.Bitmap)source;
            RedMeanList.Add(getRedStatistic(image));
            return 1;

            //this.image = (System.Drawing.Bitmap)source;
           

            
            
        }
        private double getRedStatistic( System.Drawing.Bitmap image){
            ImageStatistics statistic = new ImageStatistics(image);
            
            return statistic.Red.Mean;

           // this.show.Source = (WriteableBitmap)(rotatedImage);

        }
        public void ApllyforAllImages(List<IRandomAccessStream> lista,ref List<double> RedMeanList)
        {

            foreach (IRandomAccessStream stream in lista)
            {
                start(stream);

                RedMeanList =this.RedMeanList;
            }
        }
    }
}
