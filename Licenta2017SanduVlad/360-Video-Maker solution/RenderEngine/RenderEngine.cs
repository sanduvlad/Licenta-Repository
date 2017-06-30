using Entities.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Video.FFMPEG;
using Entities;

namespace RenderEngine
{
    public class RenderEngine
    {

        private static RenderEngineTransferData UsefullData = null;
        private static Bitmap TempCubeMap = null;
        private static Bitmap TempEquirectangular = null;

        private static List<List<CorespondencePoint>> Equirectangular2CubeMap_Corespondence = null;

        VideoFileReader TopFileReader = new VideoFileReader();
        VideoFileReader BottomFileReader = new VideoFileReader();
        VideoFileReader FrontFileReader = new VideoFileReader();
        VideoFileReader BackFileReader = new VideoFileReader();
        VideoFileReader RightFileReader = new VideoFileReader();
        VideoFileReader LeftFileReader = new VideoFileReader();

        object lockObject = new object();

        VideoFileWriter FinalVideoOutputWriter = new VideoFileWriter();

        public RenderEngine(RenderEngineTransferData transferData)
        {
            UsefullData = transferData;
            
            TopFileReader.Open(UsefullData.Videos.Single(v => v.Value == VideoType.Top).Key);
            BottomFileReader.Open(UsefullData.Videos.Single(v => v.Value == VideoType.Bottom).Key);
            FrontFileReader.Open(UsefullData.Videos.Single(v => v.Value == VideoType.Front).Key);
            BackFileReader.Open(UsefullData.Videos.Single(v => v.Value == VideoType.Back).Key);
            RightFileReader.Open(UsefullData.Videos.Single(v => v.Value == VideoType.Right).Key);
            LeftFileReader.Open(UsefullData.Videos.Single(v => v.Value == VideoType.Left).Key);

            FinalVideoOutputWriter.Open(UsefullData.OutputFilePath, UsefullData.OutputWidth, UsefullData.OutputHeight);
        }

        public async Task StartProcessing()
        {
            InitializeMatrices();
            DelayVideos();
            await RenderThread();
        }

        private void DelayVideos()
        {
            if (UsefullData.VideosEntryPoints[VideoType.Top].Seconds > 0 
                && UsefullData.VideosEntryPoints[VideoType.Top].Seconds < TopFileReader.FrameCount / TopFileReader.FrameRate)
            {
                DelayVideo(TopFileReader, UsefullData.VideosEntryPoints[VideoType.Top].Seconds);
            }
            if (UsefullData.VideosEntryPoints[VideoType.Back].Seconds > 0
                && UsefullData.VideosEntryPoints[VideoType.Back].Seconds < BackFileReader.FrameCount / BackFileReader.FrameRate)
            {
                DelayVideo(BackFileReader, UsefullData.VideosEntryPoints[VideoType.Back].Seconds);
            }
            if (UsefullData.VideosEntryPoints[VideoType.Bottom].Seconds > 0
                && UsefullData.VideosEntryPoints[VideoType.Bottom].Seconds < BottomFileReader.FrameCount / BottomFileReader.FrameRate)
            {
                DelayVideo(BottomFileReader, UsefullData.VideosEntryPoints[VideoType.Bottom].Seconds);
            }
            if (UsefullData.VideosEntryPoints[VideoType.Left].Seconds > 0
                && UsefullData.VideosEntryPoints[VideoType.Left].Seconds < LeftFileReader.FrameCount / LeftFileReader.FrameRate)
            {
                DelayVideo(LeftFileReader, UsefullData.VideosEntryPoints[VideoType.Left].Seconds);
            }
            if (UsefullData.VideosEntryPoints[VideoType.Right].Seconds > 0
                && UsefullData.VideosEntryPoints[VideoType.Right].Seconds < RightFileReader.FrameCount / RightFileReader.FrameRate)
            {
                DelayVideo(RightFileReader, UsefullData.VideosEntryPoints[VideoType.Right].Seconds);
            }
            if (UsefullData.VideosEntryPoints[VideoType.Front].Seconds > 0
                && UsefullData.VideosEntryPoints[VideoType.Front].Seconds < FrontFileReader.FrameCount / FrontFileReader.FrameRate)
            {
                DelayVideo(FrontFileReader, UsefullData.VideosEntryPoints[VideoType.Front].Seconds);
            }
            System.GC.Collect();
        }

