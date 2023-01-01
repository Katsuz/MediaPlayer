using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MediaPlayerProject.DataClass
{
    public class Song : INotifyPropertyChanged
    {
        [JsonProperty("iD")]
        public int ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("singer")]
        public string Singer { get; set; }

        [JsonProperty("album")]
        public string Album { get; set; }

        [JsonProperty("dateAdded")]
        public DateTime DateAdded { get; set; }

        [JsonProperty("duration")]
        public TimeSpan Duration { get; set; }

        [JsonProperty("absolutePath")]
        public string AbsolutePath {get; set; }


        public BitmapImage Thumnail { get; set; }

        [JsonProperty("isMP3")]
        public string IsMp3 { get; set; }

        [JsonProperty("isMP4")]
        public string IsMp4 { get; set; }

        [JsonConstructor]
        public Song(int iD, string name, string singer, string album, string dateAdded, string duration, string absolutePath, string thumnail)
        {
            ID = iD;
            Name = name;
            Singer = singer;
            Album = album;
            DateAdded = DateTime.Parse(dateAdded);
            Duration = TimeSpan.Parse(duration);
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
            TagLib.File songFile = TagLib.File.Create(absolutePath);
            BitmapImage bitmap = new BitmapImage();
            if (songFile.Tag.Pictures.Length != 0)
            {
                TagLib.IPicture pic = songFile.Tag.Pictures[0];
                MemoryStream ms = new MemoryStream(pic.Data.Data);
                ms.Seek(0, SeekOrigin.Begin);

                // ImageSource for System.Windows.Controls.Image
                bitmap.BeginInit();
                bitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bitmap.UriSource = null;
                bitmap.StreamSource = ms;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
            }
            else
            {
                bitmap = new BitmapImage(new Uri("/Images/default_thumbnail.png", UriKind.Relative));
            }
            this.Thumnail = bitmap;
        }

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
