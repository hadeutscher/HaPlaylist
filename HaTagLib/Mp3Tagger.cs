/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;

namespace HaTagLib
{
    public class Mp3Tagger : IHaTagger
    {
        private SortedSet<string> tags = new SortedSet<string>();
        private Dictionary<string, string> valueTags = new Dictionary<string, string>();
        private OpenFileDescriptor f = null;
        private TagLib.Id3v2.Tag t = null;
        private TagLib.Id3v2.UserTextInformationFrame utif;
        private bool dirty = false;

        public Mp3Tagger(string path, bool write = true)
        {
            f = Utils.OpenFile(path, write);
            t = (TagLib.Id3v2.Tag)f.TagLibFile.GetTag(TagLib.TagTypes.Id3v2, true);
            if (t == null)
            {
                SafeClose();
                throw new HaException("MP3 file has no Id3v2 tag");
            }

            // ID3 is lame as hell, doesnt actually have textual description for tags (unlike beast-ass Xiph)
            valueTags.Add("album", FixString(t.Album));
            valueTags.Add("albumartist", FixString(Aggregate(t.AlbumArtists)));
            valueTags.Add("artist", FixString(Aggregate(t.Performers)));
            valueTags.Add("year", t.Year.ToString());

            IEnumerable<TagLib.Id3v2.UserTextInformationFrame> frames = t.GetFrames<TagLib.Id3v2.UserTextInformationFrame>().Where(x => x.Description.ToLower() == "hatag");
            if (frames.Count() > 0)
            {
                utif = frames.ElementAt(0);
                string data = utif.Text.Length == 0 ? "" : utif.Text[0];
                data.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(x => tags.Add(x));
            }
            else
            {
                utif = new TagLib.Id3v2.UserTextInformationFrame("hatag");
                utif.TextEncoding = TagLib.StringType.Latin1;
                t.AddFrame(utif);
            }
        }

        private string Aggregate(string[] source)
        {
            if (source == null || source.Length == 0)
                return "";
            else
                return source.Aggregate((x, y) => x + "," + y);
        }

        private string FixString(string source)
        {
            if (source == null)
                return "";
            else
                return source.ToLower();
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
                utif.Text = new string[] { Utils.SafeAggregate(tags) };
                f.TagLibFile.Save();
                dirty = false;
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
