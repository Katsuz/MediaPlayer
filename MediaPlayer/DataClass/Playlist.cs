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

        public int NumberOfSong { get; set; }
        public int NextID { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        //public Song PlayingSong { get; private set; }

        public string Name {get; set;}

        public Playlist(string name)
        {
            this.ListSong = new ObservableCollection<Song>();
            this.Name = name;
            this.NumberOfSong = 0;
            this.NextID = 0;
        }
        public Playlist()
        {
            this.ListSong = new ObservableCollection<Song>();
            this.Name = "new playlist";
            this.NumberOfSong = 0;
            this.NextID = 0;
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
                if ((this.ListSong.Where(item => item.AbsolutePath == file).Count() != 0) && (this.NumberOfSong != 0))
                {
                    Console.WriteLine(this.ListSong.Where(item => item.AbsolutePath == file).Count());
                    continue;
                }
                else
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
                    this.NumberOfSong += 1;
                    this.NextID += 1;
                    int ID = this.NextID;
                    this.AddSong(new Song(ID, name, singer, album, duration, absolutePath, bitmap));
                }
            }
        }

        public bool RemoveSong(Song song)
        {
            this.NumberOfSong -= 1;
            return ListSong.Remove(song);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
