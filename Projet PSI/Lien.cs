using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetPSI
{
    public class Lien
    {
        public int noeud1;
        public int noeud2;
        public double poids;

        /// <summary>
        /// Constructeur naturel du lien.
        /// </summary>
        /// <param name="noeud1">ID du premier noeud.</param>
        /// <param name="noeud2">ID du deuxième noeud.</param>
        /// <param name="poids">Poids du lien.</param>
        public Lien(int noeud1, int noeud2, double poids = 1)
        {
            this.noeud1 = noeud1;
            this.noeud2 = noeud2;
            this.poids = poids;
        }

        /// <summary>
        /// Propriété noeud 1.
        /// </summary>
        public int Noeud1
        {
            get { return noeud1; } set { noeud1 = value; }
        }

        /// <summary>
        /// Propriété noeud 2.
        /// </summary>
        public int Noeud2
        {
            get { return noeud2; } set { noeud2 = value; }
        }

        /// <summary>
        /// Propriété poids.
        /// </summary>
        public double Poids
        {
            get { return poids; } set { poids = value; }
        }




    }
}
