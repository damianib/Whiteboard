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
    class ServeurSimple
    {

        private Mutex mutex = new Mutex();
        private List<ObjetAffichable> listObject = new List<ObjetAffichable>();
        private List<Client> clients = new List<Client>();


        private int actualID = 0;
        private int actualIDClient = 0;

        public void demarerServeur()
        {
            Thread th = new Thread(new ThreadStart(listen));
            th.Start();
            th.Join();
        }
        private void select(int idClient, int id)
        {
            Monitor.Enter(clients);
            int idLock = clients[idClient].ObjectLocked;
            if(idLock != -1)
            {
                listObject[idLock].m_clientLocker = -1; //Unselect the object
                clients[idClient].ObjectLocked = -1; // Telle the client the object have been unselected
                clients[idClient].deselect(idLock);
                clients[idClient].modif(idLock, listObject[idLock]);
            }
            
            if(id<listObject.Count && id>= 0)
            {

                if (listObject[id].m_clientLocker == -1)
                {
                    
                    listObject[id].m_clientLocker = idClient;
                    clients[idClient].ObjectLocked = id;
                    clients[idClient].select(id);


                }
            }
            Monitor.Exit(clients);
            
        }
        private void deselect(int idClient, int id)
        {
            Monitor.Enter(clients);
            int idLock = clients[idClient].ObjectLocked;
            if (idLock != -1)
            {
                listObject[idLock].m_clientLocker = -1; //Unselect the object
                clients[idClient].ObjectLocked = -1; // Telle the client the object have been unselected
                clients[idClient].deselect(idLock);
                clients[idClient].modif(idLock, listObject[idLock]);
            }

            Monitor.Exit(clients);
        }
        private void delete(int idClient, int id)
        {
            Monitor.Enter(clients);
            if (clients[idClient].ObjectLocked == id)
            {
                listObject[id].m_o = "";
                foreach (Client client in clients)
                {
                    client.delete(id);
                }
                actualID++;
            }
            Monitor.Exit(clients);
            display();
        }
        private void add(int idClient, Object o)
        {
            
            Monitor.Enter(clients);
            listObject.Add(new ObjetAffichable(actualID,o));
            foreach (Client client in clients)
            {
                client.add(actualID, o);
            }
            actualID++;
            Monitor.Exit(clients);
            display();
        }
        private void modif(int idClient, int id, Object o)
        {
            
            Monitor.Enter(clients);
            if (clients[idClient].ObjectLocked == id)
            {
                
                foreach (Client client in clients)
                {
                    client.modif(id, o);
                }
            }
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

                Thread th = new Thread(new ParameterizedThreadStart(establishConnection));
                th.Start(client);

            }
        }

        private void establishConnection(Object o)
        {
            
            TcpClient client = (TcpClient) o;
            Monitor.Enter(clients);
            
            Client interfaceCl = new Client(client, actualIDClient, add, select, deselect, delete, modif);
            actualIDClient++;


            interfaceCl.start();
            clients.Add(interfaceCl);
            Monitor.Exit(clients);
        }

        private void display()
        {
            foreach(ObjetAffichable o in listObject)
            {
                Console.WriteLine((String)o.m_o);
            }
        }
    }
}