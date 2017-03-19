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

        public IEnumerable<Token> Parse(string Text)
        {
            State = States.Starting;

            //Some initialization
            var result = new List<Token>();
            var openingQuote = default(char);
            var builder = new StringBuilder();
            var newLine = Environment.NewLine;

            void pushLexeme()
            {
                result[result.Count-1].Lexeme = builder.ToString();
                builder.Clear();
            }

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
                                result.Add(new Token("", TokenCategories.Word));
                                State = States.BuildingWord;
                                builder.Append(c);
                                break;

                            case char c when (c == ' ' || c == '\t'):
                                result.Add(new Token(c, TokenCategories.Space));
                                break;

                            case char c when (Digits.Contains(c)):
                                result.Add(new Token("", TokenCategories.Integer));
                                State = States.BuildingNumber;
                                builder.Append(c);
                                break;

                            case char c when (Separators.Contains(c)):
                                result.Add(new Token(c, TokenCategories.Separator));
                                break;

                            case char c when (Brackets.Contains(c)):
                                result.Add(new Token(c, TokenCategories.Bracket));
                                break;

                            case char c when (Quotes.Contains(c)):
                                result.Add(new Token(c, TokenCategories.Quote));
                                openingQuote = c;
                                result.Add(new Token("", TokenCategories.String));
                                State = States.BuildingLine;
                                break;

                            case char c when (c == RadixPoint):
                                if (i < Text.Length && Digits.Contains(Text[i + 1]))
                                {
                                    result.Add(new Token("", TokenCategories.Real));
                                    State = States.BuildingNumber;
                                    builder.Append(c);
                                    break;
                                }
                                result.Add(new Token(c, TokenCategories.Separator));
                                break;

                            case char c when (c == '\n' || c == '\r'):
                                break;

                            case '-':
                                result.Add(new Token("-", TokenCategories.Minus));
                                break;

                            case '+':
                                result.Add(new Token("+", TokenCategories.Plus));
                                break;

                            case '*':
                                result.Add(new Token("*", TokenCategories.Asterisk));
                                if (i > 0 && Text[i - 1] == '/')
                                {
                                    result.Add(new Token("", TokenCategories.Comment));
                                    State = States.BuildingLines;
                                }
                                break;

                            case '/':
                                result.Add(new Token("/", TokenCategories.Slash));
                                if (i > 0 && Text[i - 1] == '/')
                                {
                                    result.Add(new Token("", TokenCategories.Comment));
                                    State = States.BuildingLine;
                                }
                                break;

                            case '\\':
                                result.Add(new Token("\\", TokenCategories.Backslash));
                                break;

                            default:
                                result.Add(new Token(chr, TokenCategories.Unknown));
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
                                    pushLexeme();
                                    result.Add(new Token("", result[result.Count - 1].Category));
                                }
                                break;

                            case '*':
                                var last = Text.Length - 1;
                                if (i < last && Text[i + 1] == '/')
                                {
                                    pushLexeme();
                                    State = States.Adding;
                                    result.Add(new Token("*", TokenCategories.Asterisk));
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
                                pushLexeme();
                                State = States.Adding;
                                break;

                            case char c when (Quotes.Contains(c)):
                                if (result[result.Count - 1].Category == TokenCategories.String && c == openingQuote)
                                {
                                    pushLexeme();
                                    State = States.Adding;
                                    result.Add(new Token(c, TokenCategories.Quote));
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
                                result[result.Count - 1].Category = TokenCategories.Word;
                                break;

                            case char c when (c == RadixPoint):
                                builder.Append(c);
                                result[result.Count - 1].Category = TokenCategories.Real;
                                break;

                            default:
                                pushLexeme();
                                State = States.Adding;
                                result.Add(GetToken(chr));
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
                                pushLexeme();
                                State = States.Adding;
                                break;

                            default:
                                pushLexeme();
                                State = States.Adding;
                                result.Add(GetToken(chr));
                                break;
                        }
                        #endregion
                        break;
                }
            }
            State = States.Ending;
            //Some final operations
            State = States.Done;
            return result.AsEnumerable();
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
