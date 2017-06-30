using _360_Video_Maker.ResourceLogic;
using Entities.Enums;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
using System.Windows.Shapes;

namespace _360_Video_Maker
{
    /// <summary>
    /// Interaction logic for CubeMapEditor1.xaml
    /// </summary>
    public partial class CubeMapEditor1 : Window
    {

        private ResourceManager _resourceManager = ResourceManager.GetInstance();

        public CubeMapEditor1()
        {
            InitializeComponent();
        }

        private void OpenVideoFile(VideoType type)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Video files (*.avi, *.mp4) | *.avi; *.mp4";

            if (ofd.ShowDialog() == true)
            {
                _resourceManager.AddVideoFile(ofd.FileName, type);
                _resourceManager.AddVideoEntryPoint(type, TimeSpan.FromSeconds(0));
            }
        }

        private BitmapImage GetBitmapImageFrom(Bitmap b)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memory = new MemoryStream())
            {
                b.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        private void TopFileBrowser_Click(object sender, RoutedEventArgs e)
        {
            OpenVideoFile(VideoType.Top);
            TopVideoPreview.Source = GetBitmapImageFrom(_resourceManager.GetPreviewBitmap(VideoType.Top));
        }

        private void LeftFileBrowser_Click(object sender, RoutedEventArgs e)
        {
            OpenVideoFile(VideoType.Left);
            LeftVideoPreview.Source = GetBitmapImageFrom(_resourceManager.GetPreviewBitmap(VideoType.Left));
        }

        private void RightFileBrowser_Click(object sender, RoutedEventArgs e)
        {
            OpenVideoFile(VideoType.Right);
            RightVideoPreview.Source = GetBitmapImageFrom(_resourceManager.GetPreviewBitmap(VideoType.Right));
        }

        private void FrontFileBrowser_Click(object sender, RoutedEventArgs e)
        {
            OpenVideoFile(VideoType.Front);
            FrontVideoPreview.Source = GetBitmapImageFrom(_resourceManager.GetPreviewBitmap(VideoType.Front));
        }

        private void BackFileBrowser_Click(object sender, RoutedEventArgs e)
        {
            OpenVideoFile(VideoType.Back);
            BackVideoPreview.Source = GetBitmapImageFrom(_resourceManager.GetPreviewBitmap(VideoType.Back));
        }

        private void DownFileBrowser_Click(object sender, RoutedEventArgs e)
        {
            OpenVideoFile(VideoType.Bottom);
            BottomVideoPreview.Source = GetBitmapImageFrom(_resourceManager.GetPreviewBitmap(VideoType.Bottom));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
