using _360_Video_Maker.RenderingLogic;
using _360_Video_Maker.ResourceLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _360_Video_Maker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EditorPage EP = new EditorPage();
        private CubeMapEditor1 CME1 = new CubeMapEditor1();

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowState = WindowState.Maximized;
            MainFrame.Content = EP;
        }

        private void GoToRenderer_MenuItem(object sender, RoutedEventArgs e)
        {
            //MainFrame.Content = RP;
        }

        private void GoToEditor_MenuItem(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = EP;
        }

        private void GoToCubeMapEditor_MenuItem(object sender, RoutedEventArgs e)
        {
            CME1.ShowDialog(); //cubeMapeEditor
            EP.GotFocus_Custom();
        }

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //render click
            var t = Task.Run(() => { 
            RenderEngineFacade renderEngineFacade = new RenderEngineFacade();
            renderEngineFacade.Intiliaze();
            renderEngineFacade.Start();
            });
            await t;
        }
    }
}
