using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MediaPlayerProject.DataClass
{
    public class Playlist : INotifyPropertyChanged, ICloneable
    {
        public ObservableCollection<Song> ListSong { get; set; }

        public int NumberOfSong { get => ListSong.Count ; }

        public event PropertyChangedEventHandler PropertyChanged;

        //public Song PlayingSong { get; private set; }

        public string Name {get; set;}

        public Playlist(string name)
        {
            this.ListSong = new ObservableCollection<Song>();
            this.Name = name;
        }
        public Playlist()
        {
            this.ListSong = new ObservableCollection<Song>();
            this.Name = "new playlist";
        }
        public bool AddSong(Song song)
        {
            this.ListSong.Add(song);
            return true;
        }

        public void addSongsToPlaylist(string[] filenames)
        {
            foreach (string file in  filenames)
            {
                TagLib.File songFile = TagLib.File.Create(file);
                String name = songFile.Tag.Title;
                String singer = songFile.Tag.Performers[0];
                String album = songFile.Tag.Album;
                TimeSpan duration = songFile.Properties.Duration;
                String absolutePath = file;

                TagLib.IPicture pic = songFile.Tag.Pictures[0];
                MemoryStream ms = new MemoryStream(pic.Data.Data);
                ms.Seek(0, SeekOrigin.Begin);

                // ImageSource for System.Windows.Controls.Image
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.EndInit();

                this.AddSong(new Song(1, name, singer, album, duration, absolutePath, bitmap));
            }
        }

        public bool RemoveSong(Song song)
        {
            this.ListSong.Remove(song);
            return true;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
