using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace WhiteboardWPF
{

    /// <summary>
    /// Client controler, this class will modify the main window according to the server instruction, and will also enable the main window to issue request to the server
    /// </summary>
    class Client
    {
        

        /// <summary>
        /// 
        /// </summary>
        public string m_nameBoard { get; set; }

        /// <summary>
        /// Id of the connexion
        /// </summary>
        public int m_idCOnnexion { get; set; }

        /// <summary>
        /// Reference to the main window
        /// </summary>
        private MainWindow mainWindow;

        /// <summary>
        /// Limitor for various use
        /// </summary>
        private Char m_limitor;

        /// <summary>
        /// Connexion  with the server
        /// </summary>
        Connexion connexionServer;

        /// <summary>
        /// Create a client that will connect to the given IP
        /// </summary>
        /// <param name="ip">The IP to connect to</param>
        /// <param name="mainWindow">Reference to the main window</param>
        public Client(String ip, MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            m_limitor = '\n';
            connexionServer = new Connexion(ip, runInstruction, m_limitor);
        }




        /**************************/
        /**Connexion instructions**/
        /**************************/

        /// <summary>
        /// Create a board with random name
        /// </summary>
        public void createBoard()
        {
            try
            {
                connexionServer.start("", true);
            }
            catch( Exception e)
            {
                mainWindow.doShowError(e.Message);
            }
            
        }

        /// <summary>
        /// Create a bo board with given name
        /// </summary>
        /// <param name="stringNom">The board the app will try to connect to</param>
        public void createBoard(String stringNom)
        {
            try
            {
                connexionServer.start(stringNom, true);
            }
            catch(Exception e)
            {
                mainWindow.doShowError(e.Message);
            }
        }

        /// <summary>
        /// Allow to join an existing board
        /// </summary>
        /// <param name="stringNom">The board the user want to connect to</param>
        /// <param name="canCreate">Tell if the server is allowed to create a new board if the asked one is not found</param>
        public void joinBoard(String stringNom, bool canCreate)
        {
            try
            {
                connexionServer.start(stringNom, false, canCreate);
            }
            catch (Exception e)
            {
                mainWindow.doShowError(e.Message);
            }
        }



        /*******************************************************************/
        /**Method the server will invoke to send instruction to the client**/
        /*******************************************************************/

        /// <summary>
        /// Function that is to be called by the "connexionServer" object when it recieve a full instruction
        /// </summary>
        /// <param name="str">The instruction to treat</param>
        private void runInstruction(String str)
        {

            String instructionName = str.Substring(0, 3);

            if (instructionName.Equals("ADD")) //Adding request
            {

                
                int i = 3;
                while (i < str.Length && str[i] != m_limitor && str[i] != ' ')
                {
                    i++;
                }
                int id = int.Parse(str.Substring(3, i - 3));
                BoardElement b = ObjectConverter.ReconvertElement(str.Substring(i + 1));
                b.id = id;
                mainWindow.doAdd(b);
            }
            if (instructionName.Equals("SEL")) //Selecting request
            {
                int i = 3;
                while (i < str.Length && str[i] != m_limitor && str[i] != ' ')
                {
                    i++;
                }
                int id = int.Parse(str.Substring(3, i - 3));
                mainWindow.doSelect(id);
            }
            if (instructionName.Equals("DES")) //Deselecting request
            {
                mainWindow.doDeselect();
            }
            if (instructionName.Equals("DEL")) //Deletig request
            {
                int i = 3;
                while (i < str.Length && str[i] != m_limitor && str[i] != ' ')
                {
                    i++;
                }
                int id = int.Parse(str.Substring(3, i - 3));
                mainWindow.doDelete(id);
            }
            if (instructionName.Equals("CLR")) //Request to clear the board
            {
                mainWindow.doClearAll();
            }
            if (instructionName.Equals("INF")) //Give info to the clients
            {
                String[] infos = str.Substring(3).Split(' ');
                m_idCOnnexion = Convert.ToInt32(infos[0]);
                m_nameBoard = infos[1];
            }
            if (instructionName.Equals("EXT")) //Tell the client to exit
            {
                mainWindow.doClearAll();
                connexionServer.stop();

            }
            if (instructionName.Equals("ERR")) //Indicate an error to the client
            {
                string text = str.Substring(3);
                mainWindow.doShowError(text);

            }
            
        }



        /*********************************************************************/
        /**Method the main window can invoke to issue request to the server***/
        /*********************************************************************/


        /// <summary>
        /// Method to invoke to ask the server to add an object
        /// </summary>
        /// <param name="b">Object to be added</param>
        public void ask_add (BoardElement b)
        {
            connexionServer.addInstruction("ADD" + b.GetString());
        }

        /// <summary>
        /// Method to invoke to ask the server to select an object
        /// </summary>
        /// <param name="id">Id of the object to be selected</param>
        public void ask_select(int id)
        {
            connexionServer.addInstruction("SEL" + Convert.ToString(id));

        }

        /// <summary>
        /// Method to invoke to ask the server to deselect all object
        /// </summary>
        public void ask_deselect()
        {
            connexionServer.addInstruction("DES");
        }

        /// <summary>
        /// Method to invoke to ask the server to delete an object
        /// </summary>
        /// <param name="id">ID of the obect to be deleted</param>
        public void ask_delete(int id)
        {
            connexionServer.addInstruction("DEL" + Convert.ToString(id));
        }

        /// <summary>
        /// Method to invoke to ask the server to modify an object
        /// </summary>
        /// <param name="id"></param>
        /// <param name="b"></param>
        public void ask_modif(int id, BoardElement b)
        {
            connexionServer.addInstruction("MOD" + Convert.ToString(id)+" "+b.GetString());
        }

        /// <summary>
        /// Method to invoke to ask the server to clear the board
        /// </summary>
        public void ask_clear_all()
        {
            connexionServer.addInstruction("CLR");
        }

        /// <summary>
        /// Change the connexion IP
        /// </summary>
        /// <param name="ip">New IP to be set</param>
        public void changeIP(String ip)
        {
            connexionServer.m_ip = ip;
        }

        /// <summary>
        /// Get the connexion IP
        /// </summary>
        /// <returns>The connexion IP</returns>
        public string getIp()
        {
            return connexionServer.m_ip;
        }
    }
}
