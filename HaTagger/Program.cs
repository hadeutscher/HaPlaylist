/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using HaSyntaxLib;
using HaTagLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HaTagger
{
    class Program
    {
        static List<HaSyntax.HaSyntaxElement> tag_changes = new List<HaSyntax.HaSyntaxElement>();

        static void Error()
        {
            Console.WriteLine("Usage: HaTagger.exe path [tag0 tag1 tag2 ...]");
            Environment.Exit(0);
        }

        static void TagFile(string path)
        {
            try
            {
                bool ro = tag_changes.Count == 0;
                using (IHaTagger ht = TaggerFactory.CreateTagger(path))
                {
                    if (ro)
                    {
                        // Display file tags
                        Console.WriteLine(path + ": " + Utils.SafeAggregate(ht.Tags).ToUpper());
                    }
                    else
                    {
                        // Apply tag changes
                        foreach (HaSyntax.HaSyntaxElement element in tag_changes)
                        {
                            if (element.Flag)
                            {
                                ht.AddTag(element.Element);
                            }
                            else
                            {
                                ht.RemoveTag(element.Element);
                            }
                        }
                        ht.Save();
                    }
                }
            }
            catch (HaException)
            {
                Console.WriteLine(string.Format("FAILED ({0})", path));
            }
        }

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Error();
            }

            // Parse tags
            string path = args[0];
            tag_changes = args.Skip(1).Select(x => new HaSyntax.HaSyntaxElement(x.ToLower())).ToList();
            if (tag_changes.Any(x => x.Element.Length == 0))
            {
                Error();
            }

            // Operate on either file or recursive listing of directory
            List<string> paths = Utils.ExpandDirectories(new List<string> { path });
            if (paths.Count == 0)
            {
                Error();
            }
            else
            {
                paths.ForEach(x => TagFile(x));
            }
        }
    }
}
