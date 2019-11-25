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
    class Connexion
    {

        public String m_ip { set; get; }

        private TcpClient m_tcpClient;
        private String instruction = "";
        private Mutex mut = new Mutex();
        private Char m_limitor;

        public delegate void executer(String instruction);

        private executer m_executor;

        private ConcurrentQueue<String> instructionToSend = new ConcurrentQueue<String>();
        private ConcurrentQueue<String> instructionToTreat = new ConcurrentQueue<String>();
        private Thread theradReception;
        private Thread theradEmission;
        private Thread threadTreatment;
        private int sizeLeft = 0;

        public bool isActive { private set; get; }
        public int id { private set; get; }
        public Connexion(String ip, executer executor, Char limitor = '\n')
        {

            isActive = false;
            m_ip = ip;
            //m_tcpClient = tcpClient;
            m_executor = executor;
            m_limitor = limitor;

            theradReception = new Thread(new ThreadStart(receive));
            theradEmission = new Thread(new ThreadStart(broadcast));
            threadTreatment = new Thread(new ThreadStart(treatInstruction));

            instructionToSend = new ConcurrentQueue<String>();
            instructionToTreat = new ConcurrentQueue<String>();
    }

        

        public void addInstruction(string str)
        {
            if (isActive)
            {
                instructionToSend.Enqueue(str);
            }
        }

        public bool start(String nom, bool first = false)
        {
            
            if (isActive)
            {
                exit();
            }
            Console.WriteLine("Name " + nom + " isFirst " + first);
            
            theradReception = new Thread(new ThreadStart(receive));
            theradEmission = new Thread(new ThreadStart(broadcast));
            threadTreatment = new Thread(new ThreadStart(treatInstruction));

            instructionToSend = new ConcurrentQueue<String>();
            instructionToTreat = new ConcurrentQueue<String>();



            m_tcpClient = new TcpClient();
            m_tcpClient.Connect(m_ip, 5035);
            

            string sending = "";
            isActive = true;
            if (!first)
            {
                sending = "connect " + nom + " -1";
            }
            else if(!nom.Equals(""))
            {
                sending = "init " + nom + " -1";
            }
            else
            {
                sending = "initNoName r -1";
            }

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(sending);
            m_tcpClient.GetStream().Write(bytes, 0, bytes.Length);
            theradReception.Start();
            theradEmission.Start();
            threadTreatment.Start();

            return true;
        }

        public void stop()
        {


            isActive = false;
            theradReception.Abort();
            threadTreatment.Abort();
            theradEmission.Abort();
            if (m_tcpClient.Connected)
            {
                m_tcpClient.Close();
            }
            
            instruction = "";
            instructionToSend = new ConcurrentQueue<String>();
            instructionToTreat = new ConcurrentQueue<String>();
        }

        public void exit()
        {

            instructionToSend.Enqueue("EXT");
            theradReception.Abort();
            threadTreatment.Abort();
            while (isActive && instructionToSend.Count > 0) ;
            isActive = false;
            theradEmission.Join();
            
            if (m_tcpClient.Connected && isActive)
            {
                m_tcpClient.Close();
            }

            instruction = "";
            instructionToSend = new ConcurrentQueue<String>();
            instructionToTreat = new ConcurrentQueue<String>();
        }


        private void treatString(string newData)
        {
            mut.WaitOne();
            int pos = 0;
            while (pos < newData.Length)
            {
                
                if (sizeLeft == 0)
                {
                    while (pos < newData.Length && newData[pos] != ' ')
                    {
                        instruction += newData[pos];
                        pos++;
                    }
                    if (pos < newData.Length)
                    {
                        
                        pos += 1;
                        sizeLeft = int.Parse(instruction);
                        instruction = "";
                        
                    }
                }
                else if (sizeLeft <= newData.Length - pos)
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
            try
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
            catch (ThreadAbortException e)
            {

            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("treat "+e.Message);
                isActive = false;
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
                    string temp = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
                    instructionToTreat.Enqueue(temp);
                }
            }
            catch (ThreadAbortException e)
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                isActive = false;
            }
            
        }

        private void broadcast()
        {
            try {
                NetworkStream stream = m_tcpClient.GetStream();
                String str = "";
                while (isActive)
                {
                    if (instructionToSend.TryDequeue(out str))
                    {
                        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str.Length.ToString() +" " + str);
                        stream.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            catch (ThreadAbortException e)
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("broadCast "+e.Message);
                isActive = false;
            }
        }
    }
}
