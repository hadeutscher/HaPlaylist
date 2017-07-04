using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HaPlaylist.Grammar
{
    public class StringVisitor : HaGrammarBaseVisitor<string>
    {
        public override string VisitString([NotNull] HaGrammarParser.StringContext context)
        {
            if (context.GetToken(HaGrammarLexer.ID, 0) != null)
            {
                return context.GetToken(HaGrammarLexer.ID, 0).GetText();
            }
            else
            {
                var text = context.GetToken(HaGrammarLexer.TEXT, 0).GetText();
                return text.Substring(1, text.Length - 2);
            }
        }
    }

    public class ExpressionEvaluator : HaGrammarBaseVisitor<BooleanExpression>
    {
        public override BooleanExpression VisitBooleanExpression([NotNull] HaGrammarParser.BooleanExpressionContext context)
        {
            if (context.functionExpression() != null)
            {
                return Visit(context.functionExpression());
            }
            else if (context.GetToken(HaGrammarLexer.AND, 0) != null)
            {
                if (context.booleanExpression().Length == 1)
                {
                    // This can happen when AND is signed alone because the expression has ended (and we need an exit path from the rule)

                    return Visit(context.booleanExpression().First());
                }
                return new AndBooleanExpression(context.booleanExpression().Select(x => Visit(x)));
            }
            else if (context.GetToken(HaGrammarLexer.OR, 0) != null)
            {
                return new OrBooleanExpression(context.booleanExpression().Select(x => Visit(x)));
            }
            else if (context.GetToken(HaGrammarLexer.NOT, 0) != null)
            {
                return new NotBooleanExpression(Visit(context.booleanExpression().First()));
            }
            else if (context.GetToken(HaGrammarLexer.OPEN, 0) != null)
            {
                return Visit(context.booleanExpression().First());
            }
            else if (context.GetToken(HaGrammarLexer.TRUE, 0) != null)
            {
                return new BooleanBooleanExpression(true);
            }
            else
            {
                return new SimpleBooleanExpression(context.GetToken(HaGrammarLexer.ID, 0).GetText());
            }
        }

        public override BooleanExpression VisitFunctionExpression([NotNull] HaGrammarParser.FunctionExpressionContext context)
        {
            var function = context.GetToken(HaGrammarLexer.ID, 0).GetText();
            var argument = new StringVisitor().Visit(context.@string());
            return new FunctionBooleanExpression(function, argument);
        }
    }

    public class QueryEvaluator : HaGrammarBaseVisitor<IEnumerable<Song>>
    {
        SongLibrary library;

        public QueryEvaluator(SongLibrary library)
        {
            this.library = library;
        }

        public override IEnumerable<Song> VisitQuery([NotNull] HaGrammarParser.QueryContext context)
        {
            return Visit(context.sequenceExpression());
        }

        public override IEnumerable<Song> VisitSequenceExpression([NotNull] HaGrammarParser.SequenceExpressionContext context)
        {
            return context.ratioExpression().Select(x => Visit(x)).Aggregate((x, y) => x.Concat(y));
        }

        public override IEnumerable<Song> VisitRatioExpression([NotNull] HaGrammarParser.RatioExpressionContext context)
        {
            var expressions = context.booleanExpression();
            var evaluator = new ExpressionEvaluator();
            if (expressions.Length == 1)
            {
                var parsed_expression = evaluator.Visit(expressions.First());
                return library.RandomSelect(parsed_expression);
            }
            else
            {
                var song_lists = new List<List<Song>>();
                var weights = new List<int>();
                double min_unit_size = double.MaxValue;
                for (int i = 0; i < expressions.Length; i++)
                {
                    BooleanExpression parsed_expression = evaluator.Visit(expressions[i]);
                    var list = library.RandomSelect(parsed_expression).ToList();
                    song_lists.Add(list);

                    int weight = int.Parse(context.GetToken(HaGrammarLexer.NUMBER, i).GetText());
                    weights.Add(weight);

                    double unit_size = list.Count / (double)weight;
                    if (unit_size < min_unit_size)
                        min_unit_size = unit_size;
                }
                var result_song_lists = new List<IEnumerable<Song>>();
                for (int i = 0; i < expressions.Length; i++)
                {
                    int songs_amount = (int)Math.Floor(weights[i] * min_unit_size);
                    result_song_lists.Add(song_lists[i].Take(songs_amount));
                }
                return SongLibrary.Shuffle(new HashSet<Song>(result_song_lists.Aggregate((x, y) => x.Concat(y))));
            }
        }
    }
}
