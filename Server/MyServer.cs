using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TypeRacers.Server
{
    class Program
    {
        //Console app for an echo server
        static void Main()
        { 
            //new instance of server and starting 
            var serv = new Server();
            serv.ServerSetup();
        }
    }
}