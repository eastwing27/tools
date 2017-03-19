using System.Collections.Generic;

namespace Eastwing.Tools.Parser
{
    public static class ExtentionMethods
    {
        public static IEnumerable<Token> Parse(this string Source, ParseMachine Parser)
        {
            return Parser.Parse(Source);
        }

        public static IEnumerable<Token> Parse(this string Source)
        {
            return Source.Parse(new ParseMachine());
        }
    }
}
