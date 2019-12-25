using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeRacers.ViewModel
{
    public class FirstViewModel
    {
        public string Username
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                Model.Model.NameClient(value);
            }
        }
    }
}
