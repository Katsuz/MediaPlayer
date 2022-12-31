using MediaPlayerProject.View;
using System;
using System.IO;
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
using System.Reflection.Emit;
using Microsoft.Win32;
using System.Windows.Threading;
using MediaPlayerProject.DataClass;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MediaPlayerProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        // Khởi tạo tài nguyên
        private System.Windows.Controls.Button curBtn;
        public MediaPlayer Player = new MediaPlayer();
        public DispatcherTimer Timer;
        private ObservableCollection<DataClass.Playlist> testPlaylist = new ObservableCollection<DataClass.Playlist>();
        public MediaPlayerProject.DataClass.Playlist CurPlaylist;

        public event PropertyChangedEventHandler PropertyChanged;

        public int CurSongIndex { get; set; }
        public ObservableCollection<DataClass.Playlist> TestPlaylist { get => testPlaylist; set => testPlaylist = value; }
        public DataClass.Playlist RecentOpened { get; set; }
        public DataClass.Song CurSong { get; set; }
        public void ChangeView(UserControl view)
        {
            MainDisplay.Children.Clear();
            MainDisplay.Children.Add(view);
        }

        private void ResetBtn()
        {
            Style style = Application.Current.FindResource("menuButton") as Style;
            curBtn.Style = style;
        }

        private void HighlightBtn(System.Windows.Controls.Button btnName)
        {
            curBtn = btnName;
            Style style = Application.Current.FindResource("menuButtonChoosed") as Style;
            curBtn.Style = style;
        }

        public void ChangeCurBtnTo(System.Windows.Controls.Button btnName)
        {
            ResetBtn();
            HighlightBtn(btnName);
        }

        public MainWindow()
        {
            InitializeComponent();
            RecentOpened = new MediaPlayerProject.DataClass.Playlist("Recent Opened Songs");
            TestPlaylist.Add(RecentOpened);
            CurPlaylist = RecentOpened;
            playlistBox.ItemsSource = TestPlaylist;
            CurSongIndex = -1;
            CurSong = new Song("Choose a Song");
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeView(new Home(this));
            HighlightBtn(HomeBtn);
            PauseBtn.Visibility= Visibility.Collapsed;
            PlayBtn.Visibility= Visibility.Visible;
            Player.MediaOpened += Player_MediaOpened;
        }

        private void Player_MediaOpened(object sender, EventArgs e)
        {
            int hours = Player.NaturalDuration.TimeSpan.Hours;
            int minutes = Player.NaturalDuration.TimeSpan.Minutes;
            int seconds = Player.NaturalDuration.TimeSpan.Seconds;
            totalTime.Text = $"{hours}:{minutes}:{seconds}";

            // cập nhật max value của slider
            slider.Maximum = Player.NaturalDuration.TimeSpan.TotalSeconds;
        }

        private void ButtonHome_Click(object sender, RoutedEventArgs e)
        {
            ChangeView(new Home(this));
            ChangeCurBtnTo(HomeBtn);
        }

        private void BtnRecentPlayed_Click(object sender, RoutedEventArgs e)
        {
            ChangeView(new RecentPlayed());
            ChangeCurBtnTo(RecentPlayedBtn);
        }

        private void NowPlaying_Clicked(object sender, RoutedEventArgs e)
        {
            ChangeView(new NowPlaying(this));
            ChangeCurBtnTo(NowPlayingBtn);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            int hours = Player.Position.Hours;
            int minutes = Player.Position.Minutes;
            int seconds = Player.Position.Seconds;
            slider.Value = hours*3600 + minutes*60 + seconds;
            currentPosition.Text = $"{hours}:{minutes}:{seconds}";
            Title = $"{hours}:{minutes}:{seconds}";
        }

        public void OpenSong(string absolutePath)
        {
            if (absolutePath == null) { return; }
            Player.Open(new Uri(absolutePath, UriKind.Absolute));
            Player.Play();
            Timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 1, 0) };
            Timer.Tick += Timer_Tick;
            Timer.Start();
            PlayBtn.Visibility = Visibility.Collapsed;
            PauseBtn.Visibility = Visibility.Visible;
        }
        private void OpenFile_Clicked(object sender, RoutedEventArgs e)
        {
            ChangeCurBtnTo(OpenFileBtn);
            var openFileScreen = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Sound Files|*.mp3| Video Files|*.mp4",
                //InitialDirectory = @"C:\",
                Title = "Please select music to be played."
            };
            if (openFileScreen.ShowDialog() == true)
            {
                int oldIndex = CurSongIndex;
                CurSongIndex = RecentOpened.ListSong.Count;
                RecentOpened.addSongsToPlaylist(openFileScreen.FileNames);
                if (CurSongIndex == RecentOpened.ListSong.Count)
                {
                    CurSongIndex = oldIndex;
                    string messageBoxText = "All choosed files had already been opened before.";
                    string caption = "Adding Music Notification";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Information;
                    MessageBoxResult result;

                    result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
                }
                else
                {
                    CurPlaylist = RecentOpened;
                    CurSong = RecentOpened.ListSong[CurSongIndex];
                    OpenSong(CurSong.AbsolutePath);
                }
            }
            ChangeView(new Home(this));
            ChangeCurBtnTo(HomeBtn);
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurSong.AbsolutePath == null) { return; }
            PauseBtn.Visibility = Visibility.Collapsed;
            PlayBtn.Visibility = Visibility.Visible;
            Player.Pause();
            Timer.Stop();
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurSong.AbsolutePath == null) { return; }
            PlayBtn.Visibility = Visibility.Collapsed;
            PauseBtn.Visibility = Visibility.Visible;
            Player.Play();
            Timer.Start();
        }

        //private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    // Lay gia tri hien tai cua slide
        //    // Cap nhat vao player

        //    double value = slider.Value;
        //    TimeSpan newPosition = TimeSpan.FromSeconds(value);
        //    Player.Position = newPosition;
        //}

        private void ChangeVolBtnIconTo(string type)
        {
            volumeBtn.Tag = type;
        }

        private void ProcessVolBtnIcon()
        {
            if (Player.Volume == 0)
            {
                if (volumeBtn.Tag.ToString() == "VolumeOff")
                {
                    ChangeVolBtnIconTo("VolumeLow");
                    Player.Volume = 0.1;
                }
                else
                {
                    ChangeVolBtnIconTo("VolumeMute");
                }
            }
            else if (Player.Volume < 0.3)
            {
                ChangeVolBtnIconTo("VolumeLow");
            }
            else if (Player.Volume <= 0.7)
            {
                ChangeVolBtnIconTo("VolumeMedium");
            }
            else if (Player.Volume > 0.7)
            {
                ChangeVolBtnIconTo("VolumeHigh");
            }
        }

        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Lay gia tri hien tai cua volume slider
            // Cap nhat vao player
            double newVolume = sliderVolume.Value/10;
            Player.Volume = newVolume;
            ProcessVolBtnIcon();
        }

        private void VolumeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (volumeBtn.Tag.ToString() != "VolumeOff")
            {
                Player.Volume = 0;
                ChangeVolBtnIconTo("VolumeOff");
            }
            else
            {
                double newVolume = sliderVolume.Value / 10;
                Player.Volume = newVolume;
                ProcessVolBtnIcon();
            }
        }

        private void AddPlaylistBtn_Click(object sender, RoutedEventArgs e)
        {
            var addPlaylistWindow = new AddPlaylist();
            if (addPlaylistWindow.ShowDialog() == true)
            {
                var newPlaylist = (MediaPlayerProject.DataClass.Playlist)addPlaylistWindow.NewPlaylist.Clone();
                TestPlaylist.Add(newPlaylist);
            }
            else
            {
                Title = "KHONG CO DU LIEU";
            }
        }

        private void PlaylistBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = playlistBox.SelectedIndex;
            if ( i >= 0)
            {
                CurPlaylist = TestPlaylist[i];
                ChangeView(new View.Playlist(CurPlaylist, this));
            }
            playlistBox.SelectedIndex = -1;
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (CurSongIndex != -1)
            {
                if (CurSongIndex > 0)
                {
                    CurSongIndex -= 1;
                    CurSong = CurPlaylist.ListSong[CurSongIndex];
                    OpenSong(CurSong.AbsolutePath);
                }
                else
                {
                    CurSongIndex = CurPlaylist.ListSong.Count - 1;
                    CurSong = CurPlaylist.ListSong[CurSongIndex];
                    OpenSong(CurSong.AbsolutePath);
                }
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (CurSongIndex != -1)
            {
                if (CurSongIndex < CurPlaylist.ListSong.Count - 1)
                {
                    CurSongIndex += 1;
                    CurSong = CurPlaylist.ListSong[CurSongIndex];
                    OpenSong(CurSong.AbsolutePath);
                }
                else
                {
                    CurSongIndex = 0;
                    CurSong = CurPlaylist.ListSong[CurSongIndex];
                    OpenSong(CurSong.AbsolutePath);
                }
            }
        }

        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            // Lay gia tri hien tai cua slide
            // Cap nhat vao player

            if (Player.HasAudio)
            {
                double value = slider.Value;
                TimeSpan newPosition = TimeSpan.FromSeconds(value);
                Player.Position = newPosition;
                Player.Play();
                Timer.Start();
                PlayBtn.Visibility = Visibility.Collapsed;
                PauseBtn.Visibility = Visibility.Visible;
            }
        }

        private void Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (Player.HasAudio)
            {
                Player.Pause();
                Timer.Stop();
                PlayBtn.Visibility = Visibility.Visible;
                PauseBtn.Visibility = Visibility.Collapsed;
            }
        }
    }   
}
