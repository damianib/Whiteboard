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

        private Mutex mutex = new Mutex();
        private Dictionary<int, BoardElement> allBoardElements = new Dictionary<int, BoardElement>();

        private Dictionary<int, Client> clients = new Dictionary<int, Client>();


        private int actualID = 0;
        private int actualIDClient = 0;

        private String name;

        public CommonWhiteBoard(String name)
        {
            this.name = name;
        }

        public void demarerServeur()
        {
            Thread th = new Thread(new ThreadStart(listen));
            th.Start();
            th.Join();
        }
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
                Client interfaceCl = new Client(client, id, do_add, select, do_deselect, do_delete, do_modif, do_clearAll, do_reset_client);
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
                Client interfaceCl = new Client(client, id, do_add, select, do_deselect, do_delete, do_modif, do_clearAll, do_reset_client);
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
                clients[idClient].send_deselect(idLock);
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
        private void do_deselect(int idClient, int id)
        {
            Monitor.Enter(clients);
            int idLock = clients[idClient].ObjectLocked;
            if (idLock != -1)
            {
                allBoardElements[idLock].m_clientLocker = -1; //Unselect the object
                clients[idClient].ObjectLocked = -1; // Telle the client the object have been unselected
                clients[idClient].send_deselect(idLock);
                clients[idClient].send_add(idLock, allBoardElements[idLock]);
            }

            Monitor.Exit(clients);
        }
        private void do_delete(int idClient, int id)
        {
            Monitor.Enter(clients);
            if (clients[idClient].ObjectLocked == id)
            {
                allBoardElements[id].m_o = "";
                foreach (Client client in clients.Values)
                {
                    client.send_delet(id);
                }
            }
            Monitor.Exit(clients);
        }
        private void do_add(int idClient, Object o)
        {
            
            Monitor.Enter(clients);

            BoardElement b = ObjectConverter.reconvertElement(actualID, (String)o);
            allBoardElements.Add(actualID, b);
            foreach (Client client in clients.Values)
            {
                //client.send_add(actualID, o);
                Console.WriteLine(actualID);
                client.send_add(b.m_id, b.GetString());
            }
            actualID++;
            Monitor.Exit(clients);
        }
        private void do_modif(int idClient, int id, Object o)
        {
            
            Monitor.Enter(clients);
            BoardElement b = ObjectConverter.reconvertElement(id, (String)o);
            if (clients[idClient].ObjectLocked == id)
            {
                
                foreach (Client client in clients.Values)
                {
                    client.send_add(id, b.GetString());
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
            foreach (Client client in clients.Values)
            {
                client.send_clear_all();
            }
            allBoardElements.Clear();
            actualID = 0;
            Monitor.Exit(clients);

        }
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


        private void do_reset_client(int idClient)
        {
            Monitor.Enter(clients);
            clients[idClient].send_clear_all();
            foreach(int key in allBoardElements.Keys)
            {
                clients[idClient].send_add(key, allBoardElements[key].GetString());
            }   
            Monitor.Exit(clients);
        }

        private void establishConnection(Object o)
        {
            
            TcpClient client = (TcpClient) o;
            Monitor.Enter(clients);

            int idClient = actualIDClient;
            actualIDClient++;
            Client interfaceCl = new Client(client, idClient, do_add, select, do_deselect, do_delete, do_modif, do_clearAll, do_reset_client);
            interfaceCl.start();


            clients.Add(idClient, interfaceCl);
            do_reset_client(idClient);
            
            Monitor.Exit(clients);
        }

        private void clear_all()
        {
            
            Monitor.Enter(clients);
            allBoardElements.Clear();
            foreach (Client client in clients.Values)
            {
                client.send_clear_all();
                client.ObjectLocked = -1;
            }

            actualID = 0;
            Monitor.Exit(clients);
        }
    }
}