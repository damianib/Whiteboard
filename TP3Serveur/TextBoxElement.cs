﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServeur
{
    /// <summary>
    /// Classe correspondant au TextBoxElement du Client
    /// </summary>
    class TextBoxElement : BoardElement
    {
        

        public double X { get; set; }
        public double Y { get; set; }

        public string Text { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }

        public TextBoxElement(double height, double width, string text, double x, double y)
        {
            this.Height = height;
            this.Width = width;
            this.Text = text;
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Constructeur effectuant la conversion String vers TextBoxElement
        /// </summary>
        /// <param name="id">ID unique de l'objet</param>
        /// <param name="str">String contenant les attributs de la TextBox</param>
        public TextBoxElement(int id, string str)
        {
            this.m_id = id;
            char separator = '\u0000';
            string[] strlst = str.Split(separator);

            Text = strlst[0];

            Height = Double.Parse(strlst[1]);

            Width = Double.Parse(strlst[2]);

            X = Double.Parse(strlst[3]);

            Y = Double.Parse(strlst[4]);
        }

        /// <summary>
        /// Renvoie le TextBoxElement sous forme de string pour transmission au client
        /// </summary>
        /// <returns>String représentant l'objet</returns>
        public override string GetString() 
        {
            string str = "";
            char separator = '\u0000';

            str += "txb" + this.Text + separator + this.Height.ToString() + separator + this.Width.ToString() + separator + this.X + separator + this.Y;

            return str;
        }

    }
}
