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

namespace MediaPlayerProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        // Khởi tạo tài nguyên
        private System.Windows.Controls.Button curBtn;
        private MediaPlayer Player = new MediaPlayer();
        private DispatcherTimer Timer;
        private Song testSong;
        private ObservableCollection<MediaPlayerProject.DataClass.Playlist> testPlaylist = new ObservableCollection<MediaPlayerProject.DataClass.Playlist>();
        private int CurSongIndex { get; set; }
        

        private void changeView(UserControl view)
        {
            MainDisplay.Children.Clear();
            MainDisplay.Children.Add(view);
        }

        private void resetBtn()
        {
            Style style = Application.Current.FindResource("menuButton") as Style;
            curBtn.Style = style;
        }

        private void highlightBtn(System.Windows.Controls.Button btnName)
        {
            curBtn = btnName;
            Style style = Application.Current.FindResource("menuButtonChoosed") as Style;
            curBtn.Style = style;
        }

        private void changeCurBtnTo(System.Windows.Controls.Button btnName)
        {
            resetBtn();
            highlightBtn(btnName);
        }

        public MainWindow()
        {
            InitializeComponent();
            testPlaylist.Add(new MediaPlayerProject.DataClass.Playlist("test"));
            playlistBox.ItemsSource = testPlaylist;
            CurSongIndex = -1;
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            changeView(new Home());
            highlightBtn(HomeBtn);
            PlayBtn.Visibility= Visibility.Collapsed;
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
            changeView(new Home());
            changeCurBtnTo(HomeBtn);
        }

        private void BtnRecentPlayed_Click(object sender, RoutedEventArgs e)
        {
            changeView(new RecentPlayed());
            changeCurBtnTo(RecentPlayedBtn);
        }

        private void NowPlaying_Clicked(object sender, RoutedEventArgs e)
        {
            changeView(new NowPlaying());
            changeCurBtnTo(NowPlayingBtn);
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
            Player.Open(new Uri(absolutePath, UriKind.Absolute));
            Player.Play();
            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 0, 1, 0); ;
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }
        private void OpenFile_Clicked(object sender, RoutedEventArgs e)
        {
            changeCurBtnTo(OpenFileBtn);
            var openFileScreen = new OpenFileDialog();
            openFileScreen.Multiselect= true;
            openFileScreen.Filter = "Sound Files|*.mp3| Video Files|*.mp4";
            openFileScreen.InitialDirectory = @"C:\";
            openFileScreen.Title = "Please select music to be played.";
            if (openFileScreen.ShowDialog() == true)
            {
                CurSongIndex = testPlaylist[0].ListSong.Count;
                testPlaylist[0].addSongsToPlaylist(openFileScreen.FileNames);
                OpenSong(testPlaylist[0].ListSong[CurSongIndex].AbsolutePath);
            }
            changeView(new Home());
            changeCurBtnTo(HomeBtn);
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            PauseBtn.Visibility = Visibility.Collapsed;
            PlayBtn.Visibility = Visibility.Visible;
            Player.Pause();
            Timer.Stop();
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            PlayBtn.Visibility = Visibility.Collapsed;
            PauseBtn.Visibility = Visibility.Visible;
            Player.Play();
            Timer.Start();
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Lay gia tri hien tai cua slide
            // Cap nhat vao player

            double value = slider.Value;
            TimeSpan newPosition = TimeSpan.FromSeconds(value);
            Player.Position = newPosition;
        }

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

        private void sliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Lay gia tri hien tai cua volume slider
            // Cap nhat vao player
            double newVolume = sliderVolume.Value/10;
            Player.Volume = newVolume;
            ProcessVolBtnIcon();
        }

        private void volumeBtn_Click(object sender, RoutedEventArgs e)
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

        private void addPlaylistBtn_Click(object sender, RoutedEventArgs e)
        {
            var addPlaylistWindow = new AddPlaylist();
            if (addPlaylistWindow.ShowDialog() == true)
            {
                var newPlaylist = (MediaPlayerProject.DataClass.Playlist)addPlaylistWindow.NewPlaylist.Clone();
                testPlaylist.Add(newPlaylist);
            }
            else
            {
                Title = "KHONG CO DU LIEU";
            }
        }

        private void playlistBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = playlistBox.SelectedIndex;
            if ( i >= 0)
            {
                var playlist = testPlaylist[i];
                changeView(new View.Playlist(playlist));
            }
            playlistBox.SelectedIndex = -1;
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {

            OpenSong(testPlaylist[0].ListSong[CurSongIndex + 1].AbsolutePath);
        }
    }   
}
