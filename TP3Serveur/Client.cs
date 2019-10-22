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
    class Client
    {
        public delegate void selector(int idClient, int id);
        public delegate void addFunction(int idClient, Object o);
        public delegate void modifFunction(int idClient, int id, Object o);
        public delegate void clearFunction(int idClient);

        private selector m_select;
        private selector m_deselect;
        private selector m_delete;
        private addFunction m_add;
        private modifFunction m_modif;
        private clearFunction m_clear_all;
        private char m_limitor;

        private Connexion connexionClient;

        public int ObjectLocked = -1;
        public int idClient { get; private set; }
        public Client(TcpClient tcpClient, int id, addFunction addI, selector selectI, selector deselectI, selector deleteI, modifFunction modifI)
        {

            m_limitor = '\n';

            m_limitor = Convert.ToChar(Int16.Parse("feff001e"));
            idClient = id;
            connexionClient = new Connexion(tcpClient, runInstruction, m_limitor);
            m_add = addI;
            m_select = selectI;
            m_deselect = deselectI;
            m_delete = deleteI;
            m_modif = modifI;
        }

        public Client(TcpClient tcpClient, int id, addFunction addI, selector selectI, selector deselectI, selector deleteI, modifFunction modifI, clearFunction clearI, string limitor = "\n")
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
                
                m_add(idClient, ObjectConverter.getObject(str.Substring(3)));
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
                int i = 3;
                while (i < str.Length && str[i] != m_limitor && str[i] != ' ')
                {
                    i++;
                }
                int id = int.Parse(str.Substring(3, i - 3));
                m_deselect(idClient, id);
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
                m_modif(idClient, id, ObjectConverter.getObject(str.Substring(i + 1)));
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
        public void send_deselect(int id)
        {
            connexionClient.addInstruction("DES" + Convert.ToString(id));
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
