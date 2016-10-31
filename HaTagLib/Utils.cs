/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HaTagLib
{
    public static class Utils
    {
        public static OpenFileDescriptor OpenFile(string path, bool write = false)
        {
            return new OpenFileDescriptor(path, write);
        }

        public static TagLib.ByteVector GetBytes(string text)
        {
            byte[] buf = Encoding.ASCII.GetBytes(text);
            return new TagLib.ByteVector(buf, buf.Length);
        }

        public static string SafeAggregate(IEnumerable<string> source)
        {
            if (source.Count() == 0)
            {
                return "";
            }
            else
            {
                return source.Aggregate((x, y) => x + "," + y);
            }
        }

        public static List<string> ExpandDirectories(IEnumerable<string> file_list)
        {
            List<string> result = new List<string>();
            foreach (string path in file_list)
            {
                if (Directory.Exists(path))
                {
                    ExpandDirectoriesRecursive(new DirectoryInfo(path), result);
                }
                else if (File.Exists(path))
                {
                    result.Add(path);
                }
            }
            return result;
        }

        private static void ExpandDirectoriesRecursive(DirectoryInfo curr_dir, List<string> output_list)
        {
            foreach (DirectoryInfo subdir in curr_dir.EnumerateDirectories())
            {
                ExpandDirectoriesRecursive(subdir, output_list);
            }
            foreach (FileInfo file in curr_dir.EnumerateFiles())
            {
                output_list.Add(file.FullName);
            }
        }
    }
}
