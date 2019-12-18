using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeRacers.Model
{
    class LocalGeneratedText
    {
        //Path for the .txt document
        private static readonly string ExePath = AppDomain.CurrentDomain.BaseDirectory + "Texts.txt";

        //Genertes random lines from the text document
        public static string GetText()
        {
            List<string> lines = GetLines().ToList();
            var random = new Random();
            int randomLine = random.Next(lines.Count);
            return lines[randomLine];
        }

        private static IEnumerable<string> GetLines()
        {
            return File.ReadAllLines(ExePath);
        }
    }
}
