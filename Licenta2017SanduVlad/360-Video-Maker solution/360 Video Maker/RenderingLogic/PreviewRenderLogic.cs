using _360_Video_Maker.ResourceLogic;
using Entities.Enums;
using PreviewRender;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoInformationRetrieval;

namespace _360_Video_Maker.RenderingLogic
{
    public class PreviewRenderLogic
    {

        //private VideoInformationRetrieval.VideoInformationRetrieval vfr;

        public PreviewRenderLogic()
        {
            
        }

        public Bitmap GetProjection(int prevWidth, int prevHeight, int second)
        {
            try
            {
                var videos = ResourceManager.GetInstance().GetAllVideoPaths();
                PreviewRender.PreviewRender pr = new PreviewRender.PreviewRender(prevWidth, prevHeight);


                return pr.GenerateProjection(videos, second, ResourceManager.GetInstance().GetVideosEntryPoints());
            }
            catch(Exception e)
            {
                return new Bitmap(100, 100);
            }
        }
    }
}
