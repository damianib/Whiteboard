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


        public delegate void selector(int id);
        public delegate void addFunction(BoardElement b);
        public delegate void clearFunction();


        public string m_nomServer { get; set; }
        public int idConnexion { get; set; }

        private MainWindow mainWindow;

        /*private selector m_select;
        private selector m_delete;
        private addFunction m_add;
        private clearFunction m_clear_all;
        private clearFunction m_deselect; */


        private Char m_limitor;

        Connexion connexionServer;

        
        

        /*public Client(String ip, addFunction add_recieve, selector select_recieve, clearFunction deselect_recieve, selector delete_recieve, clearFunction clear_receive)
        {
            m_limitor = '\n';

            connexionServer = new Connexion(ip, runInstruction, m_limitor);


            m_add = add_recieve;
            m_select = select_recieve;
            m_deselect = deselect_recieve;
            m_delete = delete_recieve;
            m_clear_all = clear_receive;
        } */

        public Client(String ip, MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            m_limitor = '\n';

            connexionServer = new Connexion(ip, runInstruction, m_limitor);


            
        }

        /*public void start()
        {
            connexionServer.start(m_nomServer);
        }*/

        public void createBoard()
        {
            connexionServer.start("", true);
        }
        public void createBoard(String stringNom)
        {
            connexionServer.start(stringNom, true);
        }
        public void joinBoard(String stringNom)
        {
            connexionServer.start(stringNom);
        }
        private void runInstruction(String str)
        {

            String instructionName = str.Substring(0, 3);

            if (instructionName.Equals("ADD"))
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
            if (instructionName.Equals("SEL"))
            {
                int i = 3;
                while (i < str.Length && str[i] != m_limitor && str[i] != ' ')
                {
                    i++;
                }
                int id = int.Parse(str.Substring(3, i - 3));
                mainWindow.doSelect(id);
            }
            if (instructionName.Equals("DES"))
            {
                mainWindow.doDeselect();
            }
            if (instructionName.Equals("DEL"))
            {
                int i = 3;
                while (i < str.Length && str[i] != m_limitor && str[i] != ' ')
                {
                    i++;
                }
                int id = int.Parse(str.Substring(3, i - 3));
                mainWindow.doDelete(id);
            }
            if (instructionName.Equals("CLR"))
            {
                mainWindow.doClearAll();
            }
            if (instructionName.Equals("INF"))
            {
                Console.WriteLine("HEREEEE");
                String[] infos = str.Substring(3).Split(' ');
                idConnexion = Convert.ToInt32(infos[0]);
                m_nomServer = infos[1];
            }
            if (instructionName.Equals("EXT"))
            {
                mainWindow.doClearAll();
                connexionServer.stop();

            }
            if (instructionName.Equals("ERR"))
            {
                string text = str.Substring(3);
                Console.WriteLine("Erreur :" + text);
                mainWindow.doShowError(text);

            }
            /*if (instructionName.Equals("MOD"))
            {
                int i = 3;
                while (i < str.Length && str[i] != m_limitor && str[i] != ' ')
                {
                    i++;
                }
                int id = int.Parse(str.Substring(3, i - 3));
                BoardElement b = 
                m_modif(str.Substring(i + 1));
            } */
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