        private void DelayVideo(VideoFileReader fileReader, int seconds)
        {
            for (int i = 0; i < fileReader.FrameRate * seconds; i++)
            {
                fileReader.ReadVideoFrame();
                if (i % 50 == 0) System.GC.Collect();
            }
            System.GC.Collect();
        }

        private async Task RenderThread()
        {
            int frameRate = TopFileReader.FrameRate;
            TimeSpan fps30 = TimeSpan.FromMilliseconds(33);
            for (int i = 0; i < UsefullData.VideoDuration.Seconds * frameRate; i++)
            {
                System.GC.Collect();
               
                Bitmap cubemap = new Bitmap(UsefullData.OutputWidth, (UsefullData.OutputWidth / 4) * 3);
                Bitmap equirect = new Bitmap(UsefullData.OutputWidth, UsefullData.OutputHeight);
                int _singleCubeFaceEdgeDimension = UsefullData.OutputWidth / 4;

                Bitmap top = i + UsefullData.VideosEntryPoints[VideoType.Top].Seconds * frameRate < 0 
                    || i > TopFileReader.FrameCount ? new Bitmap(TopFileReader.Width, TopFileReader.Height) : TopFileReader.ReadVideoFrame();
                Bitmap down = i + UsefullData.VideosEntryPoints[VideoType.Bottom].Seconds * frameRate < 0 
                    || i > BottomFileReader.FrameCount ? new Bitmap(BottomFileReader.Width, BottomFileReader.Height) : BottomFileReader.ReadVideoFrame();
                Bitmap front = i + UsefullData.VideosEntryPoints[VideoType.Front].Seconds * frameRate < 0 
                    || i > FrontFileReader.FrameCount ? new Bitmap(FrontFileReader.Width, FrontFileReader.Height) : FrontFileReader.ReadVideoFrame();
                Bitmap back = i + UsefullData.VideosEntryPoints[VideoType.Back].Seconds * frameRate < 0 
                    || i > BackFileReader.FrameCount ? new Bitmap(BackFileReader.Width, BackFileReader.Height) : BackFileReader.ReadVideoFrame();
                Bitmap left = i + UsefullData.VideosEntryPoints[VideoType.Left].Seconds * frameRate < 0 
                    || i > LeftFileReader.FrameCount ? new Bitmap(LeftFileReader.Width, LeftFileReader.Height) : LeftFileReader.ReadVideoFrame();
                Bitmap right = i + UsefullData.VideosEntryPoints[VideoType.Right].Seconds * frameRate < 0 
                    || i > RightFileReader.FrameCount ? new Bitmap(RightFileReader.Width, RightFileReader.Height) : RightFileReader.ReadVideoFrame();

                CreateCubeMap(cubemap, _singleCubeFaceEdgeDimension, top, down, front, back, left, right);
                
                for (int j = 0; j < UsefullData.OutputHeight; j++)
                {
                    for (int ji = 0; ji < UsefullData.OutputWidth; ji++)
                    {
                        equirect.SetPixel(
                                    Equirectangular2CubeMap_Corespondence[j][ji].p1.x, Equirectangular2CubeMap_Corespondence[j][ji].p1.y,
                                    cubemap.GetPixel(Equirectangular2CubeMap_Corespondence[j][ji].p2.x, Equirectangular2CubeMap_Corespondence[j][ji].p2.y
                                    ));
                    }
                }
                lock (lockObject)
                {
                    FinalVideoOutputWriter.WriteVideoFrame(equirect, TimeSpan.FromMilliseconds(fps30.Milliseconds * i));
                }
                
                System.GC.Collect();
            }
            FinalVideoOutputWriter.Close();
            #region codcomentat
            // List<Task<Bitmap>> tasks = new List<Task<Bitmap>>();
            //List<Bitmap> cubemapsForEachThread = new List<Bitmap>();
            //for (int thr = 0; thr < Environment.ProcessorCount; thr++)
            //{
            //cubemapsForEachThread.Add(cubemap);
            // }

            //foreach (var cubemap in cubemapsForEachThread)
            //{
            //    var task = Task<Bitmap>.Run(() =>
            //    {
            //        Bitmap equirect = new Bitmap(UsefullData.OutputWidth, UsefullData.OutputHeight);
            //System.GC.Collect();
            //  return equirect;
            //});
            //tasks.Add(task);
            //await task;
            // }
            #region not threaded
            //for (int thr = 0; thr < Environment.ProcessorCount; thr++)
            //{
            //    var task = Task<Bitmap>.Run(() =>
            //    {
            //        Bitmap cubemap = new Bitmap(UsefullData.OutputWidth, (UsefullData.OutputWidth / 4) * 3);
            //        Bitmap equirect = new Bitmap(UsefullData.OutputWidth, UsefullData.OutputHeight);
            //        int _singleCubeFaceEdgeDimension = UsefullData.OutputWidth / 4;

            //        Bitmap top = i + thr + UsefullData.VideosEntryPoints[VideoType.Top].Seconds * frameRate < 0 || i + thr > TopFileReader.FrameCount ? new Bitmap(TopFileReader.Width, TopFileReader.Height) : TopFileReader.ReadVideoFrame();
            //        Bitmap down = i + thr + UsefullData.VideosEntryPoints[VideoType.Bottom].Seconds * frameRate < 0 || i + thr > BottomFileReader.FrameCount ? new Bitmap(BottomFileReader.Width, BottomFileReader.Height) : BottomFileReader.ReadVideoFrame();
            //        Bitmap front = i + thr + UsefullData.VideosEntryPoints[VideoType.Front].Seconds * frameRate < 0 || i + thr > FrontFileReader.FrameCount ? new Bitmap(FrontFileReader.Width, FrontFileReader.Height) : FrontFileReader.ReadVideoFrame();
            //        Bitmap back = i + thr + UsefullData.VideosEntryPoints[VideoType.Back].Seconds * frameRate < 0 || i + thr > BackFileReader.FrameCount ? new Bitmap(BackFileReader.Width, BackFileReader.Height) : BackFileReader.ReadVideoFrame();
            //        Bitmap left = i + thr + UsefullData.VideosEntryPoints[VideoType.Left].Seconds * frameRate < 0 || i + thr > LeftFileReader.FrameCount ? new Bitmap(LeftFileReader.Width, LeftFileReader.Height) : LeftFileReader.ReadVideoFrame();
            //        Bitmap right = i + thr + UsefullData.VideosEntryPoints[VideoType.Right].Seconds * frameRate < 0 || i + thr > RightFileReader.FrameCount ? new Bitmap(RightFileReader.Width, RightFileReader.Height) : RightFileReader.ReadVideoFrame();

            //        CreateCubeMap(cubemap, _singleCubeFaceEdgeDimension, top, down, front, back, left, right);


            //        for (int j = 0; j < UsefullData.OutputHeight; j ++)
            //        {
            //            for (int ji = 0; ji < UsefullData.OutputWidth; ji++)
            //            {
            //                equirect.SetPixel(
            //                    Equirectangular2CubeMap_Corespondence[j][ji].p1.x, Equirectangular2CubeMap_Corespondence[j][ji].p1.y,
            //                    cubemap.GetPixel(Equirectangular2CubeMap_Corespondence[j][ji].p2.x, Equirectangular2CubeMap_Corespondence[j][ji].p2.y
            //                    ));
            //            }
            //        }

            //        return equirect;
            //    });
            //    tasks.Add(task);
            //    await task;
            #endregion;
            // Task.WaitAll(tasks.ToArray());
            // int thr1 = 0;
            // foreach (var t in tasks)
            //{
            //FinalVideoOutputWriter.Open(UsefullData.OutputFilePath, UsefullData.OutputWidth, UsefullData.OutputHeight);
            //FinalVideoOutputWriter.Close();
            //  thr1++;
            //}
            #endregion;
        }


