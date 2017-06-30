using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace VideoInformationRetrieval
{
    public class VideoFileReader : AForge.Video.FFMPEG.VideoFileReader
    {
        public Bitmap GetFrameAtIndex(int second, TimeSpan ts)
        {
            int index = 0;
            TimeSpan videoDuration  = TimeSpan.FromSeconds((double)this.FrameCount / this.FrameRate);
            
            if (ts.Add(TimeSpan.FromSeconds(second)).Seconds > videoDuration.Seconds)
            {
                return CreateBlackBitmap(this.Width, this.Height);
            }

            if (ts.Add(TimeSpan.FromSeconds(second)).Seconds < 0)
            {
                return CreateBlackBitmap(this.Width, this.Height);
            }

            index = ts.Add(TimeSpan.FromSeconds(second)).Seconds * this.FrameRate;

            try
            {
                for (int i = 0; i < index; i++)
                {
                    base.ReadVideoFrame();
                    if (i % 50 == 0) System.GC.Collect();
                }
            }
            catch(Exception e)
            {

            }

            var ret = ReadVideoFrame();

            this.Dispose();

            return ret;
        }

        public Bitmap CreateBlackBitmap(int width, int height)
        {
            Bitmap b = new Bitmap(width, height);
            for(int i = 0;  i < width; i ++)
            {
                for (int j = 0; j < height; j ++)
                {
                    b.SetPixel(i, j, Color.Black);
                }
            }
            return b;
        }
    }
}
