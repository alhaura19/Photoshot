using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Forms;

using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;
using MetroFramework.Forms;

namespace PhotoShot
{
    public partial class Photoshot : MetroForm
    {
        static Bitmap camera = null;
        
        public ObservableCollection<FilterInfo> CameraDevice; // kamera yang dapat digunakan
        
        public VideoCaptureDevice VideoFrame; // untuk hasil capture

        public Photoshot()
        {
            InitializeComponent();
            
        }

        private void Photoshot_Load(object sender, EventArgs e)
        {
            //CameraDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice); // constructor
            CameraDevice = new ObservableCollection<FilterInfo>();
            foreach (FilterInfo filterInfo in new FilterInfoCollection(FilterCategory.VideoInputDevice))
            {
                CameraDevice.Add(filterInfo);
            }

            VideoFrame = new VideoCaptureDevice(CameraDevice[0].MonikerString);
            VideoFrame.NewFrame += new NewFrameEventHandler(video_NewFrame);

            ThreadStart f = new ThreadStart(VideoFrame.Start);
            Thread thread = new Thread(f);
            thread.Start();
            
        }

        private void video_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            //jalankan frame
            camera = (Bitmap)eventArgs.Frame.Clone();

            videoPlayer.Image = camera;
            Thread.Sleep(10);

            Sepia sp = new Sepia();
            videoPlayer.Image = sp.Apply(camera);
            Thread.Sleep(10);

            Invert i = new Invert();
            videoPlayer.Image = i.Apply(camera);
            Thread.Sleep(10);

            Grayscale g = new Grayscale(0.2125, 0.7154, 0.0721);
            videoPlayer.Image = g.Apply(camera);
            Thread.Sleep(10);

            
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            displayMiniImg.Image = (Bitmap)videoPlayer.Image.Clone();
            
            Bitmap current = (Bitmap)videoPlayer.Image.Clone();
            string filepath = @"D:\Files\SEMESTER 7\Real Time Pro\archive\PhotoShot\Output\image1.jpg";
            //string filepath = Environment.CurrentDirectory;
            int i = 0;
            while (File.Exists(filepath))
            {
                i++;
                filepath = @"D:\Files\SEMESTER 7\Real Time Pro\archive\PhotoShot\Output\image" + i +".jpg";
            }
            string fileName = System.IO.Path.Combine(filepath);
            current.Save(fileName);
            current.Dispose();
        }

        private void Photoshot_FormClosing(object sender, FormClosingEventArgs e)
        {
            VideoFrame.Stop();
        }
        

    }

}
