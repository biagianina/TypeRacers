using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public interface IRecievedInformationManager
    {
        Player Player { get; set; }
        IPlayroom Playroom { get; set; }
        void StartCommunication();
    }
}
