using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPServeur
{
    /// <summary>
    /// This abstract class implement a generic object placed on the whiteboard
    /// </summary>
    abstract class BoardElement
    {

        /// <summary>
        /// Unique id for the object
        /// </summary>
        public int m_id;
        /// <summary>
        /// Id of the client that has the lock on the object (-1 means no one)
        /// </summary>
        public int m_clientLocker = -1;
        /// <summary>
        /// Get a string repersentation of the board element for transmission purpose
        /// </summary>
        /// <returns>String representing the objetc</returns>
        public abstract string GetString();
    }
}
 