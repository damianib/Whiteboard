using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace TCPServeur
{
    /// <summary>
    /// This class implement the interface beetween a specific client and the server
    /// It communicate with the server via a "Connexion" object that handle the instruction transmission
    /// </summary>
    class Client
    {
        /// <summary>
        /// Delegate for the selector fucntion type
        /// Signature for the method the Client class will invoke when it receive a selection, a deselection or a delete demand from the client
        /// </summary>
        /// <param name="idClient">The id of the client that is issuing the request</param>
        /// <param name="id">The ID of the object to be selected, deselected or deleted</param>
        public delegate void selector(int idClient, int id);

       /// <summary>
       /// Delegate for the add fucntion type
       /// Signature for the method the Client class will invoke when it receive an Adding demand from the client
       /// </summary>
       /// <param name="idClient">The id of the client that is issuing the request</param>
       /// <param name="str">The text representation of the new object</param>
        public delegate void addFunction(int idClient, String str);

        /// <summary>
        /// Delegate for the modif function type
        /// Signature for the method the client class will invoke to modifie an existing object
        /// </summary>
        /// <param name="idClient">The id of the client that is issuing the request</param>
        /// <param name="id">The id of the object that is to be modified</param>
        /// <param name="str">The string representation of the new version of the object</param>
        public delegate void modifFunction(int idClient, int id, String str);

        /// <summary>
        /// Delegate for the clearFuction type
        /// Signature for the method the client class will invoke to clear the whole board
        /// </summary>
        /// <param name="idClient">The id of the client that is issuing the request</param>
        public delegate void clearFunction(int idClient);


        /// <summary>
        /// Method the client class will invoke when receiving a selection demand
        /// </summary>
        private selector m_select;
        
        /// <summary>
        /// Method the client class will invoke when receiving a selection demand
        /// </summary>
        private selector m_delete;
        /// <summary>
        /// Method the client class will invoke when receiving a selection demand
        /// </summary>
        private addFunction m_add;
        /// <summary>
        /// Method the client class will invoke when receiving a selection demand
        /// </summary>
        private modifFunction m_modif;
        /// <summary>
        /// Method the client class will invoke when receiving a selection demand
        /// </summary>
        private clearFunction m_clear_all;
        /// <summary>
        /// Method the client class will invoke when receiving a selection demand
        /// </summary>
        private clearFunction m_reset;
        /// <summary>
        /// Method the client class will invoke when receiving a selection demand
        /// </summary>
        private clearFunction m_deselect;

        private char m_limitor;

        private Connexion connexionClient;

        public int ObjectLocked = -1;
        public int idClient { get; private set; }
        public Client(TcpClient tcpClient, int id, addFunction addI, selector selectI, clearFunction deselectI, selector deleteI, modifFunction modifI, clearFunction clearI, clearFunction resetI)
        {

            m_limitor = '\n';

            //m_limitor = Convert.ToChar(Int16.Parse("feff001e"));
            idClient = id;
            connexionClient = new Connexion(tcpClient, runInstruction, m_limitor);
            m_add = addI;
            m_select = selectI;
            m_deselect = deselectI;
            m_delete = deleteI;
            m_modif = modifI;
            m_clear_all = clearI;
            m_reset = resetI;
        }

        public Client(TcpClient tcpClient, int id, addFunction addI, selector selectI, clearFunction deselectI, selector deleteI, modifFunction modifI, clearFunction clearI, clearFunction resetI,string limitor = "\n")
        {

            Console.WriteLine("ERREUR LAAAAAAA");
            m_limitor = '\n';

            idClient = id;
            connexionClient = new Connexion(tcpClient, runInstruction, m_limitor);
            m_add = addI;
            m_select = selectI;
            m_deselect = deselectI;
            m_delete = deleteI;
            m_modif = modifI;
            m_clear_all = clearI;
            m_reset = resetI;
        }
        public void start()
        {
            connexionClient.start();
        }
        public void runInstruction(String str)
        {
            String instructionName = str.Substring(0, 3);
            if (instructionName.Equals("ADD"))
            {
                
                m_add(idClient,str.Substring(3));
            }
            if (instructionName.Equals("SEL"))
            {
                
                int i = 3;
                while (i < str.Length && str[i] != m_limitor && str[i] != ' ')
                {
                    i++;
                }
                int id = int.Parse(str.Substring(3, i - 3));
                m_select(idClient, id);
            }
            if (instructionName.Equals("DES"))
            {
                m_deselect(idClient);
            }
            if (instructionName.Equals("DEL"))
            {
                int i = 3;
                while (i < str.Length && str[i] != m_limitor && str[i] != ' ')
                {
                    i++;
                }
                int id = int.Parse(str.Substring(3, i - 3));
                m_delete(idClient, id);
            }
            if (instructionName.Equals("MOD"))
            {
                
                int i = 3;
                while (i < str.Length && str[i] != m_limitor && str[i] != ' ')
                {
                    i++;
                }
                int id = int.Parse(str.Substring(3, i - 3));
                m_modif(idClient, id, str.Substring(i + 1));
            }
            if (instructionName.Equals("CLR"))
            {
                m_clear_all(idClient);
            }
        }
        public void send_add(int id, Object o)
        {
            connexionClient.addInstruction("ADD"+ Convert.ToString(id)+" "+ ObjectConverter.getString(o));
        }
        public void send_select(int id)
        {
            connexionClient.addInstruction("SEL" + Convert.ToString(id));

        }
        public void send_deselect()
        {
            connexionClient.addInstruction("DES");
        }
        public void send_delet(int id)
        {
            connexionClient.addInstruction("DEL" + Convert.ToString(id));
        }
        /*public void send_modif(int id, Object o)
        {
            connexionClient.addInstruction("ADD" + Convert.ToString(id) + " " + ObjectConverter.getString(o));
        }*/

        public void send_clear_all()
        {
            connexionClient.addInstruction("CLR");
        }

    }
}
