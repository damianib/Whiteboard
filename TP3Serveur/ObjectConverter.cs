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
        /// <summary>
        /// Appelle le constructeur de l'objet qui contient la conversion
        /// </summary>
        /// <param name="id">ID of the object to be created</param>
        /// <param name="v">String represnetation of the object</param>
        /// <returns>BoardElement correspondant</returns> 
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
