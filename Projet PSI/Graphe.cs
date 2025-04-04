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
    public List<string> Stations;
    public Dictionary<int, List<string>> DictionnaireStations;
    public Dictionary<string, List<int>> DictionnaireCorrespondances;
    public Dictionary<int, List<Lien>> DictionnaireLiens;
    public List<Noeud> Noeuds;
    public double[,] MatriceAdj;
    /// <summary>
    /// Constructeur Naturel
    /// </summary>
    public Graphe()
    {
        DictionnaireStations = new Dictionary<int, List<string>>();
    }

    /// <summary>
    /// Cree le dictionnaire avec la deuxieme page du excel.
    /// </summary>
    /// <param name="s">Chemin fichier</param>
    /// <returns></returns>
    Dictionary<int, List<string>> CreerDictionnaire(string s)
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

    public double[,] CreerMatrice(List<Noeud> l)
    {
        int taille = l[l.Count - 1].Id + l[0].Id;
        double[,] matrice = new double[taille, taille];

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



    public List<string> CreerListeStations(Dictionary<int, List<string>> dic)
    {
        List<string> StationsUniques = new List<string>();

        foreach (var liste in dic.Values)
        {
            string station = liste[0];
            if (!StationsUniques.Contains(station)) 
            {
                 StationsUniques.Add(station);
            }
        }

        return StationsUniques;
    }

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

            if (double.TryParse(liste[3], out double poids))
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

    public void RemplirMatrice(double[,] mat, Dictionary<int, List<Lien>> dicLien)
    {
        foreach (var e in dicLien)
        {
            int id = e.Key;
            List<Lien> liens = e.Value;

            foreach (Lien lien in liens)
            {
                int i = lien.Noeud1.Id;
                int j = lien.Noeud2.Id;
                double poids = lien.Poids;

                mat[i, j] = poids; 
            }
        }
    }

    public void InstancierGraphe(string s)
    {
        this.DictionnaireStations = CreerDictionnaire(s);
        this.Stations = CreerListeStations(DictionnaireStations);
        this.Noeuds = CreerListeNoeud(DictionnaireStations);
        this.DictionnaireCorrespondances = CreerDicCorrespondance(DictionnaireStations);
        this.MatriceAdj = CreerMatrice(Noeuds);
        this.DictionnaireLiens = CreerDictionnaireLiens(DictionnaireStations);
        RemplirMatrice(MatriceAdj, DictionnaireLiens);



    }

    public void AfficherDictionnaire()
    {
        Console.WriteLine("Test");
        foreach (var kvp in DictionnaireStations)
        {
            Console.WriteLine($"ID: {kvp.Key}, Data: {string.Join(", ", kvp.Value)}");
        }
    
    }
    public void AfficherDictionnaireC()
    {
        Console.WriteLine("Test");
        foreach (var kvp in DictionnaireCorrespondances)
        {
            Console.WriteLine($"ID: {kvp.Key}, Data: {string.Join(", ", kvp.Value)}");
        }

    }
    public void AfficherDictionnaireL()
    {
        foreach (var kvp in DictionnaireLiens)
        {
            int ID = kvp.Key;
            List<Lien> liens = kvp.Value;

            Console.WriteLine($"ID: {ID}");  // Affiche l'information du noeud (en supposant que ToString() est redéfini dans la classe Noeud)

            foreach (var lien in liens)
            {
                Console.WriteLine($"  Lien entre {lien.Noeud1.Id} et {lien.Noeud2.Id}, Poids: {lien.Poids}");
            }
        }

    }
    public void AfficherListeS()
    {
        foreach (string element in Stations)
        {
            Console.WriteLine(element);
        }
    }
    public void AfficherListeN()
    {
        foreach (Noeud element in Noeuds)
        {
            Console.WriteLine(element.Id);
        }
    }
    public void AfficherMatrice()
    {
        double[,] matrice = MatriceAdj;
        int taille = 321;

        Console.Write("     "); // Espace pour les en-têtes
        for (int i = 312; i < taille; i++)
        {
            Console.Write($"{i,6}");
        }
        Console.WriteLine("\n");

        for (int i =312; i < taille; i++)
        {
            Console.Write($"{i,4} "); // En-tête de ligne

            for (int j = 312; j < taille; j++)
            {
                if (matrice[i, j] == 100000)
                    Console.Write("  INF ");
                else
                    Console.Write($"{matrice[i, j],6}");
            }
            Console.WriteLine();
        }
    }
}
}
