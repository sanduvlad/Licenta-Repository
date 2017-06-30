using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace _360_Video_Maker
{
    /// <summary>
    /// Interaction logic for VideoFileUserControl.xaml
    /// </summary>
    /// 
    public partial class VideoFileUserControl : UserControl
    {
        public string FilePath { get; private set; }
        public string FileName { get; private set; }

        public bool isSelected { get; private set; }

        private string duration = "Duration: ";
        private string framesSecond = "FPS: ";

        //private VideoInformation.VideoInformationRetrieval vir = new VideoInformation.VideoInformationRetrieval();

        public delegate void Onclick(object e);
        public event Onclick VideoClicked;


        public VideoFileUserControl(string path)
        {
            InitializeComponent();
            SetPath(path);
            SetVideoInformation(path);
        }

        public void ResetBackground()
        {
            this.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            isSelected = false;
        }


        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VideoClicked?.Invoke(this);
            this.Background = new SolidColorBrush(Color.FromRgb(25,25,100));
            isSelected = true;
        }

        internal void SetPath(string p)
        {
            FilePath = p;
            FileName = System.IO.Path.GetFileName(p);
            VideoName.Content = FileName;            
        }

        public void SelectThis()
        {
            this.Background = new SolidColorBrush(Color.FromRgb(25, 25, 100));
            isSelected = true;
        }

        public void SetVideoInformation(string p)
        {
            VideoInformation vi = new VideoInformationRetrieval.VideoInformationRetrieval().GetVideoInformation(FilePath);

            durationLabel.Content = duration + 
                (vi.duration.Minutes < 10 ? "0" + vi.duration.Minutes : vi.duration.Minutes.ToString()) 
                + ":" + (vi.duration.Seconds < 10 ? "0" 
                + vi.duration.Seconds : vi.duration.Seconds.ToString());
            framesPerSecond.Content = framesSecond + vi.frameRate;
        }
    }
}
