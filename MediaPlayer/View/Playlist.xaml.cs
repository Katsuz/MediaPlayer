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
    /// Interaction logic for Playlist.xaml
    /// </summary>
    public partial class Playlist : UserControl
    {
        private DataClass.Playlist playlist;
        private MainWindow mainWindow;

        public Playlist(DataClass.Playlist oldPlaylist, MainWindow mainWD)
        {
            InitializeComponent();
            playlist = (DataClass.Playlist)oldPlaylist.Clone();
            DataContext = playlist;
            mainWindow = mainWD;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void playAll_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.CurSongIndex = 0;
            mainWindow.OpenSong(playlist.ListSong[0].AbsolutePath);
        }
    }
}
