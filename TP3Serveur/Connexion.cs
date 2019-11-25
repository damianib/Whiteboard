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

        private int sizeLeft;
        private Char m_limitor;

        public delegate void executer(String instruction);

        private executer m_executor;

        private ConcurrentQueue<String> instructionToSend = new ConcurrentQueue<String>();
        private ConcurrentQueue<String> instructionToTreat = new ConcurrentQueue<String>();
        private Thread theradReception;
        private Thread theradEmission;
        private Thread threadTreatment;

        public bool isActive { private set; get; }
        public Connexion(TcpClient tcpClient, executer executor, Char limitor = '\n')
        {
            isActive = false;

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
            if(isActive == false)
            {
                isActive = true;
                theradReception.Start();
                theradEmission.Start();
                threadTreatment.Start();
            }
            
        }

        public void stop()
        {
            isActive = false;
            theradReception.Abort();
            theradEmission.Abort();
            threadTreatment.Abort();
            if (m_tcpClient.Connected)
            {
                m_tcpClient.Close();
            }
        }
        private void treatString(string newData)
        {
            
            mut.WaitOne();
            int pos = 0;
            while (pos < newData.Length)
            {
                if(sizeLeft == 0)
                {
                    while (pos < newData.Length && newData[pos] != ' ')
                    {
                        instruction += newData[pos];
                        pos++;
                    }
                    if(pos < newData.Length)
                    {
                        
                        pos += 1;
                        sizeLeft = int.Parse(instruction);
                        instruction = "";
                    }
                }
                else if(sizeLeft <= newData.Length-pos)
                {
                    
                    instruction += newData.Substring(pos, sizeLeft);
                    m_executor(instruction);
                    instruction = "";
                    pos += sizeLeft;
                    sizeLeft = 0;
                }
                else
                {
                    instruction += newData.Substring(pos, newData.Length - pos);
                    sizeLeft -= newData.Length - pos;
                    pos = newData.Length;
                }
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
            
            
            try
            {
                NetworkStream stream = m_tcpClient.GetStream();
                int i = 0;
                int j = 0;
                byte[] bytes = new byte[2048];
                while (isActive && (i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    Console.WriteLine("la ici");
                    string temp = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
                        instructionToTreat.Enqueue(temp);
                
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isActive = false;
                stop();
            }
        }
            


        private void broadcast()
        {

            Console.WriteLine("la et la");
            try
            {
                NetworkStream stream = m_tcpClient.GetStream();
                String str = "";
                while (isActive)
                {
                    
                    if (instructionToSend.TryDequeue(out str))
                    {
                        
                        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str.Length.ToString()+" "+ str);
                        stream.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isActive = false;
                stop();
            }

        }
    }
}
