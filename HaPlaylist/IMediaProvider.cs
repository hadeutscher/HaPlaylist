using HaPlaylist.Grammar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HaPlaylist
{
    public interface IMediaProvider
    {
        SongLibrary LoadLibrary();
        void LaunchPowerAMP(IEnumerable<string> playlist);
        void LaunchMediaPlayer(IEnumerable<string> playlist);
    }
}
