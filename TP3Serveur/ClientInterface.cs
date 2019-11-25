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
    class ClientInterface
    {
        /// <summary>
        /// Delegate for the selector fucntion type
        /// Signature for the method the ClientInterface will invoke when it receive a selection or a delete demand from the client
        /// </summary>
        /// <param name="idClient">ID of the client that is issuing the request</param>
        /// <param name="id">ID of the object to be selected or deleted</param>
        public delegate void selector(int idClient, int id);

        /// <summary>
        /// Delegate for the add fucntion type
        /// Signature for the method the ClientInterface will invoke when it receive an Adding demand from the client
        /// </summary>
        /// <param name="idClient">ID of the client that is issuing the request</param>
        /// <param name="str">String representation of the new object</param>
        public delegate void addFunction(int idClient, String str);

        /// <summary>
        /// Delegate for the modif function type
        /// Signature for the method the ClientInterface will invoke to modifie an existing object
        /// </summary>
        /// <param name="idClient">The id of the client that is issuing the request</param>
        /// <param name="id">ID of the object that is to be modified</param>
        /// <param name="str">String representation of the new version of the object</param>
        public delegate void modifFunction(int idClient, int id, String str);

        /// <summary>
        /// Delegate for the clearFuction type
        /// Signature for the method the ClientInterface will invoke to clear the whole board, reset its connexion or deselect a client's object
        /// </summary>
        /// <param name="idClient">The id of the client that is issuing the request</param>
        public delegate void clearFunction(int idClient);


        /// <summary>
        /// Method the client ClientInterface invoke when receiving a selection demand
        /// </summary>
        private selector m_select;

        /// <summary>
        /// Method the ClientInterface will invoke when receiving a deletion demand
        /// </summary>
        private selector m_delete;

        /// <summary>
        /// Method the ClientInterface will invoke when receiving a adding demand
        /// </summary>
        private addFunction m_add;

        /// <summary>
        /// Method the ClientInterface will invoke when receiving a modification demand
        /// </summary>
        private modifFunction m_modif;

        /// <summary>
        /// Method the ClientInterface will invoke when receiving a clearing demand
        /// </summary>
        private clearFunction m_clear_all;

        /// <summary>
        /// Method the ClientInterface will invoke when receiving a reseting demand (meaning reinterrogating the server about the whole board)
        /// </summary>
        private clearFunction m_reset;

        /// <summary>
        /// Method the ClientInterface will invoke when receiving a deselection demand
        /// </summary>
        private clearFunction m_deselect;

        /// <summary>
        /// char that is used as a limitor inside the textual representations
        /// </summary>
        private char m_limitor;

        /// <summary>
        /// Handle the connexion with the client
        /// </summary>
        private Connexion connexionClient;

        /// <summary>
        /// ID of the object locked by the client
        /// </summary>
        public int ObjectLocked = -1;

        /// <summary>
        /// ID of the client
        /// </summary>
        public int idClient { get; private set; }

        /// <summary>
        /// Create a client, with all relevant informations
        /// </summary>
        /// <param name="tcpClient">TCP connexion with the client</param>
        /// <param name="id">ID that is assigned to the client</param>
        /// <param name="addD">Function to invoke on adding demand</param>
        /// <param name="selectD">Fucntion to invoke on selection demand</param>
        /// <param name="deselectD">Function to invoke on deselection demand</param>
        /// <param name="deleteD">Function do invoke on deletion demand</param>
        /// <param name="modifD">Fucntion to invoke on mofification demand</param>
        /// <param name="clearD">Function to invoke on clearing demand</param>
        /// <param name="resetD">Function to invoke on reseting connexion or demand</param>
        public ClientInterface(TcpClient tcpClient, int id, addFunction addD, selector selectD, clearFunction deselectD, selector deleteD, modifFunction modifD, clearFunction clearD, clearFunction resetD)
        {

            //Set up the propertys
            m_limitor = '\n';
            idClient = id;

            //Passing runInstruction as an argument, that will be invoked by the "connexionClient" each time a full instruction is recieved
            connexionClient = new Connexion(tcpClient, runInstruction, m_limitor);

            
            m_add = addD;
            m_select = selectD;
            m_deselect = deselectD;
            m_delete = deleteD;
            m_modif = modifD;
            m_clear_all = clearD;
            m_reset = resetD;
        }

        /// <summary>
        /// Start the client interface
        /// </summary>
        public void start()
        {
            connexionClient.start();
        }

        /// <summary>
        /// Method to be invoked by the clientConnexion object each time a full instruction is recieved
        /// </summary>
        /// <param name="str">Instruction recived</param>
        public void runInstruction(String str)
        {
            ///The 3 first char represent the instruction
            String instructionName = str.Substring(0, 3);
            if (instructionName.Equals("ADD"))//If we have an adding instruction, 
            {
                //We ask the server to add the object represented by the rest of the string
                m_add(idClient,str.Substring(3));
            }
            if (instructionName.Equals("SEL"))//If we have a selecting instruction
            {
                int i = 3;

                //We go to the next limitor to know which part of the string represent the ID of the object
                while (i < str.Length && str[i] != m_limitor && str[i] != ' ') 
                {
                    i++;
                }

                //We extract the ID of the object
                int id = int.Parse(str.Substring(3, i - 3));

                //We ask the server to select the object with ID "id"
                m_select(idClient, id);
            }
            if (instructionName.Equals("DES"))//If we have a deselection instruction
            {
                //We ask the server to deselect the object selected by the client
                m_deselect(idClient);
            }
            if (instructionName.Equals("DEL"))//If we have a deletion instruction
            {
                int i = 3;
                //We go to the next limitor to know which part of the string represent the ID of the object
                while (i < str.Length && str[i] != m_limitor && str[i] != ' ')
                {
                    i++;
                }

                //We extract the ID of the Object
                int id = int.Parse(str.Substring(3, i - 3));

                //We ask the server to delete the object with ID "id"
                m_delete(idClient, id);
            }
            if (instructionName.Equals("MOD")) //If we have a modification instruction
            {
                int i = 3;
                //We go to the next limitor to know which part of the string represent the ID of the object
                while (i < str.Length && str[i] != m_limitor && str[i] != ' ')
                {
                    i++;
                }

                //We extract the ID of the Object
                int id = int.Parse(str.Substring(3, i - 3));

                //str.Substring(i + 1) extract the rest of the string, which represent the new version of the object
                //We ask the server to modify the object with ID "id" and to replace it by the object represented by the rest of the string
                m_modif(idClient, id, str.Substring(i + 1));
            }
            if (instructionName.Equals("CLR"))//If we have a clearing instruction
            {
                //We ask the server to clear the whole board
                m_clear_all(idClient);
            }
            if (instructionName.Equals("EXT")) //If we have an exiting instruction
            {

            }
        }

        /// <summary>
        /// Method to send the instruction to the client to add an object
        /// </summary>
        /// <param name="id">Id of the object</param>
        /// <param name="b">Elemnt to be added</param>
        public void send_add(int id, BoardElement b)
        {
            connexionClient.addInstruction("ADD"+ Convert.ToString(id)+" "+b.GetString());
            //                              add      with id "id"           the object b
        }

        /// <summary>
        /// Method to tell the client it has selected an object
        /// </summary>
        /// <param name="id">Id of the selected object</param>
        public void send_select(int id)
        {
            connexionClient.addInstruction("SEL" + Convert.ToString(id));
            //                            select   the oject with id "id"

        }

        /// <summary>
        /// Method to tell the client all its selected object have been deselected
        /// </summary>
        public void send_deselect()
        {
            connexionClient.addInstruction("DES");
            //                            Deselect everything
        }

        /// <summary>
        /// Method to tell the client an object has been deleted
        /// </summary>
        /// <param name="id">Id of the object to be deleted</param>
        public void send_delet(int id)
        {
            connexionClient.addInstruction("DEL" + Convert.ToString(id));
            //                            Delete    Object with ID id
        }

        /// <summary>
        /// Method to tell the client the board have been cleared
        /// </summary>
        public void send_clear_all()
        {
            connexionClient.addInstruction("CLR");
            //                              Clear the board
        }

    }
}
