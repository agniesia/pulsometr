using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace pulsometr
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Windows.Media.Capture.MediaCapture m_mediaCaptureMgr;
        private bool m_bPreviewing;
        public List<double> RedMeanList = new List<double>();
        public MainPage()
        {
            this.InitializeComponent();
            start();
            

        }
        void preview()
        {

          
            
            Task t = new Task(async () =>
            {
                var myCustomers = await btnStartPreview_Click();
            });
            t.RunSynchronously();
        }
        void start()
        {
            //var t=btnStartDevice_Click();
            Task task = new Task(async () => await btnStartDevice_Click());
            task.RunSynchronously();
            // if (result == 1) { }

            //btnStartPreview_Click();
        }


        async Task<int> btnStartDevice_Click()
        {
            try
            {

                ShowStatusMessage("Starting device");
                m_mediaCaptureMgr = new Windows.Media.Capture.MediaCapture();
                await m_mediaCaptureMgr.InitializeAsync();

                if (m_mediaCaptureMgr.MediaCaptureSettings.VideoDeviceId != "" && m_mediaCaptureMgr.MediaCaptureSettings.AudioDeviceId != "")
                {



                    btnTakePhoto.IsEnabled = true;

                    ShowStatusMessage("Device initialized successful");

                    // m_mediaCaptureMgr.RecordLimitationExceeded += new Windows.Media.Capture.RecordLimitationExceededEventHandler(RecordLimitationExceeded);
                    // m_mediaCaptureMgr.Failed += new Windows.Media.Capture.MediaCaptureFailedEventHandler(Failed);
                }


                else
                {

                    ShowStatusMessage("No VideoDevice/AudioDevice Found");
                    return 0;
                }
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception);
                return -1;

            }
            m_bPreviewing = false;
            try
            {
                ShowStatusMessage("Starting preview");
                previewElement.Source = m_mediaCaptureMgr;
                await m_mediaCaptureMgr.StartPreviewAsync();
                if ((m_mediaCaptureMgr.VideoDeviceController.Brightness != null) && m_mediaCaptureMgr.VideoDeviceController.Brightness.Capabilities.Supported)
                {
                    SetupVideoDeviceControl(m_mediaCaptureMgr.VideoDeviceController.Brightness, sldBrightness);
                }
                if ((m_mediaCaptureMgr.VideoDeviceController.Contrast != null) && m_mediaCaptureMgr.VideoDeviceController.Contrast.Capabilities.Supported)
                {
                    SetupVideoDeviceControl(m_mediaCaptureMgr.VideoDeviceController.Contrast, sldContrast);
                }
                m_bPreviewing = true;
                ShowStatusMessage("Start preview successful");

            }
            catch (Exception exception)
            {

                previewElement.Source = null;

                ShowExceptionMessage(exception);
                return -1;

            }
            return 1;
        }

        async Task<int> btnStartPreview_Click()
        {
            m_bPreviewing = false;
            try
            {
                ShowStatusMessage("Starting preview");


                m_mediaCaptureMgr.SetPreviewMirroring(true);
                previewElement.Source = m_mediaCaptureMgr;
                await m_mediaCaptureMgr.StartPreviewAsync();
                if ((m_mediaCaptureMgr.VideoDeviceController.Brightness != null) && m_mediaCaptureMgr.VideoDeviceController.Brightness.Capabilities.Supported)
                {
                    SetupVideoDeviceControl(m_mediaCaptureMgr.VideoDeviceController.Brightness, sldBrightness);
                }
                if ((m_mediaCaptureMgr.VideoDeviceController.Contrast != null) && m_mediaCaptureMgr.VideoDeviceController.Contrast.Capabilities.Supported)
                {
                    SetupVideoDeviceControl(m_mediaCaptureMgr.VideoDeviceController.Contrast, sldContrast);
                }
                m_bPreviewing = true;
                ShowStatusMessage("Start preview successful");

            }
            catch (Exception exception)
            {

                previewElement.Source = null;

                ShowExceptionMessage(exception);
                return -1;

            }
            return 1;
        }

        private void SetupVideoDeviceControl(Windows.Media.Devices.MediaDeviceControl videoDeviceControl, Slider slider)
        {
            try
            {
                if ((videoDeviceControl.Capabilities).Supported)
                {
                    slider.IsEnabled = true;
                    slider.Maximum = videoDeviceControl.Capabilities.Max;
                    slider.Minimum = videoDeviceControl.Capabilities.Min;
                    slider.StepFrequency = videoDeviceControl.Capabilities.Step;
                    double controlValue = 0;
                    if (videoDeviceControl.TryGetValue(out controlValue))
                    {
                        slider.Value = controlValue;
                    }
                }
                else
                {
                    slider.IsEnabled = false;
                }
            }
            catch (Exception e)
            {
                ShowExceptionMessage(e);
            }
        }

        // VideoDeviceControllers
        internal void sldBrightness_ValueChanged(Object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            try
            {
                bool succeeded = m_mediaCaptureMgr.VideoDeviceController.Brightness.TrySetValue(sldBrightness.Value);
                if (!succeeded)
                {
                    ShowStatusMessage("Set Brightness failed");
                }
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception);
            }
        }

        internal void sldContrast_ValueChanged(Object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            try
            {
                bool succeeded = m_mediaCaptureMgr.VideoDeviceController.Contrast.TrySetValue(sldContrast.Value);
                var t = RedMeanList.Count();

                if (!succeeded)
                {
                    ShowStatusMessage("Set Contrast failed");
                }
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception);
            }
        }

        private void ShowStatusMessage(String text)
        {
            rootPage.Text = text;
        }
        private void ShowExceptionMessage(Exception ex)
        {
           
               // rootPage.Text = ex.Source;
        }



        internal async void btnTakePhoto_Click(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

            try
            {
                ShowStatusMessage("Taking photo");
                btnTakePhoto.IsEnabled = false;
               
                ShowStatusMessage("Create photo file successful");
                ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();
                List<IRandomAccessStream> lista = new List<IRandomAccessStream>(500);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                 
	

                object Lock = new object();
                while (stopwatch.Elapsed.Seconds < 20)
                {
                 
                    
                    IRandomAccessStream mediaStream = new InMemoryRandomAccessStream();
                    await m_mediaCaptureMgr.CapturePhotoToStreamAsync(imageProperties, mediaStream);
                    lista.Add(mediaStream);
                    
                }
                

                
                ShowStatusMessage("convert");
                foreach (IRandomAccessStream streamImage in lista)
                {


                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(streamImage);
                    PixelDataProvider pixelProvider = await decoder.GetPixelDataAsync();
                    byte[] pixels = pixelProvider.DetachPixelData();
                   //Array suma=pixels.Where((x, i) => i % 4 == 2 ).ToArray();
                   //double[] tablica= new double[suma.Length];

                   //for (int i = 0; i < tablica.Length;i++ )
                   //{
                   //    var x=tablica[i] ;//= (double)suma.GetValue(i);
                   //}
                   // var t = (double)tablica.Sum() / (double)(decoder.PixelHeight * decoder.PixelWidth);
                   // RedMeanList.Add(t);

                    Windows.UI.Xaml.Media.Imaging.WriteableBitmap source = new Windows.UI.Xaml.Media.Imaging.WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                    using (Stream stream = source.PixelBuffer.AsStream())
                    {
                        await stream.WriteAsync(pixels, 0, pixels.Length);
                    }
                    AForge.Imaging.ImageStatistics statistic = new AForge.Imaging.ImageStatistics((System.Drawing.Bitmap)source);
                    RedMeanList.Add(statistic.Red.Mean);
                    //source.Invalidate();
                    //image.Source = source;
                    //await System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(500));
                }
                DataAnalisys dta = new DataAnalisys(RedMeanList);
                dta.DataManage();
               
                //Pulse pulse = new Pulse();
                //pulse.ApllyforAllImages(lista, ref RedMeanList);

                //pulse.ApllyforAllImages(lista);
                ShowStatusMessage( dta.PulseEND.ToString());
                


            }

            catch (FileNotFoundException exception)
            {
                ShowExceptionMessage(exception);
                btnTakePhoto.IsEnabled = true;
            }
        }

        private string stringList()
        {
            string text="";
            foreach (double liczba in RedMeanList)
            {
                text +=(Math.Round((255-liczba)*100)).ToString();
                text += ",";
            }
            text += "    " + RedMeanList.Count;
            return text;
        }
        private async void save_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder storageFolder = KnownFolders.MusicLibrary;

            StorageFile sampleFile = await storageFolder.CreateFileAsync("sampleX.txt");
            await Windows.Storage.FileIO.WriteTextAsync(sampleFile,stringList());
        }
        //static public string SerializeListToXml(List<double> List)
        //{
        //    try
        //    {
        //        System.Xml.Serialization.XmlSerializer xmlIzer = new System.Xml.Serialization.XmlSerializer(typeof(List<double>));
        //        var writer = new StringWriter();
        //        xmlIzer.Serialize(writer, List);
        //        System.Diagnostics.Debug.WriteLine(writer.ToString());
        //        return writer.ToString();
        //    }

        //    catch (Exception exc)
        //    {
        //        System.Diagnostics.Debug.WriteLine(exc);
        //        return String.Empty;
        //    }

        //}
    }
}
