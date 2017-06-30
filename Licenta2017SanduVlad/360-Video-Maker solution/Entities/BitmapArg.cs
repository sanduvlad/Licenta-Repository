using System;
using System.Drawing;

namespace Entities
{
    public class BitmapArg : EventArgs
    {
        private Bitmap projection;
        public BitmapArg(Bitmap b)
        {
            projection = b;
        }

        public Bitmap GetProjection()
        {
            return projection;
        }
    }
}
