using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_PSI
{
    public class Lien
    {
        public Noeud noeud1;
        public Noeud noeud2;
        public int poids;

        /// <summary>
        /// Constructeur naturel du lien.
        /// </summary>
        /// <param name="noeud1">ID du premier noeud.</param>
        /// <param name="noeud2">ID du deuxième noeud.</param>
        /// <param name="poids">Poids du lien.</param>
        public Lien(Noeud noeud1, Noeud noeud2, int poids)
        {
            this.noeud1 = noeud1;
            this.noeud2 = noeud2;
            this.poids = poids;
        }

        /// <summary>
        /// Propriété noeud 1.
        /// </summary>
        public Noeud Noeud1
        {
            get { return noeud1; }
            set { noeud1 = value; }
        }

        /// <summary>
        /// Propriété noeud 2.
        /// </summary>
        public Noeud Noeud2
        {
            get { return noeud2; }
            set { noeud2 = value; }
        }

        /// <summary>
        /// Propriété poids.
        /// </summary>
        public int Poids
        {
            get { return poids; }
            set { poids = value; }
        }




    }

}
