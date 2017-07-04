/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;

namespace HaTagLib
{
    public class FlacTagger : IHaTagger
    {
        private SortedSet<string> tags = new SortedSet<string>();
        private Dictionary<string, string> valueTags = new Dictionary<string, string>();
        private OpenFileDescriptor f = null;
        private TagLib.Ogg.XiphComment t = null;
        private bool dirty = false;

        public FlacTagger(string path, bool write = true)
        {
            f = Utils.OpenFile(path, write);
            t = (TagLib.Ogg.XiphComment)f.TagLibFile.GetTag(TagLib.TagTypes.Xiph, true);
            if (t == null)
            {
                SafeClose();
                throw new HaException("FLAC file has no Xiph tag");
            }
            foreach (string tagName in t)
            {
                valueTags.Add(tagName.ToLower(), t.GetFirstField(tagName).ToLower());
            }
            string data = t.GetFirstField("hatag");
            if (data != null)
            {
                data.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(x => tags.Add(x));
            }
        }

        private void SafeClose()
        {
            try
            {
                f.Dispose();
            }
            catch { }
        }

        public IEnumerable<string> Tags
        {
            get => tags;
        }

        public IDictionary<string, string> ValueTags
        {
            get => valueTags;
        }

        public void AddTag(string tag)
        {
            if (tags.Add(tag))
            {
                dirty = true;
            }
        }

        public void RemoveTag(string tag)
        {
            if (tags.Remove(tag))
            {
                dirty = true;
            }
        }

        public void Save()
        {
            if (dirty)
            {
                if (tags.Any(x => x.Contains(",") || x.Length == 0))
                {
                    throw new HaException("Invalid tags inserted");
                }
                t.SetField("hatag", Utils.SafeAggregate(tags));
                f.TagLibFile.Save();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    SafeClose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
