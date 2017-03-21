using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eastwing.Tools.Parser
{
    public partial class ParseMachine
    {
        public States State { get; private set; } = States.New;

        //State groups
        States[] idle = new[] { States.New, States.Done };
        public bool IsWorking  => !idle.Contains(State);

        public IEnumerable<string> Keywords { get; set; } = new string[0];
        public string Letters { get; set; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_";
        public string Digits { get; set; } = "0123456789ABCDEFabcdefXx";
        public string Quotes { get; set; } = "\"'";
        public string Separators { get; set; } = "!@#$%^&?|`~№;:,";
        public string Brackets { get; set; } = "()<>{}[]";
        public char RadixPoint { get; set; } = '.';

        public static ParseMachine Fast => new ParseMachine()
        {
            Letters = "",
            Digits = "",
            Separators = "!?,.:;",
            Brackets = "()"
        };

        public IEnumerable<Token> Parse(string Text)
        {
            State = States.Starting;

            //Some initialization
            var result = new List<Token>();
            var openingQuote = default(char);
            var builder = new StringBuilder();
            var newLine = Environment.NewLine;
            var category = default(TokenCategories);
            var worker = new Token("", TokenCategories.Unknown);

            //void pushLexeme()
            //{
            //    result[result.Count-1].Lexeme = builder.ToString();
            //    builder.Clear();
            //}

            //Working
            State = States.Adding;
            for (int i = 0; i < Text.Length; i++)
            {
                var chr = Text[i];
                //var token = result.LastOrDefault();

                switch (State)
                {
                    case States.Adding:
                        #region add
                        switch (chr)
                        {
                            case char c when (Letters.Contains(c)):
                                category = TokenCategories.Word;
                                State = States.BuildingWord;
                                builder.Append(c);
                                break;

                            case char c when (c == ' ' || c == '\t'):
                                yield return (worker.Reinit(c, TokenCategories.Space));
                                break;

                            case char c when (Digits.Contains(c)):
                                category = TokenCategories.Integer;
                                State = States.BuildingNumber;
                                builder.Append(c);
                                break;

                            case char c when (Separators.Contains(c)):
                                yield return (worker.Reinit(c, TokenCategories.Separator));
                                break;

                            case char c when (Brackets.Contains(c)):
                                yield return (worker.Reinit(c, TokenCategories.Bracket));
                                break;

                            case char c when (Quotes.Contains(c)):
                                yield return (worker.Reinit(c, TokenCategories.Quote));
                                openingQuote = c;
                                category = TokenCategories.String;
                                State = States.BuildingLine;
                                break;

                            case char c when (c == RadixPoint):
                                if (i < Text.Length && Digits.Contains(Text[i + 1]))
                                {
                                    category = TokenCategories.Real;
                                    State = States.BuildingNumber;
                                    builder.Append(c);
                                    break;
                                }
                                yield return (worker.Reinit(c, TokenCategories.Separator));
                                break;

                            case char c when (c == '\n' || c == '\r'):
                                break;

                            case '-':
                                yield return (worker.Reinit("-", TokenCategories.Minus));
                                break;

                            case '+':
                                yield return (worker.Reinit("+", TokenCategories.Plus));
                                break;

                            case '=':
                                yield return (worker.Reinit("=", TokenCategories.Equals));
                                break;

                            case '*':
                                yield return (worker.Reinit("*", TokenCategories.Asterisk));
                                if (i > 0 && Text[i - 1] == '/')
                                {
                                    category = TokenCategories.Comment;
                                    State = States.BuildingLines;
                                }
                                break;

                            case '/':
                                yield return (worker.Reinit("/", TokenCategories.Slash));
                                if (i > 0 && Text[i - 1] == '/')
                                {
                                    category = TokenCategories.Comment;
                                    State = States.BuildingLine;
                                }
                                break;

                            case '\\':
                                yield return (worker.Reinit("\\", TokenCategories.Backslash));
                                break;

                            default:
                                //yield return (worker.Reinit(chr, TokenCategories.Unknown));
                                category = TokenCategories.Unknown;
                                State = States.BuildingUnknown;
                                builder.Append(chr);
                                break;
                        }
                        #endregion
                        break;

                    case States.BuildingLines:
                        #region build lines

                        switch (chr)
                        {
                            case char c when (newLine.Contains(c)):
                                if (i > 0 && !newLine.Contains(Text[i - 1]))
                                {
                                    yield return worker.Reinit(builder,category);
                                    builder.Clear();
                                    //result.Add(new Token("", result[result.Count - 1].Category));
                                }
                                break;

                            case '*':
                                var last = Text.Length - 1;
                                if (i < last && Text[i + 1] == '/')
                                {
                                    yield return worker.Reinit(builder, category);
                                    yield return(worker.Reinit("*", TokenCategories.Asterisk));
                                    builder.Clear();
                                    State = States.Adding;
                                    break;
                                }
                                builder.Append('*');
                                break;

                            default:
                                builder.Append(chr);
                                break;
                        }
                        #endregion
                        break;

                    case States.BuildingLine:
                        #region build line
                        switch (chr)
                        {
                            case char c when (newLine.Contains(c)):
                                yield return worker.Reinit(builder, category);
                                builder.Clear();
                                State = States.Adding;
                                break;

                            case char c when (Quotes.Contains(c)):
                                if (category == TokenCategories.String && c == openingQuote)
                                {
                                    yield return worker.Reinit(builder, category);
                                    yield return worker.Reinit(c, TokenCategories.Quote);
                                    builder.Clear();
                                    State = States.Adding;
                                    break;
                                }
                                builder.Append(c);
                                break;

                            default:
                                builder.Append(chr);
                                break;
                        }
                        #endregion
                        break;

                    case States.BuildingNumber:
                        #region build number
                        switch (chr)
                        {
                            case char c when (Digits.Contains(c)):
                                builder.Append(c);
                                break;

                            case char c when (Letters.Contains(c)):
                                builder.Append(c);
                                category = TokenCategories.Word;
                                break;

                            case char c when (c == RadixPoint):
                                builder.Append(c);
                                category = TokenCategories.Real;
                                break;

                            case char c when (Environment.NewLine.Contains(c)):
                                yield return worker.Reinit(builder, category);
                                builder.Clear();
                                State = States.Adding;
                                break;

                            default:
                                yield return worker.Reinit(builder, category);
                                yield return (GetToken(chr));
                                builder.Clear();
                                State = States.Adding;
                                break;
                        }
                        #endregion
                        break;

                    case States.BuildingWord:
                        #region build word
                        switch (chr)
                        {
                            case char c when (Letters.Contains(c) || Digits.Contains(c)):
                                builder.Append(c);
                                break;

                            case char c when (Environment.NewLine.Contains(c)):
                                if (Keywords.Contains(builder.ToString()))
                                    category = TokenCategories.Keyword;
                                yield return worker.Reinit(builder, category);
                                builder.Clear();
                                State = States.Adding;
                                break;

                            default:
                                if (Keywords.Contains(builder.ToString()))
                                    category = TokenCategories.Keyword;
                                yield return worker.Reinit(builder, category);
                                yield return (GetToken(chr));
                                builder.Clear();
                                State = States.Adding;
                                break;
                        }
                        #endregion
                        break;

                    case States.BuildingUnknown:
                        switch (chr)
                        {
                            case char c when (Separators.Contains(c) ||
                                              Brackets.Contains(c)   ||
                                              Quotes.Contains(c)     ||
                                              c == RadixPoint        ||
                                              c == ' ' || c == '\t'):
                                if (Keywords.Contains(builder.ToString()))
                                    category = TokenCategories.Keyword;
                                yield return worker.Reinit(builder, category);
                                yield return GetToken(c);
                                builder.Clear();
                                State = States.Adding;
                                break;

                            case char c when (newLine.Contains(c)):
                                if (Keywords.Contains(builder.ToString()))
                                    category = TokenCategories.Keyword;
                                yield return worker.Reinit(builder, category);
                                builder.Clear();
                                State = States.Adding;
                                break;

                            default:
                                builder.Append(chr);
                                break;
                        }
                        break;
                }
            }
            State = States.Done;
        }

        public Token GetToken(char Source)
        {
            switch (Source)
            {
                case char c when (Letters.Contains(c)):
                    return new Token(c, TokenCategories.Word);

                case char c when (Digits.Contains(c)):
                    return new Token(c, TokenCategories.Integer);

                case char c when (Brackets.Contains(c)):
                    return new Token(c, TokenCategories.Bracket);

                case char c when (Quotes.Contains(c)):
                    return new Token(c, TokenCategories.Quote);

                case char c when (Separators.Contains(c) || c == RadixPoint):
                    return new Token(c, TokenCategories.Separator);

                case char c when (c == ' ' || c == '\t'):
                    return new Token(c, TokenCategories.Space);

                case '=':
                    return new Token("=", TokenCategories.Equals);

                case '+':
                    return new Token("+", TokenCategories.Plus);

                case '-':
                    return new Token("-", TokenCategories.Minus);

                case '*':
                    return new Token("*", TokenCategories.Asterisk);

                case '/':
                    return new Token("/", TokenCategories.Slash);

                case '\\':
                    return new Token("\\", TokenCategories.Backslash);

                default:
                    return new Token(Source, TokenCategories.Unknown);
            }
        }
    }
}
