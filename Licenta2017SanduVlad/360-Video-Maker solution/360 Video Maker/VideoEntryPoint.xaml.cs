using Entities.Enums;
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
    /// Interaction logic for VideoEntryPoint.xaml
    /// </summary>
    public partial class VideoEntryPoint : UserControl
    {

        public VideoType VideoEntryType { get; set; }

        public delegate void VideoEntryPointChanged(object sender, TimeSpan value);
        public event VideoEntryPointChanged ValueChanged;

        public TimeSpan timeSpan = TimeSpan.FromSeconds(0);

        public VideoEntryPoint(string videoName)
        {
            InitializeComponent();
            this.TimeLapseEntryPoint.Text = TimeSpan.FromSeconds(0).ToString();
            this.VideoName.Content = videoName;
        }

        private void TimeLapseEntryPoint_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                timeSpan = TimeSpan.Parse(TimeLapseEntryPoint.Text);
                ValueChanged?.Invoke(this, this.timeSpan);
            }
            catch { }
        }
    }
}
