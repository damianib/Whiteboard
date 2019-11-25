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

        /// <summary>
        /// Ask if a client existes
        /// </summary>
        /// <param name="id">Id of the client</param>
        /// <returns>true if the client with of id "id" exists</returns>
        public bool hasClient(int id)
        {
            return clients.ContainsKey(id);
        }

        /// <summary>
        /// Establish connexion with a client
        /// </summary>
        /// <param name="client">TCP connexion with the client</param>
        /// <param name="idClient">ID assigned to the client (-1 means the whiteboard has to attribute the id)</param>
        public void startConnexion(TcpClient client, int idClient = -1)
        {
            
            //We do not want to client to access to the "clients" list at the same time
            Monitor.Enter(clients);
            int id = idClient; //Id that will be assigned to the client
            if (id == -1) //If no specific ID is asked, we assign a new ID to the client
            {
                id = actualIDClient;
                actualIDClient++;
            }
            if (!clients.ContainsKey(id))//If the client does not already exists, we add it to the board
            {
                ClientInterface interfaceCl = new ClientInterface(client, id, this);
                clients.Add(id, interfaceCl);
            }
            
            //Starting the client
            clients[id].start();

            

            //Seting up all initial inforamtions
            do_reset_client(id);

            //Other client can access the "clients" list
            Monitor.Exit(clients);
            
        }
        
        /// <summary>
        /// Client select an object
        /// </summary>
        /// <param name="idClient">Client issuing the request</param>
        /// <param name="id">Object to be selected</param>
        public void do_select(int idClient, int id)
        {
            //One client at a time accesses "clients"
            Monitor.Enter(clients);

            //Actual object locked
            int idLock = clients[idClient].ObjectLocked;

            if(idLock != -1)//If the client is already locking an object
            {
                allBoardElements[idLock].m_clientLocker = -1; //Set the object to free
                clients[idClient].ObjectLocked = -1; //Tell the client it's not selecting an object anymore
                clients[idClient].send_deselect(); // Tell the client the object have been unselected
                clients[idClient].send_add(idLock, allBoardElements[idLock]); //If the client was trying to modify it's selected object
                                                                               //We ensure it has an up-to-date object by sending the actual version
            }
            
            if(allBoardElements.ContainsKey(id)) //If the object does exists
            {

                if (allBoardElements[id].m_clientLocker == -1) //If no one else is lockig the object
                {

                    allBoardElements[id].m_clientLocker = idClient; //The object is selected by the client
                    clients[idClient].ObjectLocked = id; //The client is selectig the object
                    clients[idClient].send_select(id); //Tell the client it has selected the object


                }
            }
            Monitor.Exit(clients);
            
        }

        /// <summary>
        /// Client deselect an object
        /// </summary>
        /// <param name="idClient"></param>
        public void do_deselect(int idClient)
        {
            //One client at a time access "clients"
            Monitor.Enter(clients);

            int idLock = clients[idClient].ObjectLocked;
            if (idLock != -1)//If we actually have a selected object
            {
                allBoardElements[idLock].m_clientLocker = -1; //Set the object to free
                clients[idClient].ObjectLocked = -1; // Telle the client the object have been unselected
                clients[idClient].send_deselect(); // Tell the client the object have been unselected
                clients[idClient].send_add(idLock, allBoardElements[idLock]);//If the client was trying to modify it's selected object
                                                                             //We ensure it has an up-to-date object by sending the actual version
            }

            Monitor.Exit(clients);
        }

        /// <summary>
        /// Client delete an object
        /// </summary>
        /// <param name="idClient">Id of the client</param>
        /// <param name="id">Object to be deleted</param>
        public void do_delete(int idClient, int id)
        {

            //One client at a time access "clients"
            Monitor.Enter(clients);
            if (clients[idClient].ObjectLocked == id) //If the client has a lock on the object
            {
                allBoardElements.Remove(id); //We remove the object
                clients[idClient].ObjectLocked = -1;
                foreach (ClientInterface client in clients.Values)
                {
                    //We tell all client the object has been deleted
                    client.send_delet(id);
                }
            }
            Monitor.Exit(clients);
        }

        /// <summary>
        /// Client add an object
        /// </summary>
        /// <param name="idClient">Id of the client</param>
        /// <param name="str">String representing the object</param>
        public void do_add(int idClient, String str)
        {
            //One client at a time access "clients"
            Monitor.Enter(clients);

            //Creating an object with id acutalID from string str
            BoardElement b = ObjectConverter.reconvertElement(actualID, str);

            //Adding the object
            allBoardElements.Add(actualID, b);
            foreach (ClientInterface client in clients.Values)
            {
                //telling all clients to add the object
                client.send_add(b.m_id, b);
            }

            //The minimum unatributed ID is increasing
            actualID++;
            Monitor.Exit(clients);
        }

        /// <summary>
        /// Client want to modify an object
        /// </summary>
        /// <param name="idClient">Id of the client</param>
        /// <param name="id">Id of the object to be modified</param>
        /// <param name="str">String representation of the new version of the object</param>
        public void do_modif(int idClient, int id, String str)
        {
            //One client at a time access "clients"
            Monitor.Enter(clients);

            //Cerating the new version of the object from str
            BoardElement b = ObjectConverter.reconvertElement(id, str);

            if (clients[idClient].ObjectLocked == id) //If the client has the lock on the object
            {
                allBoardElements[id] = b;
                foreach (ClientInterface client in clients.Values)
                {
                    //We update the value for each client
                    client.send_add(id, allBoardElements[id]);
                }
            }
            else
            {
                //Strange things happens, a client tried to access forbidden data
                //to avoid any missunderstanding, we resend all information to the client
                do_reset_client(idClient);
            }
            Monitor.Exit(clients);
        }

        /// <summary>
        /// Client want to clear the whole board
        /// </summary>
        /// <param name="idClient"></param>
        public void do_clearAll(int idClient)
        {
            //One client at a time access "clients"
            Monitor.Enter(clients);
            foreach (ClientInterface client in clients.Values)
            {
                //Deselect every selected object
                client.ObjectLocked = -1;
                client.send_deselect();
                //Tell all client the board has ben reseted
                client.send_clear_all();
            }

            //Reset the board
            allBoardElements.Clear();
            actualID = 0;
            Monitor.Exit(clients);

        }

        /// <summary>
        /// Reset all information of a client (deleting and re-addign everything from a client point of view)
        /// </summary>
        /// <param name="idClient">Id of the client</param>
        public void do_reset_client(int idClient)
        {
            
            //One client at a time access "clients"
            Monitor.Enter(clients);
            clients[idClient].send_clear_all(); //Tell the client to clear its board
            foreach(int key in allBoardElements.Keys)
            {
                //Give him again all the informations
                clients[idClient].send_add(key, allBoardElements[key]);
            }
            clients[idClient].send_info();
            Monitor.Exit(clients);
        }

    }
}