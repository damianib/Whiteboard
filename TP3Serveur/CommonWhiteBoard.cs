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
    class CommonWhiteBoard
    {
        /// <summary>
        /// Dictionnary that contains all the element on the board, indexed by they IDs
        /// </summary>
        private Dictionary<int, BoardElement> allBoardElements = new Dictionary<int, BoardElement>();

        /// <summary>
        /// Dictionnary that contains all the client that are interacting with this board, indexed by their IDs
        /// </summary>
        private Dictionary<int, ClientInterface> clients = new Dictionary<int, ClientInterface>();

        /// <summary>
        /// actual minimal id not atributed to any object
        /// </summary>
        private int actualID = 0;

        /// <summary>
        /// actual minimal id not atributed to any client
        /// </summary>
        private int actualIDClient = 0;

        /// <summary>
        /// Name of the whiteboard
        /// </summary>
        private String name;

        /// <summary>
        /// Create a whiteboard
        /// </summary>
        /// <param name="name">Whiteboard's name</param>
        public CommonWhiteBoard(String name)
        {
            this.name = name;
        }

        /*
        public void demarerServeur()
        {
            Thread th = new Thread(new ThreadStart(listen));
            th.Start();
            th.Join();
        }*/
        public bool hasClient(int id)
        {
            return clients.ContainsKey(id);
        }
        public void startConnexion(TcpClient client, int idClient = -1)
        {
            
            Monitor.Enter(clients);
            int id = idClient;
            if (id == -1)
            {
                id = actualIDClient;
                actualIDClient++;
            }
            if (!clients.ContainsKey(id))
            {
                ClientInterface interfaceCl = new ClientInterface(client, id, do_add, select, do_deselect, do_delete, do_modif, do_clearAll, do_reset_client);
                clients.Add(id, interfaceCl);
            }
            
            clients[id].start();
            do_reset_client(id);
            Monitor.Exit(clients);
            
        }

        public void startConnexion(TcpClient client, int idClient, String nameWB)
        {

            Monitor.Enter(clients);
            int id = idClient;
            if (id == -1)
            {
                id = actualIDClient;
                actualIDClient++;
            }
            if (!clients.ContainsKey(id))
            {
                ClientInterface interfaceCl = new ClientInterface(client, id, do_add, select, do_deselect, do_delete, do_modif, do_clearAll, do_reset_client);
                clients.Add(id, interfaceCl);
            }
            clients[id].start();
            do_reset_client(id);
            Monitor.Exit(clients);

        }

        private void select(int idClient, int id)
        {
            Monitor.Enter(clients);
            int idLock = clients[idClient].ObjectLocked;
            if(idLock != -1)
            {
                allBoardElements[idLock].m_clientLocker = -1; //Unselect the object
                clients[idClient].ObjectLocked = -1; // Telle the client the object have been unselected
                clients[idClient].send_deselect();
                clients[idClient].send_add(idLock, allBoardElements[idLock]);
            }
            
            if(allBoardElements.ContainsKey(id))
            {

                if (allBoardElements[id].m_clientLocker == -1)
                {

                    allBoardElements[id].m_clientLocker = idClient;
                    clients[idClient].ObjectLocked = id;
                    clients[idClient].send_select(id);


                }
            }
            Monitor.Exit(clients);
            
        }
        private void do_deselect(int idClient)
        {
            Monitor.Enter(clients);
            int idLock = clients[idClient].ObjectLocked;
            if (idLock != -1)
            {
                allBoardElements[idLock].m_clientLocker = -1; //Unselect the object
                clients[idClient].ObjectLocked = -1; // Telle the client the object have been unselected
                clients[idClient].send_deselect();
                clients[idClient].send_add(idLock, allBoardElements[idLock]);
            }

            Monitor.Exit(clients);
        }
        private void do_delete(int idClient, int id)
        {
            Monitor.Enter(clients);
            if (clients[idClient].ObjectLocked == id)
            {
                //allBoardElements[id].m_o = "";
                allBoardElements.Remove(id);
                foreach (ClientInterface client in clients.Values)
                {
                    client.send_delet(id);
                }
            }
            Monitor.Exit(clients);
        }
        private void do_add(int idClient, String str)
        {
            
            Monitor.Enter(clients);

            BoardElement b = ObjectConverter.reconvertElement(actualID, str);
            allBoardElements.Add(actualID, b);
            foreach (ClientInterface client in clients.Values)
            {
                //client.send_add(actualID, o);
                Console.WriteLine(actualID);
                client.send_add(b.m_id, b);
            }
            actualID++;
            Monitor.Exit(clients);
        }
        private void do_modif(int idClient, int id, String str)
        {
            
            Monitor.Enter(clients);
            BoardElement b = ObjectConverter.reconvertElement(id, str);
            if (clients[idClient].ObjectLocked == id)
            {
                
                foreach (ClientInterface client in clients.Values)
                {
                    client.send_add(id, b);
                }
            }
            else
            {
                do_reset_client(idClient);
            }
            Monitor.Exit(clients);
        }

        private void do_clearAll(int idClient)
        {
            Monitor.Enter(clients);
            foreach (ClientInterface client in clients.Values)
            {
                client.send_clear_all();
            }
            allBoardElements.Clear();
            actualID = 0;
            Monitor.Exit(clients);

        }

        /*
        private void listen()
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

                Thread th = new Thread((Object o)=>{ establishConnection(o); } );//new ParameterizedThreadStart(establishConnection));
                
                th.Start(client);

            }
        }
        */

        private void do_reset_client(int idClient)
        {
            Monitor.Enter(clients);
            clients[idClient].send_clear_all();
            foreach(int key in allBoardElements.Keys)
            {
                clients[idClient].send_add(key, allBoardElements[key]);
            }   
            Monitor.Exit(clients);
        }

        private void establishConnection(Object o)
        {
            
            TcpClient client = (TcpClient) o;
            Monitor.Enter(clients);

            int idClient = actualIDClient;
            actualIDClient++;
            ClientInterface interfaceCl = new ClientInterface(client, idClient, do_add, select, do_deselect, do_delete, do_modif, do_clearAll, do_reset_client);
            interfaceCl.start();


            clients.Add(idClient, interfaceCl);
            do_reset_client(idClient);
            
            Monitor.Exit(clients);
        }

        private void clear_all()
        {
            
            Monitor.Enter(clients);
            allBoardElements.Clear();
            foreach (ClientInterface client in clients.Values)
            {
                client.send_clear_all();
                client.ObjectLocked = -1;
            }

            actualID = 0;
            Monitor.Exit(clients);
        }
    }
}