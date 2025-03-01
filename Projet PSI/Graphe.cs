using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ProjetPSI
{
    public class Graphe
    {
        public Dictionary<int, List<Lien>> adjacence;
        private string type;
        private int nbsommets;
        private double[,] matriceAdjacence;

        /// <summary>
        /// Constructeur Naturel
        /// </summary>
        /// <param name="fichier">Chemin du fichier</param>
        /// <param name="type">Type du graphe.</param>
        public Graphe(string fichier, string type)
        {
            this.type = type;
            adjacence = new Dictionary<int, List<Lien>>();
            string[] lignes = File.ReadAllLines(fichier);

            if (type == "liste")
            {
                foreach (string ligne in lignes)
                {
                    string ligne1 = ligne.Replace("(", "").Replace(")", "").Replace("\t", " ").Replace(",", " ");
                    string[] ligne2 = ligne1.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (ligne2.Length >= 2 && int.TryParse(ligne2[0], out int id1) && int.TryParse(ligne2[1], out int id2))
                    {
                        double poids = 1;
                        if (ligne2.Length > 2 && double.TryParse(ligne2[2], out double Poids))
                        {
                            poids = Poids;
                        }
                        AjouterNoeud(id1);
                        AjouterNoeud(id2);
                        AjouterLien(id1, id2, poids);
                    }
                }
                this.nbsommets = adjacence.Count;
            }
            else if (type == "matrice")
            {
                this.nbsommets = lignes.Length;
                matriceAdjacence = new double[nbsommets, nbsommets];

                for (int i = 0; i < nbsommets; i++)
                {
                    string[] valeurs = lignes[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < nbsommets; j++)
                    {
                        if (double.TryParse(valeurs[j], out double poids))
                        {
                            matriceAdjacence[i, j] = poids;
                        }
                        else
                        {
                            matriceAdjacence[i, j] = 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Ajoute un nœud.
        /// </summary>
        /// <param name="id">ID du Noeud</param>

        public void AjouterNoeud(int id)
        {
            if (!adjacence.ContainsKey(id))
            {
                adjacence[id] = new List<Lien>();
            }
        }

        /// <summary>
        /// Ajoute un lien
        /// </summary>
        /// <param name="id1">Premier noeud</param>
        /// <param name="id2">Deuxième noeud</param>
        /// <param name="poids">Poids de l'arête</param>
        public void AjouterLien(int id1, int id2, double poids = 1)
        {
            if (!adjacence.ContainsKey(id1) || !adjacence.ContainsKey(id2)) return;

            Lien lien = new Lien(id1, id2, poids);
            adjacence[id1].Add(lien);
            adjacence[id2].Add(lien);
        }
        /// <summary>
        /// Parcours en largeur
        /// </summary>
        /// <param name="depart">Sommet de départ</param>
        public void ParcoursLargeur(int depart)
        {
            if (!adjacence.ContainsKey(depart)) return;

            List<int> file = new List<int>();
            List<int> visites = new List<int>();
            file.Add(depart);
            visites.Add(depart);

            while (file.Count > 0)
            {
                int noeud = file[0];
                file.RemoveAt(0);
                Console.Write(noeud + " ");

                foreach (Lien lien in adjacence[noeud].OrderBy(l => l.Noeud1 == noeud ? l.Noeud2 : l.Noeud1))
                {
                    int voisin = (lien.Noeud1 == noeud) ? lien.Noeud2 : lien.Noeud1;
                    if (!visites.Contains(voisin))
                    {
                        visites.Add(voisin);
                        file.Add(voisin);
                    }
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Parcours en profondeur
        /// </summary>
        /// <param name="depart">Sommet de départ</param>
        public void ParcoursProfondeur(int depart)
        {
            if (!adjacence.ContainsKey(depart)) return;

            List<int> visites = new List<int>();
            ParcoursProfondeurRecursive(depart, visites);
            Console.WriteLine();
        }

        private void ParcoursProfondeurRecursive(int noeud, List<int> visites)
        {
            if (visites.Contains(noeud)) return;

            visites.Add(noeud);
            Console.Write(noeud + " ");

            foreach (Lien lien in adjacence[noeud].OrderBy(l => l.Noeud1 == noeud ? l.Noeud2 : l.Noeud1))
            {
                int voisin = (lien.Noeud1 == noeud) ? lien.Noeud2 : lien.Noeud1;
                ParcoursProfondeurRecursive(voisin, visites);
            }
        }
        /// <summary>
        /// Vérifie si le graphe contient un cycle
        /// </summary>
        public bool ContientCycle()
        {
            List<int> visites = new List<int>();
            List<int> pile = new List<int>();

            foreach (int noeud in adjacence.Keys)
            {
                if (!visites.Contains(noeud))
                {
                    pile.Add(noeud);
                    while (pile.Count > 0)
                    {
                        int courant = pile[pile.Count - 1];
                        pile.RemoveAt(pile.Count - 1);

                        if (!visites.Contains(courant))
                        {
                            visites.Add(courant);
                            foreach (Lien lien in adjacence[courant])
                            {
                                int voisin = (lien.Noeud1 == courant) ? lien.Noeud2 : lien.Noeud1;
                                if (visites.Contains(voisin)) return true;
                                pile.Add(voisin);
                            }
                        }
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Vérifie si le graphe est connexe
        /// </summary>
        public bool EstConnexe()
        {
            if (adjacence.Count == 0) return false;

            List<int> file = new List<int>();
            List<int> visites = new List<int>();

            int premierNoeud = adjacence.Keys.First();
            file.Add(premierNoeud);
            visites.Add(premierNoeud);

            while (file.Count > 0)
            {
                int noeud = file[0];
                file.RemoveAt(0);

                foreach (Lien lien in adjacence[noeud])
                {
                    int voisin = (lien.Noeud1 == noeud) ? lien.Noeud2 : lien.Noeud1;
                    if (!visites.Contains(voisin))
                    {
                        visites.Add(voisin);
                        file.Add(voisin);
                    }
                }
            }

            return visites.Count == adjacence.Count;
        }
    }

    /// <summary>
    /// Classe permettant la visualisation graphique d'un graphe.
    /// </summary>
    public class GrapheVisualizer : Form
    {
        private Graphe graphe;
        private Dictionary<int, Point> positions;

        public GrapheVisualizer(Graphe graphe)
        {
            this.graphe = graphe;
            this.positions = new Dictionary<int, Point>();

            this.Text = "Visualisation du Graphe";
            this.Size = new Size(800, 600);
            this.Paint += new PaintEventHandler(DessinerGraphe);

            CalculerPositionsCirculaires();
        }

        private void CalculerPositionsCirculaires()
        {
            int rayon = 200;
            int centreX = this.ClientSize.Width / 2;
            int centreY = this.ClientSize.Height / 2;
            int n = graphe.adjacence.Count;
            double angleStep = 2 * Math.PI / n;

            int i = 0;
            foreach (var noeud in graphe.adjacence.Keys)
            {
                int x = (int)(centreX + rayon * Math.Cos(i * angleStep));
                int y = (int)(centreY + rayon * Math.Sin(i * angleStep));
                positions[noeud] = new Point(x, y);
                i++;
            }
        }

        private void DessinerGraphe(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);
            Font font = new Font("Arial", 12, FontStyle.Bold);
            Brush brush = Brushes.Black;
            Pen edgePen = new Pen(Color.Black, 2);
            Brush nodeBrush = Brushes.Blue;
            int nodeSize = 30;

            foreach (var noeud in graphe.adjacence)
            {
                Point p1 = positions[noeud.Key];
                foreach (var lien in noeud.Value)
                {
                    int voisin = (lien.Noeud1 == noeud.Key) ? lien.Noeud2 : lien.Noeud1;
                    if (positions.ContainsKey(voisin))
                    {
                        Point p2 = positions[voisin];
                        g.DrawLine(edgePen, p1, p2);
                    }
                }
            }

            foreach (var noeud in positions)
            {
                Point p = noeud.Value;
                g.FillEllipse(nodeBrush, p.X - nodeSize / 2, p.Y - nodeSize / 2, nodeSize, nodeSize);
                SizeF textSize = g.MeasureString(noeud.Key.ToString(), font);
                g.DrawString(noeud.Key.ToString(), font, brush, p.X - textSize.Width / 2, p.Y - textSize.Height / 2);
            }
        }

        public static void AfficherGraphe(Graphe graphe)
        {
            Application.EnableVisualStyles();
            Application.Run(new GrapheVisualizer(graphe));
        }
    }
}
