using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MediaPlayerProject.DataClass
{
    public class Playlist : INotifyPropertyChanged, ICloneable
    {
        [JsonProperty("listSong")]
        public ObservableCollection<Song> ListSong { get; set; }

        [JsonProperty("numberOfSong")]
        public int NumberOfSong { get; set; }

        [JsonProperty("nextID")] 
        public int NextID { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        //public Song PlayingSong { get; private set; }

        [JsonProperty("name")]
        public string Name {get; set;}

        public BitmapImage CoverImage { get; set; }
        public string Visibility { get; set; }

        [JsonConstructor]
        public Playlist(ObservableCollection<Song> listSong, int numberOfSong, int nextID, string name, string coverImage, string visibility)
        {
            ListSong = listSong;
            NumberOfSong = numberOfSong;
            NextID = nextID;
            Name = name;
            if (ListSong.Count != 0) { CoverImage = listSong[0].Thumnail; }
            else { CoverImage = new BitmapImage(new Uri("/Images/default_thumbnail.png", UriKind.Relative)); }
            Visibility = visibility;
        }

        public Playlist(string name)
        {
            this.ListSong = new ObservableCollection<Song>();
            this.Visibility = "Visible";
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
                    continue;
                }
                else
                {
                    TagLib.File songFile = TagLib.File.Create(file);

                    String name = "Unknown Song";
                    if (songFile.Tag.Title != null)
                    {
                        name = songFile.Tag.Title;
                    }

                    String singer = "Unknown Singer";
                    if (songFile.Tag.Performers.Length != 0)
                    {
                        singer = songFile.Tag.Performers[0];
                    }    
                    
                    String album = "Unknown Album";
                    if (songFile.Tag.Album != null)
                    {
                        album = songFile.Tag.Album;
                    }

                    TimeSpan duration = new TimeSpan();
                    if (songFile.Properties.Duration != null)
                    {
                        duration = songFile.Properties.Duration;
                    }

                    String absolutePath = file;


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

                    if (this.NumberOfSong == 0)
                    {
                        this.CoverImage = bitmap;
                    }

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
