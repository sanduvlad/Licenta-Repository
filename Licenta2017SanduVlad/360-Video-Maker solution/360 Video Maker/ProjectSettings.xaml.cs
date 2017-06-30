using _360_Video_Maker.ResourceLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _360_Video_Maker
{
    /// <summary>
    /// Interaction logic for ProjectSettings.xaml
    /// </summary>
    public partial class ProjectSettings : Window
    {
        public ProjectSettings()
        {
            InitializeComponent();

            previewWidth.SetCurrentValue(800);
            previewHeight.SetCurrentValue(400);
            previewWidth.IncrementValue = 1;
            previewHeight.IncrementValue = 1;

            outputHeight.IncrementValue = 1;
            outputWidth.IncrementValue = 1;
            outputHeight.SetCurrentValue( 800);
            outputWidth.SetCurrentValue( 1600);

            outputVideoDuration.Text = TimeSpan.FromSeconds(30).ToString();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ResourceManager.GetInstance().previewRenderHeight = (int)previewHeight.CurrentValue;
            ResourceManager.GetInstance().previewRenderWidth = (int)previewWidth.CurrentValue;
            ResourceManager.GetInstance().outputHeight = (int)outputHeight.CurrentValue;
            ResourceManager.GetInstance().outputWidth = (int)outputWidth.CurrentValue;
            ResourceManager.GetInstance().outputVideoDuration = TimeSpan.Parse(outputVideoDuration.Text);

            this.Hide();
        }
    }
}
