using Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoInformationRetrieval;

namespace VideoInformationRetrieval
{
    public class VideoInformationRetrieval
    {

        public VideoInformation GetVideoInformation (string filePath)
        {
            VideoFileReader VFR = new VideoFileReader();
            VFR.Open(filePath);

            VideoInformation vi = new VideoInformation() {
                frameRate = VFR.FrameRate,
                frames = VFR.FrameCount,
                duration = TimeSpan.FromSeconds((double)VFR.FrameCount / VFR.FrameRate)
            };

            VFR.Close();
            VFR.Dispose();

            return vi;
        }

        public Bitmap GetVideoFrame(string filePath, int second, TimeSpan timeSpan)
        {
            VideoFileReader VFR = new VideoFileReader();
            try
            {
                VFR.Open(filePath);
            }
            catch(Exception e)
            {
                VFR.Close();
                VFR.Dispose();
            }

            var ret = VFR.GetFrameAtIndex(second, timeSpan);
            VFR.Close();
            VFR.Dispose();
            System.GC.Collect();
            return ret;
        }
    }
}