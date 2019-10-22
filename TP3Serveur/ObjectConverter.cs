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

        public static BoardElement reconvertElement(int id, string v)
        {
            if (v.Substring(0, 3).Equals("str"))
            {
                return new StrokeElement(id, v.Substring(3));
            }
            else
            {
                return new TextBoxElement(id, v.Substring(3));
            }
            
        }
    }
}
