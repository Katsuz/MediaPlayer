using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayerProject.DataClass
{
    public class Playlist : INotifyPropertyChanged, ICloneable
    {
        private readonly List<Song> _playlist;

        public event PropertyChangedEventHandler PropertyChanged;

        //public Song PlayingSong { get; private set; }

        public string Name {get; set;}

        public Playlist(string name)
        {
            this._playlist = new List<Song>();
            this.Name = name;
        }
        public Playlist()
        {
            this._playlist = new List<Song>();
            this.Name = "new playlist";
        }
        public bool AddSong(Song song)
        {
            this._playlist.Add(song);
            return true;
        }

        public bool RemoveSong(Song song)
        {
            this._playlist.Remove(song);
            return true;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
