using AForge.Video.FFMPEG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace VideoInformation
{
    public class VideoInformationRetrieval
    {
        VideoFileReader vfr = new VideoFileReader();
        Accord.Video.IVideoSource videoSource = new Accord.Video.MJPEGStream(); 
        public List<string> GetBasicVideoInfo(string filePath)
        {
            videoSource.
            vfr.Open(filePath);
            var fps = vfr.FrameRate.ToString();
            var duration = (((double)vfr.FrameCount / (double)vfr.FrameRate) / 60) + ":" + (((double)vfr.FrameCount / (double)vfr.FrameRate) % 60);

            return new List<string> { "", ""};
        }
    }
}
