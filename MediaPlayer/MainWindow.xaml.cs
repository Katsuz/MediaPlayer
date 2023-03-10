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
using System.Windows.Controls.Primitives;
using Newtonsoft.Json;
using MediaPlayerProject.Converter;
using Newtonsoft.Json.Linq;
using NHotkey.Wpf;
using NHotkey;

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

        private ObservableCollection<DataClass.Playlist> listOfPlaylists = new ObservableCollection<DataClass.Playlist>();
        public MediaPlayerProject.DataClass.Playlist CurPlaylist { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public int CurSongIndex { get; set; }
        public ObservableCollection<DataClass.Playlist> ListOfPlaylists { get => listOfPlaylists; set => listOfPlaylists = value; }
        public DataClass.Playlist RecentOpened { get; set; }
        public DataClass.Playlist RecentPlayed_P { get; set; }
        public DataClass.Song CurSong { get; set; }

        public ObservableCollection<Song> NextList { get; set; }
        public ObservableCollection<Song> PreviousList { get; set; }
        public ObservableCollection<Song> QueueList { get; set; }
        public bool IsRepeat { get; set; }
        public bool IsShuffle { get; set; }
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

        private void ReadData()
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory;
            string absolutePath = $"{folder}{"/Database/listOfPlaylists.json"}";
            try 
            {
                string listOfPlaylistJson = File.ReadAllText(absolutePath);
                ListOfPlaylists = JsonConvert.DeserializeObject<ObservableCollection<DataClass.Playlist>>(listOfPlaylistJson);
                RecentOpened = ListOfPlaylists[0];
                RecentPlayed_P = ListOfPlaylists[1];
                playlistBox.ItemsSource = listOfPlaylists;
            }
            catch (Exception error)
            {
                RecentOpened = new MediaPlayerProject.DataClass.Playlist("Recent Opened Songs");
                RecentOpened.Visibility = "Hidden";
                RecentPlayed_P = new MediaPlayerProject.DataClass.Playlist("Recent Played Songs");
                RecentPlayed_P.Visibility = "Hidden";
                ListOfPlaylists.Add(RecentOpened);
                ListOfPlaylists.Add(RecentPlayed_P);
                playlistBox.ItemsSource = ListOfPlaylists;
            }
        }

        private void ReadQueue()
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory;
            string absolutePath = $"{folder}{"/Database/queue.json"}";
            try
            {
                string queueJson = File.ReadAllText(absolutePath);
                QueueList = JsonConvert.DeserializeObject<ObservableCollection<DataClass.Song>>(queueJson);
            }
            catch (Exception error)
            {
                QueueList= new ObservableCollection<DataClass.Song>();
            }
        }

        private void ReadSettings()
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory;
            string absolutePath = $"{folder}{"/Database/settings.json"}";
            try
            {
                string settingstJson = File.ReadAllText(absolutePath);
                DataClass.Settings settings = JsonConvert.DeserializeObject<DataClass.Settings>(settingstJson);

                foreach (DataClass.Playlist playlist in ListOfPlaylists)
                {
                    if (playlist.Name == settings.CurPlaylistName)
                    {
                        CurPlaylist= playlist;
                    }
                }

                CurSongIndex = settings.CurSongIndex;

                foreach (DataClass.Song song in CurPlaylist.ListSong)
                {
                    if (song.AbsolutePath == settings.CurSongAbsolutePath)
                    {
                        CurSong = song;
                    }
                }

                if (CurSong != null)
                {
                    OpenSong(CurSong.AbsolutePath);
                }    
                else
                {
                    CurSongIndex = -1;
                    CurSong = new Song("Choose a Song");
                    CurSong.IsMp3 = "Visible";
                    CurSong.IsMp4 = "Collapsed";
                }    
                MakeNextList(IsShuffle, false);
                TimeSpan newPosition = TimeSpan.FromSeconds(settings.P_Hours*3600 + settings.P_Minutes*60 + settings.P_Seconds);
                Player.Position = newPosition;
                slider.Value = settings.P_Hours * 3600 + settings.P_Minutes * 60 + settings.P_Seconds;
                currentPosition.Text = $"{settings.P_Hours}:{settings.P_Minutes}:{settings.P_Seconds}";
                PauseBtn.Visibility = Visibility.Collapsed;
                PlayBtn.Visibility = Visibility.Visible;
                Player.Pause();
                

                IsShuffle = settings.IsShuffle;
                IsRepeat = settings.IsRepeat;
            }
            catch (Exception error)
            {
                CurPlaylist = RecentOpened;
                CurSongIndex = -1;
                CurSong = new Song("Choose a Song");
                CurSong.IsMp3 = "Visible";
                CurSong.IsMp4 = "Collapsed";
                IsRepeat = false;
                IsShuffle = false;
            }
        }


        private void OnVolumeIncrement(object sender, HotkeyEventArgs e)
        {
            sliderVolume.Value++;
            e.Handled = true;
        }

        private void OnVolumeDecrement(object sender, HotkeyEventArgs e)
        {
            sliderVolume.Value--;
            e.Handled = true;
        }

        private void OnNextSong(object sender, HotkeyEventArgs e)
        {
            if (CurSongIndex != -1)
            {
                UpdateNextList(false, true, false, IsShuffle, IsRepeat);
                OpenSong(CurSong.AbsolutePath);
            }
            e.Handled = true;
        }

        private void OnPreviousSong(object sender, HotkeyEventArgs e)
        {
            if (CurSongIndex != -1)
            {
                UpdateNextList(false, false, true, IsShuffle, IsRepeat);
                OpenSong(CurSong.AbsolutePath);
            }
            e.Handled = true;
        }

        private void OnChangeState(object sender, HotkeyEventArgs e)
        {
            if (CurSong.AbsolutePath == null) { return; }
            if (PauseBtn.Visibility == Visibility.Collapsed)
            {
                PlayBtn.Visibility = Visibility.Collapsed;
                PauseBtn.Visibility = Visibility.Visible;
                Player.Play();
                Timer.Start();
            }
            else if (PauseBtn.Visibility == Visibility.Visible)
            {
                PauseBtn.Visibility = Visibility.Collapsed;
                PlayBtn.Visibility = Visibility.Visible;
                Player.Pause();
                Timer.Stop();
            }
            e.Handled = true;
        }

        private void OnPlay(object sender, HotkeyEventArgs e)
        {
            if (CurSong.AbsolutePath == null) { return; }
            PlayBtn.Visibility = Visibility.Collapsed;
            PauseBtn.Visibility = Visibility.Visible;
            Player.Play();
            Timer.Start();
            e.Handled = true;
        }

        public MainWindow()
        {
            InitializeComponent();
            HotkeyManager.Current.AddOrReplace("VolumeIncrement", Key.Up, ModifierKeys.Control, OnVolumeIncrement);
            HotkeyManager.Current.AddOrReplace("VolumeDecrement", Key.Down, ModifierKeys.Control, OnVolumeDecrement);
            HotkeyManager.Current.AddOrReplace("NextSong", Key.Right, ModifierKeys.Control, OnNextSong);
            HotkeyManager.Current.AddOrReplace("PreviousSong", Key.Left, ModifierKeys.Control, OnPreviousSong);
            HotkeyManager.Current.AddOrReplace("ChangeState", Key.Space, ModifierKeys.Control, OnChangeState);

            curBtn = HomeBtn;
            Player.MediaOpened += Player_MediaOpened;
            Player.MediaEnded += Player_MediaEnded;
            Player.ScrubbingEnabled = true;
            slider.DataContext = Player;
            currentPosition.DataContext = Player;
            ReadData();
            ReadSettings();
            ReadQueue();
            ListOfPlaylists.RemoveAt(1);
            //ListOfPlaylists.RemoveAt(0);
            //ListOfPlaylists.RemoveAt(0);
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

        private void Player_MediaEnded(object sender, EventArgs e)
        {
            UpdateNextList(false, true, false, IsShuffle, IsRepeat);
            OpenSong(CurSong.AbsolutePath);
        }

        private void ButtonHome_Click(object sender, RoutedEventArgs e)
        {
            ChangeView(new Home(this));
            ChangeCurBtnTo(HomeBtn);
        }

        private void BtnRecentPlayed_Click(object sender, RoutedEventArgs e)
        {
            ChangeView(new RecentPlayed(this));
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
            if (CurSong.AbsolutePath != null) { AddToRecentPlayed(CurSong); }
            Player.Open(new Uri(absolutePath, UriKind.Absolute));
            Player.Play();
            Timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 1, 0) };
            Timer.Tick += Timer_Tick;
            Timer.Start();
            PlayBtn.Visibility = Visibility.Collapsed;
            PauseBtn.Visibility = Visibility.Visible;
        }

        public void ShuffleNextList()
        {
            var rnd = new Random();
            var listSong = NextList.OrderBy(item => rnd.Next());
            NextList = new ObservableCollection<Song>();
            foreach (Song song in listSong)
            {
                NextList.Add(song);
            }
        }

        public void CutShortNextList()
        {
            for (int i = 0; i < NextList.Count; i++)
            {
                if (NextList[i].AbsolutePath != CurSong.AbsolutePath)
                {
                    PreviousList.Add(NextList[i]);
                    NextList.RemoveAt(i);
                    i -= 1;
                }    
                else
                {
                    NextList.RemoveAt(i);
                    break;
                }    
            }
        }

        public void MakeNextList(bool shuffle, bool repeatTime)
        {
            NextList = new ObservableCollection<Song>();
            PreviousList = new ObservableCollection<Song>();
            foreach (Song song in CurPlaylist.ListSong)
            {
                NextList.Add(song);
                //if (song.AbsolutePath != CurSong.AbsolutePath)
                //{
                //    //NextList.Add(new Song(song.ID, song.Name, song.Singer, song.Album, song.Duration, song.AbsolutePath, song.Thumnail));
                //    NextList.Add(song);
                //}
            }
            if (!repeatTime)
            {
                CutShortNextList();
            }    
            else
            {
                CurSong = NextList[0];
                CutShortNextList();
            }
            if (shuffle) { ShuffleNextList(); }

            
        }

        public void UpdateNextList(bool needNewOne,bool next, bool previous, bool shuffle, bool repeat)
        {
            if (needNewOne) 
            { 
                MakeNextList(shuffle, false);
                return;
            }

            if (next)
            {
                if (QueueList.Count > 0)
                {
                    CurSong = QueueList[0];
                    QueueList.RemoveAt(0);
                    return;
                }    
                if (NextList.Count == 0)
                {
                    if (repeat)
                    {
                        MakeNextList(shuffle, true);
                    }
                    //else if ()
                    //{
                    //    CurSong = new Song("Choose a song or a playlist to play!");
                    //    string messageBoxText = "Your music queue ended.";
                    //    string caption = "Music Notification";
                    //    MessageBoxButton button = MessageBoxButton.OK;
                    //    MessageBoxImage icon = MessageBoxImage.Information;
                    //    MessageBoxResult result;

                    //    result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
                    //}
                    return;
                }
                if (PreviousList.Count == 0)
                {
                    PreviousList.Add(CurSong);
                }
                else if (PreviousList.Last().AbsolutePath != CurSong.AbsolutePath)
                {
                    PreviousList.Add(CurSong);
                }    

                CurSong = NextList[0];
                NextList.RemoveAt(0);
                return;
            }

            if (previous)
            {
                if (PreviousList.Count == 0) { return; }
                if (NextList.Count == 0)
                {
                    if (CurPlaylist.ListSong.Contains(CurSong))
                    {
                        NextList.Insert(0, CurSong);
                    }
                }
                else if(NextList.First().AbsolutePath != CurSong.AbsolutePath)
                {
                    if (CurPlaylist.ListSong.Contains(CurSong))
                    {
                        NextList.Insert(0, CurSong);
                    }
                }

                CurSong = PreviousList.Last();
                PreviousList.Remove(PreviousList.Last());
                return;
            }
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
                    Console.WriteLine(CurSong.Name);
                    OpenSong(CurSong.AbsolutePath);

                    for (int i = CurSongIndex + 1; i < RecentOpened.ListSong.Count; i++)
                    {
                        QueueList.Add(RecentOpened.ListSong[i]);
                    }    

                    //PauseBtn.Visibility = Visibility.Collapsed;
                    //PlayBtn.Visibility = Visibility.Visible;
                    //Player.Pause();
                    //Timer.Stop();
                    UpdateNextList(true, false, false, IsShuffle, IsRepeat);
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
                ListOfPlaylists.Add(newPlaylist);
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
                //CurPlaylist = TestPlaylist[i];
                ChangeView(new View.Playlist(ListOfPlaylists[i], this));
                ResetBtn();
            }
            playlistBox.SelectedIndex = -1;
        }

        public void AddToRecentPlayed(Song song)
        { 
            if (RecentPlayed_P.ListSong.Count== 0) { RecentPlayed_P.ListSong.Insert(0, song); }
            else if (song.AbsolutePath != RecentPlayed_P.ListSong[0].AbsolutePath)
            {
                RecentPlayed_P.ListSong.Insert(0, song);
            }
            if (RecentPlayed_P.ListSong.Count > 30)
            {
                RecentPlayed_P.ListSong.Remove(RecentPlayed_P.ListSong.Last());
            }    
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (CurSongIndex != -1)
            {
                UpdateNextList(false, false, true, IsShuffle, IsRepeat);
                OpenSong(CurSong.AbsolutePath);
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (CurSongIndex != -1)
            {
                UpdateNextList(false, true, false, IsShuffle, IsRepeat);
                OpenSong(CurSong.AbsolutePath);
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
            }
        }

        private void Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (Player.HasAudio)
            {

            }
        }

        private void Shuffle_Click(object sender, RoutedEventArgs e)
        {
            if (IsShuffle)
            {
                ShuffleIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.ShuffleDisabled;
                IsShuffle= false;
            }
            else
            {
                ShuffleIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.Shuffle;
                IsShuffle = true;
            }
            MakeNextList(IsShuffle, false);
        }

        private void Repeat_Click(object sender, RoutedEventArgs e)
        {
            if (IsRepeat)
            {
                RepeatIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.RepeatOff;
                IsRepeat = false;
            }
            else
            {
                RepeatIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.Repeat;
                IsRepeat = true;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory;

            string absolutePath = $"{folder}{"/Database/listOfPlaylists.json"}";
            listOfPlaylists.Insert(1, RecentPlayed_P);
            //listOfPlaylists.Insert(0, RecentOpened);
            string listOfPlaylistJson = JsonConvert.SerializeObject(listOfPlaylists, Formatting.Indented);
            File.WriteAllText(absolutePath, listOfPlaylistJson);

            absolutePath = $"{folder}{"/Database/queue.json"}";
            string queueJson = JsonConvert.SerializeObject(QueueList, Formatting.Indented);
            File.WriteAllText(absolutePath, queueJson);

            absolutePath = $"{folder}{"/Database/settings.json"}";
            DataClass.Settings settings = new Settings(Player.Position.Hours, Player.Position.Minutes, Player.Position.Seconds,
                IsShuffle, IsRepeat, CurSongIndex, CurPlaylist.Name, CurSong.AbsolutePath);
            string settingsJson = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(absolutePath, settingsJson);
        }

        private void Minisize_Click(object sender, RoutedEventArgs e)
        {
            if(this.WindowState == WindowState.Normal)
            {
                this.WindowState= WindowState.Minimized;
            }
            else if(this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }
        }



    }   
}
