using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for NowPlaying.xaml
    /// </summary>
    public partial class NowPlaying : UserControl, INotifyPropertyChanged
    {
        private MainWindow mainWindow;
        private DataClass.Playlist playlist;
        public NowPlaying(MainWindow mainWD)
        {
            InitializeComponent();
            mainWindow = mainWD;
            DataContext = mainWD;
            NextListView.ItemsSource = mainWindow.NextList;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
