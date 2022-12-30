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
        private int ID { get; set; }
        private string Name { get; set; }
        private string Singer { get; set; }
        private string Album { get; set; }
        private TimeSpan Duration { get; set; }
        private string AbsolutePath {get; set; }
        private BitmapImage thumnail { get; set; }

        public Song(int iD, string name, string singer, string album, TimeSpan duration, string absolutePath, BitmapImage thumnail)
        {
            ID = iD;
            Name = name;
            Singer = singer;
            Album = album;
            Duration = duration;
            AbsolutePath = absolutePath;
            this.thumnail = thumnail;
        }

        public Song(string name)
        {
            Name = name;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
