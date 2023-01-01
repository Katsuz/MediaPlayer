using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MediaPlayerProject.DataClass
{
    public class Song : INotifyPropertyChanged
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Singer { get; set; }
        public string Album { get; set; }
        public DateTime DateAdded { get; set; }
        public TimeSpan Duration { get; set; }
        public string AbsolutePath {get; set; }
        public BitmapImage Thumnail { get; set; }

        public string IsMp3 { get; set; }
        public string IsMp4 { get; set; }

        public Song(int iD, string name, string singer, string album, TimeSpan duration, string absolutePath, BitmapImage thumnail)
        {
            ID = iD;
            Name = name;
            Singer = singer;
            Album = album;
            DateAdded = new DateTime();
            Duration = duration;
            AbsolutePath = absolutePath;
            if (absolutePath.EndsWith(".mp3"))
            {
                IsMp3 = "Visible";
                IsMp4 = "Collapsed";
            } 
            else if (absolutePath.EndsWith(".mp4"))
            {
                IsMp4 = "Visible";
                IsMp3 = "Collapsed";
            }
            this.Thumnail = thumnail;
        }

        public Song(string name)
        {
            Name = name;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
