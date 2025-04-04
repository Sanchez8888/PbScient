namespace Projet_PSI
{
    internal static class Program
    {
        static void Main(string[] args)
        {

            Graphe graphe = new Graphe();
            string filePath = System.IO.Path.Combine(Application.StartupPath, "MetroParisFinal.xlsx");
            graphe.InstancierGraphe(filePath);
            graphe.OuvrirFenetre();
        }

    }
}