//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Eastwing.Tools.Parser;
//using System.IO;
//using System.Diagnostics;

//namespace Eastwing.Tools
//{
//    class Program
//    {
//        public static void Main(string[] args)
//        {
//            var parser = new Analyzer();
//            var str = File.ReadAllText("test.txt");

//            var s = new Stopwatch();
//            s.Start();

//            var p = new Analyzer()
//            {
//                Letters = "ФБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫБЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыбэюя",
//                Brackets = "",
//                Digits = ""
//            };

//            var result = str.Parse(p);

//            Console.WriteLine(s.Elapsed.TotalMilliseconds);
//            s.Reset();

//            Console.WriteLine(result.Count());
//            //foreach (var lexeme in result)
//            //{
//            //    Console.WriteLine(lexeme.Full);
//            //}

//            Console.Read();
//        }
//    }
//}
