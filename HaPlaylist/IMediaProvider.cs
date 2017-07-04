/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using HaPlaylist.Grammar;
using System.Collections.Generic;

namespace HaPlaylist
{
    public interface IMediaProvider
    {
        SongLibrary LoadLibrary();
        void LaunchPowerAMP(IEnumerable<string> playlist);
        void LaunchMediaPlayer(IEnumerable<string> playlist);
    }
}