        private void CreateCubeMap(Bitmap cubemap, int _singleCubeFaceEdgeDimension, Bitmap top, Bitmap down, Bitmap front, Bitmap back, Bitmap left, Bitmap right)
        {
            if (top == null)
            {
                cubemap = new Bitmap(UsefullData.OutputWidth, (UsefullData.OutputWidth / 4) * 3);
                return;
            }
            top = new Bitmap(top, _singleCubeFaceEdgeDimension, _singleCubeFaceEdgeDimension);
            down = new Bitmap(down, _singleCubeFaceEdgeDimension, _singleCubeFaceEdgeDimension);
            front = new Bitmap(front, _singleCubeFaceEdgeDimension, _singleCubeFaceEdgeDimension);
           back  = new Bitmap(back, _singleCubeFaceEdgeDimension, _singleCubeFaceEdgeDimension);
            left = new Bitmap(left, _singleCubeFaceEdgeDimension, _singleCubeFaceEdgeDimension);
            right = new Bitmap(right, _singleCubeFaceEdgeDimension, _singleCubeFaceEdgeDimension);

            for (int x = 0; x < cubemap.Width; x++)
            {
                for (int y = 0; y < cubemap.Height; y++)
                {
                    if (x >= _singleCubeFaceEdgeDimension * 2 && x < _singleCubeFaceEdgeDimension * 3)
                        if (y >= 0 && y < _singleCubeFaceEdgeDimension * 1)
                            cubemap.SetPixel(x, y, top.GetPixel(x % (_singleCubeFaceEdgeDimension * 2), y)); //UP

                    if (x >= _singleCubeFaceEdgeDimension * 1 && x < _singleCubeFaceEdgeDimension * 2)
                        if (y >= _singleCubeFaceEdgeDimension * 1 && y < _singleCubeFaceEdgeDimension * 2)
                            cubemap.SetPixel(x, y, left.GetPixel(x % _singleCubeFaceEdgeDimension, y % _singleCubeFaceEdgeDimension * 1));//Left

                    if (x >= 0 && x < _singleCubeFaceEdgeDimension * 1)
                        if (y >= _singleCubeFaceEdgeDimension * 1 && y < _singleCubeFaceEdgeDimension * 2)
                            cubemap.SetPixel(x, y, back.GetPixel(x, y % _singleCubeFaceEdgeDimension * 1));//Back

                    if (x >= _singleCubeFaceEdgeDimension * 2 && x < _singleCubeFaceEdgeDimension * 3)
                        if (y >= _singleCubeFaceEdgeDimension * 1 && y < _singleCubeFaceEdgeDimension * 2)
                            cubemap.SetPixel(x, y, front.GetPixel(x % (_singleCubeFaceEdgeDimension * 2), y % _singleCubeFaceEdgeDimension * 1));//Front

                    if (x >= _singleCubeFaceEdgeDimension * 3 && x < _singleCubeFaceEdgeDimension * 4)
                        if (y >= _singleCubeFaceEdgeDimension * 1 && y < _singleCubeFaceEdgeDimension * 2)
                            cubemap.SetPixel(x, y, right.GetPixel(x % (_singleCubeFaceEdgeDimension * 3), y % _singleCubeFaceEdgeDimension * 1));//Right

                    if (x >= _singleCubeFaceEdgeDimension * 2 && x < _singleCubeFaceEdgeDimension * 3)
                        if (y >= _singleCubeFaceEdgeDimension * 2 && y < _singleCubeFaceEdgeDimension * 3)
                            cubemap.SetPixel(x, y, down.GetPixel(x % (_singleCubeFaceEdgeDimension * 2), y % (_singleCubeFaceEdgeDimension * 2)));//Down
                }
            }
        }

