using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eastwing.Tools.Parser
{
    public class Analyzer
    {
        public IEnumerable<string> Keywords { get; set; } = new List<string>().AsEnumerable();

        public string Letters { get; set; } = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюяABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_";
        public string Digits { get; set; } = "0123456789";
        public string Quotes { get; set; } = "\"'";
        public string Separators { get; set; } = "!@#$%^&?|`~№;:";
        public string Brackets { get; set; } = "()<>{}[]";
        public char RadixPoint { get; set; } = '.';

        private static Category[] SetCategories = new Category[] { Category.String, Category.Integer, Category.Real, Category.Word, Category.Keyword};

        private Token OutToken(ref Token token, object lexContainer, Category cat)
        {
            string lexeme = lexContainer.ToString();
            if (cat == Category.Word && Keywords.Contains(lexeme))
                cat = Category.Keyword;

            return token.Reinit(lexeme, cat);
        }

        public IEnumerable<Token> Parse (string Text)
        {
            var cToken = new Token("", Category.Void);
            var sbLex = new StringBuilder();
            var curCat = new Category();
            var stringOpener = default(char);


            for (int i = 0; i < Text.Length; i++)
            {
                if (curCat == Category.String && !(Quotes.Contains(Text[i]) && Text[i] == stringOpener))
                    sbLex.Append(Text[i]);
                else
                    switch (Text[i])
                    {
                        case ' ': case '\t':
                            yield return OutToken(ref cToken, sbLex, curCat);
                            yield return cToken.Reinit(Text[i].ToString(), Category.Space);
                            curCat = Category.Void;
                            sbLex.Clear();
                            break;

                        case '\n': case '\r':
                            yield return OutToken(ref cToken, sbLex, curCat);
                            curCat = Category.Void;
                            sbLex.Clear();
                            break;

                        case char c when (Letters.Contains(c)):
                            if (curCat != Category.Word)
                                curCat = Category.Word;
                            sbLex.Append(c);
                            break;

                        case char c when (Digits.Contains(c)):
                            if (curCat != Category.Integer && curCat != Category.Real && curCat != Category.Word)
                                curCat = Category.Integer;
                            sbLex.Append(c);
                            break;

                        case char c when (c == RadixPoint):
                            if (i + 1 != Text.Length)
                            {
                                if (curCat == Category.Integer)
                                {
                                    if (Digits.Contains(Text[i + 1]))
                                    {
                                        curCat = Category.Real;
                                        sbLex.Append(c);
                                    }
                                    else
                                    {
                                        if (curCat != Category.Void)
                                            yield return OutToken(ref cToken, sbLex, curCat);
                                        yield return OutToken(ref cToken, c, Category.Separator);
                                        curCat = Category.Void;
                                        sbLex.Clear();
                                    }
                                }
                                else
                                {
                                    if (curCat != Category.Void)
                                        yield return OutToken(ref cToken, sbLex, curCat);
                                    yield return OutToken(ref cToken, c, Category.Separator);
                                    curCat = Category.Void;
                                    sbLex.Clear();
                                }
                            }
                            break;

                        case char c when (Quotes.Contains(c)):
                            if (curCat != Category.String)
                            {
                                stringOpener = c;
                                curCat = Category.String;
                                yield return cToken.Reinit(c.ToString(), Category.Quote);
                            }
                            else
                            {
                                if (c == stringOpener)
                                {
                                    yield return cToken.Reinit(sbLex.ToString(), Category.String);
                                    yield return cToken.Reinit(c.ToString(), Category.Quote);
                                    curCat = Category.Void;
                                    sbLex.Clear();
                                }
                                else
                                    sbLex.Append(c);
                            }
                            break;

                        case char c when (Separators.Contains(c)):
                            if (curCat != Category.Void)
                                yield return OutToken(ref cToken, sbLex, curCat);
                            yield return OutToken(ref cToken, Text[i], Category.Separator);
                            curCat = Category.Void;
                            sbLex.Clear();
                            break;

                        case char c when (Brackets.Contains(c)):
                            if (curCat != Category.Void)
                                yield return OutToken(ref cToken, sbLex, curCat);
                            yield return OutToken(ref cToken, Text[i], Category.Bracket);
                            curCat = Category.Void;
                            sbLex.Clear();
                            break;

                        case '=':
                            if (curCat != Category.Void)
                                yield return OutToken(ref cToken, sbLex, curCat);
                            yield return OutToken(ref cToken, Text[i], Category.Equals);
                            curCat = Category.Void;
                            sbLex.Clear();
                            break;

                        case '+':
                            if (curCat != Category.Void)
                                yield return OutToken(ref cToken, sbLex, curCat);
                            yield return OutToken(ref cToken, Text[i], Category.Asterisk);
                            curCat = Category.Void;
                            sbLex.Clear();
                            break;

                        case '/':
                            if (curCat != Category.Void)
                                yield return OutToken(ref cToken, sbLex, curCat);
                            yield return OutToken(ref cToken, Text[i], Category.Slash);
                            curCat = Category.Void;
                            sbLex.Clear();
                            break;

                        case '\\':
                            if (curCat != Category.Void)
                                yield return OutToken(ref cToken, sbLex, curCat);
                            yield return OutToken(ref cToken, Text[i], Category.Backslash);
                            curCat = Category.Void;
                            sbLex.Clear();
                            break;

                        default:
                            if (curCat != Category.Void)
                                yield return OutToken(ref cToken, sbLex, curCat);
                            yield return OutToken(ref cToken, Text[i], Category.Unknown);
                            curCat = Category.Void;
                            sbLex.Clear();
                            break;
                    }

                if (i == Text.Length - 1)
                {
                    if (SetCategories.Contains(curCat) && curCat != Category.Void)
                        yield return OutToken(ref cToken, sbLex, curCat);
                }
            }
        }
    }
}
