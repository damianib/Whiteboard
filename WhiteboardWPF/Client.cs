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

        public void ask_add (BoardElement b)
        {
            connexionServer.addInstruction("ADD" + b.GetString());
        }
        public void ask_select(int id)
        {
            connexionServer.addInstruction("SEL" + Convert.ToString(id));

        }
        public void ask_deselect()
        {
            connexionServer.addInstruction("DES");
        }
        public void ask_delete(int id)
        {
            connexionServer.addInstruction("DEL" + Convert.ToString(id));
        }
        public void ask_modif(int id, BoardElement b)
        {
            connexionServer.addInstruction("MOD" + Convert.ToString(id)+" "+b.GetString());
        }

        public void ask_clear_all()
        {
            connexionServer.addInstruction("CLR");
        }

        public void changeIP(String ip)
        {
            connexionServer.m_ip = ip;
        }

        public string getIp()
        {
            return connexionServer.m_ip;
        }
    }
}
