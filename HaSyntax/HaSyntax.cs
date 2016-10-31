/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;

namespace HaSyntaxLib
{
    public class HaSyntax
    {
        public class HaSyntaxElement
        {
            string element;
            bool flag;

            public HaSyntaxElement(string element)
            {
                this.element = element.ToLower();
                flag = true;
                if (this.element.StartsWith("!"))
                {
                    flag = false;
                    this.element = this.element.Substring(1);
                }
            }

            public string Element
            {
                get
                {
                    return element;
                }
                set
                {
                    element = value.ToLower();
                }
            }

            public bool Flag
            {
                get
                {
                    return flag;
                }
                set
                {
                    flag = value;
                }
            }
        }

        HashSet<string> keys = new HashSet<string>();
        List<List<HaSyntaxElement>> conds = new List<List<HaSyntaxElement>>();

        public HaSyntax(string syntax)
        {
            foreach (string syngroup in syntax.Split(new string[] { " or ", "||", "|" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()))
            {
                List<HaSyntaxElement> elements = new List<HaSyntaxElement>();
                bool negate = false;
                foreach (string word in syngroup.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()))
                {
                    if (word == "not")
                    {
                        negate = true;
                    }
                    else
                    {
                        HaSyntaxElement element = new HaSyntaxElement(word);
                        if (negate)
                        {
                            element.Flag = !element.Flag;
                            negate = false;
                        }
                        keys.Add(element.Element);
                        elements.Add(element);
                    }
                }
                if (elements.Count > 0)
                {
                    conds.Add(elements);
                }
            }
        }

        public bool Validate(IEnumerable<string> tags)
        {
            HashSet<string> tagset = new HashSet<string>(tags.Select(x => x.ToLower()));
            tagset.Add("true");
            tagset.Remove("false");
            foreach (List<HaSyntaxElement> cond in conds)
            {
                bool passed = true;
                foreach (HaSyntaxElement element in cond)
                {
                    bool hastag = tagset.Contains(element.Element);
                    if (element.Flag ^ hastag)
                    {
                        passed = false;
                        break;
                    }
                }
                if (passed)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
