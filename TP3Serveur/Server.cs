using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections;

namespace TCPServeur
{
    /// <summary>
    /// This class represent the server
    /// </summary>
    class Server
    {

        /// <summary>
        /// Containe 
        /// </summary>
        Dictionary<String, CommonWhiteBoard> whiteBoards = new Dictionary<String, CommonWhiteBoard>();
        String ip = "127.0.0.1";

        /// <summary>
        /// Create a server
        /// </summary>
        /// <param name="ip">ip of the server</param>
        public Server(String ip)
        {
            this.ip = ip;
        }

        /// <summary>
        /// Listen for client to connect
        /// </summary>
        public void listen()
        {
            //On crée le serveur en lui spécifiant le port sur lequel il devra écouter.
            IPAddress localAddr = IPAddress.Parse(ip);
            TcpListener server = new TcpListener(localAddr, 5035);
            server.Start();
            while (true)
            {
                Console.Write("Waiting for a connection... ");
                TcpClient client = server.AcceptTcpClient();
                //Establish the connexion on a new thread (we do not want the server to freeze while, for example, waiting for a long user)
                Thread th = new Thread(() => { establishConnection(client); });
                th.Start();
            }
        }

        /// <summary>
        /// Establish a connexion beetween the client and the server
        /// </summary>
        /// <param name="client"></param>
        public void establishConnection(TcpClient client)
        {
            
            try { 
                //TcpClient client = (TcpClient)o;
                NetworkStream stream = client.GetStream();
                int i;
                byte[] bytes = new byte[2048];

                //Read what the user want
                //The client send something like "instrucion desiredName desiredID"
                //Desired ID is not yet used, but in further implementation, it would allow the user to reconnect with the same session
                if ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    string name = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
                    String[] infos = name.Split(' ');
                    String instruction = infos[0]; //Get the instruction
                    String nomWhiteBoard = infos[1];
                    int idConnection = Convert.ToInt32(infos[2]);
                    if (instruction.Equals("connect")) //Client want to connect to an existing server
                    {

                        if (whiteBoards.ContainsKey(nomWhiteBoard)) //If the server exits
                        {
                            whiteBoards[nomWhiteBoard].startConnexion(client, idConnection);
                        }
                        else
                        { 
                            string error = "ERRNo such whiteboard : " + nomWhiteBoard;
                            byte[] fBytes = System.Text.Encoding.UTF8.GetBytes(error.Length.ToString() +" "+ error); //Send the error to the client
                            client.GetStream().Write(fBytes, 0, fBytes.Length);
                            client.Close();
                        }
                    }
                    else if (instruction.Equals("init")) //Client want to launch a server
                    {
                        
                        Monitor.Enter(whiteBoards); //We want that only one client at a time can create a whiteboard
                        if (!whiteBoards.ContainsKey(nomWhiteBoard))
                        {
                            Console.WriteLine(nomWhiteBoard + "created");
                            whiteBoards.Add(nomWhiteBoard, new CommonWhiteBoard(nomWhiteBoard));
                            Monitor.Exit(whiteBoards);
                            whiteBoards[nomWhiteBoard].startConnexion(client, idConnection);
                            
                        }
                        else
                        {
                            
                            client.Close();
                        }
                        Monitor.Exit(whiteBoards);
                    }
                    else if (instruction.Equals("initNoName")) //CLient want to launch a server with a random name
                    {
                        
                        bool found = false;
                        while( !found) //We keep on generating random names, until we find an unused one
                        {
                            nomWhiteBoard = getRandomString(20); //Generate a random string
                            Monitor.Enter(whiteBoards);
                            if (!whiteBoards.ContainsKey(nomWhiteBoard))
                            {
                                whiteBoards.Add(nomWhiteBoard, new CommonWhiteBoard(nomWhiteBoard));
                                Monitor.Exit(whiteBoards);
                                whiteBoards[nomWhiteBoard].startConnexion(client, idConnection);
                                found = true;
                                
                            }
                            else
                            {
                                Monitor.Exit(whiteBoards);
                            }
                        }
                        
                    }
                    else if (instruction.Equals("createOrJoin")) //User want to create a whiteboard with the given name, or to join if it already exits
                    {
                        Monitor.Enter(whiteBoards);
                        if (!whiteBoards.ContainsKey(nomWhiteBoard)) //If the board does not exits, we create it
                        {
                            whiteBoards.Add(nomWhiteBoard, new CommonWhiteBoard(nomWhiteBoard));
                            Monitor.Exit(whiteBoards);
                            whiteBoards[nomWhiteBoard].startConnexion(client, idConnection);

                        }
                        else //If the board exists, we join it
                        {
                            Monitor.Exit(whiteBoards);
                            whiteBoards[nomWhiteBoard].startConnexion(client, idConnection);
                        }
                    }

                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Monitor.Exit(whiteBoards);
            }
        }

        /// <summary>
        /// Return a random alphanumeric string
        /// </summary>
        /// <param name="size">Size of the string</param>
        /// <returns></returns>
        private String getRandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i<size; i++)
            {
                builder.Append(Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65))));
            }
            return builder.ToString();
        }
    }
}
