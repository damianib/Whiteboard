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
        public delegate void modifFunction(int id, Object o);
        public delegate void clearFunction();

        private selector m_select;
        private selector m_deselect;
        private selector m_delete;
        private modifFunction m_add;
        private modifFunction m_modif;
        private clearFunction m_clear_all;
        

        private Char m_limitor;

        Connexion connexionServer;

        public Client(TcpClient tcpClient, modifFunction add_recieve, selector select_recieve, selector deselect_recieve, selector delete_recieve, modifFunction modif_recieve)
        {
            m_limitor = '\n';

            connexionServer = new Connexion(tcpClient, runInstruction, m_limitor);
            
            
            m_add = add_recieve;
            m_select = select_recieve;
            m_deselect = deselect_recieve;
            m_delete = delete_recieve;
            m_modif = modif_recieve;
        }

        public Client(TcpClient tcpClient, modifFunction add_recieve, selector select_recieve, selector deselect_recieve, selector delete_recieve, modifFunction modif_recieve, clearFunction clear_receive)
        {
            m_limitor = '\n';

            connexionServer = new Connexion(tcpClient, runInstruction, m_limitor);


            m_add = add_recieve;
            m_select = select_recieve;
            m_deselect = deselect_recieve;
            m_delete = delete_recieve;
            m_modif = modif_recieve;
            m_clear_all = clear_receive;
        }

        public void start()
        {
            connexionServer.start();
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
                m_add(id, ObjectConverter.getObject(str.Substring(i+1)));
            }
            if (instructionName.Equals("SEL"))
            {
                int i = 3;
                while (i < str.Length && str[i] != m_limitor && str[i] != ' ')
                {
                    i++;
                }
                int id = int.Parse(str.Substring(3, i - 3));
                m_select(id);
            }
            if (instructionName.Equals("DES"))
            {
                int i = 3;
                while (i < str.Length && str[i] != m_limitor && str[i] != ' ')
                {
                    i++;
                }
                int id = int.Parse(str.Substring(3, i - 3));
                m_deselect(id);
            }
            if (instructionName.Equals("DEL"))
            {
                int i = 3;
                while (i < str.Length && str[i] != m_limitor && str[i] != ' ')
                {
                    i++;
                }
                int id = int.Parse(str.Substring(3, i - 3));
                m_delete(id);
            }
            if (instructionName.Equals("MOD"))
            {
                int i = 3;
                while (i < str.Length && str[i] != m_limitor && str[i] != ' ')
                {
                    i++;
                }
                int id = int.Parse(str.Substring(3, i - 3));
                m_modif(id, str.Substring(i + 1));
            }
        }

        public void ask_add (Object o)
        {
            connexionServer.addInstruction("ADD" +ObjectConverter.getString(o));

        }
        public void ask_select(int id)
        {
            connexionServer.addInstruction("SEL" + Convert.ToString(id));

        }
        public void ask_deselect(int id)
        {
            connexionServer.addInstruction("DES" + Convert.ToString(id));
        }
        public void ask_delete(int id)
        {
            connexionServer.addInstruction("DEL" + Convert.ToString(id));
        }
        public void ask_modif(int id, Object o)
        {
            connexionServer.addInstruction("MOD" + Convert.ToString(id)+" "+ObjectConverter.getString(o));
        }

        public void ask_clear_all()
        {
            connexionServer.addInstruction("CLR");
        }
    }
}
