using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServeur
{
    class ObjetAffichable
    {
        public int m_id;
        public object m_o;
        public int m_clientLocker = -1;

        public ObjetAffichable(int id, Object o)
        {
            m_id = id;
            m_o = o;
        }

    }
}
