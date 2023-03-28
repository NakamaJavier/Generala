using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml; 

namespace Generala
{
    internal class Methods
    {
        public static PlayersScore[] Initializate()
        {
            int nPlayers;
            var playersNames = new List<string>();
            nPlayers = NumberPlayers();
            EnterNames(playersNames, nPlayers);
            PlayersScore[] players = new PlayersScore[nPlayers];
            for (int i = 0; i < nPlayers; i++)
            {
                players[i] = new PlayersScore();
                players[i].name = playersNames[i];
            }
            return players;
            

        }
        static int NumberPlayers()
        {
            var nPLayers = 0;
            string input = "";
            while (input.ToLower() != "si" && input.ToLower() != "s") //Ver la condicion
            {
                Console.Clear();
                Console.WriteLine("Introduzca el numero de jugadores (Max 8)");
                nPLayers = Convert.ToInt16(Console.ReadLine());
                Console.Clear();
                input = "";
                if (nPLayers > 0 && nPLayers < 9)
                {
                    Console.WriteLine("El numero de jugadores es: {0} \nEs correcto? Si o No", nPLayers);
                    while (input.ToLower() != "si" && input.ToLower() != "s" && input.ToLower() != "no" && input.ToLower() != "n")
                        input = Console.ReadLine();
                }
                else
                    Console.WriteLine("Invalid amount of players, is has to be a number between 1 to 8");
            }
            return nPLayers;
        }
        static void EnterNames(List<string> namesList, int nPlayers)

