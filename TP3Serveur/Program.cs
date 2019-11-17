

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPServeur
{
    public class Program
    {
        

        private static void Main(string[] args)
        {
            Console.WriteLine("CECI EST LE SERVEUR");
            //CommonWhiteBoard server = new CommonWhiteBoard();
            //server.demarerServeur();
            Server server = new Server();
            server.addWB("coucou");
            server.listen();
        }
       
    }
}
