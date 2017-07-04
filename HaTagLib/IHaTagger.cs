/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;

namespace HaTagLib
{
    public interface IHaTagger : IDisposable
    {
        IEnumerable<string> Tags { get; }
        IDictionary<string, string> ValueTags { get; }
        void AddTag(string tag);
        void RemoveTag(string tag);
        void Save();
    }
}
