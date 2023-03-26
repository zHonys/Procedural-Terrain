namespace Terrain
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (Game game = new("Basis", 600, 600, 60))
            {
                game.Run();
            }
        }
    }
}