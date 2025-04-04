using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using static OfficeOpenXml.ExcelErrorValue;
using System.Windows.Forms;

namespace Projet_PSI
{
    public class Graphe
    {
        public List<string> Stations;
        public Dictionary<int, List<string>> DictionnaireStations;
        public Dictionary<int, List<string>> DictionnaireLongLat;
        public Dictionary<int, List<double>> ValeursLongLat;
        public Dictionary<string, List<int>> DictionnaireCorrespondances;
        public Dictionary<int, List<Lien>> DictionnaireLiens;
        public List<Noeud> Noeuds;
        public int[,] MatriceAdj;
        /// <summary>
        /// Constructeur Naturel
        /// </summary>
        public Graphe()
        {
            this.DictionnaireStations = new Dictionary<int, List<string>>();
            this.DictionnaireLongLat = new Dictionary<int, List<string>>();
            this.DictionnaireCorrespondances = new Dictionary<string, List<int>>();
            this.ValeursLongLat = new Dictionary<int, List<double>>();
            this.DictionnaireLiens = new Dictionary<int, List<Lien>>();
            this.Noeuds = new List<Noeud>();
            this.Stations = new List<string>();
        }

        /// <summary>
        /// Cree le dictionnaire avec la deuxieme page du excel.
        /// </summary>
        /// <param name="s">Chemin fichier</param>
        /// <returns></returns>
        Dictionary<int, List<string>> CreerDictionnaire2(string s)
        {

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            var dict = new Dictionary<int, List<string>>();

            FileInfo fichier = new FileInfo(s);
            using (ExcelPackage package = new ExcelPackage(fichier))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                int lignes = worksheet.Dimension.Rows;
                int colonnes = worksheet.Dimension.Columns;

                for (int i = 2; i <= lignes; i++)
                {
                    int stationId = int.Parse(worksheet.Cells[i, 1].Text);
                    var stationData = new List<string>();

                    for (int j = 2; j <= colonnes; j++)
                    {
                        stationData.Add(worksheet.Cells[i, j].Text);
                    }

                    dict[stationId] = stationData;
                }
            }

            return dict;
        }
        /// <summary>
        /// Cree le dictionnaire avec la premiere page du excel.
        /// </summary>
        /// <param name="s">Chemin</param>
        /// <returns></returns>
        Dictionary<int, List<string>> CreerDictionnaire1(string s)
        {

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            var dict = new Dictionary<int, List<string>>();

            FileInfo fichier = new FileInfo(s);
            using (ExcelPackage package = new ExcelPackage(fichier))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int lignes = worksheet.Dimension.Rows;
                int colonnes = worksheet.Dimension.Columns;

                for (int i = 2; i <= lignes; i++)
                {
                    int stationId = int.Parse(worksheet.Cells[i, 1].Text);
                    var stationData = new List<string>();

                    for (int j = 2; j <= colonnes; j++)
                    {
                        stationData.Add(worksheet.Cells[i, j].Text);
                    }

                    dict[stationId] = stationData;
                }
            }

