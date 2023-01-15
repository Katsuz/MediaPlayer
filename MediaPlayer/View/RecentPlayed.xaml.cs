using MediaPlayerProject.DataClass;
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
    /// Interaction logic for RecentPlayed.xaml
    /// </summary>
    public partial class RecentPlayed : UserControl
    {
        private MainWindow mainWindow;
        public RecentPlayed(MainWindow mainWD)
        {
            InitializeComponent();
            mainWindow = mainWD;
            DataContext= mainWD;
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            Button selected = (Button)sender;
            string songAbsolutePath = (string)selected.Tag;
            DataClass.Song selectedSong = null;
            for (int i = 0; i < mainWindow.RecentPlayed_P.ListSong.Count; i++)
            {
                if (mainWindow.RecentPlayed_P.ListSong[i].AbsolutePath == songAbsolutePath)
                {
                    selectedSong = mainWindow.RecentPlayed_P.ListSong[i];
                }
            }
            mainWindow.CurSong = selectedSong;
            mainWindow.RecentPlayed_P.ListSong.Add(selectedSong);
            mainWindow.CurPlaylist = mainWindow.RecentPlayed_P;
            mainWindow.MakeNextList(mainWindow.IsShuffle, false);
            mainWindow.OpenSong(selectedSong.AbsolutePath);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Button selected = (Button)sender;
            string songAbsolutePath = (string)selected.Tag;
            DataClass.Song selectedSong = null;
            for (int i = 0; i < mainWindow.RecentPlayed_P.ListSong.Count; i++)
            {
                if (mainWindow.RecentPlayed_P.ListSong[i].AbsolutePath == songAbsolutePath)
                {
                    selectedSong = mainWindow.RecentPlayed_P.ListSong[i];
                }
            }
            mainWindow.QueueList.Add(selectedSong);
        }
    }
}
