using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServeur
{
    /// <summary>
    /// Convertisseur de string reçu du client vers le type souhaité
    /// </summary>
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

        /// <summary>
        /// Appelle le constructeur de l'objet qui contient la conversion
        /// </summary>
        /// <param name="id">ID unique de l'objet</param>
        /// <param name="v">String contenant les attributs et le code décrivant le type du BoardElement</param>
        /// <returns> BoardElement correspondant</returns
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
