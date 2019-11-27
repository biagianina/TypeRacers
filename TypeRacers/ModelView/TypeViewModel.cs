using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeRacers.Model;
using Client;

namespace TypeRacers.ModelView
{
    internal class TypeViewModel
    {
        //creates an instance of the model with the text received from the server
        public TypeViewModel()
        {           
            Model = new TypeModel(new Client.Client().GetMessageFromServer());
        }

        public TypeModel Model { get; }
    }
}
