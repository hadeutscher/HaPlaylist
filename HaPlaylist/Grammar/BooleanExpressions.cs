using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HaPlaylist.Grammar
{
    public interface BooleanExpression
    {
        bool Evaluate(EvaluationContext context);
    }

    public class BooleanBooleanExpression : BooleanExpression
    {
        private bool value;

        public BooleanBooleanExpression(bool value)
        {
            this.value = value;
        }

        public bool Evaluate(EvaluationContext context)
        {
            return value;
        }
    }

    public class SimpleBooleanExpression : BooleanExpression
    {
        private string value;

        public SimpleBooleanExpression(string value)
        {
            this.value = value;
        }

        public bool Evaluate(EvaluationContext context)
        {
            return context.BooleanVariables.Contains(value);
        }
    }

    public class NotBooleanExpression : BooleanExpression
    {
        private BooleanExpression value;

        public NotBooleanExpression(BooleanExpression value)
        {
            this.value = value;
        }

        public bool Evaluate(EvaluationContext context)
        {
            return !value.Evaluate(context);
        }
    }

    public class AndBooleanExpression : BooleanExpression
    {
        private IEnumerable<BooleanExpression> values;

        public AndBooleanExpression(IEnumerable<BooleanExpression> values)
        {
            this.values = values;
        }

        public bool Evaluate(EvaluationContext context)
        {
            return values.All(x => x.Evaluate(context));
        }
    }

    public class OrBooleanExpression : BooleanExpression
    {
        private IEnumerable<BooleanExpression> values;

        public OrBooleanExpression(IEnumerable<BooleanExpression> values)
        {
            this.values = values;
        }

        public bool Evaluate(EvaluationContext context)
        {
            return values.Any(x => x.Evaluate(context));
        }
    }

    public class FunctionBooleanExpression : BooleanExpression
    {
        private string function;
        private string argument;

        public FunctionBooleanExpression(string function, string argument)
        {
            this.function = function;
            this.argument = argument;
        }

        public bool Evaluate(EvaluationContext context)
        {
            string value;
            Func<string, bool> valueFunc;
            if (context.ValueVariables.TryGetValue(function, out value))
            {
                // Optimize for static values
                return value.Contains(argument);
            }
            else if (context.FunctionVariables.TryGetValue(function, out valueFunc))
            {
                return valueFunc(argument);
            }
            else
            {
                return false;
            }
        }
    }
}
