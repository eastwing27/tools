using System.Collections.Generic;

namespace Eastwing.Tools.Parser
{
    public static class Extenders
    {
        public static IEnumerable<Token> Parse(this string Source, Analyzer Parser)
        {
            return Parser.Parse(Source);
        }

        public static IEnumerable<Token> Parse(this string Source)
        {
            return Source.Parse(new Analyzer());
        }
    }
}
