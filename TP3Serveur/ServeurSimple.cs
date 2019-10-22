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
    class ServeurSimple
    {

        private Mutex mutex = new Mutex();
        private List<ObjetAffichable> listObject = new List<ObjetAffichable>();
        private Dictionary<int, BoardElement> allBoardElements = new Dictionary<int, BoardElement>();
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
                clients[idClient].send_deselect(idLock);
                clients[idClient].send_add(idLock, listObject[idLock]);
            }
            
            if(id<listObject.Count && id>= 0)
            {

                if (listObject[id].m_clientLocker == -1)
                {
                    
                    listObject[id].m_clientLocker = idClient;
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
                listObject[idLock].m_clientLocker = -1; //Unselect the object
                clients[idClient].ObjectLocked = -1; // Telle the client the object have been unselected
                clients[idClient].send_deselect(idLock);
                clients[idClient].send_add(idLock, listObject[idLock]);
            }

            Monitor.Exit(clients);
        }
        private void do_delete(int idClient, int id)
        {
            Monitor.Enter(clients);
            if (clients[idClient].ObjectLocked == id)
            {
                listObject[id].m_o = "";
                foreach (Client client in clients)
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
            listObject.Add(new ObjetAffichable(actualID,o));
            allBoardElements.Add(actualID, b);
            foreach (Client client in clients)
            {
                //client.send_add(actualID, o);
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
                
                foreach (Client client in clients)
                {
                    client.send_add(id, b.GetString());
                }
            }
            Monitor.Exit(clients);
        }

        private void do_clearAll(int idClient)
        {
            Monitor.Enter(clients);
            foreach (Client client in clients)
            {
                client.send_clear_all();
        
            }
            allBoardElements.Clear();
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
            
            Client interfaceCl = new Client(client, actualIDClient, do_add, select, do_deselect, do_delete, do_modif, do_clearAll);
            actualIDClient++;


            interfaceCl.start();
            clients.Add(interfaceCl);
            foreach(ObjetAffichable oo in listObject)
            {
                interfaceCl.send_add(oo.m_id, oo.m_o);
            }
            Monitor.Exit(clients);
        }

        private void clear_all()
        {
            
            Monitor.Enter(clients);
            listObject = new List<ObjetAffichable>();
            foreach (Client client in clients)
            {
                client.send_clear_all();
                client.ObjectLocked = -1;
            }

            listObject.Clear();
            actualID = 0;
            Monitor.Exit(clients);
        }
    }
}

