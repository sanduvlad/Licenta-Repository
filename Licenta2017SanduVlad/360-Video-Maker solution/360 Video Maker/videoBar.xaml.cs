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
using System.Drawing;

namespace _360_Video_Maker
{
    /// <summary>
    /// Interaction logic for VideoBar.xaml
    /// </summary>
    public partial class VideoBar : UserControl
    {

        public System.Drawing.Color Color { get; set; }

        public VideoBar()
        {
            InitializeComponent();
            rectangle.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(Color.R, Color.G, Color.B));
        }

        public VideoBar SetColor(System.Drawing.Color c)
        {
            this.Color = c;
            this.rectangle.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(Color.R, Color.G, Color.B));
            this.InvalidateVisual();
            return this;
        }
    }
}
