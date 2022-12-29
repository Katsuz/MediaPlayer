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
        private void OpenFile_Clicked(object sender, RoutedEventArgs e)
        {
            changeCurBtnTo(OpenFileBtn);
            var openFileScreen = new OpenFileDialog();
            openFileScreen.Filter = "Sound Files|*.mp3| Video Files|*.mp4";
            openFileScreen.InitialDirectory = @"C:\";
            openFileScreen.Title = "Please select music to be played.";
            if (openFileScreen.ShowDialog() == true)
            {
                Player.Open(new Uri(openFileScreen.FileName,UriKind.Absolute));
                Player.Play();
                Timer = new DispatcherTimer();
                Timer.Interval = new TimeSpan(0, 0, 0, 1, 0); ;
                Timer.Tick += Timer_Tick;
                Timer.Start();
            }
            changeView(new Home());
            changeCurBtnTo(NowPlayingBtn);
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            PauseBtn.Visibility = Visibility.Collapsed;
            PlayBtn.Visibility = Visibility.Visible;
            Player.Pause();
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            PlayBtn.Visibility = Visibility.Collapsed;
            PauseBtn.Visibility = Visibility.Visible;
            Player.Play();
        }
    }   
}
