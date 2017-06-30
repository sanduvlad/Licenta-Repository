using _360_Video_Maker.ResourceLogic;
using Entities;
using Microsoft.Win32;
using RenderEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _360_Video_Maker.RenderingLogic
{
    public class RenderEngineFacade
    {
        public RenderEngineFacade()
        {

        }

        private RenderEngineTransferData usefullData = new RenderEngineTransferData();

        public void Intiliaze()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Video files (*.avi, *.mp4) | *.avi; *.mp4";
            if (saveFileDialog.ShowDialog() == true)
            {
                usefullData.OutputFilePath = saveFileDialog.FileName;
            }

            usefullData.OutputHeight = ResourceManager.GetInstance().outputHeight;
            usefullData.OutputWidth = ResourceManager.GetInstance().outputWidth;
            usefullData.VideoDuration = ResourceManager.GetInstance().outputVideoDuration;
            usefullData.Videos = ResourceManager.GetInstance().GetAllVideoPaths();
            usefullData.VideosEntryPoints = ResourceManager.GetInstance().GetVideosEntryPoints();
        }

        public async Task Start()
        {
            RenderEngine.RenderEngine re = new RenderEngine.RenderEngine(usefullData);
            await re.StartProcessing();
        }
    }
}