        private void InitializeMatrices()
        {
            double u, v; //Normalised texture coordinates, from 0 to 1, starting at lower left corner
            double phi, theta; //Polar coordinates
            int cubeFaceWidth = UsefullData.OutputWidth / 4, cubeFaceHeight = (UsefullData.OutputWidth / 4);
            Equirectangular2CubeMap_Corespondence = new List<List<CorespondencePoint>>();
            //for each pixel in equirectangular image
            for (int j = 0; j < UsefullData.OutputHeight; j++)
            {
                v = 1 - ((double)j / UsefullData.OutputHeight);
                theta = v * Math.PI;

                Equirectangular2CubeMap_Corespondence.Add(new List<CorespondencePoint>());

                for (int i = 0; i < UsefullData.OutputWidth; i++)
                {
                    
                    //Columns start from the left
                    u = ((double)i / UsefullData.OutputWidth);
                    phi = u * 2 * Math.PI;

                    double x, y, z; //Unit vector
                    x = Math.Sin(phi) * Math.Sin(theta) * -1;
                    y = Math.Cos(theta);
                    z = Math.Cos(phi) * Math.Sin(theta) * -1;

                    double xa, ya, za;
                    double a;
                    double mx, my, mz;
                    //a = Math.Max(new double[3] { Math.Abs(x), Math.Abs(y), Math.Abs(z) });
                    mx = Math.Abs(x);
                    my = Math.Abs(y);
                    mz = Math.Abs(z);

                    if (mx > my && mx > mz)
                        a = mx;
                    else
                        if (my > mx && my > mz)
                        a = my;
                    else
                        a = mz;


                    //Vector Parallel to the unit vector that lies on one of the cube faces
                    xa = x / a;
                    ya = y / a;
                    za = z / a;

                    //Color color;
                    int xPixel, yPixel;
                    int xOffset, yOffset;

                    if (xa == 1)
                    {
                        //Right
                        xPixel = (int)((((za + 1f) / 2f) - 1f) * cubeFaceWidth);
                        xOffset = 3 * cubeFaceWidth; //Offset
                        yPixel = (int)((((ya + 1f) / 2f)) * cubeFaceHeight);
                        yOffset = cubeFaceHeight; //Offset
                    }
                    else if (xa == -1)
                    {
                        //Left
                        xPixel = (int)((((za + 1f) / 2f)) * cubeFaceWidth);
                        xOffset = cubeFaceWidth;
                        yPixel = (int)((((ya + 1f) / 2f)) * cubeFaceHeight);
                        yOffset = cubeFaceHeight;
                    }
                    else if (ya == 1)
                    {
                        //Up
                        xPixel = (int)((((xa + 1f) / 2f)) * cubeFaceWidth);
                        xOffset = 2 * cubeFaceWidth;
                        yPixel = (int)((((za + 1f) / 2f) - 1f) * cubeFaceHeight);
                        yOffset = 2 * cubeFaceHeight;
                    }
                    else if (ya == -1)
                    {
                        //Down
                        xPixel = (int)((((xa + 1f) / 2f)) * cubeFaceWidth);
                        xOffset = 2 * cubeFaceWidth;
                        yPixel = (int)((((za + 1f) / 2f)) * cubeFaceHeight);
                        yOffset = 0;
                    }
                    else if (za == 1)
                    {
                        //Front
                        xPixel = (int)((((xa + 1f) / 2f)) * cubeFaceWidth);
                        xOffset = 2 * cubeFaceWidth;
                        yPixel = (int)((((ya + 1f) / 2f)) * cubeFaceHeight);
                        yOffset = cubeFaceHeight;
                    }
                    else if (za == -1)
                    {
                        //Back
                        xPixel = (int)((((xa + 1f) / 2f) - 1f) * cubeFaceWidth);
                        xOffset = 0;
                        yPixel = (int)((((ya + 1f) / 2f)) * cubeFaceHeight);
                        yOffset = cubeFaceHeight;
                    }
                    else
                    {
                        //Debug.LogWarning("Unknown face, something went wrong");
                        xPixel = 0;
                        yPixel = 0;
                        xOffset = 0;
                        yOffset = 0;
                    }

                    xPixel = Math.Abs(xPixel);
                    yPixel = Math.Abs(yPixel);

                    xPixel += xOffset;
                    yPixel += yOffset;

                    if (yPixel >= UsefullData.OutputHeight)
                        yPixel -= 1;
                    if (xPixel >= UsefullData.OutputWidth)
                        xPixel -= 1;
                    //color = CubeMap.GetPixel(xPixel, yPixel);

                    //equiTexture.SetPixel(i, j, color);
                    Equirectangular2CubeMap_Corespondence[j].Add(new CorespondencePoint()
                    {
                        p1 = new Point() { x = i, y = j },
                        p2 = new Point() { x = xPixel, y = yPixel }
                    });
                }
            }
        }

        private void MapCubeMaptoEquirectangularMap()
        {
            throw new NotImplementedException();
        }

        private void MapFacesToCubeMap()
        {

        }
    }

    internal class CorespondencePoint
    {
        public Point p1 { get; set; }
        public Point p2 { get; set; }
    }

    internal class Point
    {
        public int x { get; set; }
        public int y { get; set; }
    }
}
