/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using HaTagLib;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HaTaggerGUI
{
    public class Data : PropertyNotifierBase
    {
        private HashSet<string> loadedPaths = new HashSet<string>();

        public Data()
        {
        }

        public void AddFiles(IEnumerable<string> paths, int index)
        {
            paths = Utils.ExpandDirectories(paths);
            foreach (string path in paths)
            {
                string lowerPath = path.ToLower();
                if (!loadedPaths.Contains(lowerPath))
                {
                    try
                    {
                        LoadedFile f = new LoadedFile(path);
                        LoadedFiles.Insert(index++, f);
                        loadedPaths.Add(lowerPath);
                    }
                    catch (HaException) { }
                }
            }
        }

        public void RemoveFiles(IEnumerable<LoadedFile> files)
        {
            foreach (LoadedFile f in files)
            {
                LoadedFiles.Remove(f);
                loadedPaths.Remove(f.Filename.ToLower());
            }
        }

        private ObservableCollection<LoadedFile> _loadedFiles = null;
        public ObservableCollection<LoadedFile> LoadedFiles
        {
            get
            {
                return _loadedFiles ?? (_loadedFiles = new ObservableCollection<LoadedFile>());
            }
            set
            {
                SetField(ref _loadedFiles, value);
            }
        }

        private ListDropTarget _dropHandler = null;
        public ListDropTarget DropHandler
        {
            get
            {
                return _dropHandler ?? (_dropHandler = new ListDropTarget(this));
            }
            set
            {
                SetField(ref _dropHandler, value);
            }
        }
    }
}
