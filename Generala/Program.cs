
using static Generala.Methods;

namespace Generala
{
    public class Program
    {
        static void Main(string[] args)
        {
            bool endGame = false;
            PlayersScore[] players = Methods.Initializate();
            for (int i = 0; i < players[0].score.Length && endGame == false; i++)
            {
                for (var j = 0; j < players.Length && endGame==false; j++)
                {
                    endGame=PlayRound(players, j);
                    if (endGame==true)
                        Console.WriteLine("El jugador {0} gano por Generala Servida", players[j].name);
                }
            }
            if (!endGame)
            {
                var bestScore = 0;
                var winners = new List<int>();
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].score.Sum() > bestScore)
                    {
                        bestScore = players[i].score.Sum();
                        winners.Clear();
                        winners.Add(i);
                    }
                    else if (players[i].score[i] == bestScore)
                        winners.Add(i);
                }
                if (winners.Count > 1)
                    Console.WriteLine("Los ganadores con {0} puntos fueron:", bestScore);
                else
                    Console.WriteLine("El ganador con {0} fue:", bestScore);
                foreach (var player in winners)
                    Console.WriteLine(players[player].name);
                ShowTableScore(players);
            }
        }
    }
}