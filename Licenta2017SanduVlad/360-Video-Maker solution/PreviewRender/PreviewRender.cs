using Entities.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoInformationRetrieval;

namespace PreviewRender
{
    public class PreviewRender
    {

        #region CubeMap
        private Bitmap Front;
        private Bitmap Left;
        private Bitmap Right;
        private Bitmap Up;
        private Bitmap Down;
        private Bitmap Back;

        private Bitmap CubeMap;
        #endregion;

        private int _singleCubeFaceEdgeDimension;


        private Bitmap EquirectangularProjection;

        public PreviewRender(int width, int height)
        {
            EquirectangularProjection = new Bitmap(width, height);
            _singleCubeFaceEdgeDimension = width / 4;
        }


        public Bitmap GenerateProjection(Dictionary<string, VideoType> videos, int second, Dictionary<VideoType, TimeSpan> videosEntryPoint)
        {
            int widthHeight = EquirectangularProjection.Width / 4;

            string frontPath = videos.Single(v => v.Value == VideoType.Front).Key;
            string leftPath = videos.Single(v => v.Value == VideoType.Left).Key;
            string rightPath = videos.Single(v => v.Value == VideoType.Right).Key;
            string upPath = videos.Single(v => v.Value == VideoType.Top).Key;
            string downPath = videos.Single(v => v.Value == VideoType.Bottom).Key;
            string backPath = videos.Single(v => v.Value == VideoType.Back).Key;


            Front = new Bitmap(new VideoInformationRetrieval.VideoInformationRetrieval().
                GetVideoFrame(frontPath, second, videosEntryPoint[VideoType.Front]), _singleCubeFaceEdgeDimension, _singleCubeFaceEdgeDimension);
            Left = new Bitmap(new VideoInformationRetrieval.VideoInformationRetrieval().
                GetVideoFrame(leftPath, second, videosEntryPoint[VideoType.Left]), _singleCubeFaceEdgeDimension, _singleCubeFaceEdgeDimension);
            Right = new Bitmap(new VideoInformationRetrieval.VideoInformationRetrieval().
                GetVideoFrame(rightPath, second, videosEntryPoint[VideoType.Right]), _singleCubeFaceEdgeDimension, _singleCubeFaceEdgeDimension);
            Up = new Bitmap(new VideoInformationRetrieval.VideoInformationRetrieval().
                GetVideoFrame(upPath, second, videosEntryPoint[VideoType.Top]), _singleCubeFaceEdgeDimension, _singleCubeFaceEdgeDimension);
            Down = new Bitmap(new VideoInformationRetrieval.VideoInformationRetrieval().
                GetVideoFrame(downPath, second, videosEntryPoint[VideoType.Bottom]), _singleCubeFaceEdgeDimension, _singleCubeFaceEdgeDimension);
            Back = new Bitmap(new VideoInformationRetrieval.VideoInformationRetrieval().
                GetVideoFrame(backPath, second, videosEntryPoint[VideoType.Back]), _singleCubeFaceEdgeDimension, _singleCubeFaceEdgeDimension);

            GenerateCubeMap();

            return EquirectangularProjection = ConvertToEquirectangular(EquirectangularProjection.Width, EquirectangularProjection.Height);
        }

