using Microsoft.Win32;
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
            playlist = oldPlaylist;
            DataContext = playlist;
            mainWindow = mainWD;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void PlayAll_Click(object sender, RoutedEventArgs e)
        {
            if (playlist.ListSong.Count == 0) { return; }
            mainWindow.QueueList.Clear();
            mainWindow.CurPlaylist = playlist;
            mainWindow.CurSongIndex = 0;
            mainWindow.CurSong = playlist.ListSong[0];
            mainWindow.MakeNextList(mainWindow.IsShuffle, false);
            mainWindow.OpenSong(mainWindow.CurSong.AbsolutePath);
        }

        private void PlaySingle_Click(object sender, RoutedEventArgs e)
        {
            Button selected = (Button)sender;
            int songID = (int)selected.Tag;
            DataClass.Song selectedSong = null;
            for (int i = 0; i < playlist.ListSong.Count; i++)
            {
                if (playlist.ListSong[i].ID == songID)
                {
                    selectedSong = playlist.ListSong[i];
                }
            }
            mainWindow.CurSong = selectedSong;
            mainWindow.CurPlaylist = playlist;
            mainWindow.MakeNextList(mainWindow.IsShuffle, false);
            mainWindow.OpenSong(selectedSong.AbsolutePath);
        }

        private void DeleteSingle_Click(object sender, RoutedEventArgs e)
        {
            Button selected = (Button)sender;
            int songID = (int)selected.Tag;
            DataClass.Song selectedSong = null;
            for (int i = 0; i < playlist.ListSong.Count; i++)
            {
                if (playlist.ListSong[i].ID == songID)
                {
                    selectedSong = playlist.ListSong[i];
                }
            }
            if (selectedSong != null)
            {
                playlist.RemoveSong(selectedSong);
                //if (mainWindow.CurSong == selectedSong)
                //{
                //    playlist.RemoveSong(selectedSong);
                //    if (mainWindow.CurPlaylist.ListSong.Count != 0)
                //    {
                //        mainWindow.CurSong = mainWindow.CurPlaylist.ListSong.First();
                //    }
                //    else if (mainWindow.RecentOpened.ListSong.Count != 0)
                //    {
                //        mainWindow.CurSong = mainWindow.RecentOpened.ListSong.First();
                //        mainWindow.CurPlaylist = mainWindow.RecentOpened;
                //    }    
                //    else
                //    {
                //        mainWindow.CurSong = new DataClass.Song("Open a Song to play");
                //        mainWindow.CurPlaylist = mainWindow.RecentOpened;
                //    }    
                //    mainWindow.OpenSong(mainWindow.CurSong.AbsolutePath);
                //    mainWindow.PauseBtn.Visibility = Visibility.Collapsed;
                //    mainWindow.PlayBtn.Visibility = Visibility.Visible;
                //    mainWindow.Player.Pause();
                //    mainWindow.Timer.Stop();
                //}    
                //else
                //{
                //    playlist.RemoveSong(selectedSong);
                //}    
            }
        }

        private void AddMusic_Click(object sender, RoutedEventArgs e)
        {
            var openFileScreen = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Sound Files|*.mp3| Video Files|*.mp4",
                DefaultExt = ".mp3",
                //InitialDirectory = @"C:\",
                Title = "Please select music to be played."
            };
            if (openFileScreen.ShowDialog() == true)
            {
                int oldIndex = mainWindow.CurSongIndex;
                if (mainWindow.CurSongIndex == -1){ mainWindow.CurSongIndex = playlist.ListSong.Count; }
                playlist.addSongsToPlaylist(openFileScreen.FileNames);
                if (mainWindow.CurSongIndex == playlist.ListSong.Count)
                {
                    mainWindow.CurSongIndex = oldIndex;
                    string messageBoxText = "All choosed files had already been added to this playlist before.";
                    string caption = "Adding Music Notification";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Information;
                    MessageBoxResult result;

                    result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
                }
                else
                {
                    if (mainWindow.CurSong.AbsolutePath == null)
                    {
                        mainWindow.CurSong = playlist.ListSong[mainWindow.CurSongIndex];
                        mainWindow.UpdateNextList(true, false, false, mainWindow.IsShuffle,mainWindow.IsRepeat);
                        mainWindow.OpenSong(mainWindow.CurSong.AbsolutePath);
                        mainWindow.PauseBtn.Visibility = Visibility.Collapsed;
                        mainWindow.PlayBtn.Visibility = Visibility.Visible;
                        mainWindow.Player.Pause();
                        mainWindow.Timer.Stop();
                    }    
                }
            }
        }

        private void DeletePlaylist_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.ListOfPlaylists.Remove(playlist);
            mainWindow.ChangeView(new Home(mainWindow));
            mainWindow.ChangeCurBtnTo(mainWindow.HomeBtn);
        }

        private void AddSingle_Click(object sender, RoutedEventArgs e)
        {
            Button selected = (Button)sender;
            int songID = (int)selected.Tag;
            DataClass.Song selectedSong = null;
            for (int i = 0; i < playlist.ListSong.Count; i++)
            {
                if (playlist.ListSong[i].ID == songID)
                {
                    selectedSong = playlist.ListSong[i];
                }
            }
            mainWindow.QueueList.Add(selectedSong);
        }
    }
}
