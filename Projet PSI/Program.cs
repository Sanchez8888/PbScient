namespace ProjetPSI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Graphe graphe = new Graphe("soc-karate.txt","liste");
            GrapheVisualizer.AfficherGraphe(graphe);
            Console.WriteLine(graphe.ToString());
        }
    }
}
