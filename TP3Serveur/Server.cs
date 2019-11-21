﻿using System;
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
    class Server
    {
        Dictionary<String, CommonWhiteBoard> whiteBoards = new Dictionary<String, CommonWhiteBoard>();

        public void addWB(String nom)
        {
            CommonWhiteBoard wb = new CommonWhiteBoard(nom);
            whiteBoards.Add(nom, wb);
        }
        public void listen()
        {
            Console.WriteLine("Préparation à l'écoute...");

            //On crée le serveur en lui spécifiant le port sur lequel il devra écouter.
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            TcpListener server = new TcpListener(localAddr, 5035);

            server.Start();
            while (true)
            {
                Console.Write("Waiting for a connection... ");
                TcpClient client = server.AcceptTcpClient();
                Thread th = new Thread(() => { establishConnection(client); });
                th.Start();
            }

        }

        public void establishConnection(TcpClient client)
        {
            try { 
                //TcpClient client = (TcpClient)o;
                NetworkStream stream = client.GetStream();
                int i;
                byte[] bytes = new byte[2048];
                if ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    string name = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
                    String[] infos = name.Split(' ');
                    String instruction = infos[0];
                    String nomWhiteBoard = infos[1];
                    int idConnection = Convert.ToInt32(infos[2]);

                    if (instruction.Equals("connect"))
                    {
                        
                        if (whiteBoards.ContainsKey(nomWhiteBoard))
                        {
                            //bytes = System.Text.Encoding.UTF8.GetBytes(Convert.ToString(idConnection));
                            //stream.Write(bytes, 0, bytes.Length);
                            whiteBoards[nomWhiteBoard].startConnexion(client, idConnection);

                            
                        }
                        else
                        {
                            client.Close();
                        }
                    }
                    else if (instruction.Equals("init"))
                    {
                        
                        Monitor.Enter(whiteBoards);
                        if (!whiteBoards.ContainsKey(nomWhiteBoard))
                        {
                            whiteBoards.Add(nomWhiteBoard, new CommonWhiteBoard(nomWhiteBoard));
                            Monitor.Exit(whiteBoards);
                            whiteBoards[nomWhiteBoard].startConnexion(client, idConnection, nomWhiteBoard);
                            
                        }
                        else
                        {
                            Monitor.Exit(whiteBoards);
                            client.Close();
                        }
                    }
                    else if (instruction.Equals("initNoName"))
                    {
                        bool found = false;
                        while( !found)
                        {
                            nomWhiteBoard = getRandomString(20);
                            Monitor.Enter(whiteBoards);
                            if (!whiteBoards.ContainsKey(nomWhiteBoard))
                            {
                                whiteBoards.Add(nomWhiteBoard, new CommonWhiteBoard(nomWhiteBoard));
                                Monitor.Exit(whiteBoards);
                                whiteBoards[nomWhiteBoard].startConnexion(client, idConnection, "");
                                found = true;
                            }
                            else
                            {
                                Monitor.Exit(whiteBoards);
                            }
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
