using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayerProject.DataClass
{
    internal class Settings
    {
        public int P_Hours { get; set; }
        public int P_Minutes { get; set;}
        public int P_Seconds { get; set;}
        public bool IsShuffle { get; set; }
        public bool IsRepeat { get; set; }
        public int CurSongIndex { get; set; }
        public string CurPlaylistName { get; set; }
        public string CurSongAbsolutePath { get; set; }
        public Settings() { }
        public Settings(int p_Hours, int p_Minutes, int p_Seconds, bool isShuffle, bool isRepeat, int curSongIndex, string curPlaylistName, string curSongAbsolutePath)
        {
            P_Hours = p_Hours;
            P_Minutes = p_Minutes;
            P_Seconds = p_Seconds;
            IsShuffle = isShuffle;
            IsRepeat = isRepeat;
            CurSongIndex = curSongIndex;
            CurPlaylistName = curPlaylistName;
            CurSongAbsolutePath = curSongAbsolutePath;
        }
    }
}
