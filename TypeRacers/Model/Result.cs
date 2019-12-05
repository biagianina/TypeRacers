using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeRacers.Model
{
    public class Result
    {
        public string BookName { get; set; }

        public string AuthorName { get; set; }

        public int PlayerSpeed { get; set; }

        public int PlayerTime { get; set; }

        public int Accuracy { get; set; }

        public List<string> Mistakes;
    }
}
