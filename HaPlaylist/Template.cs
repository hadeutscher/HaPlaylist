/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

namespace HaPlaylist
{
    public class Template
    {
        private string name;
        private string value;

        public Template(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public string Name { get => name; set => name = value; }
        public string Value { get => value; set => this.value = value; }
    }
}
