using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPServeur
{
    abstract class BoardElement
    {

        public int m_id;
        public object m_o;
        public int m_clientLocker = -1;
        public abstract string GetString();
    }
}
