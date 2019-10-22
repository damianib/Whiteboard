using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServeur
{
    class ObjectConverter
    {
        public static Object getObject(String str)
        {
            return str;
        }

        public static String getString(Object o)
        {
            return (String)o;
        }

        internal static object reconvertElement(string v)
        {
            throw new NotImplementedException();
        }
    }
}
