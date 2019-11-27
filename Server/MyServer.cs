using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace echo
{
    class Program
    {
        //Console app for an echo server
        static void Main()
        {
            //try to make a class to not open console when we run it ?? help
            var serv = new Server();
            serv.ServerSetup();
        }

    }
}