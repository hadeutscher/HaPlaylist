using System;
using System.Collections.Generic;
using System.Text;

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
