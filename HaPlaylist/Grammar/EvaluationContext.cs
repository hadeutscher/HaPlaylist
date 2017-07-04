using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HaPlaylist.Grammar
{
    public class EvaluationContext
    {
        private ICollection<string> booleanVariables;
        private IDictionary<string, string> valueVariables;
        private IDictionary<string, Func<string, bool>> functionVariables;

        public EvaluationContext(ICollection<string> booleanVariables, IDictionary<string, string> valueVariables, IDictionary<string, Func<string, bool>> functionVariables)
        {
            this.booleanVariables = booleanVariables;
            this.valueVariables = valueVariables;
            this.functionVariables = functionVariables;
        }

        public ICollection<string> BooleanVariables { get => booleanVariables; set => booleanVariables = value; }
        public IDictionary<string, string> ValueVariables { get => valueVariables; set => valueVariables = value; }
        public IDictionary<string, Func<string, bool>> FunctionVariables { get => functionVariables; set => functionVariables = value; }
    }
}