        private void GenerateCubeMap()
        {
            CubeMap = new Bitmap(_singleCubeFaceEdgeDimension * 4, _singleCubeFaceEdgeDimension * 3);
            for (int x = 0; x < CubeMap.Width; x++)
            {
                for (int y = 0; y < CubeMap.Height; y ++)
                {
                    if (x >= _singleCubeFaceEdgeDimension * 2 && x < _singleCubeFaceEdgeDimension * 3)
                        if (y >= 0 && y < _singleCubeFaceEdgeDimension * 1)
                            CubeMap.SetPixel(x, y, Up.GetPixel(x %( _singleCubeFaceEdgeDimension * 2), y)); //UP

                    if (x >= _singleCubeFaceEdgeDimension * 1 && x < _singleCubeFaceEdgeDimension * 2)
                        if (y >= _singleCubeFaceEdgeDimension * 1 && y < _singleCubeFaceEdgeDimension * 2)
                            CubeMap.SetPixel(x, y, Left.GetPixel(x % _singleCubeFaceEdgeDimension, y % _singleCubeFaceEdgeDimension * 1));//Left

                    if (x >= 0 && x < _singleCubeFaceEdgeDimension * 1)
                        if (y >= _singleCubeFaceEdgeDimension * 1 && y < _singleCubeFaceEdgeDimension * 2)
                            CubeMap.SetPixel(x, y, Back.GetPixel(x, y % _singleCubeFaceEdgeDimension * 1));//Back

                    if (x >= _singleCubeFaceEdgeDimension * 2 && x < _singleCubeFaceEdgeDimension * 3)
                        if (y >= _singleCubeFaceEdgeDimension * 1 && y < _singleCubeFaceEdgeDimension * 2)
                            CubeMap.SetPixel(x, y, Front.GetPixel(x %( _singleCubeFaceEdgeDimension * 2), y % _singleCubeFaceEdgeDimension * 1));//Front

                    if (x >= _singleCubeFaceEdgeDimension * 3 && x < _singleCubeFaceEdgeDimension * 4)
                        if (y >= _singleCubeFaceEdgeDimension * 1  && y < _singleCubeFaceEdgeDimension * 2)
                            CubeMap.SetPixel(x, y, Right.GetPixel(x % (_singleCubeFaceEdgeDimension * 3), y % _singleCubeFaceEdgeDimension * 1));//Right

                    if (x >= _singleCubeFaceEdgeDimension *2 && x < _singleCubeFaceEdgeDimension * 3)
                        if (y >= _singleCubeFaceEdgeDimension * 2 && y < _singleCubeFaceEdgeDimension * 3)
                            CubeMap.SetPixel(x, y, Down.GetPixel(x % (_singleCubeFaceEdgeDimension * 2), y % (_singleCubeFaceEdgeDimension * 2)));//Down
                }
            }
        }
        
        /// https://stackoverflow.com/questions/34250742/converting-a-cubemap-into-equirectangular-panorama
        public Bitmap ConvertToEquirectangular(int outputWidth, int outputHeight)
        {
            Bitmap equiTexture = new Bitmap(outputWidth, outputHeight);
            double u, v; //Normalised texture coordinates, from 0 to 1, starting at lower left corner
            double phi, theta; //Polar coordinates
            int cubeFaceWidth, cubeFaceHeight;

            cubeFaceWidth = CubeMap.Width / 4; //4 horizontal faces
            cubeFaceHeight = CubeMap.Height / 3; //3 vertical faces


            for (int j = 0; j < equiTexture.Height; j++)
            {
                //Rows start from the bottom
                v = 1 - ((double)j / equiTexture.Height);
                theta = v * Math.PI;

                for (int i = 0; i < equiTexture.Width; i++)
                {
                    //Columns start from the left
                    u = ((double)i / equiTexture.Width);
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

                    Color color;
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
                        xOffset = 2* cubeFaceWidth;
                        yPixel = (int)((((za + 1f) / 2f) - 1f) * cubeFaceHeight);
                        yOffset = 2 * cubeFaceHeight;
                    }
                    else if (ya == -1)
                    {
                        //Down
                        xPixel = (int)((((xa + 1f) / 2f)) * cubeFaceWidth);
                        xOffset = 2* cubeFaceWidth;
                        yPixel = (int)((((za + 1f) / 2f)) * cubeFaceHeight);
                        yOffset = 0;
                    }
                    else if (za == 1)
                    {
                        //Front
                        xPixel = (int)((((xa + 1f) / 2f)) * cubeFaceWidth);
                        xOffset = 2* cubeFaceWidth;
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
                    if (yPixel >= outputHeight)
                        yPixel -= 1;
                    if (xPixel >= outputWidth)
                        xPixel -= 1;
                    color = CubeMap.GetPixel(xPixel, yPixel);
                    equiTexture.SetPixel(i, j, color);
                }
            }
            return equiTexture;
        }
    }
}
