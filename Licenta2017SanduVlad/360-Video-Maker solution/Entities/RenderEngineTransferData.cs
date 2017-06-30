using Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    public class RenderEngineTransferData
    {
        public Dictionary<string, VideoType> Videos { get; set; }
        public int OutputWidth { get; set; }
        public int OutputHeight { get; set; }
        public TimeSpan VideoDuration { get; set; }
        public Dictionary<VideoType, TimeSpan> VideosEntryPoints { get; set; }

        public string OutputFilePath { get; set; }
    }
}
