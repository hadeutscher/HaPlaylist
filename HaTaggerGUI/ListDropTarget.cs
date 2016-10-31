/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using GongSolutions.Wpf.DragDrop;
using System.Windows;

namespace HaTaggerGUI
{
    public class ListDropTarget : IDropTarget
    {
        private Data parent;

        public ListDropTarget(Data parent)
        {
            this.parent = parent;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is DataObject && ((DataObject)dropInfo.Data).GetDataPresent(DataFormats.FileDrop))
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        public int GetAfterFromDropInfo(IDropInfo dropInfo)
        {
            if ((dropInfo.InsertPosition & RelativeInsertPosition.AfterTargetItem) != 0)
            {
                if (dropInfo.TargetItem == null)
                    return parent.LoadedFiles.Count;
                else
                    return parent.LoadedFiles.IndexOf((LoadedFile)dropInfo.TargetItem) + 1;
            }
            else if ((dropInfo.InsertPosition & RelativeInsertPosition.BeforeTargetItem) != 0)
            {
                if (dropInfo.TargetItem == null)
                    return 0;
                else
                    return parent.LoadedFiles.IndexOf((LoadedFile)dropInfo.TargetItem);
            }
            else
            {
                return 0;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is DataObject && ((DataObject)dropInfo.Data).GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])((DataObject)dropInfo.Data).GetData(DataFormats.FileDrop);
                /*/
                List<string> files_list = files.ToList();
                files_list.Sort();
                files = files_list.ToArray();
                //*/
                parent.AddFiles(files, GetAfterFromDropInfo(dropInfo));
            }
        }
    }
}
