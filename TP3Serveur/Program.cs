

using System;
using System.Collections.Generic;
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
            

            
            if (args.Length == 0)
            {

                var host = Dns.GetHostEntry(Dns.GetHostName());
                List<String> ipList = new List<String>();
                foreach(IPAddress ip in host.AddressList)
                {
                    if(ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipList.Add(ip.ToString());
                    }
                    
                }

                for(int i = 0; i< ipList.Count; i++)
                {
                    Console.WriteLine(i + " " + ipList[i]);
                }
            
                Console.WriteLine(ipList.Count + " local adress: 127.0.0.1");
                Console.WriteLine(ipList.Count + 1 + " cancel");
                Console.WriteLine("Select an IP beetween the previous ones");
                int choice = ipList.Count + 2;
                int nbTry = 0;
                while ((choice > ipList.Count || choice < 0) && nbTry < 10)
                {
                    try
                    {
                        choice = Int32.Parse(Console.ReadLine());
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                }
                if(choice < ipList.Count && choice >= 0)
                {
                    Server server = new Server(ipList[choice].ToString());
                    server.listen();
                }
                else if(choice == ipList.Count)
                {
                    Server server = new Server("127.0.0.1");
                    server.listen();
                }
                else
                {
                    Console.WriteLine("Exiting");
                }
            }



        }
       
    }
}
