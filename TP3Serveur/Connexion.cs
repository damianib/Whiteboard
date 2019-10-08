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
    class Connexion
    {
        private TcpClient m_tcpClient;
        private Mutex mut = new Mutex();
        private String instruction = "";
        private String m_limitor;

        public delegate void executer(String instruction);

        private executer m_executor;

        private ConcurrentQueue<String> instructionToSend = new ConcurrentQueue<String>();
        private ConcurrentQueue<String> instructionToTreat = new ConcurrentQueue<String>();
        private Thread theradReception;
        private Thread theradEmission;
        private Thread threadTreatment;

        public bool isActive { private set; get; }
        public Connexion(TcpClient tcpClient, executer executor, string limitor = "\n")
        {
            isActive = true;

            m_tcpClient = tcpClient;
            m_executor = executor;
            m_limitor = limitor;

            theradReception = new Thread(new ThreadStart(receive));
            theradEmission = new Thread(new ThreadStart(broadcast));
            threadTreatment = new Thread(new ThreadStart(treatInstruction));
        }



        public void addInstruction(string str)
        {
            instructionToSend.Enqueue(str);
        }

        public void start()
        {
            isActive = true;
            theradReception.Start();
            theradEmission.Start();
            threadTreatment.Start();
        }

        public void stop()
        {
            isActive = false;
            theradReception.Join();
            theradEmission.Join();
            threadTreatment.Join();
        }
        private void treatString(string newData)
        {

            mut.WaitOne();
            int limit = 0;
            for (int i = 0; i < newData.Length; i++)
            {
                if (newData[i] == m_limitor[0])
                {
                    instruction += newData.Substring(limit, i - limit);

                    m_executor(instruction);

                    instruction = "";
                    limit = i + 1;
                }
            }
            if (limit < newData.Length)
            {
                instruction += newData.Substring(limit, newData.Length - limit);
            }
            mut.ReleaseMutex();
        }



        private void treatInstruction()
        {
            while (isActive)
            {
                String str = "";
                if (instructionToTreat.TryDequeue(out str))
                {
                    treatString(str);
                }
            }
        }

        private void receive()
        {
            NetworkStream stream = m_tcpClient.GetStream();
            int i = 0;
            int j = 0;
            byte[] bytes = new byte[2048];
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0 && isActive)
            {
                string temp = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
                instructionToTreat.Enqueue(temp);
            }
        }

        private void broadcast()
        {
            NetworkStream stream = m_tcpClient.GetStream();
            String str = "";
            while (isActive)
            {
                if (instructionToSend.TryDequeue(out str))
                {
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str + "\n");
                    stream.Write(bytes, 0, bytes.Length);
                }
            }

        }
    }
}
