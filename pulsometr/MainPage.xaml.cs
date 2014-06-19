using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.MediaProperties;
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
        public MainPage()
        {
            this.InitializeComponent();
            btnStartDevice_Click();
            btnStartPreview_Click();
        }
        public void start()
        {
            btnStartDevice_Click();
            btnStartPreview_Click();

        }
        internal async void btnStartDevice_Click()
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
                }
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception);
            }
        }

        internal async void btnStartPreview_Click()
        {
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
            }
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
            rootPage.Text=ex.Source;
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

                while (stopwatch.Elapsed.Seconds < 20)
                {


                    IRandomAccessStream mediaStream = new InMemoryRandomAccessStream();


                    await m_mediaCaptureMgr.CapturePhotoToStreamAsync(imageProperties, mediaStream);

                    lista.Add(mediaStream);




                    // Get the elapsed time as a TimeSpan value.
                }




               

                //var photoStream = await m_photoStorageFile.OpenAsync(Windows.Storage.FileAccessMode.Read);

                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(lista[20]);
                PixelDataProvider pixelProvider = await decoder.GetPixelDataAsync();
                byte[] pixels = pixelProvider.DetachPixelData();
                BitmapDecoder decoder1 = await BitmapDecoder.CreateAsync(lista[1]);
                PixelDataProvider pixelProvider1 = await decoder1.GetPixelDataAsync();
                byte[] pixels1 = pixelProvider1.DetachPixelData();
                //działa
                ShowStatusMessage("File open successful");
                var bmpimg = new BitmapImage();
                var bmpimg1 = new BitmapImage();

                //bmpimg.SetSource(mediaStream);
                //bmpimg.SetSource(mediaStream);
                //                imageElement1.Source = bmpimg;
                ShowStatusMessage("");

            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception);
                btnTakePhoto.IsEnabled = true;
            }
        }
    }
}
