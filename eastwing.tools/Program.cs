using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eastwing.Tools.Parser;

namespace Eastwing.Tools
{
    class Program
    {
        public static void Main(string[] args)
        {
            var parser = new Analyzer();
            var str = Console.ReadLine();

            var result = parser.Parse(str);

            Console.WriteLine(result.Count());
            foreach (var lexeme in result)
            {
                Console.WriteLine(lexeme.Full);
            }

            Console.Read();
        }
    }
}
