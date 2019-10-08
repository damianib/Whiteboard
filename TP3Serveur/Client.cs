﻿using System;
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

        private selector m_select;
        private selector m_deselect;
        private selector m_delete;
        private addFunction m_add;
        private modifFunction m_modif;

        private Connexion connexionClient;

        public int ObjectLocked = -1;
        public int idClient { get; private set; }
        public Client(TcpClient tcpClient, int id, addFunction addI, selector selectI, selector deselectI, selector deleteI, modifFunction modifI, string limitor = "\n")
        {
            
            idClient = id;
            connexionClient = new Connexion(tcpClient, runInstruction);
            m_add = addI;
            m_select = selectI;
            m_deselect = deselectI;
            m_delete = deleteI;
            m_modif = modifI;
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
                while (i < str.Length && str[i] != '\n' && str[i] != ' ')
                {
                    i++;
                }
                int id = int.Parse(str.Substring(3, i - 3));
                m_select(idClient, id);
            }
            if (instructionName.Equals("DES"))
            {
                int i = 3;
                while (i < str.Length && str[i] != '\n' && str[i] != ' ')
                {
                    i++;
                }
                int id = int.Parse(str.Substring(3, i - 3));
                m_deselect(idClient, id);
            }
            if (instructionName.Equals("DEL"))
            {
                int i = 3;
                while (i < str.Length && str[i] != '\n' && str[i] != ' ')
                {
                    i++;
                }
                int id = int.Parse(str.Substring(3, i - 3));
                m_delete(idClient, id);
            }
            if (instructionName.Equals("MOD"))
            {
                
                int i = 3;
                while (i < str.Length && str[i] != '\n' && str[i] != ' ')
                {
                    i++;
                }
                int id = int.Parse(str.Substring(3, i - 3));
                m_modif(idClient, id, ObjectConverter.getObject(str.Substring(i + 1)));
            }
        }
        public void add(int id, Object o)
        {
            connexionClient.addInstruction("ADD"+ Convert.ToString(id)+" "+ ObjectConverter.getString(o));
        }
        public void select(int id)
        {
            connexionClient.addInstruction("SEL" + Convert.ToString(id));

        }
        public void deselect(int id)
        {
            connexionClient.addInstruction("DES" + Convert.ToString(id));
        }
        public void delete(int id)
        {
            connexionClient.addInstruction("DEL" + Convert.ToString(id));
        }
        public void modif(int id, Object o)
        {
            connexionClient.addInstruction("MOD" + Convert.ToString(id) + " " + ObjectConverter.getString(o));
        }

    }
}
