using Entities.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _360_Video_Maker.ResourceLogic
{
    public class ResourceManager
    {
        #region Singleton Logic
        static ResourceManager Instance;
        private ResourceManager()
        {

        }

        public static ResourceManager GetInstance()
        {
            if (Instance == null)
            {
                Instance = new ResourceManager();
            }

            return Instance;
        }
        #endregion

        #region Settings
        public int previewRenderWidth { get; set; } = 800;
        public int previewRenderHeight { get; set; } = 400;
        public int outputWidth { get; set; } = 1600;
        public int outputHeight { get; set; } = 800;
        public TimeSpan outputVideoDuration { get; set; } = TimeSpan.FromSeconds(30);
        #endregion;

        private Dictionary<string, VideoType> VideoPaths = new Dictionary<string, VideoType>();

        private Bitmap FrontVideo;
        private Bitmap BackVideo;
        private Bitmap LeftVideo;
        private Bitmap RightVideo;
        private Bitmap UpVideo;
        private Bitmap DownVideo;

        private Dictionary<VideoType, TimeSpan> VideosEntryPoints = new Dictionary<VideoType, TimeSpan>();


        public Bitmap GetFrame(int videoIndex, int videoFrameIndex)
        {
            return null;
        }

        public Bitmap GetPreviewBitmap(VideoType type)
        {
            Bitmap b = new Bitmap(100, 100);
            string path = VideoPaths.FirstOrDefault(v => v.Value == type).Key;
            if (false == string.IsNullOrEmpty(path))
            {
                b = new VideoInformationRetrieval.VideoInformationRetrieval().GetVideoFrame(path, 0, TimeSpan.FromSeconds(0));
            }
            return b;
        }

        public void DeleteVideoFromList(int videoIndex)
        {
            //VideoPaths.RemoveAt(videoIndex);
        }

        public Dictionary<string, VideoType> GetAllVideoPaths()
        {
            return VideoPaths;
        }

        public Dictionary<VideoType, TimeSpan> GetVideosEntryPoints()
        {
            return VideosEntryPoints;
        }

        public void AddVideoEntryPoint(VideoType vt, TimeSpan ts)
        {
            if (VideosEntryPoints.ContainsKey(vt) == false)
            {
                VideosEntryPoints.Add(vt, ts);
            }
            else
            {
                VideosEntryPoints[vt] = ts;
            }
        }

        public bool AllVideosAreLoaded()
        {
            return VideoPaths.Count == 6;
        }

        public void AddVideoFile(string path, VideoType type)
        {
            if (VideoPaths.Count(v => v.Value == type) == 1)
            {
                string existingPath = VideoPaths.Single(v => v.Value == type).Key;
                VideoPaths.Remove(existingPath);
                //VideoPaths.Remove(VideoPaths.Single(v => v.Value == type).Key);
                VideoPaths.Add(path, type);
            }
            else
            {
                VideoPaths.Add(path, type);
            }
        }

        public void AddVideoFiles(List<string> paths)
        {
            //VideoPaths.AddRange(paths);
        }
    }
}
