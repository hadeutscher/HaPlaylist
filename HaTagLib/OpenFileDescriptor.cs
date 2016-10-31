/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.IO;

namespace HaTagLib
{
    public class OpenFileDescriptor : IDisposable
    {
        private FileStream fs;
        private TagLib.File f;
        
        public OpenFileDescriptor(string path, bool write)
        {
            fs = write ? File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None)
                : File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            f = write ? TagLib.File.Create(new TagLib.StreamFileAbstraction(Path.GetFileName(path), fs, fs))
                : TagLib.File.Create(new TagLib.StreamFileAbstraction(Path.GetFileName(path), fs, null));
        }

        private void SafeClose()
        {
            try
            {
                f.Dispose();
            }
            catch { }
            try
            {
                fs.Dispose();
            }
            catch { }
        }

        public FileStream FileStream
        {
            get
            {
                return fs;
            }
        }

        public TagLib.File TagLibFile
        {
            get
            {
                return f;
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
