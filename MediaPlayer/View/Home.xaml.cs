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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MediaPlayerProject.View
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        private MainWindow mainWindow;
        private VideoDrawing videoDrawing = new VideoDrawing();
        private DrawingBrush brush;

        public Home(MainWindow mainWD)
        {
            InitializeComponent();
            mainWindow = mainWD;
            DataContext = mainWD;
            videoDrawing.Rect = new Rect(0, 0, 420, 280);
            videoDrawing.Player = mainWindow.Player;
            brush= new DrawingBrush(videoDrawing);
            VideoPlace.Background = brush;
        }
    }
}
