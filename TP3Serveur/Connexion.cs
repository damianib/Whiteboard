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
    /// <summary>
    /// This class implement the connection protocol with a specific client
    /// It deals with reconstructing instruction from the TCP stream
    /// 
    /// A instruction will have the following form:
    ///      a blanck
    ///       |
    ///       |
    ///     12 ADD..............
    ///     ^  |---------------|
    ///     |   The instruction
    ///     |
    ///   size of
    /// the instruction
    /// 
    /// </summary>
    class Connexion
    {

        /// <summary>
        /// TCP Socket that enable the server to speak with the client
        /// </summary>
        private TcpClient m_tcpClient;

        /// <summary>
        /// Temporary storage for instruction that are not yet complete (for example if they are sent through several TCP blocks
        /// </summary>
        private String instruction = "";

        /// <summary>
        /// Store the number of missing characters to complete the "instruction"
        /// </summary>
        private int sizeLeft;

        /// <summary>
        /// Delegate for the fucntion the Conection will have to send the instruction to
        /// </summary>
        /// <param name="instruction"></param>
        public delegate void executer(String instruction);

        /// <summary>
        /// Function that the connexion will "feed" with instructions
        /// </summary>
        private executer m_executor;

        /// <summary>
        /// Queue of instruction that have to be send through the TCP connexion
        /// </summary>
        private ConcurrentQueue<String> instructionToSend = new ConcurrentQueue<String>();

        /// <summary>
        /// Queue of instruction that are to be treated
        /// </summary>
        private ConcurrentQueue<String> dataToTreat = new ConcurrentQueue<String>();

        /// <summary>
        /// Thread handling the data reception
        /// </summary>
        private Thread theradReception;

        /// <summary>
        /// Thread handling the data emission
        /// </summary>
        private Thread theradEmission;

        /// <summary>
        /// Thread handling the instruction treatment
        /// </summary>
        private Thread threadTreatment;

        /// <summary>
        /// Boolean to know if either the connexion is active or not
        /// </summary>
        public bool isActive { private set; get; }

        /// <summary>
        /// Create a Connexion with the client
        /// </summary>
        /// <param name="tcpClient">TCP socket that connect the server to the client</param>
        /// <param name="executor">Fucntion to be executed with the instructions</param>
        public Connexion(TcpClient tcpClient, executer executor)
        {
            isActive = false;

            m_tcpClient = tcpClient;
            m_executor = executor;

            theradReception = new Thread(new ThreadStart(receive));
            theradEmission = new Thread(new ThreadStart(broadcast));
            threadTreatment = new Thread(new ThreadStart(treatData));
        }


        /// <summary>
        /// Add an instruction that will be send to the server
        /// </summary>
        /// <param name="str">Instrucction to be send</param>
        public void addInstruction(string str)
        {
            instructionToSend.Enqueue(str);
        }

        /// <summary>
        /// Starting the connexion
        /// </summary>
        public void start()
        {
            if(isActive == false)//We only start if it is not already started
            {
                isActive = true;
                theradReception.Start();
                theradEmission.Start();
                threadTreatment.Start();
            }
   
        }

        /// <summary>
        /// Stoping the connexion
        /// </summary>
        public void stop()
        {

            isActive = false;//The connexion will not be active anymore

            //Aborting all thread
            theradReception.Abort();
            theradEmission.Abort();
            threadTreatment.Abort();

            //Closing the socket (if it has not already been done)
            if (m_tcpClient.Connected)
            {
                m_tcpClient.Close();
            }
        }

        /// <summary>
        /// This function is used to treat all the new data that is recieved from the client
        /// It is call for each string that is waiting in "dataToTreat"
        /// </summary>
        /// <param name="newData"></param>
        private void treatString(string newData)
        {
            //Position in the string
            int pos = 0;

            while (pos < newData.Length) //While the whole string has not been treated
            {
                
                if (sizeLeft == 0) //If we do not know the size of the instruction, it means that we are actually reading the instruction size
                {
                    while (pos < newData.Length && newData[pos] != ' ') //We keep on until we find a blank
                    {
                        instruction += newData[pos];
                        pos++;
                    }
                    if(pos < newData.Length) //If we have not reach the en of the string, it means we have found a blank
                    {
                        
                        pos += 1;
                        sizeLeft = int.Parse(instruction); //So it means "instrucion" store the size of the incoming instruction, we update sizeLeft
                        instruction = "";
                    }
                }
                else //sizeLeft>0, so we are actually trating an instruction
                if (sizeLeft <= newData.Length-pos) // if sizeLeft <= newData.Length-pos, it means the string is large enough to contain the full incoming instruction
                {
                    instruction += newData.Substring(pos, sizeLeft); 
                    m_executor(instruction); //We treat the isntruction
                    instruction = "";
                    pos += sizeLeft; //We updated the cursor position
                    sizeLeft = 0;
                }
                else //The instruction is too large to be contained in the string
                {
                    instruction += newData.Substring(pos, newData.Length - pos); //We store the local state in instruction
                    sizeLeft -= newData.Length - pos; // We read (newData.Length - pos) characters from the string, so there is (sizeLeft - (newData.Length - pos)) left to read to complete the instruction
                    pos = newData.Length;
                }
            }

        }


        /// <summary>
        /// Function that treat the data stored in "dataToTreat"
        /// </summary>
        private void treatData()
        {
            try
            {
                while (isActive)
                {
                    String str = "";
                    if (dataToTreat.TryDequeue(out str)) //Get the data left
                    {
                        treatString(str);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
            }
            isActive = false;
            stop();
        }


        /// <summary>
        /// Fucntion that handle the data reception from the client
        /// </summary>
        private void receive()
        {
            
            
            try
            {
                NetworkStream stream = m_tcpClient.GetStream();
                int i = 0;
                int j = 0;
                byte[] bytes = new byte[2048];
                while (isActive && (i = stream.Read(bytes, 0, bytes.Length)) != 0) //read the data from the socket
                {
                    
                    string temp = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
                    dataToTreat.Enqueue(temp); //Set the data to be treated
                
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
            }
            isActive = false;
            stop();
        }
            

        /// <summary>
        /// Function that send the instruction in "instructionToSend" to the client
        /// </summary>
        private void broadcast()
        {

            try
            {
                NetworkStream stream = m_tcpClient.GetStream();
                String str = "";
                while (isActive)
                {
                    
                    if (instructionToSend.TryDequeue(out str))
                    {
                        
                        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str.Length.ToString()+" "+ str);
                        stream.Write(bytes, 0, bytes.Length); //send the instruction to the client
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            isActive = false;
            stop();

        }
    }
}
