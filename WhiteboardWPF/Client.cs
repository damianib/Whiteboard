﻿using System;
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

        private selector m_select;
        private selector m_deselect;
        private selector m_delete;
        private addFunction m_add;
        private clearFunction m_clear_all;
        

        private Char m_limitor;

        Connexion connexionServer;

        public Client(TcpClient tcpClient, addFunction add_recieve, selector select_recieve, selector deselect_recieve, selector delete_recieve)
        {
            m_limitor = '\n';

            connexionServer = new Connexion(tcpClient, runInstruction, m_limitor);
            
            
            m_add = add_recieve;
            m_select = select_recieve;
            m_deselect = deselect_recieve;
            m_delete = delete_recieve;
        }

        public Client(TcpClient tcpClient, addFunction add_recieve, selector select_recieve, selector deselect_recieve, selector delete_recieve, clearFunction clear_receive)
        {
            m_limitor = '\n';

            connexionServer = new Connexion(tcpClient, runInstruction, m_limitor);


            m_add = add_recieve;
            m_select = select_recieve;
            m_deselect = deselect_recieve;
            m_delete = delete_recieve;
            m_clear_all = clear_receive;
        }

        public void start()
        {
            
            connexionServer.start(m_nomServer);
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
                m_add(b);
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
            if (instructionName.Equals("CLR"))
            {
                m_clear_all();
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
        public void ask_deselect(int id)
        {
            connexionServer.addInstruction("DES" + Convert.ToString(id));
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
            Console.WriteLine("Coucou");
            connexionServer.addInstruction("CLR");
        }
    }
}
