using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace echo
{
    public class ContestText
    {
        //Path for the .txt document
        private readonly string ExePath = AppDomain.CurrentDomain.BaseDirectory + "Texts.txt";

        //Genertes random lines from the text document
        public string GetText()
        {
            List<string> lines = GetLines().ToList();
            var random = new Random();
            int randomLine = random.Next(lines.Count);
            return lines[randomLine];
        }

        private IEnumerable<string> GetLines()
        {
            return File.ReadAllLines(ExePath);
        }
    }
}