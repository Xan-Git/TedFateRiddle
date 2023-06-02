namespace TedFateRiddle
{
    /*
     * Add GUI with CSS/HTML and port to play in browser with Blazor webassembly
     * Make program play itself to find and record winning solutions
     * More intently track stats of available tarots, which ones are blocked and why, etc.
     */

    internal class Program
    {
        static void Main(string[] args)
        {
            PlayRound();



            Console.Write("Play Again? \n [Y] - 'Yes' \n [Any Key] - 'No' \n ...)");
            var replay = Console.ReadLine();

            if (replay == "Y" || replay == "y")
            {
                PlayRound();
            }


        }

        static void PlayRound()
        {
            Console.WriteLine("START GAME \n");

            // Load Numbers
            var tarots = GenerateData();

            // Init Score
            int myScore = 0;
            int fateScore = 0;
            int selection;

            // Play Game
            

            while (tarots.Where(t => !t.IsOwned).Count() > 0)
            {
                Console.Write("\nChoose a Tarot: ");
                selection = int.Parse(Console.ReadLine());

                ChooseTarot(selection);
                OutputStatus();
            }

            if (myScore > fateScore)
            {
                Console.WriteLine("You Win!");
            }




            void VerifyTarot(Tarot tarot)
            {
                if (tarot is null)
                {
                    throw new ArgumentNullException("Tarot is null");
                }
                if (tarot.Identity < 1)
                {
                    throw new ArgumentOutOfRangeException("No tarrots are less than one");
                }
                if (tarot.Identity > 23)
                {
                    throw new ArgumentOutOfRangeException("No tarrots greater than 23");
                }
                if (tarot.IsOwned)
                {
                    throw new ArgumentOutOfRangeException("Chosen tarrot is no longer in play");
                }

                if (!TarotHasFactorsInPlay(tarot))
                {
                    throw new ArgumentOutOfRangeException("Chosen tarrot has no factors remaining on the board");
                }
            }

            void ChooseTarot(int tarotVal)
            {
                var tarot = tarots[tarotVal - 1];

                // Verify tarot is selectable
                try
                {
                    VerifyTarot(tarot);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nINVALID TAROT");
                    Console.WriteLine(ex.Message + "\n\n");
                    return;
                }

                // Add to my score
                myScore += tarot.Identity;
                tarot.IsOwned = true;

                // Add factors to fate's score
                foreach (var factor in tarot.Factors)
                {
                    if (!tarots[factor - 1].IsOwned)
                    {
                        fateScore += factor;
                        tarots[factor - 1].IsOwned = true;
                        tarots[factor - 1].IsFates = true;
                    }
                }


            }

            bool TarotHasFactorsInPlay(Tarot tarot)
            {
                foreach (var factor in tarot.Factors)
                {
                    if (!tarots[factor - 1].IsOwned)
                    {
                        return true;
                    }
                }

                return false;
            }

            void OutputScore()
            {
                Console.WriteLine("\n \n -------------------------");
                Console.WriteLine($"My Score: {myScore}");
                Console.WriteLine($"Fate's Score: {fateScore}");
            }

            void OutputStatus()
            {
                Console.WriteLine($"My Score: {myScore}");
                Console.WriteLine($"Fate's Score: {fateScore}");
                Console.WriteLine("\n------- DECK STATUS: --------");

                string availableTarots = "Available Tarots: ";
                foreach (var tarot in tarots.Where(t => !t.IsOwned && TarotHasFactorsInPlay(t)))
                {
                    availableTarots += $"{{{tarot.Identity}}} ";
                }
                Console.WriteLine(availableTarots + "\n");


                string myTarots = "My Tarots: ";
                foreach (var tarot in tarots.Where(t => t.IsOwned && !t.IsFates))
                {
                    myTarots += $"{{{tarot.Identity}}} ";
                }
                Console.WriteLine(myTarots);

                string fatesTarots = "Fate's Tarots: ";
                foreach (var tarot in tarots.Where(t => t.IsFates))
                {
                    fatesTarots += $"{{{tarot.Identity}}} ";
                }
                Console.WriteLine(fatesTarots + "\n\n");
            }
        }


        

        static List<Tarot> GenerateData()
        {
            var tarots = new List<Tarot>();

            for (int i = 1; i <= 23; i++)
            {
                Tarot tarot = new Tarot() { Identity = i, Factors = Factorize(i)};
                tarots.Add(tarot);
            }

            return tarots;
        }

        static List<int> Factorize(int num)
        {
            List<int> factors = new List<int>();

            for (int i = 1; i < num; i++)
            {
                if (num % i == 0)
                {
                    factors.Add(i);
                }
            }

            return factors;
        }
    }



    internal class Tarot
    {
        public List<int> Factors { get; set; }     

        public int Identity { get; set; }
        public bool IsOwned { get; set; } = false;
        public bool IsFates { get; set; }

        public override string ToString()
        {
            string factors = "";

            foreach (var factor in Factors)
            {
                factors += $"{factor.ToString()},";
            }

            return $"Identity: {(Identity >= 10 ? $"{Identity}" : $"{Identity}." )} Factors: {factors} {(IsOwned ? "TAKEN" : "")}";
        }

    }


    
}