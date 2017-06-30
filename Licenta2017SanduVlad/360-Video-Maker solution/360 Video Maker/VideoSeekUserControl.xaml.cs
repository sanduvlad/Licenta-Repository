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
    /// Interaction logic for VideoSeekUserControl.xaml
    /// </summary>
    public partial class VideoSeekUserControl : UserControl
    {
        public VideoSeekUserControl()
        {
            InitializeComponent();
        }

        private double widthPercentage = 0;

        

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //this.seekerTop.Margin = new Thickness(
            //    //seekerTop.Margin.Left,
            //    e.GetPosition(seekerTop).X,
            //    seekerTop.Margin.Top,
            //    seekerTop.Margin.Right,
            //    seekerTop.Margin.Bottom
            //    );
        }

        public delegate void SeekderChangedFunction(object sender, double value);
        public event SeekderChangedFunction SeekerChangedValue;

        private void seekerTop_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                double mousePoint = e.GetPosition(this.referenceGrid).X;
                if (mousePoint < 0)
                {
                    mousePoint = 0 - 5;
                }

                if(mousePoint > this.referenceGrid.ActualWidth)
                {
                    mousePoint = referenceGrid.ActualWidth;
                }

                this.seekerGrid.Margin = new Thickness(
                    //seekerTop.Margin.Left,
                    mousePoint- 5,
                    seekerGrid.Margin.Top,
                    seekerGrid.Margin.Right,
                    seekerGrid.Margin.Bottom
                    );
                widthPercentage = (mousePoint * 100.0) / this.referenceGrid.ActualWidth;
            }
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SeekerChangedValue?.Invoke(this, widthPercentage);
        }

        private void seekerTop_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SeekerChangedValue?.Invoke(this, widthPercentage);
        }

        private void seekerTop_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //this.seekerTop.Margin = new Thickness(
            //    //seekerTop.Margin.Left,
            //    e.GetPosition(seekerTop).X,
            //    seekerTop.Margin.Top,
            //    seekerTop.Margin.Right,
            //    seekerTop.Margin.Bottom
            //    );
        }

        private void referenceGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.seekerGrid.Margin = new Thickness(

                widthPercentage * this.referenceGrid.ActualWidth / 100,
                    seekerGrid.Margin.Top,
                    seekerGrid.Margin.Right,
                    seekerGrid.Margin.Bottom

                );
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                double mousePoint = e.GetPosition(this.referenceGrid).X;
                if (mousePoint < 0)
                {
                    mousePoint = 0 - 5;
                }

                if (mousePoint > this.referenceGrid.ActualWidth)
                {
                    mousePoint = referenceGrid.ActualWidth;
                }

                this.seekerGrid.Margin = new Thickness(
                    //seekerTop.Margin.Left,
                    mousePoint - 5,
                    seekerGrid.Margin.Top,
                    seekerGrid.Margin.Right,
                    seekerGrid.Margin.Bottom
                    );
                widthPercentage = (mousePoint * 100.0) / this.referenceGrid.ActualWidth;
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                double mousePoint = e.GetPosition(this.referenceGrid).X;
                if (mousePoint < 0)
                {
                    mousePoint = 0 - 5;
                }

                if (mousePoint > this.referenceGrid.ActualWidth)
                {
                    mousePoint = referenceGrid.ActualWidth;
                }

                this.seekerGrid.Margin = new Thickness(
                    //seekerTop.Margin.Left,
                    mousePoint - 5,
                    seekerGrid.Margin.Top,
                    seekerGrid.Margin.Right,
                    seekerGrid.Margin.Bottom
                    );
                widthPercentage = (mousePoint * 100.0) / this.referenceGrid.ActualWidth;
            }
        }

        public void AddNewVideoBar(VideoBar vidB, System.Drawing.Color c)
        {
            videoBarsGrid.Children.Add(vidB.SetColor(c));
        }

        
    }
}
