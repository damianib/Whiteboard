

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
        
        /// <summary>
        /// Main program of the server part
        /// It takes cares of laucnining the server
        /// 
        /// </summary>
        /// <param name="args">Get as an input the IP that will be the server IP, if no argument is passed, the programm will look for available IPV4 adress, and ask the user to choose beetween them</param>
        private static void Main(string[] args)
        {
            //If we have no argument, we will ask the user to choose an IP adress
            if (args.Length == 0)
            {

                var host = Dns.GetHostEntry(Dns.GetHostName()); //Will get all the host Entry
                List<String> ipList = new List<String>(); //Will containt the available IPv4 adresses

                //Ge through all available IP adress, and check the ones compatibles with IPv4
                foreach(IPAddress ip in host.AddressList)
                {
                    
                    if(ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipList.Add(ip.ToString());
                    }
                    
                }

                //Print all the available, with a number associated
                for(int i = 0; i< ipList.Count; i++)
                {
                    Console.WriteLine(i + " " + ipList[i]);
                }
            
                //Posibility to launch the server locally
                Console.WriteLine(ipList.Count + " local adress: 127.0.0.1");

                //Possibility to cancel
                Console.WriteLine(ipList.Count + 1 + " cancel");



                Console.WriteLine("Select an IP beetween the previous ones");
                int choice = ipList.Count + 2;
                int nbTry = 0;

                //Get the user choice
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
                if(choice < ipList.Count && choice >= 0) //If the user choosed an available IP
                {
                    Server server = new Server(ipList[choice].ToString());
                    server.listen();
                }
                else if(choice == ipList.Count) //If the user choosed to launch the server locally
                {
                    Server server = new Server("127.0.0.1");
                    server.listen();
                }
                else
                {
                    Console.WriteLine("Exiting");
                }
            }
            else
            {
                //args[0] contain the user desired IP
                Server server = new Server(args[0]);
                server.listen();
            }


        }
       
    }
}