            return dict;
        }

        /// <summary>
        /// Cree la liste des noeuds existants
        /// </summary>
        /// <param name="dic">Dictionnaire2</param>
        /// <returns></returns>
        public List<Noeud> CreerListeNoeud(Dictionary<int, List<string>> dic)
        {
            List<Noeud> ListeNoeuds = new List<Noeud>();

            foreach (var n in dic)
            {
                Noeud noeud = new Noeud(n.Key);
                ListeNoeuds.Add(noeud);
            }

            return ListeNoeuds;
        }

        /// <summary>
        /// Cree une matrice a partir des noeuds existants
        /// </summary>
        /// <param name="l">Liste des noeuds</param>
        /// <returns></returns>
        public int[,] CreerMatrice(List<Noeud> l)
        {
            int taille = l[l.Count - 1].Id + l[0].Id;
            int[,] matrice = new int[taille, taille];

            for (int i = 0; i < taille; i++)
            {
                for (int j = 0; j < taille; j++)
                {
                    if (i != j)
                        matrice[i, j] = 100000;
                    else
                        matrice[i, j] = 0;
                }
            }

            return matrice;
        }



        /// <summary>
        /// Cree une liste des stations existantes.
        /// </summary>
        /// <param name="dic">Dictionnaire2</param>
        /// <returns></returns>
        public List<string> CreerListeStations(Dictionary<int, List<string>> dic)
        {
            List<string> StationsUniques = new List<string>();

            foreach (var liste in dic.Values)
            {
                string station = liste[0];
                     StationsUniques.Add(station);
            }

            return StationsUniques;
        }

        /// <summary>
        /// Cree le dictionnaire contenants les liens des ID
        /// </summary>
        /// <param name="dic">Dictionnaire2</param>
        /// <returns></returns>
        public Dictionary<int, List<Lien>> CreerDictionnaireLiens(Dictionary<int, List<string>> dic)
        {
            Dictionary<int, List<Lien>> dicLiens = new Dictionary<int, List<Lien>>();
            foreach (var e in dic)
            {
                int idActuel = e.Key;
                List<string> liste = e.Value;
                string terme0 = liste[0];
                string terme1 = liste[1];
                string terme2 = liste[2];
                string terme4 = liste[4];

                if (int.TryParse(liste[3], out int poids))
                {
                    if (!string.IsNullOrEmpty(terme1) && int.TryParse(terme1, out int idNoeud1))
                    {
                        if (!dicLiens.ContainsKey(idActuel))
                        {
                            dicLiens[idActuel] = new List<Lien>();
                        }
                        dicLiens[idActuel].Add(new Lien(Noeuds[idActuel-1], Noeuds[idNoeud1-1], poids));
                    }
                    if (!string.IsNullOrEmpty(terme2) && int.TryParse(terme2, out int idNoeud2))
                    {
                        if (!dicLiens.ContainsKey(idActuel))
                        {
                            dicLiens[idActuel] = new List<Lien>();
                        }
                        dicLiens[idActuel].Add(new Lien(Noeuds[idActuel - 1], Noeuds[idNoeud2 - 1], poids));
                    }
                    if (!string.IsNullOrEmpty(terme4) && int.TryParse(terme4, out int poidsCor))
                    {
                        List<int> Cor = DictionnaireCorrespondances[terme0];
                        foreach (int i in Cor)
                        {
                            if (!dicLiens.ContainsKey(idActuel))
                            {
                                dicLiens[idActuel] = new List<Lien>();
                            }
                            if(idActuel != i)
                            {
                                dicLiens[idActuel].Add(new Lien(Noeuds[idActuel - 1], Noeuds[i-1], poidsCor));
                            }
                        }
                        
                    }
                }



            }
            return dicLiens;

        }

        /// <summary>
        /// Cree dictionnaire contenant toute les correspondances
        /// </summary>
        /// <param name="dic">Dic2</param>
        /// <returns></returns>
        public static Dictionary<string, List<int>> CreerDicCorrespondance(Dictionary<int, List<string>> dic)
        {
            Dictionary<string, List<int>> dicCor = new Dictionary<string, List<int>>();
            foreach (var e in dic)
            {
                List<string> liste = e.Value;

                if (liste[4] != "")
                {
                    string nomStation = liste[0];

                    
                    if (dicCor.ContainsKey(nomStation))
                    {
                        dicCor[nomStation].Add(e.Key);
                    }
                    else
                    {
                        dicCor[nomStation] = new List<int> { e.Key };
                    }
                }
            }
            return dicCor;
        }

        /// <summary>
        /// Rempli la matrice d'adjacence avec les liens du dictionnaire des liens
        /// </summary>
        /// <param name="mat">Matrice d'adjacence</param>
        /// <param name="dicLien">Dictionnaire Liens</param>
        public void RemplirMatrice(int[,] mat, Dictionary<int, List<Lien>> dicLien)
        {
            foreach (var e in dicLien)
            {
                int id = e.Key;
                List<Lien> liens = e.Value;

                foreach (Lien lien in liens)
                {
                    int i = lien.Noeud1.Id;
                    int j = lien.Noeud2.Id;
                    int poids = lien.Poids;

                    mat[i, j] = poids; 
                }
            }
        }

        /// <summary>
        /// Prend la longitude et la latitude des ID
        /// </summary>
        /// <param name="dic">Dic1</param>
        /// <returns></returns>
        public Dictionary<int, List<double>> ExtraireLongLat(Dictionary<int, List<string>> dic)
        {
            var valeursLongLat = new Dictionary<int, List<double>>();

            foreach (var e in dic)
            {
                int key = e.Key;
                List<string> valeurs = e.Value;


               if (double.TryParse(valeurs[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double valeur4) && double.TryParse(valeurs[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double valeur5))
               {
                   valeursLongLat[key] = new List<double> { valeur4, valeur5 };
               }
               



            }

            return valeursLongLat;
        }


        /// <summary>
        /// Algorithme Djikstra retournant la distance entre le debut et les autres sommets ainsi que le chemin parcouru
        /// </summary>
        /// <param name="mat">matrice d'adjacence</param>
        /// <param name="debut">id du sommet de debut</param>
        /// <returns></returns>
        public (int[] distance, List<List<int>> chemins) Djikstra(int[,] mat, int debut)
        {
            int n = mat.GetLength(0) - 1;
            bool[] visite = new bool[n + 1];
            int[] distance = new int[n + 1];
            int[] pred = new int[n + 1];

            for (int i = 1; i <= n; i++)
            {
                distance[i] = 100000;
                pred[i] = -1; 
            }
            distance[debut] = 0;

            for (int count = 1; count <= n; count++)
            {
                int u = -1;
                int minDist = 100000;

                for (int i = 1; i <= n; i++)
                {
                    if (!visite[i] && distance[i] < minDist)
                    {
                        minDist = distance[i];
                        u = i;
                    }
                }

                if (u == -1) break;

                visite[u] = true;

                for (int v = 1; v <= n; v++)
                {
                    if (!visite[v] && mat[u, v] > 0)
                    {
                        int nvDist = distance[u] + mat[u, v];
                        if (nvDist < distance[v])
                        {
                            distance[v] = nvDist;
                            pred[v] = u; 
                        }
                    }
                }
            }

            List<List<int>> chemins = new List<List<int>>();
            for (int i = 1; i <= n; i++)
            {
                List<int> chemin = new List<int>();
                for (int v = i; v != -1; v = pred[v]) 
                {
                    chemin.Add(v);
                }
                chemin.Reverse(); 
                chemins.Add(chemin);
            }

            return (distance, chemins); 
        }


        /// <summary>
        /// Algorithme bellmanford
        /// </summary>
        /// <param name="mat">Matrice D'adjacence</param>
        /// <param name="debut">id du sommet de debut</param>
        /// <returns></returns>
        public int[] BellmanFord(int[,] mat, int debut)
        {
            int n = mat.GetLength(0) - 1; 
            int[] distance = new int[n + 1]; 

            for (int i = 1; i <= n; i++)
            {
                distance[i] = 100000; 
            }
            distance[debut] = 0; 

            for (int i = 1; i <= n - 1; i++)
            {
                for (int u = 1; u <= n; u++)
                {
                    for (int v = 1; v <= n; v++)
                    {
                        if (mat[u, v] > 0 && distance[u] + mat[u, v] < distance[v])
                        {
                            distance[v] = distance[u] + mat[u, v];
                        }
                    }
                }
            }

            return distance;
        }

        /// <summary>
        /// Algorithme floyd marshall
        /// </summary>
        /// <param name="mat">Matrice D'adjacence</param>
        /// <returns></returns>
        public int[,] FloydWarshall(int[,] mat)
        {
            int n = mat.GetLength(0) - 1; 
            int[,] distance = new int[n + 1, n + 1]; 

            
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    if (i == j)
                    {
                        distance[i, j] = 0;
                    }
                    else if (mat[i, j] == 0)
                    {
                        distance[i, j] = 100000;
                    }
                    else
                    {
                        distance[i, j] = mat[i, j];
                    }
                }
            }

            for (int k = 1; k <= n; k++)
            {
                for (int i = 1; i <= n; i++)
                {
                    for (int j = 1; j <= n; j++)
                    {
                        if (distance[i, j] > distance[i, k] + distance[k, j])
                        {
                            distance[i, j] = distance[i, k] + distance[k, j];
                        }
                    }
                }
            }

            return distance;
        }

        /// <summary>
        /// Calcule la distance entre 2 longitude/latitude
        /// </summary>
        /// <param name="lon1"></param>
        /// <param name="lat1"></param>
        /// <param name="lon2"></param>
        /// <param name="lat2"></param>
        /// <returns></returns>
        public double CalculerDistance(double lon1, double lat1, double lon2, double lat2)
        {
            lat1 = lat1 * (Math.PI / 180);
            lon1 = lon1 * (Math.PI / 180);
            lat2 = lat2 * (Math.PI / 180);
            lon2 = lon2 * (Math.PI / 180);

            double dLat = lat2 - lat1;
            double dLon = lon2 - lon1;

            double a = Math.Pow(Math.Sin(dLat / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dLon / 2), 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return 6371000 * c;
        }

        /// <summary>
        /// Trouve la station la plus prohe par rapport a une longitude/latitude
        /// </summary>
        /// <param name="valeursLongLat"></param>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        public int TrouverStationProche(Dictionary<int, List<double>> valeursLongLat, double lon, double lat)
        {
            double distanceMin = double.MaxValue;
            int stationProche = -1;

            foreach (var e in valeursLongLat)
            {
                int key = e.Key;
                List<double> valeurs = e.Value;

                if (valeurs.Count >= 2)
                {
                    double lonStat = valeurs[0];  
                    double latStat = valeurs[1];  

                    double distance = CalculerDistance(lon, lat, lonStat, latStat);

                    
                    if (distance < distanceMin)
                    {
                        distanceMin = distance;
                        stationProche = key;
                    }
                }
            }

            return stationProche;
        }

        /// <summary>
        /// Instancie le graphe par rapport a un fichier excel
        /// </summary>
        /// <param name="s">chemin fichier</param>
        public void InstancierGraphe(string s)
        {
            this.DictionnaireStations = CreerDictionnaire2(s);
            this.DictionnaireLongLat = CreerDictionnaire1(s);
            this.Stations = CreerListeStations(DictionnaireStations);
            this.Noeuds = CreerListeNoeud(DictionnaireStations);
            this.DictionnaireCorrespondances = CreerDicCorrespondance(DictionnaireStations);
            this.MatriceAdj = CreerMatrice(Noeuds);
            this.DictionnaireLiens = CreerDictionnaireLiens(DictionnaireStations);
            this.ValeursLongLat = ExtraireLongLat(DictionnaireLongLat);
            RemplirMatrice(MatriceAdj, DictionnaireLiens);



        }

        /// <summary>
        /// Ouvre l'interface
        /// </summary>
        public void OuvrirFenetre()
        {
            Form form = new Form
            {
                Text = "Calcul Trajet",
                Size = new System.Drawing.Size(500, 550),
                StartPosition = FormStartPosition.CenterScreen
            };

            Label labelLongitude1 = new Label { Text = "Longitude 1", Location = new System.Drawing.Point(30, 30), AutoSize = true };
            TextBox textLongitude1 = new TextBox { Location = new System.Drawing.Point(150, 30), Width = 150 };

            Label labelLatitude1 = new Label { Text = "Latitude 1", Location = new System.Drawing.Point(30, 70), AutoSize = true };
            TextBox textLatitude1 = new TextBox { Location = new System.Drawing.Point(150, 70), Width = 150 };

            Label labelLongitude2 = new Label { Text = "Longitude 2", Location = new System.Drawing.Point(30, 110), AutoSize = true };
            TextBox textLongitude2 = new TextBox { Location = new System.Drawing.Point(150, 110), Width = 150 };

            Label labelLatitude2 = new Label { Text = "Latitude 2", Location = new System.Drawing.Point(30, 150), AutoSize = true };
            TextBox textLatitude2 = new TextBox { Location = new System.Drawing.Point(150, 150), Width = 150 };

            form.Controls.Add(labelLongitude1);
            form.Controls.Add(textLongitude1);
            form.Controls.Add(labelLatitude1);
            form.Controls.Add(textLatitude1);
            form.Controls.Add(labelLongitude2);
            form.Controls.Add(textLongitude2);
            form.Controls.Add(labelLatitude2);
            form.Controls.Add(textLatitude2);


            Button buttonCalculer = new Button
            {
                Text = "Calculer trajet",
                Location = new System.Drawing.Point(150, 200),
                AutoSize = true
            };
            form.Controls.Add(buttonCalculer);


            buttonCalculer.Click += (sender, e) =>
            {

                double lon1, lat1, lon2, lat2;
                bool okLon1 = double.TryParse(textLongitude1.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out lon1);
                bool okLat1 = double.TryParse(textLatitude1.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out lat1);
                bool okLon2 = double.TryParse(textLongitude2.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out lon2);
                bool okLat2 = double.TryParse(textLatitude2.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out lat2);

                if (okLon1 && okLat1 && okLon2 && okLat2)
                {

                    int station1 = TrouverStationProche(ValeursLongLat, lat1, lon1);
                    int station2 = TrouverStationProche(ValeursLongLat, lat2, lon2);

                    var (distances, chemins) = Djikstra(MatriceAdj, station1);

                    int distanceStation2 = distances[station2];

                    List<int> cheminStation2 = chemins[station2 - 1];

                    MessageBox.Show(
                        $"Station 1 : {Stations[station1-1]}\n" +
                        $"Station 2 : {Stations[station2-1]}\n\n" +
                        $"Distance (minutes) : {distanceStation2}\n" +
                        $"Chemin : {string.Join(" → ", cheminStation2.Select(i => Stations[i-1]))}",
                        "Résultat Trajet"
                    );
                }
                else
                {
                    MessageBox.Show("Erreur lors de la conversion des valeurs. Vérifiez vos entrées.", "Erreur");
                }
            };

            form.ShowDialog();
        }

    }

}
