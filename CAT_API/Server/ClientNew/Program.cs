using Options;
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Host
{
    class Program
    {
        static void Main(string[] args)
        {
            StartServer server = new StartServer("127.0.0.1", 12345);
            server.Start();
        }
    }
}
