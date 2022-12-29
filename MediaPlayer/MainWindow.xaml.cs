using MediaPlayer.View;
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

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        // Khởi tạo tài nguyên
        private System.Windows.Controls.Button curBtn;
        

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

        private void OpenFile_Clicked(object sender, RoutedEventArgs e)
        {
            changeCurBtnTo(OpenFileBtn);
            var openFileScreen = new OpenFileDialog();
            openFileScreen.Filter = "Sound Files|*.mp3| Video Files|*.mp4";
            openFileScreen.InitialDirectory = @"C:\";
            openFileScreen.Title = "Please select music to be played.";
            if (openFileScreen.ShowDialog() == true)
            {
                Console.WriteLine(openFileScreen.FileName);
            }


            changeView(new NowPlaying());
            changeCurBtnTo(NowPlayingBtn);
        }
    }   
}
