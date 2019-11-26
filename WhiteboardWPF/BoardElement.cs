using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Ink;

namespace WhiteboardWPF
{

    /// <summary>
    /// Classe générique des éléments affichables sur le tableau
    /// </summary>
    public abstract class BoardElement
    {
        /// <summary>
        /// Identifiant unique de l'objet sur le Client
        /// </summary>
        public int id;

        /// <summary>
        /// Renvoie l'élément sous forme de string pour transmission au serveur
        /// </summary>
        /// <returns>String représentant l'objet</returns>
        public abstract string GetString();

        /// <summary>
        /// Ajout de l'élément au tableau
        /// </summary>
        /// <param name="window"></param>
        /// <param name="ink"></param>
        public abstract void AddToCanvas(MainWindow window, InkCanvas ink);

        /// <summary>
        /// Supprime l'élément du tableau
        /// </summary>
        /// <param name="window"></param>
        /// <param name="ink"></param>
        public abstract void DeleteFromCanvas(MainWindow window, InkCanvas ink);

        /// <summary>
        /// Sélectionne l'élément dans l'InkCanvas
        /// </summary>
        /// <param name="window"></param>
        /// <param name="ink"></param>
        public abstract void selectInCanvas(MainWindow window, InkCanvas ink);

        /// <summary>
        /// Renvoie l'élément affichable contenu par le BoardElement sous le type générique Object pour un traitement indifférencié
        /// </summary>
        /// <returns>Contenu sous forme Object</returns>
        public abstract Object getElement();

        /// <summary>
        /// Update the position of the object
        /// </summary>
        /// <param name="inkCanvas">The ink canvas in which the object is placed</param>
        internal abstract void updatePosition(InkCanvas inkCanvas);
    }
}
