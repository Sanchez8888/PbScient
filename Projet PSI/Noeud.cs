using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetPSI
{
    public class Noeud
    {
        public int id;

        /// <summary>
        /// Contructeur naturel du noeud.
        /// </summary>
        /// <param name="id">ID du noeud.</param>
        public Noeud(int id)
        {
            this.id = id;
        }

        /// <summary>
        /// Propriété ID.
        /// </summary>
        public int Id
        {
            get { return id; }
        }
    }

}
