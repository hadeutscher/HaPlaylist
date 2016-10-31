/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using HaTagLib;

namespace HaTaggerGUI
{
    public class LoadedFile : PropertyNotifierBase
    {
        private string filename;
        private string tags;

        public LoadedFile(string path)
        {
            filename = path;
            Reload();
        }

        public void Reload()
        {
            using (IHaTagger ht = TaggerFactory.CreateTagger(filename, false))
            {
                Tags = Utils.SafeAggregate(ht.Tags).ToUpper();
            }
        }

        public string Filename
        {
            get
            {
                return filename;
            }
        }
        
        public string Tags
        {
            get
            {
                return tags;
            }
            private set
            {
                SetField(ref tags, value);
            }
        }
    }
}
