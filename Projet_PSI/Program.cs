
using MySql.Data.MySqlClient;


namespace Projet_PSI
{
    internal static class Program
    {
        static void Main(string[] args)
        {

            Graphe graphe = new Graphe();
            string startupPath = Application.StartupPath;
            string filePath = System.IO.Path.Combine(Application.StartupPath, "MetroParisFinal.xlsx");
            Console.WriteLine(startupPath);
            graphe.InstancierGraphe(filePath);

            List<string> resultats = graphe.TrouverTrajet(1, 67);
            foreach (string ligne in resultats)
            {
                Console.WriteLine(ligne);
            }

            Connection Connect = new Connection("site",graphe);
            BDD BDD = new BDD("site");
            List<List<string>> plats = BDD.GetDataPlats();
            Console.WriteLine("e");
            Connect.OuvrirIU();




        }

    }
}