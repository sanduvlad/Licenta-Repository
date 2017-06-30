using _360_Video_Maker.RenderingLogic;
using _360_Video_Maker.ResourceLogic;
using Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace _360_Video_Maker
{
    /// <summary>
    /// Interaction logic for EditorPage.xaml
    /// </summary>
    /// 
    public partial class EditorPage : Page
    {

        //UIFacade.UIFacade Facade = new UIFacade.UIFacade();
        //EventsManager eventManager;
        int CurrentSelectedVideo = -1;
        private bool videosAllreadyLoaded = false;

        private int previewWitdth = 800;
        private int previewHeight = 400;

        private int _seekerSecond = 0;

        object lockObject = new object();


        private List<System.Drawing.Color> colosVideosBar = new List<System.Drawing.Color>() { System.Drawing.Color.FromArgb(255, 121,121,146), System.Drawing.Color.FromArgb(255,127,114,92), System.Drawing.Color.FromArgb(255,130,58,59) };

        private ProjectSettings projSettingsWindow = new ProjectSettings();

        private ResourceManager _resouceManager = ResourceManager.GetInstance();

        public EditorPage()
        {
            InitializeComponent();

            //eventManager = new EventsManager(EquirectangularImageControl);
            this.videoSeekControl.SeekerChangedValue += VideoSeekControl_SeekerChangedValue;
            //eventManager.PreviewRenderComplete += UpdatePreviewRender;
        }

        private async void VideoSeekControl_SeekerChangedValue(object sender, double value)
        {
            if (videosAllreadyLoaded == true)
            {
                _seekerSecond = Convert.ToInt32(value * (ResourceManager.GetInstance().outputVideoDuration.Seconds / 100.0));
                var b= GetBitmapImageFrom( await StartPreviewRender());
                this.EquirectangularImageControl.Source = b;
            }
        }

        private void UpdatePreviewRender(object sender, EventArgs e)
        {
            //EquirectangularImageControl.Source = ((BitmapArg)e).GetProjection();
            //throw new NotImplementedException();
        }
        
        private void OpenVideoFiles_Event(object sender, RoutedEventArgs e)
        {
            //List<string> paths = Facade.ImportVideoFiles();


            //foreach (string p in paths)
            //{
            //    VideoFileUserControl vfuc = new VideoFileUserControl();
            //    vfuc.VideoClicked += Vfuc_VideoClicked;
            //    vfuc.SetPath(p);
            //    vfuc.SetVideoInformation(p);

            //    //VideoFilesList.Children.Add(vfuc);

            //    eventManager.NewVideoAdded();
            //}
        }

        private void UpdateUICoord(int videoIndex)
        {
            //Coordinates coord = eventManager.VideoChanged(videoIndex);
            //U_Coordinate_Control.SetCurrentValue(coord.U);
            //V_Coordinate_Control.SetCurrentValue(coord.V);
            //Hoz_Coordinate_Control.SetCurrentValue(coord.Hoz_FOV);
            //Vert_Coordinate_Control.SetCurrentValue(coord.Vert_FOV);
            //Rot_Coordinate_Control.SetCurrentValue(coord.Rotation);
        }

        private void ScrollViewer_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Delete)
            //{
            //    for (int i = 0; i < VideoFilesList.Children.Count; i++)
            //    {
            //        var control = VideoFilesList.Children[i] as VideoFileUserControl;
            //        if (control.isSelected == true)
            //        {
            //            Facade.DeleteFileFromLibrary(i);
            //            VideoFilesList.Children.RemoveAt(i);
            //            eventManager.VideoDeletedFromList(i);
            //            break;
            //        }
            //    }
            //}
        }

        public void GotFocus_Custom()
        {
            Page_GotFocus(null, null);
        }

        private async void Page_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_resouceManager.AllVideosAreLoaded() && videosAllreadyLoaded == false)
            {
                Random r = new Random();
                for (int i = 0; i < 6; i++)
                {
                    VideoFileUserControl videoFileUserControl = new VideoFileUserControl(_resouceManager.GetAllVideoPaths().Keys.ElementAt(i));
                    videoFileUserControl.VideoClicked += Vfuc_VideoClicked;
                    CubeMapListVideos.Children.Add(videoFileUserControl);

                    VideoEntryPoint vep = new VideoEntryPoint(System.IO.Path.GetFileName(_resouceManager.GetAllVideoPaths().Keys.ElementAt(i)));
                    vep.VideoEntryType = _resouceManager.GetAllVideoPaths().Values.ElementAt(i);
                    vep.ValueChanged += Vep_ValueChanged;
                    VideosEntryPointsList.Children.Add(vep);

                    this.videoSeekControl.AddNewVideoBar(new VideoBar(), colosVideosBar[i % 3]);
                }

                BitmapImage b = GetBitmapImageFrom(await StartPreviewRender());
                EquirectangularImageControl.Source = b;
                videosAllreadyLoaded = true;
                System.GC.Collect();
            }
        }

        private async Task<Bitmap> StartPreviewRender()
        {
            var taskRender = Task<Bitmap>.Run(() =>
            {
                object lockObj = new object(); lock (lockObj)
                {
                    return (new PreviewRenderLogic().GetProjection(ResourceManager.GetInstance().previewRenderWidth,
                        ResourceManager.GetInstance().previewRenderHeight,
                        _seekerSecond));
                }
            });
            return await taskRender;
        }

        private async void Vep_ValueChanged(object sender, TimeSpan value)
        {
            _resouceManager.AddVideoEntryPoint(((VideoEntryPoint)sender).VideoEntryType, value);
            BitmapImage b = GetBitmapImageFrom(await StartPreviewRender());
            EquirectangularImageControl.Source = b;
        }

        private void Vfuc_VideoClicked(object e)
        {
            for (int i = 0; i < CubeMapListVideos.Children.Count; i++)
            {
                if (!CubeMapListVideos.Children[i].Equals(e))
                {
                    ((VideoFileUserControl)CubeMapListVideos.Children[i]).ResetBackground();
                }
            }
        }
        
        /// https://stackoverflow.com/questions/6484357/converting-bitmapimage-to-bitmap-and-vice-versa
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

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.EquirectangularImageControl.Width = this.EquirectangularPreviewControl.ActualWidth;
            this.EquirectangularImageControl.Height = this.EquirectangularPreviewControl.ActualHeight;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            projSettingsWindow.ShowDialog();
        }
    }
}
