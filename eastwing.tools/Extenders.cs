using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eastwing.Tools.Parser;

namespace Eastwing.Tools
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
