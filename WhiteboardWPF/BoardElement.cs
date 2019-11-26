﻿using System;
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
        /// <returns></returns> String représentant l'objet
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
        /// <returns></returns> Contenu sous forme Object
        public abstract Object getElement();
        internal abstract void updatePosition(InkCanvas inkCanvas);
    }
}
