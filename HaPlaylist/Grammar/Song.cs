using System;
using System.Collections.Generic;
using System.Text;

namespace HaPlaylist.Grammar
{
    public class Song
    {
        private string path;
        private EvaluationContext evaluationContext;

        public Song(string path, EvaluationContext evaluationContext)
        {
            this.path = path;
            this.evaluationContext = evaluationContext;
        }

        public string Path { get => path; set => path = value; }
        public EvaluationContext EvaluationContext { get => evaluationContext; set => evaluationContext = value; }
    }
}
