/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System.IO;

namespace HaTagLib
{
    public static class TaggerFactory
    {
        public static IHaTagger CreateTagger(string path, bool write = true)
        {
            string ext = Path.GetExtension(path).ToLower();
            if (ext == ".flac")
            {
                return new FlacTagger(path, write);
            }
            else if (ext == ".mp3")
            {
                return new Mp3Tagger(path, write);
            }
            else
            {
                throw new HaException("Unable to create tagger for extension " + ext);
            }
        }
    }
}