        {
            var input = "";
            for (int i = 0; i < nPlayers; i++)
            {
                bool ok = false;
                while (ok==false) 
                {
                    Console.Clear();
                    Console.WriteLine("Introduzca el nombre del jugador N {0}:", i + 1);
                    input = Console.ReadLine();
                    if (!namesList.Contains(input.ToLower(), StringComparer.OrdinalIgnoreCase))
                    {
                        namesList.Add(input);
                        ok= true;
                    }
                    else
                        Console.WriteLine("Ese nombre ya existe. Introduzca otro");
                } 
            }
            Console.Clear();
            Console.WriteLine("Los {0} jugadores son:\n", nPlayers);
            for(int i = 0; i <namesList.Count; i++)
                Console.WriteLine("{0}) {1}", i + 1, namesList[i]);
            Console.WriteLine("\nDesea continuar con estos nombres? Si o No");
            input = Console.ReadLine();
            while (input.ToLower() != "si" && input.ToLower() != "s" && input.ToLower() != "no" && input.ToLower() != "n")
                input = Console.ReadLine();
            if (input.ToLower() != "si" && input.ToLower() != "s")
            {
                namesList.Clear();
                EnterNames(namesList, nPlayers);
            }
            Console.Clear();
        }
        internal class PlayersScore
        {
            public string name;
            public int[] score;
            public PlayersScore()
            {
                name = "";
                score = new int[(int)references.GeneralaDoble+2] {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1};
            }
        }
        public static bool PlayRound(PlayersScore[] player,int nPlayer)
        {
            var tries = 0;
            var index = 0;
            ConsoleKeyInfo input;
            var numbers = new List<int>();
            RollDice(numbers, 5);
            numbers.Sort();
            bool confirm = false;
            var select = new bool[] { false, false, false, false, false };
            do
            {
                index = 0;
                for (int i = 0; i < select.Length; i++)
                    select[i] = false;
                do
                {
                    Console.WriteLine("ES EL TURNO DE:" + player[nPlayer].name+"\n\n");
                    Console.WriteLine("Elige los dados que desees quedarte, desplazandote con las flechas y apretando enter para bloquear y desbloquear el dado\n" +
                   "Aprieta R para tirar de nuevo o F para finalizar ( TAB para ver la tabla de puntuacion)\n");
                    for (int i = 0; i < numbers.Count; i++)
                    {
                        if (index == i)
                        {

                            Console.BackgroundColor = ConsoleColor.White;
                            if (select[i] == false)
                                Console.ForegroundColor = ConsoleColor.Black;
                            else Console.ForegroundColor = ConsoleColor.Blue;
                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.Black;
                            if (select[i] == false)
                                Console.ForegroundColor = ConsoleColor.White;
                            else Console.ForegroundColor = ConsoleColor.Blue;
                        }
                        ShowDice(numbers[i]);
                    }
                    input = Console.ReadKey(true);
                    switch (input.Key)
                    {
                        case ConsoleKey.UpArrow:
                            if (index > 0)
                                index--;
                            break;
                        case ConsoleKey.DownArrow:
                            if (index < numbers.Count - 1)
                                index++;
                            break;
                        case ConsoleKey.Enter:
                            select[index] = !select[index];
                            break;
                        case ConsoleKey.Tab:
                            ShowTableScore(player);
                            break;
                        case ConsoleKey.R:
                            tries++;
                            break;
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Clear();

                } while (input.Key != ConsoleKey.R && input.Key != ConsoleKey.F);
                if (input.Key != ConsoleKey.F)
                {
                    for (int i = select.Length - 1; i >= 0; i--)
                    {
                        if (select[i] == false)
                            numbers.RemoveAt(i);
                    }
                    var amount = select.Count(s => s == false);

                    RollDice(numbers, amount);
                    numbers.Sort();
                }
            } while (tries < 2 && input.Key != ConsoleKey.F);
            var results = CalculateScore(player[nPlayer],numbers,tries);
            if (results[(int)references.Generala] == 1000 || results[(int)references.GeneralaDoble] == 1000)
                return true;
            index = 0;
            do
            {
                Console.WriteLine("ES EL TURNO DE:" + player[nPlayer].name + "\n\n");
                Console.WriteLine("Ha finalizado sus tiros. Sus dados son:\n\n");
                foreach (var number in numbers)
                    ShowDice(number);
                Console.WriteLine("Elija la opcion de puntaje que quiera. Desplazandote con las flechas del teclado confirmando con ENTER");
                var options = 0;
                var save=0;
                for (int i = 0; i < results.Length; i++)
                {
                    if (results[i] != 0 && player[nPlayer].score[i] < 0)
                    {
                        options++;
                        if (options == index + 1)
                        {
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.Black;
                            save = i;
                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        if (i <= (int)references.Seis)
                        {
                            Console.Write("Anotar al {0} : {1}", (i + 1), results[i]);
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("");
                        }
                        switch (i)
                        {
                            case (int)references.Escalera:
                                Console.WriteLine("Anotar Escalera:{0}", results[i]);
                                break;
                            case (int)references.Full:
                                Console.WriteLine("Anotar Full:{0}", results[i]);
                                break;
                            case (int)references.Poker:
                                Console.WriteLine("Anotar Poker:{0}", results[i]);
                                break;
                            case (int)references.Generala:
                                Console.WriteLine("Anotar Generala:{0}", results[i]);
                                break;
                            case (int)references.GeneralaDoble:
                                Console.WriteLine("Anotar Generala Doble:{0}", results[i]);
                                break;
                            case (int)references.GeneralaDoble+1:
                                Console.WriteLine("Tachar una Opcion");
                                break;
                        }
                    }
                }
                input = Console.ReadKey(true);
                switch (input.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (index > 0)
                            index--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (index < options-1)
                            index++;
                        break;
                    case ConsoleKey.Enter:
                        if (options == index + 1)
                            confirm = Tachar(player[nPlayer]);
                        else
                        {
                            player[nPlayer].score[save] = results[save];
                            confirm = true;
                        }
                        break;
                    case ConsoleKey.R:
                        break;
                    case ConsoleKey.Tab:
                        ShowTableScore(player);
                        break;
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Clear();
            } while (input.Key != ConsoleKey.Enter || confirm==false);
            return false;
        }
        public static void ShowDice(int numero)
        {
            
            switch (numero)
            {
                case 1:
                    {
                        Console.WriteLine("┌───────┐\r\n│       │\r\n│   O   │\r\n│       │\r\n└───────┘");
                        break;
                    }

                case 2:
                    {
                        Console.WriteLine("┌───────┐\r\n│ O     │\r\n│       │\r\n│     O │\r\n└───────┘");
                        break;
                    }
                case 3:
                    {
                        Console.WriteLine("┌───────┐\r\n│ O     │\r\n│   O   │\r\n│     O │\r\n└───────┘");
                        break;
                    }
                case 4:
                    {
                        Console.WriteLine("┌───────┐\r\n│ O   O │\r\n│       │\r\n│ O   O │\r\n└───────┘");
                        break;
                    }
                case 5:
                    {
                        Console.WriteLine("┌───────┐\r\n│ O   O │\r\n│   O   │\r\n│ O   O │\r\n└───────┘");
                        break;
                    }
                case 6:
                    {
                        Console.WriteLine("┌───────┐\r\n│ O   O │\r\n│ O   O │\r\n│ O   O │\r\n└───────┘");
                        break;
                    }

            }
        }
        public static void RollDice(List<int> numbers, int dices)
        {
            var random = new Random();
            for (int i = 0; i < dices; i++)
            {
                numbers.Add(random.Next(1, 7));
            }
        }
        public static int[] CalculateScore(PlayersScore player,List<int> dices,int tries)
        {
            //FALTA QUE VERIFIQUE QUE COSAS LE QUEDAN Y CUALES NO
            var result = new int[] {0,0,0,0,0,0,0,0,0,0,0,1};
            for(var i=0;i<= (int)references.Seis; i++)
                    result[i] = (i + 1) * dices.Count(a => a == i+1);
            result[(int)references.Escalera] = Ladder(dices,tries);
            result[(int)references.Full] = Full(dices,tries);
            result[(int)references.Poker] = Poker(dices,tries);
            result[(int)references.Generala] = Generala(player,dices,tries);
            result[(int)references.GeneralaDoble] = DGenerala(player, dices, tries);
            return result;
        }
        internal enum references
        {
            Uno= 0,
            Dos = 1,
            Tres = 2,
            Cuatro = 3,
            Cinco = 4,
            Seis = 5,
            Escalera = 6,
            Full = 7,
            Poker = 8,
            Generala = 9,
            GeneralaDoble=10
        }
        public static int Ladder(List<int> dices,int tries)
        {
            var result = 0;
            var ok = "";
            for(var i=0;i<dices.Count-1;i++)
            {
                if (ok != "no")
                {
                    if (dices[i] != dices[i + 1]-1)
                        ok = "no";
                }
                if (ok != "no" && i == dices.Count -2)
                    ok = "yes";
            }
            if (dices[0]==1 && dices[1]==3)
            {
                ok = "";
                for (var i =1; i < dices.Count - 1; i++)
                {
                    if (ok != "no")
                    {
                        if (dices[i] != dices[i + 1]-1)
                            ok = "no";
                    }
                    if (ok != "no" && i == dices.Count - 2)
                        ok = "yes";
                }
            }
            if( ok== "yes")
            {
                if (tries == 0)
                    result = 25;
                else
                    result = 20;
            }
            return result;
        }
        public static int Full(List<int> dices, int tries)
        {
            var result = 0;
            bool ok = false;
            if((dices.Count(n => n == dices[0])==2 && dices.Count(n => n == dices[2])==3)||(dices.Count(n => n == dices[0]) == 3 && dices.Count(n => n == dices[3]) == 2))
                ok = true;
            if ( ok == true)
            {
                if (tries == 0)
                    result = 35;
                else
                    result = 30;
            }
            return result;
        }
        public static int Poker(List<int> dices, int tries)
        {
            var result = 0;
            bool ok = false;
            if(dices.Count(n => n == dices[0]) == 4 || dices.Count(n => n == dices[1]) == 4)
                ok = true;
            if (ok == true)
            {
                if (tries == 0)
                    result = 45;
                else
                    result = 40;
            }
            return result;
        }
        public static int Generala(PlayersScore player,List<int> dices, int tries)
        {
            var result = 0;
            bool ok = false;
            if (player.score[(int)references.Generala]<10)
                if (dices.Count(n => n == dices[0]) == 5)
                ok = true;
            if (ok == true)
            {
                if (tries == 0)
                    result = 1000;
                else
                    result = 50;
            }
            return result;
        }
        public static int DGenerala(PlayersScore player,List<int> dices, int tries)
        {
            var result = 0;
            bool ok = false;
            if (player.score[(int)references.Generala] != 0 && player.score[(int)references.Generala] != -1)
            {
                if (dices.Count(n => n == dices[0]) == 5)
                    ok = true;
            }
            if (ok == true)
            {
                if (tries == 0)
                    result=1000;
                else
                    result = 100;
            }
            return result;
        }
        
        public static bool Tachar(PlayersScore player)
        {
            ConsoleKeyInfo input;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            bool done = false;
            var index = 0;
            do
            {
                Console.WriteLine("ES EL TURNO DE:" + player.name + "\n\n");
                Console.WriteLine("Elige la opcion que desee tachar .Desplazandote con las flechas del teclado confirmando con ENTER\n");
                var options = 0;
                var save = 0;
                for (int i = 0; i < player.score.Length-1; i++)
                {
                    if (player.score[i] == -1 && i!= (int)references.Generala || player.score[i] == -1 && player.score[(int)references.GeneralaDoble]==0)
                    {
                            options++;
                            if (options == index + 1)
                            {
                                Console.BackgroundColor = ConsoleColor.White;
                                Console.ForegroundColor = ConsoleColor.Black;
                                save = i;
                            }
                            else
                            {
                                Console.BackgroundColor = ConsoleColor.Black;
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            if (i < 6 || i == 7 || i == 8)
                                Console.WriteLine("Tachar el {0}", (references)i);
                            else
                                Console.WriteLine("Tachar la {0}", (references)i);
                    }
                }
                input = Console.ReadKey(true);
                switch (input.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (index > 0)
                            index--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (index < options-1)
                            index++;
                        break;
                    case ConsoleKey.Enter:
                        player.score[save] = 0;
                        done = true;
                        break;
                    case ConsoleKey.Backspace:
                        done = false;
                        break;
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Clear();
            } while (input.Key != ConsoleKey.Enter && input.Key != ConsoleKey.Backspace);
            return done;
        }
        public static void ShowTableScore(PlayersScore[] players)
        {
            int amount = players.Length;
            var nameLenght = 6;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < amount; i++)
            {
                if (nameLenght < players[i].name.Length)
                    nameLenght = players[i].name.Length;
            }
            for (int i = 0; i <= amount; i++)
            {
                if(i==0)
                {
                    Console.Write("┌──────");
                    if (nameLenght > 5)
                    {
                        for (int j = 0; j < nameLenght-6; j++)
                            Console.Write("─");
                    }
                    Console.Write("┬─────┬─────┬─────┬─────┬─────┬─────┬─────┬─────┬─────┬─────┬─────┬─────┐\n");
                    
                    Console.Write("│Nombre");
                    if (nameLenght > 5)
                    {
                        for (int j = 0; j < nameLenght-6; j++)
                            Console.Write(" ");
                    }
                    Console.Write("│  1  │  2  │  3  │  4  │  5  │  6  │Esc. │Full │Poker│ Gen.│ GenD│TOTAL│\n");

                    Console.Write("├──────");
                    if (nameLenght > 5)
                    {
                        for (int j = 0; j < nameLenght - 6; j++)
                            Console.Write("─");
                    }
                    Console.Write("┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┤\n");
                }

                if (i > 0 && i<amount)
                {

                    Console.Write("│" + players[i - 1].name);
                    if (nameLenght > 5)
                    {
                        for (int j = 0; j < nameLenght - players[i - 1].name.Length; j++)
                            Console.Write(" "); ;
                    }
                    var sum = 0;
                    for (int j = 0; j <= (int)references.GeneralaDoble; j++)
                    {
                        if(players[i - 1].score[j]==0)
                            Console.Write("│  /  ");
                        if (players[i - 1].score[j] == -1)
                            Console.Write("│  -  ");
                        if (players[i - 1].score[j] < 10 && players[i - 1].score[j] > 0)
                            Console.Write("│  " + players[i - 1].score[j] + "  ");
                        if (players[i - 1].score[j] >= 10)
                            Console.Write("│ " + players[i - 1].score[j] + "  ");
                        if(players[i - 1].score[j]!=-1)
                        sum += players[i - 1].score[j];
                    }

                    if (sum < 10 )
                        Console.Write("│  " + sum + "  ");
                    if (sum>= 10 && sum <100)
                        Console.Write("│ " + sum + "  ");
                    if (sum >= 100)
                        Console.Write("│ " + sum + " ");

                    Console.Write("│\n");

                    Console.Write("├──────");
                    if (nameLenght > 5)
                    {
                        for (int j = 0; j < nameLenght - 6; j++)
                            Console.Write("─");
                    }
                    Console.Write("┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┤\n");
                }

                if (i == amount)
                {

                    Console.Write("│" + players[i - 1].name);
                        for (int j = 0; j < nameLenght-players[i - 1].name.Length; j++)
                            Console.Write(" "); ;

                    var sum = 0;
                    for (int j = 0; j <= (int)references.GeneralaDoble; j++)
                    {
                        if (players[i - 1].score[j] == 0)
                            Console.Write("│  /  ");
                        if (players[i - 1].score[j] == -1)
                            Console.Write("│  -  ");
                        if (players[i - 1].score[j] < 10 && players[i - 1].score[j] > 0)
                            Console.Write("│  " + players[i - 1].score[j] + "  ");
                        if (players[i - 1].score[j] >= 10)
                            Console.Write("│ " + players[i - 1].score[j] + "  ");
                        if (players[i - 1].score[j] != -1)
                            sum += players[i - 1].score[j];
                    }
                    if (sum < 0)
                        Console.Write("│  " + 0 + "  ");
                    if (sum < 10 && sum>=0)
                        Console.Write("│  " + sum + "  ");
                    if (sum >= 10 && sum < 100)
                        Console.Write("│ " + sum+ "  ");
                    if (sum >= 100)
                        Console.Write("│ " + sum + " ");

                    Console.Write("│\n");

                    Console.Write("└──────");
                    if (nameLenght > 6)

                    {
                        for (int j = 0; j < nameLenght - 6; j++)
                            Console.Write("─");
                    }
                    Console.Write("┴─────┴─────┴─────┴─────┴─────┴─────┴─────┴─────┴─────┴─────┴─────┴─────┘\n");
                }
            }
            var inpar = Console.ReadKey(true);
        }
    }
}

